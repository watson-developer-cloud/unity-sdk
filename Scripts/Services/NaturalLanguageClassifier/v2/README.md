# Natural Language Classifier

Use [Natural Language Classifier][natural_language_classifier] service to create a classifier instance by providing a set of representative strings and a set of one or more correct classes for each as training. Then use the trained classifier to classify your new question for best matching answers or to retrieve next actions for your application.

## Usage
Classify intents in natural language.

### Instantiating and authenticating the service
Before you can send requests to the service it must be instantiated and credentials must be set.
```cs
using IBM.Watson.DeveloperCloud.Services.NaturalLanguageClassifier.v2;
using IBM.Watson.DeveloperCloud.Utilities;

void Start()
{
    Credentials credentials = new Credentials(<username>, <password>, <url>);
    NaturalLanguageClassifier _naturalLanguageClassifier = new NaturalLanguageClassifier(credentials);
}
```

### Listing Classifiers
Returns an empty array if no classifiers are available.
```cs
private void GetClassifiers()
{
  if (!naturalLanguageClassifier.GetClassifiers(OnGetClassifiers))
    Log.Debug("ExampleNaturalLanguageClassifier", "Failed to get classifiers!");
}

private void OnGetClassifiers(Classifiers classifiers, string data)
{
  Log.Debug("ExampleNaturalLanguageClassifier", "Natural Language Classifier - GetClassifiers  Response: {0}", data);
}
```




### Classifying Text
The status must be Available before you can use the classifier to classify calls. Use GET /classifiers/{classifier_id} to retrieve the status.
```cs
private void Classify()
{
  if (!naturalLanguageClassifier.Classify(<classifier-id>, <input>, OnClassify))
    Log.Debug("ExampleNaturalLanguageClassifier", "Failed to classify!");
}

private void OnClassify(ClassifyResult result, string data)
{
    Log.Debug("ExampleNaturalLanguageClassifier", "Natural Language Classifier - Classify Response: {0}", data);
}
```




### Training A New Classifier
Sends data to create and train a classifier and returns information about the new classifier.
```cs
private void TrainClassifier()
{
  if (!naturalLanguageClassifier.TrainClassifier(<classifier-name>, <classifier-language>, <training-data>, OnTrainClassifier))
    Log.Debug("ExampleNaturalLanguageClassifier", "Failed to train clasifier!");
}

private void OnTrainClassifier(Classifier classifier, string data)
{
    Log.Debug("ExampleNaturalLanguageClassifier", "Natural Language Classifier - Train Classifier: {0}", data);
}
```



### Getting Information About A Classifier
Returns status and other information about a classifier
```cs
private void GetClassifier()
{
  if (!naturalLanguageClassifier.GetClassifier(<classifier-id>, OnGetClassifier))
    Log.Debug("ExampleNaturalLanguageClassifier", "Failed to get classifier {0}!", classifierId);
}

private void OnGetClassifier(Classifier classifier, string data)
{
    Log.Debug("ExampleNaturalLanguageClassifier", "Natural Language Classifier - Get Classifier {0}: {1}", classifier.classifier_id, data);
}
```




### Deleting A Classifier
Deletes a classifier
```cs
private void DeleteClassifier()
{
  if (!naturalLanguageClassifier.DeleteClassifer(<classifier-id>, OnDeleteTrainedClassifier))
    Log.Debug("ExampleNaturalLanguageClassifier", "Failed to delete clasifier {0}!", <classifier-id>);
}

private void OnDeleteTrainedClassifier(bool success, string data)
{
    Log.Debug("ExampleNaturalLanguageClassifier", "Natural Language Classifier - Delete Trained Classifier {0} | success: {1} {2}", <classifier-id>, success, data);
}
```


[natural_language_classifier]: https://www.ibm.com/watson/services/natural-language-classifier/
