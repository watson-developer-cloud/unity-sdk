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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.Services.TradeoffAnalytics.v1;

public class ExampleTradeoffAnalytics : MonoBehaviour
{
  TradeoffAnalytics m_TradeoffAnalytics = new TradeoffAnalytics();

  void Start()
  {
    Problem problemToSolve = new Problem();
    problemToSolve.subject = "Test Subject";

    List<Column> listColumn = new List<Column>();
    Column columnPrice = new Column();
    columnPrice.description = "Price Column to minimize";
    columnPrice.range = new ValueRange();
    ((ValueRange)columnPrice.range).high = 600;
    ((ValueRange)columnPrice.range).low = 0;
    columnPrice.type = "numeric";
    columnPrice.key = "price";
    columnPrice.full_name = "Price";
    columnPrice.goal = "min";
    columnPrice.is_objective = true;
    columnPrice.format = "$####0.00";

    Column columnWeight = new Column();
    columnWeight.description = "Weight Column to minimize";
    columnWeight.type = "numeric";
    columnWeight.key = "weight";
    columnWeight.full_name = "Weight";
    columnWeight.goal = "min";
    columnWeight.is_objective = true;
    columnWeight.format = "####0 g";

    Column columnBrandName = new Column();
    columnBrandName.description = "All Brand Names";
    columnBrandName.type = "categorical";
    columnBrandName.key = "brand";
    columnBrandName.full_name = "Brand";
    columnBrandName.goal = "max";
    columnBrandName.is_objective = true;
    columnBrandName.preference = new string[] { "Samsung", "Apple", "HTC" };
    columnBrandName.range = new CategoricalRange();
    ((CategoricalRange)columnBrandName.range).keys = new string[] { "Samsung", "Apple", "HTC" };

    listColumn.Add(columnPrice);
    listColumn.Add(columnWeight);
    //            listColumn.Add(columnBrandName);

    problemToSolve.columns = listColumn.ToArray();


    List<Option> listOption = new List<Option>();

    Option option1 = new Option();
    option1.key = "1";
    option1.name = "Samsung Galaxy S4";
    option1.values = new TestDataValue();
    (option1.values as TestDataValue).weight = 130;
    (option1.values as TestDataValue).brand = "Samsung";
    (option1.values as TestDataValue).price = 249;
    listOption.Add(option1);

    Option option2 = new Option();
    option2.key = "2";
    option2.name = "Apple iPhone 5";
    option2.values = new TestDataValue();
    (option2.values as TestDataValue).weight = 112;
    (option2.values as TestDataValue).brand = "Apple";
    (option2.values as TestDataValue).price = 599;
    listOption.Add(option2);

    Option option3 = new Option();
    option3.key = "3";
    option3.name = "HTC One";
    option3.values = new TestDataValue();
    (option3.values as TestDataValue).weight = 143;
    (option3.values as TestDataValue).brand = "HTC";
    (option3.values as TestDataValue).price = 299;
    listOption.Add(option3);

    problemToSolve.options = listOption.ToArray();

    m_TradeoffAnalytics.GetDilemma(OnGetDilemma, problemToSolve, false);
  }

  private void OnGetDilemma(DilemmasResponse resp)
  {
    Debug.Log("Response: " + resp);
  }

  /// <summary>
  /// Application data value.
  /// </summary>
  public class TestDataValue : IBM.Watson.DeveloperCloud.Services.TradeoffAnalytics.v1.ApplicationDataValue
  {
    public double price { get; set; }
    public double weight { get; set; }
    public string brand { get; set; }
  }

  /// <summary>
  /// Application data.
  /// </summary>
  public class TestData : IBM.Watson.DeveloperCloud.Services.TradeoffAnalytics.v1.ApplicationData
  {

  }
}
