using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace WebCrawlerAgent
{
    public class CrawlerTest
    {
        protected string _URL { get; private set; }
        protected int _numberOfMaxLinks { get; private set; }
        Uri initialUrl;
        Uri hostUrl;
        WebClient wc;
        Thread runningThread;
        string webPage;
        //Blocks the calle if there is no data.
        static BlockingCollection<Uri> frontier = new BlockingCollection<Uri>();
        static BlockingCollection<Uri> resultUrls = new BlockingCollection<Uri>();
        //Thread Safe.
        static ConcurrentDictionary<string, bool> visitedUrls = new ConcurrentDictionary<string, bool>();

        string hostAgent = "StudentAgentTest";


        public CrawlerTest(string URL, int numberOfLinks)
        {
            _URL = URL;
            initialUrl = new UriBuilder(_URL).Uri;
            _numberOfMaxLinks = numberOfLinks;
            frontier.Add(initialUrl);
            runningThread = new Thread(() => Crawl(_numberOfMaxLinks));
            runningThread.Start();

        }

        private void Crawl(int maxLinks)
        {
            Console.WriteLine("Starting to crawl");
            while (frontier.Count > 0 && visitedUrls.Count < maxLinks)
            {
                Uri url = frontier.Take();
                if (!visitedUrls.ContainsKey(url.ToString()))
                    VisitPage(url);
            }
        }
        private void VisitPage(Uri url)
        {
            visitedUrls.TryAdd(url.ToString(), true);


            try
            {
                hostUrl = new UriBuilder(url.Host).Uri;
                wc = new WebClient();
                wc.Headers.Add("StudentAgentTest", hostAgent);
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
                            frontier.Take();
                    }
                }
            }
            catch (Exception)
            {
                visitedUrls[url.ToString()] = false;
            }

            foreach (KeyValuePair<string, bool> kv in visitedUrls)
                Console.WriteLine("{0,-10} {1}", kv.Value, kv.Key);
        }

        static Uri NormalizeUrl(Uri hostUrl, string url)
        {
            return Uri.TryCreate(hostUrl, url, out Uri absoluteUrl) ? absoluteUrl : null;
        }
    }
}
