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
    Credentials credentials = new Credentials(<apikey>, <url>);
    VisualRecognition _visualRecognition = new VisualRecognition(credentials);
}
```

### Classify an image
Upload images or URLs to identify classes by default. To identify custom classifiers, include the classifier_ids or owners parameters. Images must be in .jpeg, or .png format.

For each image, the response includes a score for each class within each selected classifier. Scores range from 0 - 1 with a higher score indicating greater likelihood of the class being depicted in the image. The default threshold for reporting scores from a classifier is 0.5. We recommend an image that is a minimum of 224 x 224 pixels for best quality results.
```cs
void Classify()
{
    //  Classify using image url
	if(!_visualRecognition.Classify(<image-url>, OnClassify))
    	Log.Debug("ExampleVisualRecognition.Classify()", "Classify image failed!");

    //  Classify using image path
    if(!_visualRecognition.Classify(OnClassify, <image-path>, <classifier-owners>, <classifier-ids>, 0.5f))
        Log.Debug("ExampleVisualRecognition.Classify()", "Classify image failed!");
}

private void OnClassify(ClassifyTopLevelMultiple classify)
{
    Log.Debug("ExampleVisualRecognition.OnClassify()", "Classify result: {0}", data);
}
```

### Detect faces
Analyze faces in images and get data about them, such as estimated age, gender, plus names of celebrities. Images must be in .jpeg, or .png format. This functionality is not trainable, and does not support general biometric facial recognition.

For each image, the response includes face location, a minimum and maximum estimated age, a gender, and confidence scores. Scores range from 0 - 1 with a higher score indicating greater correlation.
```cs
void DetectFaces()
{
    //  Classify using image url
	if(!_visualRecognition.DetectFaces(<image-url>, OnDetectFaces))
        Log.Debug("ExampleVisualRecognition.DetectFaces()", "Detect faces failed!");

    //  Classify using image path
    if(!_visualRecognition.DetectFaces(OnDetectFaces, <image-path>))
        Log.Debug("ExampleVisualRecognition.DetectFaces()", "Detect faces failed!");
}

private void OnDetectFaces(FacesTopLevelMultiple multipleImages)
{
    Log.Debug("ExampleVisualRecognition.OnDetectFaces()", "Detect faces result: {0}", data);
}
```

### Create a classifier
Train a new multi-faceted classifier on the uploaded image data. A new custom classifier can be trained by several compressed (.zip) files, including files containing positive or negative images (.jpg, or .png). You must supply at least two compressed files, either two positive example files or one positive and one negative example file.

Compressed files containing positive examples are used to create “classes” that define what the new classifier is. The prefix that you specify for each positive example parameter is used as the class name within the new classifier. The `_positive_examples` suffix is required. There is no limit on the number of positive example files you can upload in a single call.

The compressed file containing negative examples is not used to create a class within the created classifier, but does define what the new classifier is not. Negative example files should contain images that do not depict the subject of any of the positive examples. You can only specify one negative example file in a single call. For more information, see [Structure of the training data][structure-of-the-training-data], and [Guidelines for good training][guidelines-for-good-training].
```cs
void TrainClassifier()
{
    if(!_visualRecognition.TrainClassifier(<classifier-name>, <class-name>, <positive-examples-path>, <negative-examples-path>, OnTrainClassifier))
        Log.Debug("ExampleVisualRecognition.TrainClassifier()", "Train classifier failed!");
}

private void OnTrainClassifier(GetClassifiersPerClassifierVerbose classifier, string data)
{
    Log.Debug("ExampleVisualRecognition.OnTrainClassifier()", "Train classifier result: {0}", data);
}
```

### Retrieve a list of custom classifiers
Retrieve a list of user-created classifiers.
```cs
void GetClassifiers()
{
	if(!_visualRecognition.GetClassifiers(OnGetClassifiers))
        Log.Debug("ExampleVisualRecognition.GetClassifiers()", "Getting classifiers failed!");
}

private void OnGetClassifiers(GetClassifiersTopLevelBrief classifiers, string data)
{
    Log.Debug("ExampleVisualRecognition.OnGetClassifiers()", "Get classifiers result: {0}", data);
}
```

### Retrieve classifier details
Retrieve information about a specific classifier.
```cs
void GetClassifier()
{
    if(!_visualRecognition.GetClassifier(<classifier-id>, OnGetClassifier))
        Log.Debug("ExampleVisualRecognition.GetClassifier()", "Getting classifier failed!");
}

private void OnGetClassifier(GetClassifiersPerClassifierVerbose classifier, string data)
{
    Log.Debug("ExampleVisualRecognition.OnGetClassifier()", "Get classifier result: {0}", data);
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
   if(!_visualRecognition.UpdateClassifier(OnUpdateClassifier, <classifier-id>, <classifier-name>, <class-name>, <positive-examples-path>))
        Log.Debug("ExampleVisualRecognition.UpdateClassifier()", "Update classifier failed!");
}

private void OnUpdateClassifier(GetClassifiersPerClassifierVerbose classifier, string data)
{
    Log.Debug("ExampleVisualRecognition.OnUpdateClassifier()", "Update classifier result: {0}", data);
}
```

### Delete a classifier
Delete a custom classifier with the specified classifier ID.
```cs
void DeleteClassifier()
{
    if(!_visualRecognition.DeleteClassifier(<classifier-id>, OnDeleteClassifier))
        Log.Debug("ExampleVisualRecognition.DeleteClassifier()", "Deleting classifier failed!");
}

private void OnDeleteClassifier(bool success)
{
    Log.Debug("ExampleVisualRecognition.OnDeleteClassifier()", "Update classifier result: {0}", success);
}
```

### Create a collection
Create a new collection of images to search. You can create a maximum of 5 collections.
```cs
void CreateCollection()
{
    if(!_visualRecognition.CreateCollection(OnCreateCollection, <collection-name>))
        Log.Debug("ExampleVisualRecognition.CreateCollectionMethod()", "Failed to create collection");
}

private void OnCreateCollection(CreateCollection collection, string data)
{
      Log.Debug("ExampleVisualRecognition.OnCreateCollection()", "Create collection result: {0}", data);
}
```

### List collections
List all custom collections.
```cs
void GetCollections()
{
    if(!_visualRecognition.GetCollections(OnGetCollections))
        Log.Debug("ExampleVisualRecognition.GetCollectionsMethod()", "Failed to get collections");
}

private void OnGetCollections(GetCollections collections, string data)
{
    Log.Debug("ExampleVisualRecognition.OnGetCollections()", "Get collections result: {0}", data);
}
```

### Retrieve collection details
Retrieve information about a specific collection.
```cs
void GetCollection()
{
    if(!_visualRecognition.GetCollection(OnGetCollection, <collection-id>))
        Log.Debug("ExampleVisualRecognition.GetCollection()", "Failed to get collection");
}

private void OnGetCollection(CreateCollection collection, string data)
{
    Log.Debug("ExampleVisualRecognition.OnGetCollection()", "Get collections result: {0}", data);
}
```

### Delete a collection
Delete a user created collection.
```cs
void DeleteCollection()
{
    if(!_visualRecognition.DeleteCollection(OnDeleteCollection, <collection-id>))
        Log.Debug("ExampleVisualRecognition.DeleteCollection()", "Failed to delete collection");
}

private void OnDeleteCollection(bool success, string data)
{
    Log.Debug("ExampleVisualRecognition.OnDeleteCollection()", "Delete collection result: {0}", data);
}
```

### Add images to a collection
Add images to a collection. Each collection can contain 1000000 images. It takes 1 second to upload 1 images, so uploading 1000000 images takes 11 days.
```cs
void AddCollectionImage()
{
    if(!_visualRecognition.AddCollectionImage(OnAddImageToCollection, <collection-id>, <collection-path>, <image-metadata>))
        Log.Debug("ExampleVisualRecognition.AddCollectionImage()", "Failed to add images to collection");
}
private void OnAddImageToCollection(CollectionsConfig images, string data)
{
    Log.Debug("ExampleVisualRecognition.OnAddImageToCollectionMethod()", "Add collectionimage result: {0}", data);
}
```

### List images in a collection
List 100 images in a collection. This returns an arbitrary selection of 100 images. Each collection can contain 1000000 images.
```cs
void GetCollectionImages()
{
    if(!_visualRecognition.GetCollectionImages(OnGetCollectionImages, <collection-id>))
        Log.Debug("ExampleVisualRecognition.GetCollectionImages()", "Failed to get collection images");
}

private void OnGetCollectionImages(GetCollectionImages collections, string data)
{
    Log.Debug("ExampleVisualRecognition.OnGetCollectionImages()", "Get collection images result: {0}", data);
}
```

### List image details
List details about a specific image in a collection.
```cs
void GetImage()
{
    if(!_visualRecognition.GetImage(OnGetImage, <collection-id>, <image-name>))
        Log.Debug("ExampleVisualRecognition.GetImage()", "Failed to get collection image");
}

private void OnGetImage(GetCollectionsBrief image, string data)
{
    Log.Debug("ExampleVisualRecognition.OnGetImage()", "Get collection image result: {0}", data);
}
```

### Delete an image
Delete an image from a collection.
```cs
void DeleteCollectionImage()
{
    if(!_visualRecognition.DeleteCollectionImage(OnDeleteCollectionImage, <collection-id>, <image-name>))
        Log.Debug("ExampleVisualRecognition.DeleteCollectionImage()", "Failed to delete collection image");
}

private void OnDeleteCollectionImage(bool success, string data)
{
    Log.Debug("ExampleVisualRecognition.OnDeleteCollectionImage()", "Delete collection image result: {0}", data);
}
```

### List metadata
View the metadata for a specific image in a collection.
```cs
void GetMetadata()
{
    if(!_visualRecognition.GetMetadata(OnGetMetadata, <collection-id>, <image-name>))
        Log.Debug("ExampleVisualRecognition.GetMetadata()", "Failed to get metadata");
}

private void OnGetMetadata(object responseObject, string data)
{
    Log.Debug("ExampleVisualRecognition.OnGetMetadata()", "Get metadata result: {0}", data);
}
```

### Delete metadata
Delete all metadata associated with an image.
```cs
void DeleteMetadata()
{
    if(!_visualRecognition.DeleteCollectionImageMetadata(OnDeleteMetadata, <collection-id>, <image-name>)
        Log.Debug("ExampleVisualRecognition.DeleteCollectionImageMetadata()", "Failed to delete image metadata");
}

private void OnDeleteMetadata(bool success, string data)
{
    Log.Debug("ExampleVisualRecognition.OnDeleteMetadata()", "Delete image metadata result: {0}", success);

}
```

### Find similar images
Upload an image to find similar images in your custom collection.
```cs
void FindSimilar()
{
    if(!visualRecognition.FindSimilar(OnFindSimilar, <collection-d>, <image-path>))
        Log.Debug("ExampleVisualRecognition.FindSimilar()", "Failed to find similar images");
}

private void OnFindSimilar(SimilarImagesConfig images, string data)
{
    Log.Debug("ExampleVisualRecognition.OnFindSimilar()", "Find similar result: {0}", data);
}
```

[visual-recognition]: https://www.ibm.com/watson/developercloud/visual-recognition/api/v3/
[structure-of-the-training-data]: https://console.bluemix.net/docs/services/visual-recognition/customizing.html#structure
[guidelines-for-good-training]: https://console.bluemix.net/docs/services/visual-recognition/customizing.html#guidelines-for-good-training
[updating-custom-classifiers]: https://console.bluemix.net/docs/services/visual-recognition/customizing.html#updating-custom-classifiers
