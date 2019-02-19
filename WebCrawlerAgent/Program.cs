using System;

namespace WebCrawlerAgent
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Insert your URL");
            string URL = Console.ReadLine();
            Console.WriteLine("Maximum of links");
            int numberOfLinks = Convert.ToInt32(Console.ReadLine());
            Crawler builder = new Crawler(URL, numberOfLinks);
            Console.ReadLine();
        }
    }
}
