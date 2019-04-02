using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace WebCrawlerAgent
{
    public class Crawler
    {
        protected string _URL { get; private set; }
        protected int _numberOfMaxLinks { get; private set; }
        protected int _MaxDepth { get; private set; }

        Uri hostUrl;
        WebClient wc;
        string webPage;
        Queue<WebLink> frontier;
        Dictionary<Uri, bool> visitedUrls;

        bool done = false;
        Thread agentThread;


        public Crawler(Queue<WebLink> URL, int numberOfLinks, int maxDepth)
        {
            frontier = URL;
            _MaxDepth = maxDepth;
            _numberOfMaxLinks = numberOfLinks;

            frontier = new Queue<WebLink>();
            visitedUrls = new Dictionary<Uri, bool>();

            wc = new WebClient();
            wc.Headers.Add(HttpRequestHeader.UserAgent, "AStudentAgentForTesting");

            agentThread = new Thread(() => Crawl());
            agentThread.Start();

        }

        private void Crawl()
        {
            while (!done)
            {
                Console.WriteLine("Starting the thread");
                while (frontier.Count > 0 && visitedUrls.Count < _numberOfMaxLinks)
                {
                    WebLink link = frontier.Dequeue();
                    if (!visitedUrls.ContainsKey(link.WebUrl))
                        VisitPage(link);
                }
            }
            agentThread.Interrupt();
        }
        private void VisitPage(WebLink link)
        {
            visitedUrls[link.WebUrl] = true;

            try
            {
                if (link.Level < _MaxDepth)
                {
                    downloadPage(webPage, link);
                }
            }
            catch (Exception)
            {
                visitedUrls[link.WebUrl] = false;
            }

        }
        public Dictionary<Uri, bool> GetVisitedUrls()
        {
            return new Dictionary<Uri, bool>(visitedUrls);
        }
        private void downloadPage(string url, WebLink webLink)
        {
            var urlTagPattern = new Regex(@"<a.*?href=[""'](?<url>.*?)[""'].*?>(?<name>.*?)</a>", RegexOptions.IgnoreCase);
            var hrefPattern = new Regex("href\\s*=\\s*(?:\"(?<1>[^\"]*)\"|(?<1>\\S+))", RegexOptions.IgnoreCase);

            var links = urlTagPattern.Matches(webPage);
            hostUrl = new UriBuilder(webLink.WebUrl.Host).Uri;

            webPage = wc.DownloadString(url);
            Console.WriteLine("Downloading...");

            foreach (Match href in links)
            {
                try
                {

                    string newUrl = hrefPattern.Match(href.Value).Groups[1].Value;
                    Uri absoluteUrl = NormalizeUrl(hostUrl, newUrl);

                    if (absoluteUrl != null
                        && absoluteUrl.Scheme == Uri.UriSchemeHttp
                        || absoluteUrl.Scheme == Uri.UriSchemeHttps)
                    {
                        if (!visitedUrls.ContainsKey(absoluteUrl)
                            && absoluteUrl.Scheme == Uri.UriSchemeHttp
                            || absoluteUrl.Scheme == Uri.UriSchemeHttps)
                            frontier.Enqueue(new WebLink(absoluteUrl, webLink.Level + 1));
                    }
                }

                catch (Exception)
                {
                    //Invalid link, nothing by now
                }
                done = true;
            }
        }

        private Uri NormalizeUrl(Uri hostUrl, string url)
        {
            Uri normalizedUrl;
            Uri.TryCreate(hostUrl, url, out normalizedUrl);
            return normalizedUrl;
        }
        public class WebLink
        {
            public Uri WebUrl { get; set; }
            public int Level { get; set; }
            public WebLink(Uri link, int level)
            {
                WebUrl = link;
                Level = level;
            }
        }
    }
}
