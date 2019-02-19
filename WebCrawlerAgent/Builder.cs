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
        Uri ub;
        Uri hostUrl;
        WebClient wc;
        string webPage;

        public Crawler(string URL)
        {
            _URL = URL;
            ub = new UriBuilder(_URL).Uri;
            hostUrl = new UriBuilder(ub.Host).Uri;
            wc = new WebClient();
            Console.WriteLine("Downloading...");

            webPage = wc.DownloadString(ub);
            //Console.WriteLine("Here is:" + webPage);

            var urlTagPattern = new Regex(@"<a.*?href=[""'](?<url>.*?)[""'].*?>(?<name>.*?)</a>", RegexOptions.IgnoreCase);
            var hrefPattern = new Regex("href\\s*=\\s*(?:\"(?<1>[^\"]*)\"|(?<1>\\S+))", RegexOptions.IgnoreCase);

            var links = urlTagPattern.Matches(webPage);

            foreach (Match href in links)
            {
                string newUrl = hrefPattern.Match(href.Value).Groups[1].Value;

                if (Uri.TryCreate(hostUrl, newUrl, out Uri absoluteUrl)
                    && absoluteUrl.Scheme == Uri.UriSchemeHttp
                    || absoluteUrl.Scheme == Uri.UriSchemeHttps)

                Console.WriteLine(absoluteUrl.ToString());
            }

        }
    }
}
