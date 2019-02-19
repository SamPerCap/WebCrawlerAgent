using System;

namespace WebCrawlerAgent
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Insert your URL");
            string URL = Console.ReadLine();
            Crawler builder = new Crawler(URL);
            Console.ReadLine();
        }
    }
}
