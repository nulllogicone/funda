using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Threading;

namespace WebServiceCrawl
{

    public class FundaCrawler
    {
        // webservice url pattern. The searchstring and page number must be replaced
        private const string UrlPattern = "http://partnerapi.funda.nl/feeds/Aanbod.svc/json/005e7c1d6f6c4f9bacac16760286e3cd/?type=koop&zo={0}&pagesize=25&page={1}";

        // internal object that holds the makelaars with the total amount of objects
        Dictionary<Makelaar, int> makelaarDictionary;

        /// <summary>
        /// Returns a dictionary with Makelaars and the total amount of objects for the specified search
        /// </summary>
        /// <param name="searchpattern">A dash-delimited string</param>
        /// <returns></returns>
        public async Task<Dictionary<Makelaar, int>> RetrieveMakelaarsAsync(string[] searcharray)
        {
            var page = 1;
            makelaarDictionary = new Dictionary<Makelaar, int>();
            await FillPageResultInDictionary(searcharray, page);
            return makelaarDictionary;
        }

        /// <summary>
        /// Helper method that loads a Json page and fills the dictionary with makelaars and amount of their objects.
        /// If there are more result pages, the method makes a recursive call to itself for the next page number.
        /// </summary>
        /// <param name="searchWords">An array with all words you want to search for</param>
        /// <param name="page">The current page to retrieve.</param>
        /// <returns>Awaitable Task (void)</returns>
        /// <remarks>The request rate is throttled down to less than 100 / min to </remarks>
        private async Task FillPageResultInDictionary(string[] searchWords, int page)
        {
            var searchString = string.Format("/{0}/", string.Join("/", searchWords));
            var url = string.Format(UrlPattern, searchString, page);
            using (var client = new HttpClient())
            using (var response = await client.GetAsync(url))
            using (var content = response.Content)
            {
                var result = await content.ReadAsStringAsync();
                dynamic resultPage = JsonConvert.DeserializeObject(result);

                foreach (var o in resultPage.Objects)
                {
                    var m = JsonConvert.DeserializeObject<Makelaar>(o.ToString());
                    if (makelaarDictionary.ContainsKey(m))
                    {
                        makelaarDictionary[m]++;
                    }
                    else
                    {
                        makelaarDictionary.Add(m, 1);
                    }
                }

                // if there are more result pages, make a recursive call for next page
                int totalPages = resultPage.Paging.AantalPaginas;
                if (totalPages > page)
                {
                    Thread.Sleep(600);
                    await FillPageResultInDictionary(searchWords,  ++page);
                }
            }
        }
    }
}
