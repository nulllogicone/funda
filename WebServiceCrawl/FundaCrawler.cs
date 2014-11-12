using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebServiceCrawl
{

    internal class FundaCrawler
    { 
        // webservice url pattern, replace the searchpattern and page number must be replaced
        private const string UrlPattern = "http://partnerapi.funda.nl/feeds/Aanbod.svc/json/005e7c1d6f6c4f9bacac16760286e3cd/?type=koop&zo={0}/p{1}";

        internal async Task<Dictionary<string, int>> RetrieveMakelaarsAsync(string searchpattern = "/amsterdam/tuin")
        {
            var returnObject = new Dictionary<string, int>();

            var url = string.Format(UrlPattern, searchpattern, 1);
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await client.SendAsync(request);
            var resstr = await response.Content.ReadAsStringAsync();
            dynamic mkls = JsonConvert.DeserializeObject(resstr);


            foreach (var o in mkls.Objects)
            {
                string m = o.MakelaarNaam;
                if (returnObject.ContainsKey(m))
                {
                    returnObject[m] ++;
                }
                else
                {
                    returnObject.Add(m,1);
                }
            }

            return returnObject;
        }
    }
}
