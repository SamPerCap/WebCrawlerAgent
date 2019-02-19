using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace WebCrawlerAgent
{
    public class Crawler
    {
        protected string _URL { get; private set; }
        Uri initialUrl;
        Uri hostUrl;
        WebClient wc;
        int numberOfMaxLinks;
        string webPage;
        static Queue<Uri> frontier = new Queue<Uri>();
        static Dictionary<string, bool> visitedUrls = new Dictionary<string, bool>();
        

        public Crawler(string URL, int numberOfLinks)
        {
            _URL = URL;
            initialUrl = new UriBuilder(_URL).Uri;
            numberOfLinks = numberOfMaxLinks;
            frontier.Enqueue(initialUrl);
            Crawl(numberOfMaxLinks);

            foreach (KeyValuePair<string, bool> kv in visitedUrls)
            {
                Console.WriteLine("{0, -10}{1}", kv.Value, kv.Key);
            }
        }

        private void Crawl(int maxLinks)
        {
            while (frontier.Count > 0 && visitedUrls.Count < maxLinks)
            {
                Uri url = frontier.Dequeue();
                if (!visitedUrls.ContainsKey(url.ToString()))
                    VisitPage(url);
            }
        }
        private void VisitPage(Uri url)
        {
            visitedUrls.Add(url.ToString(), true);

            try
            {
            hostUrl = new UriBuilder(url.Host).Uri;
            wc = new WebClient();
            webPage = wc.DownloadString(url);
            Console.WriteLine("Downloading...");

            var urlTagPattern = new Regex(@"<a.*?href=[""'](?<url>.*?)[""'].*?>(?<name>.*?)</a>", RegexOptions.IgnoreCase);
            var hrefPattern = new Regex("href\\s*=\\s*(?:\"(?<1>[^\"]*)\"|(?<1>\\S+))", RegexOptions.IgnoreCase);

            var links = urlTagPattern.Matches(webPage);
                
                foreach (Match href in links)
                {
                    string newUrl = hrefPattern.Match(href.Value).Groups[1].Value;
                    Uri absoluteUrl = NormalizeUrl(hostUrl, newUrl);
                    if (absoluteUrl != null 
                        && absoluteUrl.Scheme == Uri.UriSchemeHttp
                        || absoluteUrl.Scheme == Uri.UriSchemeHttps)
                    {
                        if (!visitedUrls.ContainsKey(absoluteUrl.ToString()))
                            frontier.Enqueue(absoluteUrl);
                    }
                }
            }
            catch (Exception)
            {
                visitedUrls[url.ToString()] = false;
            }

        }

        static Uri NormalizeUrl(Uri hostUrl, string url)
        {
            return Uri.TryCreate(hostUrl, url, out Uri absoluteUrl) ? absoluteUrl : null;
        }
    }
}
