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

using Newtonsoft.Json;

namespace IBM.Watson.Discovery.V1.Model
{
    /// <summary>
    /// Object containing the schedule information for the source.
    /// </summary>
    public class SourceSchedule
    {
        /// <summary>
        /// The crawl schedule in the specified **time_zone**.
        ///
        /// -  `five_minutes`: Runs every five minutes.
        /// -  `hourly`: Runs every hour.
        /// -  `daily`: Runs every day between 00:00 and 06:00.
        /// -  `weekly`: Runs every week on Sunday between 00:00 and 06:00.
        /// -  `monthly`: Runs the on the first Sunday of every month between 00:00 and 06:00.
        /// </summary>
        public class FrequencyValue
        {
            /// <summary>
            /// Constant DAILY for daily
            /// </summary>
            public const string DAILY = "daily";
            /// <summary>
            /// Constant WEEKLY for weekly
            /// </summary>
            public const string WEEKLY = "weekly";
            /// <summary>
            /// Constant MONTHLY for monthly
            /// </summary>
            public const string MONTHLY = "monthly";
            /// <summary>
            /// Constant FIVE_MINUTES for five_minutes
            /// </summary>
            public const string FIVE_MINUTES = "five_minutes";
            /// <summary>
            /// Constant HOURLY for hourly
            /// </summary>
            public const string HOURLY = "hourly";
            
        }

        /// <summary>
        /// The crawl schedule in the specified **time_zone**.
        ///
        /// -  `five_minutes`: Runs every five minutes.
        /// -  `hourly`: Runs every hour.
        /// -  `daily`: Runs every day between 00:00 and 06:00.
        /// -  `weekly`: Runs every week on Sunday between 00:00 and 06:00.
        /// -  `monthly`: Runs the on the first Sunday of every month between 00:00 and 06:00.
        /// Constants for possible values can be found using SourceSchedule.FrequencyValue
        /// </summary>
        [JsonProperty("frequency", NullValueHandling = NullValueHandling.Ignore)]
        public string Frequency { get; set; }
        /// <summary>
        /// When `true`, the source is re-crawled based on the **frequency** field in this object. When `false` the
        /// source is not re-crawled; When `false` and connecting to Salesforce the source is crawled annually.
        /// </summary>
        [JsonProperty("enabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Enabled { get; set; }
        /// <summary>
        /// The time zone to base source crawl times on. Possible values correspond to the IANA (Internet Assigned
        /// Numbers Authority) time zones list.
        /// </summary>
        [JsonProperty("time_zone", NullValueHandling = NullValueHandling.Ignore)]
        public string TimeZone { get; set; }
    }
}
