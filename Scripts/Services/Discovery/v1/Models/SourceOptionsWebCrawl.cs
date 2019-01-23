/**
* Copyright 2018 IBM Corp. All Rights Reserved.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/

using FullSerializer;

namespace IBM.Watson.DeveloperCloud.Services.Discovery.v1
{
    /// <summary>
    /// Object defining which URL to crawl and how to crawl it.
    /// </summary>
    public class SourceOptionsWebCrawl
    {
        /// <summary>
        /// The number of concurrent URLs to fetch. `gentle` means one URL is fetched at a time with a delay between
        /// each call. `normal` means as many as two URLs are fectched concurrently with a short delay between fetch
        /// calls. `aggressive` means that up to ten URLs are fetched concurrently with a short delay between fetch
        /// calls.
        /// </summary>
        /// <value>
        /// The number of concurrent URLs to fetch. `gentle` means one URL is fetched at a time with a delay between
        /// each call. `normal` means as many as two URLs are fectched concurrently with a short delay between fetch
        /// calls. `aggressive` means that up to ten URLs are fetched concurrently with a short delay between fetch
        /// calls.
        /// </value>
        public enum CrawlSpeedEnum
        {
            
            /// <summary>
            /// Enum GENTLE for gentle
            /// </summary>
            gentle,
            
            /// <summary>
            /// Enum NORMAL for normal
            /// </summary>
            normal,
            
            /// <summary>
            /// Enum AGGRESSIVE for aggressive
            /// </summary>
            aggressive
        }

        /// <summary>
        /// The number of concurrent URLs to fetch. `gentle` means one URL is fetched at a time with a delay between
        /// each call. `normal` means as many as two URLs are fectched concurrently with a short delay between fetch
        /// calls. `aggressive` means that up to ten URLs are fetched concurrently with a short delay between fetch
        /// calls.
        /// </summary>
        [fsProperty("crawl_speed")]
        public CrawlSpeedEnum? CrawlSpeed { get; set; }
        /// <summary>
        /// The starting URL to crawl.
        /// </summary>
        [fsProperty("url")]
        public string Url { get; set; }
        /// <summary>
        /// When `true`, crawls of the specified URL are limited to the host part of the **url** field.
        /// </summary>
        [fsProperty("limit_to_starting_hosts")]
        public bool? LimitToStartingHosts { get; set; }
        /// <summary>
        /// When `true`, allows the crawl to interact with HTTPS sites with SSL certificates with untrusted signers.
        /// </summary>
        [fsProperty("allow_untrusted_certificate")]
        public bool? AllowUntrustedCertificate { get; set; }
        /// <summary>
        /// The maximum number of hops to make from the initial URL. When a page is crawled each link on that page will
        /// also be crawled if it is within the **maximum_hops** from the initial URL. The first page crawled is 0 hops,
        /// each link crawled from the first page is 1 hop, each link crawled from those pages is 2 hops, and so on.
        /// </summary>
        [fsProperty("maximum_hops")]
        public long? MaximumHops { get; set; }
        /// <summary>
        /// The maximum milliseconds to wait for a response from the web server.
        /// </summary>
        [fsProperty("request_timeout")]
        public long? RequestTimeout { get; set; }
        /// <summary>
        /// When `true`, the crawler will ignore any `robots.txt` encountered by the crawler. This should only ever be
        /// done when crawling a web site the user owns. This must be be set to `true` when a **gateway_id** is specied
        /// in the **credentials**.
        /// </summary>
        [fsProperty("override_robots_txt")]
        public bool? OverrideRobotsTxt { get; set; }
    }

}
