using System;
using System.Linq;


namespace WebServiceCrawl
{
    class Program
    {
        static void Main(string[] args)
        {
            // configure the desired parameters
            var objectType = ObjectType.koop;
            var searchWords = new[] { "zandvoort", "tuin" };

            // map the search results to a makelaar dictionary
            var fc = new FundaCrawler();
            var makelaars = fc.RetrieveMakelaarsAsync(objectType, searchWords).Result;

            // reduce the result to the top 10 by object count
            var top = (from m in makelaars orderby m.Value descending select m).Take(10);
            foreach (var m in top)
            {
                Console.WriteLine("{0,5} \t {1}",  m.Value, m.Key.MakelaarNaam);
            }

            // wait before terminate
            Console.ReadLine();
        }
    }
}
