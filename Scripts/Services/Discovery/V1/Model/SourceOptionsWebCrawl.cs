/**
* Copyright 2018, 2019 IBM Corp. All Rights Reserved.
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

using System.Collections.Generic;
using Newtonsoft.Json;

namespace IBM.Watson.Discovery.V1.Model
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
        public class CrawlSpeedValue
        {
            /// <summary>
            /// Constant GENTLE for gentle
            /// </summary>
            public const string GENTLE = "gentle";
            /// <summary>
            /// Constant NORMAL for normal
            /// </summary>
            public const string NORMAL = "normal";
            /// <summary>
            /// Constant AGGRESSIVE for aggressive
            /// </summary>
            public const string AGGRESSIVE = "aggressive";
            
        }

        /// <summary>
        /// The number of concurrent URLs to fetch. `gentle` means one URL is fetched at a time with a delay between
        /// each call. `normal` means as many as two URLs are fectched concurrently with a short delay between fetch
        /// calls. `aggressive` means that up to ten URLs are fetched concurrently with a short delay between fetch
        /// calls.
        /// Constants for possible values can be found using SourceOptionsWebCrawl.CrawlSpeedValue
        /// </summary>
        [JsonProperty("crawl_speed", NullValueHandling = NullValueHandling.Ignore)]
        public string CrawlSpeed { get; set; }
        /// <summary>
        /// The starting URL to crawl.
        /// </summary>
        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public string Url { get; set; }
        /// <summary>
        /// When `true`, crawls of the specified URL are limited to the host part of the **url** field.
        /// </summary>
        [JsonProperty("limit_to_starting_hosts", NullValueHandling = NullValueHandling.Ignore)]
        public bool? LimitToStartingHosts { get; set; }
        /// <summary>
        /// When `true`, allows the crawl to interact with HTTPS sites with SSL certificates with untrusted signers.
        /// </summary>
        [JsonProperty("allow_untrusted_certificate", NullValueHandling = NullValueHandling.Ignore)]
        public bool? AllowUntrustedCertificate { get; set; }
        /// <summary>
        /// The maximum number of hops to make from the initial URL. When a page is crawled each link on that page will
        /// also be crawled if it is within the **maximum_hops** from the initial URL. The first page crawled is 0 hops,
        /// each link crawled from the first page is 1 hop, each link crawled from those pages is 2 hops, and so on.
        /// </summary>
        [JsonProperty("maximum_hops", NullValueHandling = NullValueHandling.Ignore)]
        public long? MaximumHops { get; set; }
        /// <summary>
        /// The maximum milliseconds to wait for a response from the web server.
        /// </summary>
        [JsonProperty("request_timeout", NullValueHandling = NullValueHandling.Ignore)]
        public long? RequestTimeout { get; set; }
        /// <summary>
        /// When `true`, the crawler will ignore any `robots.txt` encountered by the crawler. This should only ever be
        /// done when crawling a web site the user owns. This must be be set to `true` when a **gateway_id** is specied
        /// in the **credentials**.
        /// </summary>
        [JsonProperty("override_robots_txt", NullValueHandling = NullValueHandling.Ignore)]
        public bool? OverrideRobotsTxt { get; set; }
        /// <summary>
        /// Array of URL's to be excluded while crawling. The crawler will not follow links which contains this string.
        /// For example, listing `https://ibm.com/watson` also excludes `https://ibm.com/watson/discovery`.
        /// </summary>
        [JsonProperty("blacklist", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Blacklist { get; set; }
    }
}
