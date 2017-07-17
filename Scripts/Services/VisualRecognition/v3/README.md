[![NuGet](https://img.shields.io/badge/nuget-v1.0.0-green.svg?style=flat)](https://www.nuget.org/packages/IBM.WatsonDeveloperCloud.VisualRecognition.v3/)

### Visual Recognition
The IBM Watson™ [Visual Recognition][visual-recognition] service uses deep learning algorithms to identify scenes, objects, and celebrity faces in images you upload to the service. You can create and train a custom classifier to identify subjects that suit your needs.

### Installation
#### Nuget
```

PM > Install-Package IBM.WatsonDeveloperCloud.VisualRecognition.v3

```
#### Project.json
```JSON

"dependencies": {
   "IBM.WatsonDeveloperCloud.VisualRecognition.v3": "1.1.0"
}

```
### Usage
The IBM Watson™ [Visual Recognition][visual-recognition] service uses deep learning algorithms to identify scenes, objects, and faces in images you upload to the service. You can create and train a custom classifier to identify subjects that suit your needs. You can create and add images to a collection and then search that collection with your own image to find similar images. A valid API Key from Bluemix is required for all calls.

#### Instantiating and authenticating the service
Before you can send requests to the service it must be instantiated and api key must be set.
```cs
// create a Tone Analyzer Service instance
VisualRecognitionService _visualRecognition = new VisualRecognitionService();

// set the credentials
_visualRecognition.SetCredential("<apikey>");
```

#### Classify an image
Upload images or URLs to identify classes by default. To identify custom classifiers, include the classifier_ids or owners parameters. Images must be in .jpeg, or .png format.

For each image, the response includes a score for each class within each selected classifier. Scores range from 0 - 1 with a higher score indicating greater likelihood of the class being depicted in the image. The default threshold for reporting scores from a classifier is 0.5. We recommend an image that is a minimum of 224 x 224 pixels for best quality results.
```cs
//  classify using an image url
var result = _visualRecognition.Classify("<image-url>");

//  classify using image data
var result = _visualRecognition.Classify("<image-byte-data>", "<image-filename>", "<image-mimetype>");
```

#### Detect faces
Analyze faces in images and get data about them, such as estimated age, gender, plus names of celebrities. Images must be in .jpeg, or .png format. This functionality is not trainable, and does not support general biometric facial recognition.

For each image, the response includes face location, a minimum and maximum estimated age, a gender, and confidence scores. Scores range from 0 - 1 with a higher score indicating greater correlation.
```cs
//  detect faces using an image url
var result = _visualRecognition.DetectFaces("<face-url>");

//  detect faces using image data
var result = _visualRecognition.DetectFaces("<image-byte-data>", "<image-filename>", "<image-mimetype>");
```

#### Create a classifier
Train a new multi-faceted classifier on the uploaded image data. A new custom classifier can be trained by several compressed (.zip) files, including files containing positive or negative images (.jpg, or .png). You must supply at least two compressed files, either two positive example files or one positive and one negative example file.

Compressed files containing positive examples are used to create “classes” that define what the new classifier is. The prefix that you specify for each positive example parameter is used as the class name within the new classifier. The `_positive_examples` suffix is required. There is no limit on the number of positive example files you can upload in a single call.

The compressed file containing negative examples is not used to create a class within the created classifier, but does define what the new classifier is not. Negative example files should contain images that do not depict the subject of any of the positive examples. You can only specify one negative example file in a single call. For more information, see [Structure of the training data][structure-of-the-training-data], and [Guidelines for good training][guidelines-for-good-training].
```cs
//  create a dictionary of classnames and positive example data
Dictionary"<string, byte[]>" positiveExamples = new Dictionary"<string, byte[]>"();
positiveExamples.Add("<class-name>", "<positive-examples-data>");

//  create a classifier using positive example dictionary and negative example data
var result = _visualRecognition.CreateClassifier("<classifier-name>", positiveExamples, "<negative-examples-data>");
```

#### Retrieve a list of custom classifiers
Retrieve a list of user-created classifiers.
```cs
//  retrieve a brief list of classifiers
var result = _visualRecognition.GetClassifiersBrief();

//  retrieve a verbose list of classifiers
var result = _visualRecognition.GetClassifiersVerbose();
```

#### Retrieve classifier details
Retrieve information about a specific classifier.
```cs
var result = _visualRecognition.GetClassifier("<classifier-id>");
```

#### Update a classifier
Update an existing classifier by adding new classes, or by adding new images to existing classes.You cannot update a custom classifier with a free API Key.

To update the existing classifier, use several compressed (.zip) files, including files containing positive or negative images (.jpg, or .png). You must supply at least one compressed file, with additional positive or negative examples.

Compressed files containing positive examples are used to create and update “classes” to impact all of the classes in that classifier. The prefix that you specify for each positive example parameter is used as the class name within the new classifier. The `_positive_examples` suffix is required. There is no limit on the number of positive example files you can upload in a single call.

The compressed file containing negative examples is not used to create a class within the created classifier, but does define what the updated classifier is not. Negative example files should contain images that do not depict the subject of any of the positive examples. You can only specify one negative example file in a single call. For more information, see [Updating custom classifiers][updating-custom-classifiers].
```cs
//  create a dictionary of classnames and positive example data
Dictionary"<string, byte[]>" positiveExamples = new Dictionary"<string, byte[]>"();
positiveExamples.Add("<class-name>", "<positive-examples-data>");

//  updtae a classifier using positive example dictionary and negative example data
var result = _visualRecognition.UpdateClassifier("<classifier-name>", positiveExamples, "<negative=examples-data>");
```

#### Delete a classifier
Delete a custom classifier with the specified classifier ID.
```cs
var result = _visualRecognition.DeleteClassifier("<classifier-id>");
```

#### Create a collection
Create a new collection of images to search. You can create a maximum of 5 collections.
```cs
var result = _visualRecognition.CreateCollection("<collection-name>");
```

#### List collections
List all custom collections.
```cs
var result = _visualRecognition.GetCollections();
```

#### Retrieve collection details
Retrieve information about a specific collection.
```cs
var result = _visualRecognition.GetCollection("<collection-id>");
```

#### Delete a collection
Delete a user created collection.
```cs
var result = _visualRecognition.DeleteCollection("<collection-id>");
```

#### Add images to a collection
Add images to a collection. Each collection can contain 1000000 images. It takes 1 second to upload 1 images, so uploading 1000000 images takes 11 days.
```cs
 var result = _visualRecognition.AddImage("<collection-id>", "<image-data>", "<image-filename>", "<metadata-data>");
```

#### List images in a collection
List 100 images in a collection. This returns an arbitrary selection of 100 images. Each collection can contain 1000000 images.
```cs
var result = _visualRecognition.GetCollectionImages("<collection-id>");
```

#### List image details
List details about a specific image in a collection.
```cs
var result = _visualRecognition.GetImage("<collection-id>", "<image-id>");
```

#### Delete an imnage
Delete an image from a collection.
```cs
var result = _visualRecognition.DeleteImage("<collection-id>", "<image-id>");
```

#### Add or update metadata
Add metadata to a specific image in a collection. Use metadata for your own reference to identify images. You cannot filter the find_similar method by metadata.
```cs
var result = _visualRecognition.AddImageMetadata("<collection-id>", "<image-id>", "<metadata-data>");
```

#### List metadata
View the metadata for a specific image in a collection.
```cs
var result = _visualRecognition.GetMetadata("<collection-id>", "<image-id>");
```

#### Delete metadata
Delete all metadata associated with an image.
```cs
var result = _visualRecognition.DeleteImageMetadata("<collection-id>", "<image-id>");
```

#### Find similar images
Upload an image to find similar images in your custom collection.
```cs
var result = _visualRecognition.FindSimilar("<collection-id>", "<image-data>", "<image-filename>");
```

[visual-recognition]: https://www.ibm.com/watson/developercloud/visual-recognition/api/v3/
[structure-of-the-training-data]: https://www.ibm.com/watson/developercloud/doc/visual-recognition/customizing.html#structure
[guidelines-for-good-training]: https://www.ibm.com/watson/developercloud/doc/visual-recognition/customizing.html#guidelines-for-good-training
[updating-custom-classifiers]: https://www.ibm.com/watson/developercloud/doc/visual-recognition/customizing.html#updating-custom-classifiers
