using System;
using System.Collections.Generic;

namespace WebCrawlerAgent
{
    class Program
    {
        //static CrawlerTest crawler;
        static void Main(string[] args)
        {
            Console.WriteLine("Insert your URL");
            string URL = Console.ReadLine();
            Console.WriteLine("Maximum of links");
            int numberOfLinks = int.Parse(Console.ReadLine());
            Console.WriteLine("Maximum of depthing");
            int maxDepth = int.Parse(Console.ReadLine());

            UriBuilder ub = new UriBuilder(URL);

            Queue<Crawler.WebLink> initialUrls = new Queue<Crawler.WebLink>();

            initialUrls.Enqueue(new Crawler.WebLink(ub.Uri, 1));

            Crawler crawler = new Crawler(initialUrls, numberOfLinks, maxDepth);
            

            Console.WriteLine("\n\nValid links: ");
            printLinks(crawler.GetVisitedUrls(), true);

            Console.WriteLine("\n\nInvalid links: ");
            printLinks(crawler.GetVisitedUrls(), false);

            //for (int i = 0; i <=(numberoflinks*5)+5; i++)
            //{
            //   crawler = new crawlertest(url, numberoflinks);
            //    }
        }
        private static void printLinks(Dictionary<Uri, bool> links, bool isValid)
        {
            foreach (KeyValuePair<Uri, bool> kv in links)
            {
                if (kv.Value == isValid)
                {
                    Console.WriteLine("    {0}", kv.Key);
                }
            }
            Console.ReadLine();

        }
    }
}

