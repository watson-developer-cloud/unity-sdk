# Visual Recognition
The IBM Watson™ [Visual Recognition][visual-recognition] service uses deep learning algorithms to identify scenes, objects, and celebrity faces in images you upload to the service. You can create and train a custom classifier to identify subjects that suit your needs.

## Usage
The IBM Watson™ [Visual Recognition][visual-recognition] service uses deep learning algorithms to identify scenes, objects, and faces in images you upload to the service. You can create and train a custom classifier to identify subjects that suit your needs. You can create and add images to a collection and then search that collection with your own image to find similar images. A valid API Key from Bluemix is required for all calls.

### Instantiating and authenticating the service
Before you can send requests to the service it must be instantiated and api key must be set.
```cs
using IBM.Watson.DeveloperCloud.Services.VisualRecognition.v3;
using IBM.Watson.DeveloperCloud.Utilities;

void Start()
{
    Credentials credentials = new Credentials("<apikey>", "<url>");
    VisualRecognition _visualRecognition = new VisualRecognition(credentials);
}
```

### Fail handler
These examples use a common fail handler.
```cs
private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
{
    Log.Error("ExampleVisualRecognition.OnFail()", "Error received: {0}", error.ToString());
}
```

### Classify an image
Upload images or URLs to identify classes by default. To identify custom classifiers, include the classifier_ids or owners parameters. Images must be in .jpeg, or .png format.

For each image, the response includes a score for each class within each selected classifier. Scores range from 0 - 1 with a higher score indicating greater likelihood of the class being depicted in the image. The default threshold for reporting scores from a classifier is 0.5. We recommend an image that is a minimum of 224 x 224 pixels for best quality results.
```cs
void Classify()
{
    //  Classify using image url
	if(!_visualRecognition.Classify("<image-url>", OnClassify, OnFail))
    	Log.Debug("ExampleVisualRecognition.Classify()", "Classify image failed!");

    //  Classify using image path
    if(!_visualRecognition.Classify(OnClassify, OnFail, "<image-path>", "<classifier-owners>", "<classifier-ids>", 0.5f))
        Log.Debug("ExampleVisualRecognition.Classify()", "Classify image failed!");
}

private void OnClassify(ClassifyTopLevelMultiple classify, Dictionary<string, object> customData)
{
    Log.Debug("ExampleVisualRecognition.OnClassify()", "Classify result: {0}", customData["json"].ToString());
}
```

### Detect faces
Analyze faces in images and get data about them, such as estimated age, gender, plus names of celebrities. Images must be in .jpeg, or .png format. This functionality is not trainable, and does not support general biometric facial recognition.

For each image, the response includes face location, a minimum and maximum estimated age, a gender, and confidence scores. Scores range from 0 - 1 with a higher score indicating greater correlation.
```cs
void DetectFaces()
{
    //  Classify using image url
	if(!_visualRecognition.DetectFaces("<image-url>", OnDetectFaces, OnFail))
        Log.Debug("ExampleVisualRecognition.DetectFaces()", "Detect faces failed!");

    //  Classify using image path
    if(!_visualRecognition.DetectFaces(OnDetectFaces, OnFail, "<image-path>"))
        Log.Debug("ExampleVisualRecognition.DetectFaces()", "Detect faces failed!");
}

private void OnDetectFaces(FacesTopLevelMultiple multipleImages, Dictionary<string, object> customData)
{
    Log.Debug("ExampleVisualRecognition.OnDetectFaces()", "Detect faces result: {0}", customData["json"].ToString());
}
```

### Create a classifier
Train a new multi-faceted classifier on the uploaded image data. A new custom classifier can be trained by several compressed (.zip) files, including files containing positive or negative images (.jpg, or .png). You must supply at least two compressed files, either two positive example files or one positive and one negative example file.

Compressed files containing positive examples are used to create “classes” that define what the new classifier is. The prefix that you specify for each positive example parameter is used as the class name within the new classifier. The `_positive_examples` suffix is required. There is no limit on the number of positive example files you can upload in a single call.

The compressed file containing negative examples is not used to create a class within the created classifier, but does define what the new classifier is not. Negative example files should contain images that do not depict the subject of any of the positive examples. You can only specify one negative example file in a single call. For more information, see [Structure of the training data][structure-of-the-training-data], and [Guidelines for good training][guidelines-for-good-training].
```cs
void TrainClassifier()
{
    if(!_visualRecognition.TrainClassifier(OnTrainClassifier, OnFail, "<classifier-name>", "<class-name>", "<positive-examples-path>", "<negative-examples-path>"))
        Log.Debug("ExampleVisualRecognition.TrainClassifier()", "Train classifier failed!");
}

private void OnTrainClassifier(GetClassifiersPerClassifierVerbose classifier, Dictionary<string, object> customData)
{
    Log.Debug("ExampleVisualRecognition.OnTrainClassifier()", "Train classifier result: {0}", customData["json"].ToString());
}
```

### Retrieve a list of custom classifiers
Retrieve a list of user-created classifiers.
```cs
void GetClassifiers()
{
	if(!_visualRecognition.GetClassifiers(OnGetClassifiers, OnFail))
        Log.Debug("ExampleVisualRecognition.GetClassifiers()", "Getting classifiers failed!");
}

private void OnGetClassifiers(GetClassifiersTopLevelBrief classifiers, Dictionary<string, object> customData)
{
    Log.Debug("ExampleVisualRecognition.OnGetClassifiers()", "Get classifiers result: {0}", customData["json"].ToString());
}
```

### Retrieve classifier details
Retrieve information about a specific classifier.
```cs
void GetClassifier()
{
    if(!_visualRecognition.GetClassifier(OnGetClassifier, OnFail, "<classifier-id>"))
        Log.Debug("ExampleVisualRecognition.GetClassifier()", "Getting classifier failed!");
}

private void OnGetClassifier(GetClassifiersPerClassifierVerbose classifier, Dictionary<string, object> customData)
{
    Log.Debug("ExampleVisualRecognition.OnGetClassifier()", "Get classifier result: {0}", customData["json"].ToString());
}
```

### Update a classifier
Update an existing classifier by adding new classes, or by adding new images to existing classes.You cannot update a custom classifier with a free API Key.

To update the existing classifier, use several compressed (.zip) files, including files containing positive or negative images (.jpg, or .png). You must supply at least one compressed file, with additional positive or negative examples.

Compressed files containing positive examples are used to create and update “classes” to impact all of the classes in that classifier. The prefix that you specify for each positive example parameter is used as the class name within the new classifier. The `_positive_examples` suffix is required. There is no limit on the number of positive example files you can upload in a single call.

The compressed file containing negative examples is not used to create a class within the created classifier, but does define what the updated classifier is not. Negative example files should contain images that do not depict the subject of any of the positive examples. You can only specify one negative example file in a single call. For more information, see [Updating custom classifiers][updating-custom-classifiers].
```cs
private VisualRecognition _visualRecognition = new VisualRecognition();

void UpdateClassifier()
{
   if(!_visualRecognition.UpdateClassifier(OnUpdateClassifier, OnFail, "<classifier-id>", "<classifier-name>", "<class-name>", "<positive-examples-path>"))
        Log.Debug("ExampleVisualRecognition.UpdateClassifier()", "Update classifier failed!");
}

private void OnUpdateClassifier(GetClassifiersPerClassifierVerbose classifier, Dictionary<string, object> customData)
{
    Log.Debug("ExampleVisualRecognition.OnUpdateClassifier()", "Update classifier result: {0}", customData["json"].ToString());
}
```

### Delete a classifier
Delete a custom classifier with the specified classifier ID.
```cs
void DeleteClassifier()
{
    if(!_visualRecognition.DeleteClassifier(OnDeleteClassifier, OnFail, "<classifier-id>"))
        Log.Debug("ExampleVisualRecognition.DeleteClassifier()", "Deleting classifier failed!");
}

private void OnDeleteClassifier(bool success, Dictionary<string, object> customData)
{
    Log.Debug("ExampleVisualRecognition.OnDeleteClassifier()", "Update classifier result: {0}", customData["json"].ToString());
}
```

[visual-recognition]: https://www.ibm.com/watson/developercloud/visual-recognition/api/v3/
[structure-of-the-training-data]: https://console.bluemix.net/docs/services/visual-recognition/customizing.html#structure
[guidelines-for-good-training]: https://console.bluemix.net/docs/services/visual-recognition/customizing.html#guidelines-for-good-training
[updating-custom-classifiers]: https://console.bluemix.net/docs/services/visual-recognition/customizing.html#updating-custom-classifiers
