using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebServiceCrawl
{
    class Program
    {
        static void Main(string[] args)
        {
            var searchWords = new[] { "zandvoort", "tuin" };

            // map the search results to a makelaar dictionary
            var fc = new FundaCrawler();
            var makelaars = fc.RetrieveMakelaarsAsync(searchWords).Result;

            // reduce the result to the top 10 by object count
            var top = (from m in makelaars orderby m.Value descending select m).Take(10);
            foreach (var m in top)
            {
                Console.WriteLine("{0}: {1}", m.Key.MakelaarNaam, m.Value);
            }

            // wait to terminate
            Console.ReadLine();
        }
    }
}
