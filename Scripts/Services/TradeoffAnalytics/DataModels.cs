/**
* Copyright 2015 IBM Corp. All Rights Reserved.
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

namespace IBM.Watson.DeveloperCloud.Services.TradeoffAnalytics.v1
{
   
    /// <summary>
    /// This object for response of Dilemma service
    /// </summary>
    [fsObject]
    public class DilemmasResponse
    {
        public Problem problem { get; set; }
        public Resolution resolution { get; set; }
    }

	/// <summary>
	/// Problem class for Dilema service.
	/// </summary>
    [fsObject]
    public class Problem
    {
        public Column[] columns { get; set; }
        public Option[] options { get; set; }
        public string subject { get; set; }
    }

    /// <summary>
    /// Column class for Problem.
    /// </summary>
    [fsObject]
    public class Column
    {
        public string key { get; set; }
        public string type{ get; set; }
        public string description { get; set; }
        public string format { get; set; }
        public string full_name { get; set; }
        public string goal { get; set; }
        public int insignificant_loss { get; set; }
        public bool is_objective { get; set; }
        public string[] preference { get; set; }
        public Range range { get; set; }
        public string[] categoricalRange { get; set; }
        public long significant_gain{ get; set; }
        public long significant_loss{ get; set; }
    }

    /// <summary>
    /// Range class for Column.
    /// </summary>
    [fsObject]
    public class Range {};

    /// <summary>
    /// Categorical range.
    /// </summary>
    [fsObject]
    public class CategoricalRange : Range
    {
        public string[] keys { get; set; }
    }

    /// <summary>
    /// Date range.
    /// </summary>
    [fsObject]
    public class DateRange : Range
    {
        public string low { get; set; }
        public string high { get; set; }
    }

    /// <summary>
    /// Value range.
    /// </summary>
    [fsObject]
    public class ValueRange : Range
    {
        public double low { get; set; }
        public double high { get; set; }
    }

    /// <summary>
    /// Option class for Problem.
    /// </summary>
    [fsObject]
    public class Option
    {
        public ApplicationData app_data { get; set; }
        public string description_html { get; set; }
        public string key { get; set; }
        public string name { get; set; }
        public ApplicationDataValue values { get; set; }

    }

    /// <summary>
    /// Application data class for Options. Extend this for your application.
    /// </summary>
    [fsObject]
    public class ApplicationData {}

    /// <summary>
    /// Application data value for Options. Extend this for your application.
    /// </summary>
    [fsObject]
    public class ApplicationDataValue {}

    /// <summary>
    /// Resolution.
    /// </summary>
    [fsObject]
    public class Resolution
    {
        public Map map { get; set; }
        public Solution[] solutions { get; set; }
    }

    /// <summary>
    /// Map class for the resolution.
    /// </summary>
    [fsObject]
    public class Map
    {
        public Anchor[] anchors { get; set; }
        public string comments{ get; set; }
        public Config config{ get; set; }
        public Node[] nodes{ get; set; }
    }

    /// <summary>
    /// Anchor class for the Map.
    /// </summary>
    [fsObject]
    public class Anchor
    {
        public string name { get; set; }
        public Position position { get; set; }
    }

    /// <summary>
    /// Position class for the Anchor.
    /// </summary>
    [fsObject]
    public class Position
    {
        public double x { get; set; }
        public double y { get; set; }
    }

    /// <summary>
    /// Config class for the Map.
    /// </summary>
    [fsObject]
    public class Config
    {
        public Drivers drivers { get; set; }
        public Params @params { get; set; }
    }

    /// <summary>
    /// Drivers class for the Config.
    /// </summary>
    [fsObject]
    public class Drivers
    {
        public double alpha_init { get; set; }
        public double data_multiplier { get; set; }
        public double max_map_size { get; set; }
        public double r_anchor_init { get; set; }
        public double r_fin { get; set; }
        public double r_init { get; set; }
        public double training_anchors { get; set; }
        public double training_length { get; set; }
    }

    /// <summary>
    /// Params class for the Config.
    /// </summary>
    [fsObject]
    public class Params
    {
        public double alpha_init { get; set; }
        public double anchor_epoch { get; set; }
        public double map_size { get; set; }
        public double rAnchor { get; set; }
        public double rFinish { get; set; }
        public double rInit { get; set; }
        public double seed { get; set; }
        public double training_period { get; set; }
    }

    /// <summary>
    /// Metrics class for the resolution.
    /// </summary>
    [fsObject]
    public class Metrics
    {
        public double final_kappa { get; set; }
        public double kappa { get; set; }
    }

    /// <summary>
    /// Node class for the resolution
    /// </summary>
    [fsObject]
    public class Node
    {
        public Position coordinates { get; set; }
        public string[] solution_refs { get; set; }
    }

    /// <summary>
    /// Solution class for the resolution.
    /// </summary>
    [fsObject]
    public class Solution
    {
        public string[] shadow_me { get; set; }
        public string[] shadows { get; set; }
        public string solution_ref { get; set; }
        public string status { get; set; }
        public StatusCause status_cause { get; set; }
    }

    /// <summary>
    /// Status cause class for the solution.
    /// </summary>
    [fsObject]
    public class StatusCause
    {
        public string error_code { get; set; }
        public string message { get; set; }
        public string[] tokens { get; set; }
    }
}
