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
        public Problem problem { get; set;}
        public Resolution resolution { get; set;}
    };

    [fsObject]
    public class Problem
    {
        
        public Column[] columns { get; set;}
        public Option[] options { get; set;}
        public string subject { get; set;}
    };

    [fsObject]
    public class Column
    {
        public string description { get; set;}
        public string format { get; set;}
        public string full_name { get; set;}
        public string goal { get; set;}
        public int insignificant_loss { get; set;}
        public bool is_objective { get; set;}
        public string key { get; set;}
        public string[] preference { get; set;}
        public Range range { get; set;}
        public long significant_gain{ get; set;}
        public long significant_loss{ get; set;}
        public string type{ get; set;}
    };

    [fsObject]
    public class Range
    {
        
    };

    [fsObject]
    public class CategoricalRange : Range
    {
        public string[] keys { get; set;}
    }

    [fsObject]
    public class DateRange : Range
    {
        public string low { get; set;}
        public string high { get; set;}
    }

    [fsObject]
    public class ValueRange : Range
    {
        public double low { get; set;}
        public double high { get; set;}
    }

    [fsObject]
    public class Option
    {
        public ApplicationData app_data { get; set;}
        public string description_html { get; set;}
        public string key { get; set;}
        public string name { get; set;}
        public ApplicationDataValue values { get; set;}

    }


    //TODO: Needs to be fileed per application!
    [fsObject]
    public class ApplicationData
    {
        
    }

    //TODO: Needs to be fileed per application!
    [fsObject]
    public class ApplicationDataValue
    {
        public double price { get; set;}
        public double weight { get; set;}
        public string brand { get; set;}
    }


    [fsObject]
    public class Resolution
    {
        public Map map { get; set;}
        public Solution[] solutions { get; set;}
    }

    [fsObject]
    public class Map
    {
        public Anchor[] anchors { get; set;}
        public string comments{ get; set;}
        public Config config{ get; set;}
        public Node[] nodes{ get; set;}
    }

    [fsObject]
    public class Anchor
    {
        public string name { get; set;}
        public Position position { get; set;}
    }

    [fsObject]
    public class Position
    {
        public double x { get; set;}
        public double y { get; set;}
    }

    [fsObject]
    public class Config
    {
        public Drivers drivers { get; set;}
        public Params @params { get; set;}
    }

    [fsObject]
    public class Drivers
    {
        public double alpha_init { get; set;}
        public double data_multiplier { get; set;}
        public double max_map_size { get; set;}
        public double r_anchor_init { get; set;}
        public double r_fin { get; set;}
        public double r_init { get; set;}
        public double training_anchors { get; set;}
        public double training_length { get; set;}
    }

    [fsObject]
    public class Params
    {
        public double alpha_init { get; set;}
        public double anchor_epoch { get; set;}
        public double map_size { get; set;}
        public double rAnchor { get; set;}
        public double rFinish { get; set;}
        public double rInit { get; set;}
        public double seed { get; set;}
        public double training_period { get; set;}
    }

    [fsObject]
    public class Metrics
    {
        public double final_kappa { get; set;}
        public double kappa { get; set;}
    }

    [fsObject]
    public class Node
    {
        public Position coordinates { get; set;}
        public string[] solution_refs { get; set;}
    }

    [fsObject]
    public class Solution
    {
        public string[] shadow_me { get; set;}
        public string[] shadows { get; set;}
        public string solution_ref { get; set;}
        public string status { get; set;}
        public StatusCause status_cause { get; set;}
    }

    [fsObject]
    public class StatusCause
    {
        public string error_code { get; set;}
        public string message { get; set;}
        public string[] tokens { get; set;}
    }

}


/*

{
  "problem": {
    "columns": [
      {
        "description": "string",
        "format": "string",
        "full_name": "string",
        "goal": "min",
        "insignificant_loss": 0,
        "is_objective": false,
        "key": "string",
        "preference": [
          "string"
        ],
        "range": {
          "CategoricalRange": {
            "keys": [
              "string"
            ]
          },
          "DateRange": {
            "low": "string",
            "high": "string"
          },
          "ValueRange": {
            "low": 0,
            "high": 0
          }
        },
        "significant_gain": 0,
        "significant_loss": 0,
        "type": "categorical"
      }
    ],
    "options": [
      {
        "app_data": {},
        "description_html": "string",
        "key": "string",
        "name": "string",
        "values": {}
      }
    ],
    "subject": "string"
  },
  "resolution": {
    "map": {
      "anchors": [
        {
          "name": "string",
          "position": {
            "x": 0,
            "y": 0
          }
        }
      ],
      "comments": "string",
      "config": {
        "drivers": {
          "alpha_init": 0,
          "data_multiplier": 0,
          "max_map_size": 0,
          "r_anchor_init": 0,
          "r_fin": 0,
          "r_init": 0,
          "training_anchors": 0,
          "training_length": 0
        },
        "params": {
          "alpha_init": 0,
          "anchor_epoch": 0,
          "map_size": 0,
          "rAnchor": 0,
          "rFinish": 0,
          "rInit": 0,
          "seed": 0,
          "training_period": 0
        }
      },
      "metrics": {
        "final_kappa": 0,
        "kappa": 0
      },
      "nodes": [
        {
          "coordinates": {
            "x": 0,
            "y": 0
          },
          "solution_refs": [
            "string"
          ]
        }
      ]
    },
    "solutions": [
      {
        "shadow_me": [
          "string"
        ],
        "shadows": [
          "string"
        ],
        "solution_ref": "string",
        "status": "front",
        "status_cause": {
          "error_code": "missing_objective_value",
          "message": "string",
          "tokens": [
            "string"
          ]
        }
      }
    ]
  }
}

*/