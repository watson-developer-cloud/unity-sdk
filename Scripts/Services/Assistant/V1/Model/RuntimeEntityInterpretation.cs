/**
* (C) Copyright IBM Corp. 2018, 2020.
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

namespace IBM.Watson.Assistant.V1.Model
{
    /// <summary>
    /// RuntimeEntityInterpretation.
    /// </summary>
    public class RuntimeEntityInterpretation
    {
        /// <summary>
        /// The precision or duration of a time range specified by a recognized `@sys-time` or `@sys-date` entity.
        /// </summary>
        public class GranularityValue
        {
            /// <summary>
            /// Constant DAY for day
            /// </summary>
            public const string DAY = "day";
            /// <summary>
            /// Constant FORTNIGHT for fortnight
            /// </summary>
            public const string FORTNIGHT = "fortnight";
            /// <summary>
            /// Constant HOUR for hour
            /// </summary>
            public const string HOUR = "hour";
            /// <summary>
            /// Constant INSTANT for instant
            /// </summary>
            public const string INSTANT = "instant";
            /// <summary>
            /// Constant MINUTE for minute
            /// </summary>
            public const string MINUTE = "minute";
            /// <summary>
            /// Constant MONTH for month
            /// </summary>
            public const string MONTH = "month";
            /// <summary>
            /// Constant QUARTER for quarter
            /// </summary>
            public const string QUARTER = "quarter";
            /// <summary>
            /// Constant SECOND for second
            /// </summary>
            public const string SECOND = "second";
            /// <summary>
            /// Constant WEEK for week
            /// </summary>
            public const string WEEK = "week";
            /// <summary>
            /// Constant WEEKEND for weekend
            /// </summary>
            public const string WEEKEND = "weekend";
            /// <summary>
            /// Constant YEAR for year
            /// </summary>
            public const string YEAR = "year";
            
        }

        /// <summary>
        /// The precision or duration of a time range specified by a recognized `@sys-time` or `@sys-date` entity.
        /// Constants for possible values can be found using RuntimeEntityInterpretation.GranularityValue
        /// </summary>
        [JsonProperty("granularity", NullValueHandling = NullValueHandling.Ignore)]
        public string Granularity { get; set; }
        /// <summary>
        /// The calendar used to represent a recognized date (for example, `Gregorian`).
        /// </summary>
        [JsonProperty("calendar_type", NullValueHandling = NullValueHandling.Ignore)]
        public string CalendarType { get; set; }
        /// <summary>
        /// A unique identifier used to associate a recognized time and date. If the user input contains a date and time
        /// that are mentioned together (for example, `Today at 5`, the same **datetime_link** value is returned for
        /// both the `@sys-date` and `@sys-time` entities).
        /// </summary>
        [JsonProperty("datetime_link", NullValueHandling = NullValueHandling.Ignore)]
        public string DatetimeLink { get; set; }
        /// <summary>
        /// A locale-specific holiday name (such as `thanksgiving` or `christmas`). This property is included when a
        /// `@sys-date` entity is recognized based on a holiday name in the user input.
        /// </summary>
        [JsonProperty("festival", NullValueHandling = NullValueHandling.Ignore)]
        public string Festival { get; set; }
        /// <summary>
        /// A unique identifier used to associate multiple recognized `@sys-date`, `@sys-time`, or `@sys-number`
        /// entities that are recognized as a range of values in the user's input (for example, `from July 4 until July
        /// 14` or `from 20 to 25`).
        /// </summary>
        [JsonProperty("range_link", NullValueHandling = NullValueHandling.Ignore)]
        public string RangeLink { get; set; }
        /// <summary>
        /// The word in the user input that indicates that a `sys-date` or `sys-time` entity is part of an implied range
        /// where only one date or time is specified (for example, `since` or `until`).
        /// </summary>
        [JsonProperty("range_modifier", NullValueHandling = NullValueHandling.Ignore)]
        public string RangeModifier { get; set; }
        /// <summary>
        /// A recognized mention of a relative day, represented numerically as an offset from the current date (for
        /// example, `-1` for `yesterday` or `10` for `in ten days`).
        /// </summary>
        [JsonProperty("relative_day", NullValueHandling = NullValueHandling.Ignore)]
        public float? RelativeDay { get; set; }
        /// <summary>
        /// A recognized mention of a relative month, represented numerically as an offset from the current month (for
        /// example, `1` for `next month` or `-3` for `three months ago`).
        /// </summary>
        [JsonProperty("relative_month", NullValueHandling = NullValueHandling.Ignore)]
        public float? RelativeMonth { get; set; }
        /// <summary>
        /// A recognized mention of a relative week, represented numerically as an offset from the current week (for
        /// example, `2` for `in two weeks` or `-1` for `last week).
        /// </summary>
        [JsonProperty("relative_week", NullValueHandling = NullValueHandling.Ignore)]
        public float? RelativeWeek { get; set; }
        /// <summary>
        /// A recognized mention of a relative date range for a weekend, represented numerically as an offset from the
        /// current weekend (for example, `0` for `this weekend` or `-1` for `last weekend`).
        /// </summary>
        [JsonProperty("relative_weekend", NullValueHandling = NullValueHandling.Ignore)]
        public float? RelativeWeekend { get; set; }
        /// <summary>
        /// A recognized mention of a relative year, represented numerically as an offset from the current year (for
        /// example, `1` for `next year` or `-5` for `five years ago`).
        /// </summary>
        [JsonProperty("relative_year", NullValueHandling = NullValueHandling.Ignore)]
        public float? RelativeYear { get; set; }
        /// <summary>
        /// A recognized mention of a specific date, represented numerically as the date within the month (for example,
        /// `30` for `June 30`.).
        /// </summary>
        [JsonProperty("specific_day", NullValueHandling = NullValueHandling.Ignore)]
        public float? SpecificDay { get; set; }
        /// <summary>
        /// A recognized mention of a specific day of the week as a lowercase string (for example, `monday`).
        /// </summary>
        [JsonProperty("specific_day_of_week", NullValueHandling = NullValueHandling.Ignore)]
        public string SpecificDayOfWeek { get; set; }
        /// <summary>
        /// A recognized mention of a specific month, represented numerically (for example, `7` for `July`).
        /// </summary>
        [JsonProperty("specific_month", NullValueHandling = NullValueHandling.Ignore)]
        public float? SpecificMonth { get; set; }
        /// <summary>
        /// A recognized mention of a specific quarter, represented numerically (for example, `3` for `the third
        /// quarter`).
        /// </summary>
        [JsonProperty("specific_quarter", NullValueHandling = NullValueHandling.Ignore)]
        public float? SpecificQuarter { get; set; }
        /// <summary>
        /// A recognized mention of a specific year (for example, `2016`).
        /// </summary>
        [JsonProperty("specific_year", NullValueHandling = NullValueHandling.Ignore)]
        public float? SpecificYear { get; set; }
        /// <summary>
        /// A recognized numeric value, represented as an integer or double.
        /// </summary>
        [JsonProperty("numeric_value", NullValueHandling = NullValueHandling.Ignore)]
        public float? NumericValue { get; set; }
        /// <summary>
        /// The type of numeric value recognized in the user input (`integer` or `rational`).
        /// </summary>
        [JsonProperty("subtype", NullValueHandling = NullValueHandling.Ignore)]
        public string Subtype { get; set; }
        /// <summary>
        /// A recognized term for a time that was mentioned as a part of the day in the user's input (for example,
        /// `morning` or `afternoon`).
        /// </summary>
        [JsonProperty("part_of_day", NullValueHandling = NullValueHandling.Ignore)]
        public string PartOfDay { get; set; }
        /// <summary>
        /// A recognized mention of a relative hour, represented numerically as an offset from the current hour (for
        /// example, `3` for `in three hours` or `-1` for `an hour ago`).
        /// </summary>
        [JsonProperty("relative_hour", NullValueHandling = NullValueHandling.Ignore)]
        public float? RelativeHour { get; set; }
        /// <summary>
        /// A recognized mention of a relative time, represented numerically as an offset in minutes from the current
        /// time (for example, `5` for `in five minutes` or `-15` for `fifteen minutes ago`).
        /// </summary>
        [JsonProperty("relative_minute", NullValueHandling = NullValueHandling.Ignore)]
        public float? RelativeMinute { get; set; }
        /// <summary>
        /// A recognized mention of a relative time, represented numerically as an offset in seconds from the current
        /// time (for example, `10` for `in ten seconds` or `-30` for `thirty seconds ago`).
        /// </summary>
        [JsonProperty("relative_second", NullValueHandling = NullValueHandling.Ignore)]
        public float? RelativeSecond { get; set; }
        /// <summary>
        /// A recognized specific hour mentioned as part of a time value (for example, `10` for `10:15 AM`.).
        /// </summary>
        [JsonProperty("specific_hour", NullValueHandling = NullValueHandling.Ignore)]
        public float? SpecificHour { get; set; }
        /// <summary>
        /// A recognized specific minute mentioned as part of a time value (for example, `15` for `10:15 AM`.).
        /// </summary>
        [JsonProperty("specific_minute", NullValueHandling = NullValueHandling.Ignore)]
        public float? SpecificMinute { get; set; }
        /// <summary>
        /// A recognized specific second mentioned as part of a time value (for example, `30` for `10:15:30 AM`.).
        /// </summary>
        [JsonProperty("specific_second", NullValueHandling = NullValueHandling.Ignore)]
        public float? SpecificSecond { get; set; }
        /// <summary>
        /// A recognized time zone mentioned as part of a time value (for example, `EST`).
        /// </summary>
        [JsonProperty("timezone", NullValueHandling = NullValueHandling.Ignore)]
        public string Timezone { get; set; }
    }
}
