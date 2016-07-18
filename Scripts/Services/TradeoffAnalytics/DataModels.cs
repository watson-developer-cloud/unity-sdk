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
		/// <summary>
		/// The problem.
		/// </summary>
        public Problem problem { get; set; }
		/// <summary>
		/// The resolution.
		/// </summary>
        public Resolution resolution { get; set; }
    }

	/// <summary>
	/// Problem class for Dilema service.
	/// </summary>
    [fsObject]
    public class Problem
    {
		/// <summary>
		/// The problem columns.
		/// </summary>
        public Column[] columns { get; set; }
		/// <summary>
		/// The problem options.
		/// </summary>
        public Option[] options { get; set; }
		/// <summary>
		/// The problem subject.
		/// </summary>
        public string subject { get; set; }
    }

    /// <summary>
    /// Column class for Problem.
    /// </summary>
    [fsObject]
    public class Column
    {
		/// <summary>
		/// The key.
		/// </summary>
        public string key { get; set; }
		/// <summary>
		/// The type.
		/// </summary>
        public string type{ get; set; }
		/// <summary>
		/// The description.
		/// </summary>
        public string description { get; set; }
		/// <summary>
		/// The format.
		/// </summary>
        public string format { get; set; }
		/// <summary>
		/// The full name.
		/// </summary>
        public string full_name { get; set; }
		/// <summary>
		/// The goal.
		/// </summary>
        public string goal { get; set; }
		/// <summary>
		/// The insignificant loss.
		/// </summary>
        public int insignificant_loss { get; set; }
		/// <summary>
		/// Weather or not the column is the objective.
		/// </summary>
        public bool is_objective { get; set; }
		/// <summary>
		/// The column preferences.
		/// </summary>
        public string[] preference { get; set; }
		/// <summary>
		/// The range.
		/// </summary>
        public Range range { get; set; }
		/// <summary>
		/// The signficant gain.
		/// </summary>
        public long significant_gain{ get; set; }
		/// <summary>
		/// The significant loss.
		/// </summary>
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
		/// <summary>
		/// The categorical range keys.
		/// </summary>
        public string[] keys { get; set; }
    }

    /// <summary>
    /// Date range.
    /// </summary>
    [fsObject]
    public class DateRange : Range
    {
		/// <summary>
		/// The date range low value.
		/// </summary>
        public string low { get; set; }
		/// <summary>
		/// The date range high value.
		/// </summary>
        public string high { get; set; }
    }

    /// <summary>
    /// Value range.
    /// </summary>
    [fsObject]
    public class ValueRange : Range
    {
		/// <summary>
		/// The value range low value.
		/// </summary>
        public double low { get; set; }
		/// <summary>
		/// The value range high value.
		/// </summary>
        public double high { get; set; }
    }

    /// <summary>
    /// Option class for Problem.
    /// </summary>
    [fsObject]
    public class Option
    {
		/// <summary>
		/// The application data.
		/// </summary>
        public ApplicationData app_data { get; set; }
		/// <summary>
		/// The description.
		/// </summary>
        public string description_html { get; set; }
		/// <summary>
		/// The key.
		/// </summary>
        public string key { get; set; }
		/// <summary>
		/// THe name.
		/// </summary>
        public string name { get; set; }
		/// <summary>
		/// The application data values.
		/// </summary>
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
		/// <summary>
		/// The resolution map.
		/// </summary>
        public Map map { get; set; }
		/// <summary>
		/// The resolution solutions.
		/// </summary>
        public Solution[] solutions { get; set; }
    }

    /// <summary>
    /// Map class for the resolution.
    /// </summary>
    [fsObject]
    public class Map
    {
		/// <summary>
		/// The map anchors.
		/// </summary>
        public Anchor[] anchors { get; set; }
		/// <summary>
		/// The map comments.
		/// </summary>
        public string comments{ get; set; }
		/// <summary>
		/// The map config.
		/// </summary>
        public Config config{ get; set; }
		/// <summary>
		/// The map nodes.
		/// </summary>
        public Node[] nodes{ get; set; }
    }

    /// <summary>
    /// Anchor class for the Map.
    /// </summary>
    [fsObject]
    public class Anchor
    {
		/// <summary>
		/// The anchor name.
		/// </summary>
        public string name { get; set; }
		/// <summary>
		/// The anchor position.
		/// </summary>
        public Position position { get; set; }
    }

    /// <summary>
    /// Position class for the Anchor.
    /// </summary>
    [fsObject]
    public class Position
    {
		/// <summary>
		/// The X position.
		/// </summary>
        public double x { get; set; }
		/// <summary>
		/// The Y position.
		/// </summary>
        public double y { get; set; }
    }

    /// <summary>
    /// Config class for the Map.
    /// </summary>
    [fsObject]
    public class Config
    {
		/// <summary>
		/// The config drivers.
		/// </summary>
        public Drivers drivers { get; set; }
		/// <summary>
		/// The config parameters.
		/// </summary>
        public Params @params { get; set; }
    }

    /// <summary>
    /// Drivers class for the Config.
    /// </summary>
    [fsObject]
    public class Drivers
    {
		/// <summary>
		/// The drivers alpha init.
		/// </summary>
        public double alpha_init { get; set; }
		/// <summary>
		/// THe drivers data multiplier.
		/// </summary>
        public double data_multiplier { get; set; }
		/// <summary>
		/// The drivers maximum map size.
		/// </summary>
        public double max_map_size { get; set; }
		/// <summary>
		/// The drivers anchor init.
		/// </summary>
        public double r_anchor_init { get; set; }
		/// <summary>
		/// The drivers fin.
		/// </summary>
        public double r_fin { get; set; }
		/// <summary>
		/// The drivers init.
		/// </summary>
        public double r_init { get; set; }
		/// <summary>
		/// The drivers training anchors.
		/// </summary>
        public double training_anchors { get; set; }
		/// <summary>
		/// The drivers training length.
		/// </summary>
        public double training_length { get; set; }
    }

    /// <summary>
    /// Params class for the Config.
    /// </summary>
    [fsObject]
    public class Params
    {
		/// <summary>
		/// The parameters alpha init.
		/// </summary>
        public double alpha_init { get; set; }
		/// <summary>
		/// The drivers anchor epoch.
		/// </summary>
        public double anchor_epoch { get; set; }
		/// <summary>
		/// The drivers map size.
		/// </summary>
        public double map_size { get; set; }
		/// <summary>
		/// The drivers anchor.
		/// </summary>
        public double rAnchor { get; set; }
		/// <summary>
		/// The drivers finish.
		/// </summary>
        public double rFinish { get; set; }
		/// <summary>
		/// The drivers init.
		/// </summary>
        public double rInit { get; set; }
		/// <summary>
		/// The drivers seed.
		/// </summary>
        public double seed { get; set; }
		/// <summary>
		/// The drivers training period.
		/// </summary>
        public double training_period { get; set; }
    }

    /// <summary>
    /// Metrics class for the resolution.
    /// </summary>
    [fsObject]
    public class Metrics
    {
		/// <summary>
		/// The metrtics final kappa.
		/// </summary>
        public double final_kappa { get; set; }
		/// <summary>
		/// The metrics kappa.
		/// </summary>
        public double kappa { get; set; }
    }

    /// <summary>
    /// Node class for the resolution
    /// </summary>
    [fsObject]
    public class Node
    {
		/// <summary>
		/// The node coordinates.
		/// </summary>
        public Position coordinates { get; set; }
		/// <summary>
		/// The nodes solution references.
		/// </summary>
        public string[] solution_refs { get; set; }
    }

    /// <summary>
    /// Solution class for the resolution.
    /// </summary>
    [fsObject]
    public class Solution
    {
		/// <summary>
		/// The solution' shadow mes
		/// </summary>
        public string[] shadow_me { get; set; }
		/// <summary>
		/// The solution' shadows.
		/// </summary>
        public string[] shadows { get; set; }
		/// <summary>
		/// The solution' reference.
		/// </summary>
        public string solution_ref { get; set; }
		/// <summary>
		/// The solution's status.
		/// </summary>
        public string status { get; set; }
		/// <summary>
		/// The solution's status cause.
		/// </summary>
        public StatusCause status_cause { get; set; }
    }

    /// <summary>
    /// Status cause class for the solution.
    /// </summary>
    [fsObject]
    public class StatusCause
    {
		/// <summary>
		/// The status cause's error code.
		/// </summary>
        public string error_code { get; set; }
		/// <summary>
		/// The status cause's message.
		/// </summary>
        public string message { get; set; }
		/// <summary>
		/// The status cause's tokens.
		/// </summary>
        public string[] tokens { get; set; }
    }
}
