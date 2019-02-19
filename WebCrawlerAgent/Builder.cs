using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace WebCrawlerAgent
{
    public class Crawler
    {
        public string _URL { get; private set; }
        UriBuilder ub;
        WebClient wc;
        string webPage;

        public Crawler(string URL)
        {
            _URL = URL;
            ub = new UriBuilder(_URL);
            wc = new WebClient();
            Console.WriteLine("Downloading...");
            webPage = wc.DownloadString(ub.Uri.ToString());
            //Console.WriteLine("Here is:" + webPage);
            var urlTagPattern = new Regex(@"<a.*?href=[""'](?<url>.*?)[""'].*?>(?<name>.*?)</a>", RegexOptions.IgnoreCase);
            var hrefPattern = new Regex("href\\s*=\\s*(?:\"(?<1>[^\"]*)\"|(?<1>\\S+))", RegexOptions.IgnoreCase);
            var urls = urlTagPattern.Matches(webPage);

            foreach (Match url in urls)
            {
                string newUrl = hrefPattern.Match(url.Value).Groups[1].Value;
                Console.WriteLine(newUrl);
            }

        }
    }
}
