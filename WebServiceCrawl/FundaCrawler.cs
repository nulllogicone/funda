using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Threading;

namespace WebServiceCrawl
{
    /// <summary>
    /// A type safe parameter for possible object type options
    /// </summary>
    public enum ObjectType
    {
        Koop,
        Huur,
        Recreatie,
        Project,
        Bog
    }

    /// <summary>
    /// This class encapsulates the communication with the funda web service
    /// and offers a simple functions to be consumed by clients
    /// </summary>
    public class FundaCrawler
    {
        
        // webservice url pattern. The object-type, searchstring and page number must be replaced
        private const string UrlPattern = "http://partnerapi.funda.nl/feeds/Aanbod.svc/json/005e7c1d6f6c4f9bacac16760286e3cd/?type={0}&zo={1}&pagesize=25&page={2}";

        // internal object that holds the makelaars with the total amount of objects
        Dictionary<Makelaar, int> _makelaarDictionary;

        /// <summary>
        /// Returns a dictionary with Makelaars and the total amount of objects for the specified search
        /// </summary>
        /// <param name="objectType">A enumeration value with the object type to search for</param>
        /// <param name="searchArray">A array with search words</param>
        /// <returns></returns>
        public async Task<Dictionary<Makelaar, int>> RetrieveMakelaarsAsync(ObjectType objectType, string[] searchArray)
        {
            var page = 1;
            _makelaarDictionary = new Dictionary<Makelaar, int>();
            await FillPageResultInDictionary(objectType, searchArray, page);
            return _makelaarDictionary;
        }

        /// <summary>
        /// Helper method that loads a Json page and fills the dictionary with makelaars and amount of their objects.
        /// If there are more result pages, the method makes a recursive call to itself for the next page number.
        /// </summary>
        /// <param name="objectType">A enumeration value with the object type to search for</param>
        /// <param name="searchArray">A array with search words</param>
        /// <param name="page">The current page to retrieve.</param>
        /// <returns>Awaitable Task (void)</returns>
        /// <remarks>The request rate is throttled down to less than 100 / min to </remarks>
        private async Task FillPageResultInDictionary(ObjectType objectType, string[] searchArray, int page)
        {
            // prepare the url
            var searchString = $"/{string.Join("/", searchArray)}/";
            var url = string.Format(UrlPattern, objectType, searchString, page);

            // execute the http request
            using (var client = new HttpClient())
            using (var response = await client.GetAsync(url))
            using (var content = response.Content)
            {
                var result = await content.ReadAsStringAsync();
                dynamic resultPage = JsonConvert.DeserializeObject(result);

                // add the makelaar or increment his amount for all objects
                foreach (var o in resultPage.Objects)
                {
                    var m = JsonConvert.DeserializeObject<Makelaar>(o.ToString());
                    if (_makelaarDictionary.ContainsKey(m))
                    {
                        _makelaarDictionary[m]++;
                    }
                    else
                    {
                        _makelaarDictionary.Add(m, 1);
                    }
                }

                // if there are more result pages, make a recursive call to the next page
                int totalPages = resultPage.Paging.AantalPaginas;
                if (totalPages > page)
                {
                    Thread.Sleep(600);
                    await FillPageResultInDictionary(objectType, searchArray,  ++page);
                }
            }
        }
    }
}
