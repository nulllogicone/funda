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
            var fc = new FundaCrawler();
            var maks = fc.RetrieveMakelaarsAsync().Result;

            var top = (from m in maks orderby m.Value descending select m);
            foreach (var m in top)
            {
                Console.WriteLine("{0}: {1}",m.Key,m.Value);
            }

            // wait to terminate
            Console.ReadLine();

        }

    }
}
