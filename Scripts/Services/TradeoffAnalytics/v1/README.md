# Tradeoff Analytics

**NOTE:** The IBM Watson™ Tradeoff Analytics service is being deprecated. As of May 11, 2017, it will no longer be possible to create a new instance of the service on Bluemix. Existing service instances will continue to be supported until April 11, 2018.

The [Tradeoff Analytics][tradeoff_analytics] service helps people make better decisions when faced with multiple, sometimes conflicting, goals and alternatives.

## Usage
The IBM Watson™ Tradeoff Analytics service helps people make better choices when faced with a decision problem that includes multiple, often conflicting, goals and alternatives. By using mathematical filtering techniques to identify the top options based on different criteria, the service can help users explore the trade-offs between options to make complex decisions. The service also offers smart visualization via a JavaScript library for intuitive graphical exploration of trade-offs.

### Instantiating and authenticating the service
Before you can send requests to the service it must be instantiated and credentials must be set.
```cs
using IBM.Watson.DeveloperCloud.Services.TradeoffAnalytics.v1;
using IBM.Watson.DeveloperCloud.Utilities;

void Start()
{
    Credentials credentials = new Credentials(<username>, <password>, <url>);
    TradeoffAnalytics _tradeoffAnalytics = new TradeoffAnalytics(credentials);
}
```

### Get dillema
Returns a dilemma that contains the problem and a resolution. The problem contains a set of options and objectives. The resolution contains a set of optimal options, their analytical characteristics, and by default their representation on a two-dimensional space. You can optionally request that the service also return a refined set of preferable options that are most likely to appeal to the greatest number of users.
```cs
private void GetDillema()
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
  columnBrandName.preference = new string[]{"Samsung", "Apple", "HTC"};
  columnBrandName.range = new CategoricalRange();
  ((CategoricalRange)columnBrandName.range).keys = new string[]{"Samsung", "Apple", "HTC"};

  listColumn.Add(columnPrice);
  listColumn.Add(columnWeight);

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

  if(!_tradeoffAnalytics.GetDilemma(OnGetDilemma, OnFail, problemToSolve, false))
    Log.Debug("ExampleTradeoffAnalytics.GetDilemma()", "Failed to get dillema!");
}

private void OnGetDillema(DilemmasResponse resp, Dictionary<string, object> customData)
{
  Log.Debug("ExampleTradeoffAnalytics.OnGetDillema()", "Get dillema result: {0}", customData["json"].ToString());
}

private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
{
    Log.Error("ExampleTradeoffAnalytics.OnFail()", "Error received: {0}", error.ToString());
}

public class TestDataValue : IBM.Watson.DeveloperCloud.Services.TradeoffAnalytics.v1.ApplicationDataValue
{
  public double price { get; set; }
  public double weight { get; set; }
  public string brand { get; set; }
}
```

[tradeoff_analytics]: https://console.bluemix.net/docs/services/tradeoff-analytics/index.html
