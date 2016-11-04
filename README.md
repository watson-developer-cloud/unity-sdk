# Watson Developer Cloud Unity SDK
[![Build Status](https://travis-ci.org/watson-developer-cloud/unity-sdk.svg?branch=develop)](https://travis-ci.org/watson-developer-cloud/unity-sdk)

Use this SDK to build Watson-powered applications in Unity. It comes with a set of prefabs that you can use to develop a simple Watson application in just one minute.

## Table of Contents
* [Before you begin](#before-you-begin)
* [Getting the Watson SDK and adding it to Unity](#getting-the-watson-sdk-and-adding-it-to-unity)
  * [Installing the SDK source into your Unity project](#installing-the-sdk-source-into-your-unity-project)
* [Configuring your service credentials](#configuring-your-service-credentials)
* [IBM Watson Services](#ibm-watson-services)
  * [Speech to Text](#speech-to-text)
  * [Text to Speech](#text-to-speech)
  * [Language Translator](#language-translator)
  * [Dialog](#dialog)
  * [Natural Language Classifier](#natural-language-classifier)
  * [Tone Analyzer](#tone-analyzer)
  * [Tradeoff Analytics](#tradeoff-analytics)
  * [Conversation](#conversation)
  * [Visual Recognition](#visual-recognition)
  * [Alchemy Language](#alchemy-language)
  * [Personality Insights](#personality-insights)
  * [Document Conversion](#document-conversion)
  * [AlchemyData News](#alchemy-data-news)
  * [Retrieve and Rank](#retrieve-and-rank)
* [Developing a basic application in one minute](#developing-a-basic-application-in-one-minute)
* [Documentation](#documentation)
* [License](#license)
* [Contributing](#contributing)

## Before you begin
Ensure that you have the following prerequisites:

* An IBM Bluemix account. If you don't have one, [sign up][bluemix_registration].
* [Unity][get_unity]. You win! You can use the **free** Personal edition.
* Change the build settings in Unity (**File > Build Settings**) to any platform except for web player. The Watson Developer Cloud Unity SDK does not support Unity Web Player.

## Getting the Watson SDK and adding it to Unity
You can get the latest SDK release by clicking [here][latest_release].

### Installing the SDK source into your Unity project
Move the **`unity-sdk`** directory into the Assets directory of the Unity project. **Rename the SDK directory from `unity-sdk` to `Watson`.**

## Configuring your service credentials
You will need the 'username' and 'password' credentials for each service. Service credentials are different from your Bluemix account username and password.
1. Determine which services to configure.

2. If you have configured the services already, complete the following steps. Otherwise, go to step 3.
    1. Log in to Bluemix at https://bluemix.net.
    2. Navigate to the **Dashboard** on your Bluemix account.
    3. Click the **tile** for a service.
    4. Click **Service Credentials**. Note: If your browser window is too narrow, the service options may be collapsed. Click on the upward facing double arrow next to "Back to Dashboard..." on the upper left to expand the sidebar.
    5. Copy the content in the **Service Credentials** field, and paste it in the credentials field in the Config Editor (**Watson -> Config Editor**) in Unity.
    6. Click **Apply Credentials**.
    7. Repeat steps 1 - 5 for each service you want to use.
    ![services-0](http://g.recordit.co/cPa1FOGwEU.gif)
3. If you need to configure the services that you want to use, complete the following steps.
    1. In the Config Editor (**Watson -> Config Editor**), click the **Configure** button beside the service to register. The service window is displayed.
    2. Under **Add Service**, type a unique name for the service instance in the Service name field. For example, type 'my-service-name'. Leave the default values for the other options.
    3. Click **Create**.
    4. Click **Service Credentials**. Note: If your browser window is too narrow, the service options may be collapsed. Click on the upward facing double arrow next to "Back to Dashboard..." on the upper left to expand the sidebar.
    5. Copy the content in the **Service Credentials** field, and paste it in the empty credentials field in the **Config Editor** in Unity.
    6. Click **Apply Credentials**.
    7. Repeat steps 1 - 5 for each service you want to use.
    ![services-1](http://g.recordit.co/zyL5RZYXqa.gif)
4. Click **Save**, and close the Config Editor.

## IBM Watson Services
### Speech to Text
Use the [Speech to Text][speech_to_text] service to recognize the text from a .wav file. Assign the .wav file to the script in the Unity Editor. Speech to text can also be used to convert an audio stream into text.

```cs
[SerializeField]
private AudioClip m_AudioClip = new AudioClip();
private SpeechToText m_SpeechToText = new SpeechToText();

void Start()
{
  m_SpeechToText.Recognize(m_AudioClip, HandleOnRecognize);
}

void HandleOnRecognize (SpeechResultList result)
{
  if (result != null && result.Results.Length > 0)
  {
    foreach( var res in result.Results )
    {
      foreach( var alt in res.Alternatives )
      {
        string text = alt.Transcript;
        Debug.Log(string.Format( "{0} ({1}, {2:0.00})\n", text, res.Final ? "Final" : "Interim", alt.Confidence));
      }
    }
  }
}
```

### Text to Speech
Use the [Text to Speech][text_to_speech] service to get the available voices to synthesize. The Text to Speech service also supports Speech Synthesis Markup Language ([SSML][ssml]). In addition, the service supports a service-specific [expressive SSML][expressive_ssml] element. See Text To Speech service examples for examples on how to create custom Text to Speech voice models.

```cs
TextToSpeech m_TextToSpeech = new TextToSpeech();
string m_TestString = "Hello! This is Text to Speech!";
string m_ExpressiveText = "<speak version=\"1.0\"><prosody pitch=\"150Hz\">Hello! This is the </prosody><say-as interpret-as=\"letters\">IBM</say-as> Watson <express-as type=\"GoodNews\">Unity</express-as></prosody><say-as interpret-as=\"letters\">SDK</say-as>!</speak>";

void Start ()
{
  m_TextToSpeech.Voice = VoiceType.en_GB_Kate;
  m_TextToSpeech.ToSpeech(m_TestString, HandleToSpeechCallback);

  m_TextToSpeech.Voice = VoiceType.en_US_Allison;
  m_TextToSpeech.ToSpeech(m_ExpressiveText, HandleToSpeechCallback);
}

void HandleToSpeechCallback (AudioClip clip)
{
  PlayClip(clip);
}

private void PlayClip(AudioClip clip)
{
  if (Application.isPlaying && clip != null)
  {
    GameObject audioObject = new GameObject("AudioObject");
    AudioSource source = audioObject.AddComponent<AudioSource>();
    source.spatialBlend = 0.0f;
    source.loop = false;
    source.clip = clip;
    source.Play();

    GameObject.Destroy(audioObject, clip.length);
  }
}
```

### Language Translator
Select a domain, then identify or select the language of text, and then translate the text from one supported language to another.  
Example: Ask how to get to the disco in Spanish using [Language Translator][language_translator] service.

```cs
private LanguageTranslator m_Translate = new LanguageTranslator();
private string m_PhraseToTranslate = "How do I get to the disco?";

void Start ()
{
  Debug.Log("English Phrase to translate: " + m_PhraseToTranslate);
  m_Translate.GetTranslation(m_PhraseToTranslate, "en", "es", OnGetTranslation);
}

private void OnGetTranslation(Translations translation)
{
  if (translation != null && translation.translations.Length > 0)
    Debug.Log("Spanish Translation: " + translation.translations[0].translation);
}
```

### Dialog
The [Dialog service][dialog_service] was deprecated on August 15, 2016, existing instances of the service will continue to function until August 9, 2017. Users of the Dialog service should migrate their applications to use the Conversation service. See the [migration documentation][dialog_migration] to learn how to migrate your dialogs to the [Conversation service][conversation_service].

### Natural Language Classifier
Use [Natural Language Classifier][natural_language_classifier] service to create a classifier instance by providing a set of representative strings and a set of one or more correct classes for each as training. Then use the trained classifier to classify your new question for best matching answers or to retrieve next actions for your application.

The SDK contains a Test Natural Language Classifier, which contains classes for temperature and conditions. Before you develop a sample application in the next section, train the classifier on the test data.

1. Open the Natural Language Classifier Editor by clicking **Watson -> Natural Language Classifier Editor**.
2. Locate the Test Natural Language Classifier, and click **Train**. The training process begins. The process lasts a few minutes.
3. To check the status of the training process, click **Refresh**. When the status changes from Training to Available, the process is finished.
4. Replace the ClassifierID below with the Natural Language Classifier instance's ClassifierID.

```cs
private NaturalLanguageClassifier m_NaturalLanguageClassifier = new NaturalLanguageClassifier();
private string m_InputString = "Is it hot outside?";
private string m_ClassifierId = <ClassifierID>;

void Start ()
{
  Debug.Log("Input String: " + m_InputString);
  m_NaturalLanguageClassifier.Classify(m_ClassifierId, m_InputString, OnClassify);
}

private void OnClassify(ClassifyResult result)
{
  if (result != null)
  {
    Debug.Log("Classify Result: " + result.top_class);
  }
}
```

#### Managing classifiers
You can use the Natural Language Classifier Editor to import and export classifier files, and create new classifiers and edit them.

##### Importing files for existing classifiers
1. Click **Watson -> Natural Language Classifier Editor**. The Natural Language Classifier Editor window is displayed.
2. Click **Import**.
3. Navigate to the '.csv' file to import, and click **Open**. The file is imported.
4. Click **Train**.

##### Creating new classifiers
1. Click **Watson -> Natural Language Classifier Editor**. The Natural Language Classifier Editor window is displayed.
2. In the **Name** field, specify a name for the classifier.
3. Click **Create**.

### Tone Analyzer
The [Tone Analyzer][tone_analyzer] service detects emotions, social tendencies and writing style from text input.

```
ToneAnalyzer m_ToneAnalyzer = new ToneAnalyzer();
    string m_StringToTestTone = "This service enables people to discover and understand, and revise the impact of tone in their content. It uses linguistic analysis to detect and interpret emotional, social, and language cues found in text.";

	void Start () {
        m_ToneAnalyzer.GetToneAnalyze( OnGetToneAnalyze, m_StringToTestTone, "TEST");
	}

    private void OnGetToneAnalyze( ToneAnalyzerResponse resp , string data)
    {
        Debug.Log("Response: " +resp + " - " + data);
    }
```

### Tradeoff Analytics
The [Tradeoff Analytics][tradeoff_analytics] service helps people make better decisions when faced with multiple, sometimes conflicting, goals and alternatives.

```
void Start () {
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

        m_TradeoffAnalytics.GetDilemma( OnGetDilemma, problemToSolve, false );
	}

    private void OnGetDilemma( DilemmasResponse resp )
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
```

### Conversation
With the IBM Watson™ [Conversation][conversation] service you can create cognitive agents - virtual agents that combine machine learning, natural language understanding, and integrated dialog scripting tools to provide outstanding customer engagements. A workspace should be created using [Conversation tooling][conversation_tooling] and a variable `ConversationV1_ID` should be set in the Config Editor with the Workspace ID. This is required for the service status check in the `Config Editor`.

```cs
private Conversation m_Conversation = new Conversation();
private string m_WorkspaceID = "car_demo_1";
private string m_Input = "Can you unlock the door?";

void Start () {
	Debug.Log("User: " + m_Input);
	m_Conversation.Message(OnMessage, m_WorkspaceID, m_Input);
}

void OnMessage (DataModels.MessageResponse resp)
{
	foreach(DataModels.MessageIntent mi in resp.intents)
		Debug.Log("intent: " + mi.intent + ", confidence: " + mi.confidence);

	Debug.Log("response: " + resp.output.text);
}
```


### Visual Recognition
Use the [Visual Recognition][visual_recognition] service to classify an image against a default or custom trained classifier. In addition, the service can detect faces and text in an image.

#### Managing Classifiers
You can train and delete classifiers by directly accessing low level Visual Recognition methods.

##### Getting all classifiers
Get a list of all available classifiers

```cs
private VisualRecognition m_VisualRecognition = new VisualRecognition();

void Start()
{
	if(!m_VisualRecognition.GetClassifiers(OnGetClassifiers))
        Log.Debug("ExampleVisualRecognition", "Getting classifiers failed!");
}

private void OnGetClassifiers (GetClassifiersTopLevelBrief classifiers)
{
    if(classifiers != null && classifiers.classifiers.Length > 0)
    {
        foreach(GetClassifiersPerClassifierBrief classifier in classifiers.classifiers)
        {
            Log.Debug("ExampleVisualRecognition", "Classifier: " + classifier.name + ", " + classifier.classifier_id);
        }
    }
    else
    {
        Log.Debug("ExampleVisualRecognition", "Failed to get classifiers!");
    }
}
```

##### Finding a classifier
Find a classifier by name

```cs
private VisualRecognition m_VisualRecognition = new VisualRecognition();
private string m_classifierName = <Classifier Name>;

void Start()
{
	m_VisualRecognition.FindClassifier(m_classifierName, OnFindClassifier);
}

private void OnFindClassifier(GetClassifiersPerClassifierVerbose classifier)
{
    if(classifier != null)
    {
        Log.Debug("ExampleVisualRecognition", "Classifier " + m_classifierName + " found! ClassifierID: " + classifier.classifier_id);
    }
    else
    {
        Log.Debug("ExampleVisualRecognition", "Failed to find classifier by name!");
    }
}
```

Find a classifier by ID

```cs
private VisualRecognition m_VisualRecognition = new VisualRecognition();
private string m_classifierID = <Classifier ID>;

void Start()
{
	if(!m_VisualRecognition.GetClassifier(m_classifierID, OnGetClassifier))
            Log.Debug("ExampleVisualRecognition", "Getting classifier failed!");
}

private void OnGetClassifier(GetClassifiersPerClassifierVerbose classifier)
{
    if(classifier != null)
    {
        Log.Debug("ExampleVisualRecognition", "Classifier " + m_classifierID + " found! Classifier name: " + classifier.name);
    }
    else
    {
        Log.Debug("ExampleVisualRecognition", "Failed to find classifier by ID!");
    }
}
```

##### Training classifiers
Train a new classifier by uploading image data. Two compressed zip files containing at least two positive example files or one positive and one negative example file. The prefix of the positive example file is used as the classname for the new classifier `<Class Name>_positive_examples`. Negative examples zip must be named `negative_examples`. After a successful call, training the classifier takes a few minutes.

```cs
private VisualRecognition m_VisualRecognition = new VisualRecognition();

void Start()
{
	string m_positiveExamplesPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/<Class Name>_positive_examples.zip";
    string m_negativeExamplesPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/negative_examples.zip";
    if(!m_VisualRecognition.TrainClassifier("<Classifier Name>", "<Class Name>", m_positiveExamplesPath, m_negativeExamplesPath, OnTrainClassifier))
        Log.Debug("ExampleVisualRecognition", "Train classifier failed!");
}

private void OnTrainClassifier(GetClassifiersPerClassifierVerbose classifier)
{
    if(classifier != null)
    {
        Log.Debug("ExampleVisualRecognition", "Classifier is training! " + classifier);
    }
    else
    {
        Log.Debug("ExampleVisualRecognition", "Failed to train classifier!");
    }
}
```

##### Updating a classifier
Update an existing classifier by adding new classes, or by adding new images to existing classes. To update the existing classifier, use several compressed `.zip` files, including files containing positive or negative images `.jpg` or `.png`. You must supply at least one compressed file, with additional positive or negative examples.

Compressed files containing positive examples are used to add or create `classes` that define the updated classifier. The prefix that you specify for each positive example parameter is used as the class name within the classifier. The `_positive_examples` suffix is required. There is no limit on the number of positive example files you can upload in a single call.
The compressed file containing negative examples is not used to create a class within the created classifier, but does define what the new classifier is not. Negative example files should contain images that do not depict the subject of any of the positive examples. You can only specify one negative example file in a single call.

```cs
private VisualRecognition m_VisualRecognition = new VisualRecognition();

void Start()
{
	string m_positiveExamplesPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/<Class Name>_positive_examples.zip";
   if(!m_VisualRecognition.UpdateClassifier(OnUpdateClassifier, "<ClassifierID>", "<Classifier Name>", "<Class Name>", m_positiveExamplesPath))
        Log.Debug("ExampleVisualRecognition", "Update classifier failed!");
}

private void OnUpdateClassifier(GetClassifiersPerClassifierVerbose classifier)
{
    if(classifier != null)
    {
        Log.Debug("ExampleVisualRecognition", "Classifier is retraining! " + classifier);
    }
    else
    {
        Log.Debug("ExampleVisualRecognition", "Failed to update classifier!");
    }
}
```

##### Deleting classifiers
Delete a classifier by Classifier ID

```cs
private VisualRecognition m_VisualRecognition = new VisualRecognition();
private string m_classifierToDelete = "<Classifier ID>";

void Start()
{
	if(!m_VisualRecognition.DeleteClassifier(m_classifierToDelete, OnDeleteClassifier))
        Log.Debug("ExampleVisualRecognition", "Deleting classifier failed!");
}

private void OnDeleteClassifier(bool success)
{
    if(success)
    {
        Log.Debug("ExampleVisualRecognition", "Deleted classifier " + m_classifierToDelete);
    }
    else
    {
        Log.Debug("ExampleVisualRecognition", "Failed to delete classifier by ID!");
    }
}
```

#### Classifying an image
You can classify an image via URL or by posting an image. You may also define multiple owners and classifiers to classify against in the call. Supported filetypes are .gif, .jpg, .png or .zip.

###### Classify an image via URL

```cs
private VisualRecognition m_VisualRecognition = new VisualRecognition();
private string m_imageURL = "https://upload.wikimedia.org/wikipedia/commons/e/e9/Official_portrait_of_Barack_Obama.jpg";

void Start()
{
	if(!m_VisualRecognition.Classify(m_imageURL, OnClassify))
    	Log.Debug("ExampleVisualRecognition", "Classify image failed!");
}

private void OnClassify(ClassifyTopLevelMultiple classify)
{
    if(classify != null)
    {
        Log.Debug("ExampleVisualRecognition", "images processed: " + classify.images_processed);
        foreach(ClassifyTopLevelSingle image in classify.images)
        {
            Log.Debug("ExampleVisualRecognition", "\tsource_url: " + image.source_url + ", resolved_url: " + image.resolved_url);
            foreach(ClassifyPerClassifier classifier in image.classifiers)
            {
                Log.Debug("ExampleVisualRecognition", "\t\tclassifier_id: " + classifier.classifier_id + ", name: " + classifier.name);
                foreach(ClassResult classResult in classifier.classes)
                    Log.Debug("ExampleVisualRecognition", "\t\t\tclass: " + classResult.m_class + ", score: " + classResult.score + ", type_hierarchy: " + classResult.type_hierarchy);
            }
        }
    }
    else
    {
        Log.Debug("ExampleVisualRecognition", "Classification failed!");
    }
}
```

###### Classify an image by sending an image

```cs
private VisualRecognition m_VisualRecognition = new VisualRecognition();

void Start()
{
	string m_imagesPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/obama.jpg";
	    string[] m_owners = {"IBM", "me"};
	    string[] m_classifierIDs = {"default"};
	    if(!m_VisualRecognition.Classify(OnClassify, m_imagesPath, m_owners, m_classifierIDs, 0.5f))
	        Log.Debug("ExampleVisualRecognition", "Classify image failed!");
}

private void OnClassify(ClassifyTopLevelMultiple classify)
{
    if(classify != null)
    {
        Log.Debug("ExampleVisualRecognition", "images processed: " + classify.images_processed);
        foreach(ClassifyTopLevelSingle image in classify.images)
        {
            Log.Debug("ExampleVisualRecognition", "\tsource_url: " + image.source_url + ", resolved_url: " + image.resolved_url);
            foreach(ClassifyPerClassifier classifier in image.classifiers)
            {
                Log.Debug("ExampleVisualRecognition", "\t\tclassifier_id: " + classifier.classifier_id + ", name: " + classifier.name);
                foreach(ClassResult classResult in classifier.classes)
                    Log.Debug("ExampleVisualRecognition", "\t\t\tclass: " + classResult.m_class + ", score: " + classResult.score + ", type_hierarchy: " + classResult.type_hierarchy);
            }
        }
    }
    else
    {
        Log.Debug("ExampleVisualRecognition", "Classification failed!");
    }
}
```

#### Detecting faces
You can detect faces via URL or by posting an image. Supported filetypes are .gif, .jpg, .png or .zip.

###### Detect faces via URL

```cs
private VisualRecognition m_VisualRecognition = new VisualRecognition();
private string m_imageURL = "https://upload.wikimedia.org/wikipedia/commons/e/e9/Official_portrait_of_Barack_Obama.jpg";

void Start()
{
	if(!m_VisualRecognition.DetectFaces(m_imageURL, OnDetectFaces))
        Log.Debug("ExampleVisualRecognition", "Detect faces failed!");
}

private void OnDetectFaces(FacesTopLevelMultiple multipleImages)
{
    if(multipleImages != null)
    {
        Log.Debug("ExampleVisualRecognition", "images processed: {0}", multipleImages.images_processed);
        foreach(FacesTopLevelSingle faces in multipleImages.images)
        {
            Log.Debug("ExampleVisualRecognition", "\tsource_url: {0}, resolved_url: {1}", faces.source_url, faces.resolved_url);
            foreach(OneFaceResult face in faces.faces)
            {
                Log.Debug("ExampleVisualRecognition", "\t\tFace location: {0}, {1}, {2}, {3}", face.face_location.left, face.face_location.top, face.face_location.width, face.face_location.height);
                Log.Debug("ExampleVisualRecognition", "\t\tGender: {0}, Score: {1}", face.gender.gender, face.gender.score);
                Log.Debug("ExampleVisualRecognition", "\t\tAge Min: {0}, Age Max: {1}, Score: {2}", face.age.min, face.age.max, face.age.score);
                Log.Debug("ExampleVisualRecognition", "\t\tName: {0}, Score: {1}, Type Hierarchy: {2}", face.identity.name, face.identity.score, face.identity.type_hierarchy);
            }
        }
    }
    else
    {
        Log.Debug("ExampleVisualRecognition", "Detect faces failed!");
    }
}
```

###### Detect faces by sending an image

```cs
private VisualRecognition m_VisualRecognition = new VisualRecognition();

void Start()
{
	string m_faceExamplePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/obama.jpg";
    if(!m_VisualRecognition.DetectFaces(OnDetectFaces, m_faceExamplePath))
        Log.Debug("ExampleVisualRecognition", "Detect faces failed!");
}

private void OnDetectFaces(FacesTopLevelMultiple multipleImages)
{
    if(multipleImages != null)
    {
        Log.Debug("ExampleVisualRecognition", "images processed: {0}", multipleImages.images_processed);
        foreach(FacesTopLevelSingle faces in multipleImages.images)
        {
            Log.Debug("ExampleVisualRecognition", "\tsource_url: {0}, resolved_url: {1}", faces.source_url, faces.resolved_url);
            foreach(OneFaceResult face in faces.faces)
            {
                Log.Debug("ExampleVisualRecognition", "\t\tFace location: {0}, {1}, {2}, {3}", face.face_location.left, face.face_location.top, face.face_location.width, face.face_location.height);
                Log.Debug("ExampleVisualRecognition", "\t\tGender: {0}, Score: {1}", face.gender.gender, face.gender.score);
                Log.Debug("ExampleVisualRecognition", "\t\tAge Min: {0}, Age Max: {1}, Score: {2}", face.age.min, face.age.max, face.age.score);
                Log.Debug("ExampleVisualRecognition", "\t\tName: {0}, Score: {1}, Type Hierarchy: {2}", face.identity.name, face.identity.score, face.identity.type_hierarchy);
            }
        }
    }
    else
    {
        Log.Debug("ExampleVisualRecognition", "Detect faces failed!");
    }
}
```

#### Recognizing text
You can recognize text via URL or by posting an image. Supported filetypes are .gif, .jpg, .png or .zip.

###### Recognize text via URL

```cs
private VisualRecognition m_VisualRecognition = new VisualRecognition();
private string m_imageTextURL = "http://i.stack.imgur.com/ZS6nH.png";

void Start()
{
	if(!m_VisualRecognition.RecognizeText(m_imageTextURL, OnRecognizeText))
        Log.Debug("ExampleVisualRecognition", "Recognize text failed!");
}

private void OnRecognizeText(TextRecogTopLevelMultiple multipleImages)
{
    if(multipleImages != null)
    {
        Log.Debug("ExampleVisualRecognition", "images processed: {0}", multipleImages.images_processed);
        foreach(TextRecogTopLevelSingle texts in multipleImages.images)
        {
            Log.Debug("ExampleVisualRecognition", "\tsource_url: {0}, resolved_url: {1}", texts.source_url, texts.resolved_url);
            Log.Debug("ExampleVisualRecognition", "\ttext: {0}", texts.text);
            foreach(TextRecogOneWord text in texts.words)
            {
                Log.Debug("ExampleVisualRecognition", "\t\ttext location: {0}, {1}, {2}, {3}", text.location.left, text.location.top, text.location.width, text.location.height);
                Log.Debug("ExampleVisualRecognition", "\t\tLine number: {0}", text.line_number);
                Log.Debug("ExampleVisualRecognition", "\t\tword: {0}, Score: {1}", text.word, text.score);
            }
        }
    }
    else
    {
        Log.Debug("ExampleVisualRecognition", "RecognizeText failed!");
    }
}
```

###### Recognize text by sending an image

```cs
private VisualRecognition m_VisualRecognition = new VisualRecognition();

void Start()
{
	string m_textExamplePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/from_platos_apology.png";
        if(!m_VisualRecognition.RecognizeText(OnRecognizeText, m_textExamplePath))
            Log.Debug("ExampleVisualRecognition", "Recognize text failed!");
}

private void OnRecognizeText(TextRecogTopLevelMultiple multipleImages)
{
    if(multipleImages != null)
    {
        Log.Debug("ExampleVisualRecognition", "images processed: {0}", multipleImages.images_processed);
        foreach(TextRecogTopLevelSingle texts in multipleImages.images)
        {
            Log.Debug("ExampleVisualRecognition", "\tsource_url: {0}, resolved_url: {1}", texts.source_url, texts.resolved_url);
            Log.Debug("ExampleVisualRecognition", "\ttext: {0}", texts.text);
            foreach(TextRecogOneWord text in texts.words)
            {
                Log.Debug("ExampleVisualRecognition", "\t\ttext location: {0}, {1}, {2}, {3}", text.location.left, text.location.top, text.location.width, text.location.height);
                Log.Debug("ExampleVisualRecognition", "\t\tLine number: {0}", text.line_number);
                Log.Debug("ExampleVisualRecognition", "\t\tword: {0}, Score: {1}", text.word, text.score);
            }
        }
    }
    else
    {
        Log.Debug("ExampleVisualRecognition", "RecognizeText failed!");
    }
}
```

#### Similarity Search
Beta. You can create and add images to a collection and then search that collection with your own image to find similar images.

##### List Collections
Beta. List all custom collections.

```cs
void Start()
{
  m_VisualRecognition.GetCollections(OnGetCollections);
}

private void OnGetCollections(GetCollections collections, string customData)
{
  if(collections != null)
  {
    foreach (CreateCollection collection in collections.collections)
    {
      Log.Debug("VisualRecognitionExample", "collectionID: {0} | collection name: {1} | number of images: {2}", collection.collection_id, collection.name, collection.images);
    }
  }
  else
  {
    Log.Debug("VisualRecognitionExample", "Get Collections failed!");
  }
}
```

##### Create Collection
Beta. Create a new collection of images to search. You can create a maximum of 5 collections.

```cs
void Start()
{
  m_VisualRecognition.CreateCollection(OnCreateCollection, "unity-integration-test-collection");
}

private void OnCreateCollection(CreateCollection collection, string customData)
{
  if(collection != null)
  {
    Log.Debug("VisualRecognitionExample", "Create Collection succeeded!");
    Log.Debug("VisualRecognitionExample", "collectionID: {0} | collection name: {1} | collection images: {2}", collection.collection_id, collection.name, collection.images);
  }
  else
  {
    Log.Debug("VisualRecognitionExample", "Create Collection failed!");
  }
}
```

##### Get collection details
Beta. Retrieve information about a specific collection.

```cs
void Start()
{
  m_VisualRecognition.GetCollection(OnGetCollection, m_CreatedCollectionID);
}

private void OnGetCollection(CreateCollection collection, string customData)
{
  if (collection != null)
  {
    Log.Debug("VisualRecognitionExample", "Get Collection succeded!");
    Log.Debug("VisualRecognitionExample", "collectionID: {0} | collection name: {1} | collection images: {2}", collection.collection_id, collection.name, collection.images);
  }
  else
  {
    Log.Debug("VisualRecognitionExample", "Get Collection failed!");

  }
}
```

##### Add images to a collection
Beta. Add images to a collection. Each collection can contain 1000000 images.

```cs
void Start()
{
  string m_collectionImagePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/visual-recognition-classifiers/giraffe_to_classify.jpg";
  Dictionary<string, string> imageMetadata = new Dictionary<string, string>();
  imageMetadata.Add("key1", "value1");
  imageMetadata.Add("key2", "value2");
  imageMetadata.Add("key3", "value3");
  m_VisualRecognition.AddCollectionImage(OnAddImageToCollection, m_CreatedCollectionID, m_collectionImagePath, imageMetadata);
}
private void OnAddImageToCollection(CollectionsConfig images, string customData)
{
  if(images != null)
  {
    Log.Debug("VisualRecognitionExample", "Add image to collection succeeded!");
    m_CreatedCollectionImage = images.images[0].image_id;
    Log.Debug("VisualRecognitionExample", "images processed: {0}", images.images_processed);
    foreach (CollectionImagesConfig image in images.images)
    Log.Debug("VisualRecognitionExample", "imageID: {0} | image_file: {1} | image metadata: {1}", image.image_id, image.image_file, image.metadata.ToString());
  }
  else
  {
    Log.Debug("VisualRecognitionExample", "Add image to collection failed!");
  }
}
```

##### List images in a collection
Beta. List the first 100 images in a collection. Each collection can contain 1000000 images.

```cs
void Start()
{
  m_VisualRecognition.GetCollectionImages(OnGetCollectionImages, m_CreatedCollectionID);
}

private void OnGetCollectionImages(GetCollectionImages collections, string customData)
{
  if(collections != null)
  {
    Log.Debug("VisualRecognitionExample", "Get Collections succeded!");
    foreach(GetCollectionsBrief collection in collections.images)
    Log.Debug("VisualRecognitionExample", "imageID: {0} | image file: {1} | image metadataOnGetCollections: {2}", collection.image_id, collection.image_file, collection.metadata.ToString());
  }
  else
  {
    Log.Debug("VisualRecognitionExample", "Get Collections failed!");
  }
}
```

##### List image details
Beta. List details about a specific image in a collection.
```cs
void Start()
{
  m_VisualRecognition.GetImage(OnGetImage, m_CreatedCollectionID, m_CreatedCollectionImage);
}

private void OnGetImage(GetCollectionsBrief image, string customData)
{
  if(image != null)
  {
    Log.Debug("VisualRecognitionExample", "GetImage succeeded!");
    Log.Debug("VisualRecognitionExample", "imageID: {0} | created: {1} | image_file: {2} | metadata: {3}", image.image_id, image.created, image.image_file, image.metadata);
  }
  else
  {
    Log.Debug("VisualRecognitionExample", "GetImage failed!");
  }
}
```

##### List image metadata
Beta. View the metadata for a specific image in a collection.

```cs
void Start()
{
  m_VisualRecognition.GetMetadata(OnGetMetadata, m_CreatedCollectionID, m_CreatedCollectionImage);
}

private void OnGetMetadata(object responseObject, string customData)
{
  if(responseObject != null)
    Log.Debug("VisualRecognitionExample", "ResponseObject: {0}", responseObject);
}
```

##### Find similar images
Beta. Upload an image to find similar images in your custom collection.

```cs
void Start()
{
  m_VisualRecognition.FindSimilar(OnFindSimilar, m_CreatedCollectionID, m_collectionImagePath);
}

private void OnFindSimilar(SimilarImagesConfig images, string customData)
{
  if(images != null)
  {
    Log.Debug("VisualRecognitionExample", "GetSimilar succeeded!");
    Log.Debug("VisualRecognitionExample", "images processed: {0}", images.images_processed);
    foreach (SimilarImageConfig image in images.similar_images)
      Log.Debug("VisualRecognitionExample", "image ID: {0} | image file: {1} | score: {2} | metadata: {3}", image.image_id, image.image_file, image.score, image.metadata.ToString());
  }
  else
  {
    Log.Debug("VisualRecognitionExample", "GetSimilar failed!");
  }
}
```

##### Delete image metadata
Beta. Delete all metadata associated with an image.

```cs
void Start()
{
  m_VisualRecognition.DeleteCollectionImageMetadata(OnDeleteMetadata, m_CreatedCollectionID, m_CreatedCollectionImage);
}

private void OnDeleteMetadata(bool success, string customData)
{
  if (success)
    Log.Debug("VisualRecognitionExample", "Delete image metadata succeeded!");
  else
    Log.Debug("VisualRecognitionExample", "Delete image metadata failed!");
}
```

##### Delete an image
Beta. Delete an image from a collection.

```cs
void Start()
{
  m_VisualRecognition.DeleteCollectionImage(OnDeleteCollectionImage, m_CreatedCollectionID, m_CreatedCollectionImage);
}

private void OnDeleteCollectionImage(bool success, string customData)
{
  if (success)
    Log.Debug("VisualRecognitionExample", "Delete collection image succeeded!");
  else
    Log.Debug("VisualRecognitionExample", "Delete collection image failed!");
}
```

##### Delete a collection
Beta. Delete a user created collection.

```cs
void Start()
{
  m_VisualRecognition.DeleteCollection(OnDeleteCollection, m_CreatedCollectionID);
}

private void OnDeleteCollection(bool success, string customData)
{
  if(success)
    Log.Debug("VisualRecognitionExample", "Delete Collection succeeded!");
  else
    Log.Debug("VisualRecognitionExample", "Delete Collection failed!");
}
```

### Alchemy Language
Use the [Alchemy Language][alchemy_language] service to extract semantic meta-data from content such as information on people, places, companies, topics, facts, relationships, authors and languages.

#### Getting Authors
You can extract Authors from a URL or HTML source.

```cs
void Start()
{
	if(!m_AlchemyLanguage.GetAuthors(OnGetAuthors, "http://www.nytimes.com/2011/02/17/science/17jeopardy-watson.html"))
		Log.Debug("ExampleAlchemyLanguage", "Failed to get authors URL POST!");
}

private void OnGetAuthors(AuthorsData authors, string data)
{
	if(authors != null)
	{
		Log.Debug("ExampleAlchemyLanguage", "data: {0}", data);
		if(authors.authors.names.Length == 0)
			Log.Debug("ExampleAlchemyLanguage", "No authors found!");

		foreach(string name in authors.authors.names)
			Log.Debug("ExampleAlchemyLanguage", "Author " + name + " found!");
	}
	else
	{
		Log.Debug("ExampleAlchemyLanguage", "Failed to find Author!");
	}
}


```

#### Getting Concepts
You can get Concepts from a URL, HTML or Text source.

```cs
void Start()
{
	if(!m_AlchemyLanguage.GetRankedConcepts(OnGetConcepts, "http://www.nytimes.com/2011/02/17/science/17jeopardy-watson.html"))
		Log.Debug("ExampleAlchemyLanguage", "Failed to get concepts HTML POST!");
}

private void OnGetConcepts(ConceptsData concepts, string data)
{
    if(concepts != null)
    {
        Log.Debug("ExampleAlchemyLanguage", "status: {0}", concepts.status);
        Log.Debug("ExampleAlchemyLanguage", "url: {0}", concepts.url);
        Log.Debug("ExampleAlchemyLanguage", "language: {0}", concepts.language);
        if(concepts.concepts.Length == 0)
            Log.Debug("ExampleAlchemyLanguage", "No concepts found!");

        foreach(Concept concept in concepts.concepts)
            Log.Debug("ExampleAlchemyLanguage", "Concept: {0}, Relevance: {1}", concept.text, concept.relevance);
    }
    else
    {
        Log.Debug("ExampleAlchemyLanguage", "Failed to find Concepts!");
    }
}
```
#### Getting Dates
You can extract Dates from a URL, HTML or Text source.

```cs
void Start()
{
	if(!m_AlchemyLanguage.GetDates(OnGetDates, "http://www.nytimes.com/2011/02/17/science/17jeopardy-watson.html"))
	    Log.Debug("ExampleAlchemyLanguage", "Failed to get dates by URL POST");
}

private void OnGetDates(DateData dates, string data)
{
    if(dates != null)
    {
        Log.Debug("ExampleAlchemyLanguage", "status: {0}", dates.status);
        Log.Debug("ExampleAlchemyLanguage", "language: {0}", dates.language);
        Log.Debug("ExampleAlchemyLanguage", "url: {0}", dates.url);
        if(dates.dates == null || dates.dates.Length == 0)
            Log.Debug("ExampleAlchemyLanguage", "No dates found!");
        else
            foreach(Date date in dates.dates)
                Log.Debug("ExampleAlchemyLanguage", "Text: {0}, Date: {1}", date.text, date.date);
    }
    else
    {
        Log.Debug("ExampleAlchemyLanguage", "Failed to find Dates!");
    }
}
```
#### Getting Emotions
You can get Emotions from a URL, HTML or Text source.

```cs
void Start()
{
	if(!m_AlchemyLanguage.GetEmotions(OnGetEmotions, "http://www.nytimes.com/2011/02/17/science/17jeopardy-watson.html"))
	    Log.Debug("ExampleAlchemyLanguage", "Failed to get emotions by URL POST");
}

private void OnGetEmotions(EmotionData emotions, string data)
{
    if(emotions != null)
    {
        Log.Debug("ExampleAlchemyLanguage", "status: {0}", emotions.status);
        Log.Debug("ExampleAlchemyLanguage", "url: {0}", emotions.url);
        Log.Debug("ExampleAlchemyLanguage", "language: {0}", emotions.language);
        Log.Debug("ExampleAlchemyLanguage", "text: {0}", emotions.text);
        if(emotions.docEmotions == null)
            Log.Debug("ExampleAlchemyLanguage", "No emotions found!");
        else
        {
            Log.Debug("ExampleAlchemyLanguage", "anger: {0}", emotions.docEmotions.anger);
            Log.Debug("ExampleAlchemyLanguage", "disgust: {0}", emotions.docEmotions.disgust);
            Log.Debug("ExampleAlchemyLanguage", "fear: {0}", emotions.docEmotions.fear);
            Log.Debug("ExampleAlchemyLanguage", "joy: {0}", emotions.docEmotions.joy);
            Log.Debug("ExampleAlchemyLanguage", "sadness: {0}", emotions.docEmotions.sadness);
        }
    }
    else
    {
        Log.Debug("ExampleAlchemyLanguage", "Failed to find Emotions!");
    }
}
```
#### Extracting Entities
You can extract Entities from a URL, HTML or Text source.

```cs
void Start()
{
	if(!m_AlchemyLanguage.ExtractEntities(OnExtractEntities, "http://www.nytimes.com/2011/02/17/science/17jeopardy-watson.html"))
		Log.Debug("ExampleAlchemyLanguage", "Failed to get entities by URL POST");
}

private void OnExtractEntities(EntityData entityData, string data)
{
    if(entityData != null)
    {
        Log.Debug("ExampleAlchemyLanguage", "status: {0}", entityData.status);
        Log.Debug("ExampleAlchemyLanguage", "url: {0}", entityData.url);
        Log.Debug("ExampleAlchemyLanguage", "language: {0}", entityData.language);
        Log.Debug("ExampleAlchemyLanguage", "text: {0}", entityData.text);
        if(entityData == null || entityData.entities.Length == 0)
            Log.Debug("ExampleAlchemyLanguage", "No entities found!");
        else
            foreach(Entity entity in entityData.entities)
                Log.Debug("ExampleAlchemyLanguage", "text: {0}, type: {1}", entity.text, entity.type);
    }
    else
    {
        Log.Debug("ExampleAlchemyLanguage", "Failed to find Emotions!");
    }
}
```
#### Detecting Feeds
You can detect RSS Feeds from a URL source.

```cs
void Start()
{
	if(!m_AlchemyLanguage.DetectFeeds(OnDetectFeeds, "http://time.com/newsfeed/"))
	    Log.Debug("ExampleAlchemyLanguage", "Failed to get feeds by URL POST");
}

private void OnDetectFeeds(FeedData feedData, string data)
{
    if(feedData != null)
    {
        Log.Debug("ExampleAlchemyLanguage", "status: {0}", feedData.status);
        if(feedData == null || feedData.feeds.Length == 0)
            Log.Debug("ExampleAlchemyLanguage", "No feeds found!");
        else
            foreach(Feed feed in feedData.feeds)
                Log.Debug("ExampleAlchemyLanguage", "text: {0}", feed.feed);
    }
    else
    {
        Log.Debug("ExampleAlchemyLanguage", "Failed to find Feeds!");
    }
}
```
#### Extracting Keywords
You can extract Keywords form a URL, HTML or Text source.

```cs
void Start()
{
	if(!m_AlchemyLanguage.ExtractKeywords(OnExtractKeywords, "http://www.nytimes.com/2011/02/17/science/17jeopardy-watson.html"))
	    Log.Debug("ExampleAlchemyLanguage", "Failed to get keywords by URL POST");
}

private void OnExtractKeywords(KeywordData keywordData, string data)
{
    if(keywordData != null)
    {
        Log.Debug("ExampleAlchemyLanguage", "status: {0}", keywordData.status);
        Log.Debug("ExampleAlchemyLanguage", "url: {0}", keywordData.url);
        Log.Debug("ExampleAlchemyLanguage", "language: {0}", keywordData.language);
        Log.Debug("ExampleAlchemyLanguage", "text: {0}", keywordData.text);
        if(keywordData == null || keywordData.keywords.Length == 0)
            Log.Debug("ExampleAlchemyLanguage", "No keywords found!");
        else
            foreach(Keyword keyword in keywordData.keywords)
                Log.Debug("ExampleAlchemyLanguage", "text: {0}, relevance: {1}", keyword.text, keyword.relevance);
    }
    else
    {
        Log.Debug("ExampleAlchemyLanguage", "Failed to find Keywords!");
    }
}
```
#### Extracting Languages
You can extract the language of a URL, HTML or Text source.

```cs
void Start()
{
	if(!m_AlchemyLanguage.GetLanguages(OnGetLanguages, "http://www.nytimes.com/2011/02/17/science/17jeopardy-watson.html"))
	    Log.Debug("ExampleAlchemyLanguage", "Failed to get languages by text POST");
}

private void OnGetLanguages(LanguageData languages, string data)
{
    if(languages != null)
    {
        if(string.IsNullOrEmpty(languages.language))
            Log.Debug("ExampleAlchemyLanguage", "No languages detected!");
        else
        {
            Log.Debug("ExampleAlchemyLanguage", "status: {0}", languages.status);
            Log.Debug("ExampleAlchemyLanguage", "url: {0}", languages.url);
            Log.Debug("ExampleAlchemyLanguage", "language: {0}", languages.language);
            Log.Debug("ExampleAlchemyLanguage", "ethnologue: {0}", languages.ethnologue);
            Log.Debug("ExampleAlchemyLanguage", "iso_639_1: {0}", languages.iso_639_1);
            Log.Debug("ExampleAlchemyLanguage", "iso_639_2: {0}", languages.iso_639_2);
            Log.Debug("ExampleAlchemyLanguage", "iso_639_3: {0}", languages.iso_639_3);
            Log.Debug("ExampleAlchemyLanguage", "native_speakers: {0}", languages.native_speakers);
            Log.Debug("ExampleAlchemyLanguage", "wikipedia: {0}", languages.wikipedia);
        }
    }
    else
    {
        Log.Debug("ExampleAlchemyLanguage", "Failed to find Dates!");
    }
}
```
#### Getting Microformats
You can get the Microformat of a URL source.

```cs
void Start()
{
	if(!m_AlchemyLanguage.GetMicroformats(OnGetMicroformats, "http://microformats.org/wiki/hcard"))
	    Log.Debug("ExampleAlchemyLanguage", "Failed to get microformats by text POST");
}

private void OnGetMicroformats(MicroformatData microformats, string data)
{
    if(microformats != null)
    {
        Log.Debug("ExampleAlchemyLanguage", "status: {0}", microformats.status);
        Log.Debug("ExampleAlchemyLanguage", "url: {0}", microformats.url);
        if(microformats.microformats.Length == 0)
            Log.Warning("ExampleAlchemyLanguage", "No microformats found!");
        else
        {
            foreach(Microformat microformat in microformats.microformats)
                Log.Debug("ExampleAlchemyLanguage", "field: {0}, data: {1}.", microformat.field, microformat.data);
        }
    }
    else
    {
        Log.Debug("ExampleAlchemyLanguage", "Failed to find Microformats!");
    }
}
```
#### Getting Publication Dates
You can extract the publication date from a URL or HTML source.

```cs
void Start()
{
	if(!m_AlchemyLanguage.GetPublicationDate(OnGetPublicationDate, "http://www.nytimes.com/2011/02/17/science/17jeopardy-watson.html"))
	    Log.Debug("ExampleAlchemyLanguage", "Failed to get publication dates by url POST");
}

private void OnGetPublicationDate(PubDateData pubDates, string data)
{
    if(pubDates != null)
    {
        Log.Debug("ExampleAlchemyLanguage", "status: {0}", pubDates.status);
        Log.Debug("ExampleAlchemyLanguage", "url: {0}", pubDates.url);
        Log.Debug("ExampleAlchemyLanguage", "language: {0}", pubDates.language);
        if(pubDates.publicationDate != null)
            Log.Debug("ExampleAlchemyLanguage", "date: {0}, confident: {1}", pubDates.publicationDate.date, pubDates.publicationDate.confident);
        else
            Log.Debug("ExampleAlchemyLanguage", "Failed to find Publication Dates!");
    }
    else
    {
        Log.Debug("ExampleAlchemyLanguage", "Failed to find Publication Dates!");
    }
}
```
#### Getting Relations
You can extract Relations from a URL, HTML or Text source.

```cs
void Start()
{
    if(!m_AlchemyLanguage.GetRelations(OnGetRelations, "http://www.nytimes.com/2011/02/17/science/17jeopardy-watson.html"))
        Log.Debug("ExampleAlchemyLanguage", "Failed to get relations by text POST");
}

private void OnGetRelations(RelationsData relationsData, string data)
{
    if(relationsData != null)
    {
        Log.Debug("ExampleAlchemyLanguage", "status: {0}", relationsData.status);
        Log.Debug("ExampleAlchemyLanguage", "url: {0}", relationsData.url);
        Log.Debug("ExampleAlchemyLanguage", "language: {0}", relationsData.language);
        Log.Debug("ExampleAlchemyLanguage", "text: {0}", relationsData.text);
        if(relationsData.relations == null || relationsData.relations.Length == 0)
            Log.Debug("ExampleAlchemyLanguage", "No relations found!");
        else
            foreach(Relation relation in relationsData.relations)
                if(relation.subject != null && !string.IsNullOrEmpty(relation.subject.text))
                    Log.Debug("ExampleAlchemyLanguage", "Text: {0}, Date: {1}", relation.sentence, relation.subject.text);
    }
    else
    {
        Log.Debug("ExampleAlchemyLanguage", "Failed to find Relations!");
    }
}
```
#### Getting Sentiment
You can extract the Sentiment from a URL, HTML or Text source.

```cs
void Start()
{
	if(!m_AlchemyLanguage.GetTextSentiment(OnGetTextSentiment, "http://www.nytimes.com/2011/02/17/science/17jeopardy-watson.html"))
	    Log.Debug("ExampleAlchemyLanguage", "Failed to get sentiment by text POST");
}

private void OnGetTextSentiment(SentimentData sentimentData, string data)
{
    if(sentimentData != null)
    {
        Log.Debug("ExampleAlchemyLanguage", "status: {0}", sentimentData.status);
        Log.Debug("ExampleAlchemyLanguage", "url: {0}", sentimentData.url);
        Log.Debug("ExampleAlchemyLanguage", "language: {0}", sentimentData.language);
        Log.Debug("ExampleAlchemyLanguage", "text: {0}", sentimentData.text);
        if(sentimentData.docSentiment == null)
            Log.Debug("ExampleAlchemyLanguage", "No sentiment found!");
        else
            if(sentimentData.docSentiment != null && !string.IsNullOrEmpty(sentimentData.docSentiment.type))
                Log.Debug("ExampleAlchemyLanguage", "Sentiment: {0}, Score: {1}", sentimentData.docSentiment.type, sentimentData.docSentiment.score);
    }
    else
    {
        Log.Debug("ExampleAlchemyLanguage", "Failed to find Relations!");
    }
}
```
#### Getting Targeted Sentiment
You can extract a Targeted Sentiment from a URL, HTML or Text source.

```cs
void Start()
{
	if(!m_AlchemyLanguage.GetTargetedSentiment(OnGetTargetedSentiment, "http://www.nytimes.com/2011/02/17/science/17jeopardy-watson.html", "Jeopardy|Jennings|Watson"))
	    Log.Debug("ExampleAlchemyLanguage", "Failed to get targeted sentiment by text POST");
}

private void OnGetTargetedSentiment(TargetedSentimentData sentimentData, string data)
{
    if(sentimentData != null)
    {
        Log.Debug("ExampleAlchemyLanguage", "status: {0}", sentimentData.status);
        Log.Debug("ExampleAlchemyLanguage", "url: {0}", sentimentData.url);
        Log.Debug("ExampleAlchemyLanguage", "language: {0}", sentimentData.language);
        Log.Debug("ExampleAlchemyLanguage", "text: {0}", sentimentData.text);
        if(sentimentData.results == null)
            Log.Debug("ExampleAlchemyLanguage", "No sentiment found!");
        else
            if(sentimentData.results == null || sentimentData.results.Length == 0)
                Log.Warning("ExampleAlchemyLanguage", "No sentiment results!");
            else
                foreach(TargetedSentiment result in sentimentData.results)
                    Log.Debug("ExampleAlchemyLanguage", "text: {0}, sentiment: {1}, score: {2}", result.text, result.sentiment.score, result.sentiment.type);
    }
    else
    {
        Log.Debug("ExampleAlchemyLanguage", "Failed to find Relations!");
    }
}
```
#### Getting Taxonomy
You can get the Taxonomy of entities from a URL, HTML or Text source.

```cs
void Start()
{
    if(!m_AlchemyLanguage.GetRankedTaxonomy(OnGetRankedTaxonomy, "http://www.nytimes.com/2011/02/17/science/17jeopardy-watson.html"))
        Log.Debug("ExampleAlchemyLanguage", "Failed to get ranked taxonomy by text POST");
}

private void OnGetRankedTaxonomy(TaxonomyData taxonomyData, string data)
{
    if(taxonomyData != null)
    {
        Log.Debug("ExampleAlchemyLanguage", "status: {0}", taxonomyData.status);
        Log.Debug("ExampleAlchemyLanguage", "url: {0}", taxonomyData.url);
        Log.Debug("ExampleAlchemyLanguage", "language: {0}", taxonomyData.language);
        Log.Debug("ExampleAlchemyLanguage", "text: {0}", taxonomyData.text);
        if(taxonomyData.taxonomy == null)
            Log.Debug("ExampleAlchemyLanguage", "No taxonomy found!");
        else
            if(taxonomyData.taxonomy == null || taxonomyData.taxonomy.Length == 0)
                Log.Warning("ExampleAlchemyLanguage", "No taxonomy results!");
            else
                foreach(Taxonomy taxonomy in taxonomyData.taxonomy)
                    Log.Debug("ExampleAlchemyLanguage", "label: {0}, score: {1}", taxonomy.label, taxonomy.score);
    }
    else
    {
        Log.Debug("ExampleAlchemyLanguage", "Failed to find Relations!");
    }
}
```
#### Getting Text
You can exctract the Text from a URL or HTML source.

```cs
void Start()
{
	if(!m_AlchemyLanguage.GetText(OnGetText, "http://www.nytimes.com/2011/02/17/science/17jeopardy-watson.html"))
	    Log.Debug("ExampleAlchemyLanguage", "Failed to get text by text POST");
}

private void OnGetText(TextData textData, string data)
{
    if(textData != null)
    {
        Log.Debug("ExampleAlchemyLanuguage", "status: {0}", textData.status);
        Log.Debug("ExampleAlchemyLanuguage", "url: {0}", textData.url);
        Log.Debug("ExampleAlchemyLanuguage", "text: {0}", textData.text);
    }
    else
    {
        Log.Debug("ExampleAlchemyLanguage", "Failed to find text!");
    }

}
```
#### Getting Raw Text
You can exctract the Raw Text from a URL or HTML source.

```cs
void Start()
{
	if(!m_AlchemyLanguage.GetRawText(OnGetText, "http://www.nytimes.com/2011/02/17/science/17jeopardy-watson.html"))
	    Log.Debug("ExampleAlchemyLanguage", "Failed to get raw text by text POST");
}

private void OnGetText(TextData textData, string data)
{
    if(textData != null)
    {
        Log.Debug("ExampleAlchemyLanuguage", "status: {0}", textData.status);
        Log.Debug("ExampleAlchemyLanuguage", "url: {0}", textData.url);
        Log.Debug("ExampleAlchemyLanuguage", "text: {0}", textData.text);
    }
    else
    {
        Log.Debug("ExampleAlchemyLanguage", "Failed to find text!");
    }

}

```
#### Getting Title
You can extract the Title form a URL or HTML source.

```cs
void Start()
{
    if(!m_AlchemyLanguage.GetTitle(OnGetTitle, "http://www.nytimes.com/2011/02/17/science/17jeopardy-watson.html"))
        Log.Debug("ExampleAlchemyLanguage", "Failed to get title by text POST");
}

private void OnGetTitle(Title titleData, string data)
{
    if(titleData != null)
    {
        Log.Debug("ExampleAlchemyLanuguage", "status: {0}", titleData.status);
        Log.Debug("ExampleAlchemyLanuguage", "url: {0}", titleData.url);
        Log.Debug("ExampleAlchemyLanuguage", "text: {0}", titleData.title);
    }
    else
    {
        Log.Debug("ExampleAlchemyLanguage", "Failed to find title!");
    }

}
```
#### Getting Combined Data
You can combine multiple requests into one call using a Combined Data call from a URL, HTML or Text source. Allowed services in Combined Call are authors, concepts, dates, doc-emotion, entities, feeds, keywords, pub-dates, releations, doc-sentiment, taxonomy, title, page-image and image-keywords.

```cs
void Start()
{
    if(!m_AlchemyLanguage.GetCombinedData(OnGetCombinedData, "http://www.nytimes.com/2011/02/17/science/17jeopardy-watson.html"))
        Log.Debug("ExampleAlchemyLanguage", "Failed to get combined data by text POST");
}

private void OnGetCombinedData(CombinedCallData combinedData, string data)
{
    if(combinedData != null)
    {
        Log.Debug("ExampleAlchemyLanguage", "status: {0}", combinedData.status);
        Log.Debug("ExampleAlchemyLanguage", "url: {0}", combinedData.url);
        Log.Debug("ExampleAlchemyLanguage", "language: {0}", combinedData.language);
        Log.Debug("ExampleAlchemyLanguage", "text: {0}", combinedData.text);
        Log.Debug("ExampleAlchemyLanguage", "image: {0}", combinedData.image);

        if(combinedData.imageKeywords != null && combinedData.imageKeywords.Length > 0)
            foreach(ImageKeyword imageKeyword in combinedData.imageKeywords)
                Log.Debug("ExampleAlchemyLanguage", "ImageKeyword: {0}, Score: {1}", imageKeyword.text, imageKeyword.score);

        if(combinedData.publicationDate != null)
            Log.Debug("ExampleAlchemyLanguage", "publicationDate: {0}, Score: {1}", combinedData.publicationDate.date, combinedData.publicationDate.confident);

        if(combinedData.authors != null && combinedData.authors.names.Length > 0)
            foreach(string authors in combinedData.authors.names)
                Log.Debug("ExampleAlchemyLanguage", "Authors: {0}", authors);

        if(combinedData.docSentiment != null)
            Log.Debug("ExampleAlchemyLanguage", "DocSentiment: {0}, Score: {1}, Mixed: {2}", combinedData.docSentiment.type, combinedData.docSentiment.score, combinedData.docSentiment.mixed);

        if(combinedData.feeds != null && combinedData.feeds.Length > 0)
            foreach(Feed feed in combinedData.feeds)
                Log.Debug("ExampleAlchemyLanguage", "Feeds: {0}", feed.feed);

        if(combinedData.keywords != null && combinedData.keywords.Length > 0)
            foreach(Keyword keyword in combinedData.keywords)
                Log.Debug("ExampleAlchemyLanguage", "Keyword: {0}, relevance: {1}", keyword.text, keyword.relevance);

        if(combinedData.concepts != null && combinedData.concepts.Length > 0)
            foreach(Concept concept in combinedData.concepts)
                Log.Debug("ExampleAlchemyLanguage", "Concept: {0}, Relevance: {1}", concept.text, concept.relevance);

        if(combinedData.entities != null && combinedData.entities.Length > 0)
            foreach(Entity entity in combinedData.entities)
                Log.Debug("ExampleAlchemyLanguage", "Entity: {0}, Type: {1}, Relevance: {2}", entity.text, entity.type, entity.relevance);

        if(combinedData.relations != null && combinedData.relations.Length > 0)
            foreach(Relation relation in combinedData.relations)
                Log.Debug("ExampleAlchemyLanguage", "Relations: {0}", relation.subject.text);

        if(combinedData.taxonomy != null && combinedData.taxonomy.Length > 0)
            foreach(Taxonomy taxonomy in combinedData.taxonomy)
                Log.Debug("ExampleAlchemyLanguage", "Taxonomy: {0}, Score: {1}, Confident: {2}" ,taxonomy.label, taxonomy.score, taxonomy.confident);

        if(combinedData.dates != null && combinedData.dates.Length > 0)
            foreach(Date date in combinedData.dates)
                Log.Debug("ExampleAlchemyLanguage", "Dates", date.text, date.date);

        if(combinedData.docEmotions != null && combinedData.docEmotions.Length > 0)
            foreach(DocEmotions emotions in combinedData.docEmotions)
                Log.Debug("ExampleAlchemyLanguage", "Doc Emotions: anger: {0}, disgust: {1}, fear: {2}, joy: {3}, sadness: {4}", emotions.anger, emotions.disgust, emotions.fear, emotions.joy, emotions.sadness);
    }
    else
    {
        Log.Debug("ExampleAlchemyLanguage", "Failed to get combined data!");
    }
}
```


### Personality Insights
The IBM Watson™ [Personality Insights][personality_insights] service provides a Representational State Transfer (REST) Application Programming Interface (API) that enables applications to derive insights from social media, enterprise data, or other digital communications. The service uses linguistic analytics to infer individuals' intrinsic personality characteristics, including Big Five, Needs, and Values, from digital communications such as email, text messages, tweets, and forum posts. The service can automatically infer, from potentially noisy social media, portraits of individuals that reflect their personality characteristics. The service can report consumption preferences based on the results of its analysis, and for JSON content that is timestamped, it can report temporal behavior.

```cs
PersonalityInsights m_personalityInsights = new PersonalityInsights();
	private string testString = "<text-here>"";
  private string dataPath;

	void Start () {
		LogSystem.InstallDefaultReactors();

		dataPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/personalityInsights.json";

		if(!m_personalityInsights.GetProfile(OnGetProfileJson, dataPath, ContentType.TEXT_HTML, ContentLanguage.ENGLISH, ContentType.APPLICATION_JSON, AcceptLanguage.ENGLISH, true, true, true))
			Log.Debug("ExamplePersonalityInsights", "Failed to get profile!");

		if (!m_personalityInsights.GetProfile(OnGetProfileText, testString, ContentType.TEXT_HTML, ContentLanguage.ENGLISH, ContentType.APPLICATION_JSON, AcceptLanguage.ENGLISH, true, true, true))
			Log.Debug("ExamplePersonalityInsights", "Failed to get profile!");
	}
	private void OnGetProfileText(Profile profile, string data)
	{
		if (profile != null)
		{
			if (!string.IsNullOrEmpty(profile.processed_language))
				Log.Debug("TestPersonalityInsightsV3", "processed_language: {0}", profile.processed_language);

			Log.Debug("TestPersonalityInsightsV3", "word_count: {0}", profile.word_count);

			if (!string.IsNullOrEmpty(profile.word_count_message))
				Log.Debug("TestPersonalityInsightsV3", "word_count_message: {0}", profile.word_count_message);

			if (profile.personality != null && profile.personality.Length > 0)
			{
				Log.Debug("TestPersonalityInsightsV3", "Personality trait tree");
				foreach (TraitTreeNode node in profile.personality)
					LogTraitTree(node);
			}

			if (profile.values != null && profile.values.Length > 0)
			{
				Log.Debug("TestPersonalityInsightsV3", "Values trait tree");
				foreach (TraitTreeNode node in profile.values)
					LogTraitTree(node);
			}

			if (profile.needs != null && profile.personality.Length > 0)
			{
				Log.Debug("TestPersonalityInsightsV3", "Needs trait tree");
				foreach (TraitTreeNode node in profile.needs)
					LogTraitTree(node);
			}

			if (profile.behavior != null && profile.behavior.Length > 0)
			{
				Log.Debug("TestPersonalityInsightsV3", "Behavior tree");
				foreach (BehaviorNode behavior in profile.behavior)
				{
					Log.Debug("TestPersonalityInsightsV3", "trait_id: {0}", behavior.trait_id);
					Log.Debug("TestPersonalityInsightsV3", "name: {0}", behavior.name);
					Log.Debug("TestPersonalityInsightsV3", "category: {0}", behavior.category);
					Log.Debug("TestPersonalityInsightsV3", "percentage: {0}", behavior.percentage.ToString());
					Log.Debug("TestPersonalityInsightsV3", "----------------");
				}
			}

			if (profile.consumption_preferences != null && profile.consumption_preferences.Length > 0)
			{
				Log.Debug("TestPersonalityInsightsV3", "ConsumptionPreferencesCategories");
				foreach (ConsumptionPreferencesCategoryNode categoryNode in profile.consumption_preferences)
					LogConsumptionPreferencesCategory(categoryNode);
			}
		}
	}

	private void OnGetProfileJson(Profile profile, string data)
	{
		if (profile != null)
		{
			if (!string.IsNullOrEmpty(profile.processed_language))
				Log.Debug("TestPersonalityInsightsV3", "processed_language: {0}", profile.processed_language);

			Log.Debug("TestPersonalityInsightsV3", "word_count: {0}", profile.word_count);

			if (!string.IsNullOrEmpty(profile.word_count_message))
				Log.Debug("TestPersonalityInsightsV3", "word_count_message: {0}", profile.word_count_message);

			if (profile.personality != null && profile.personality.Length > 0)
			{
				Log.Debug("TestPersonalityInsightsV3", "Personality trait tree");
				foreach (TraitTreeNode node in profile.personality)
					LogTraitTree(node);
			}

			if (profile.values != null && profile.values.Length > 0)
			{
				Log.Debug("TestPersonalityInsightsV3", "Values trait tree");
				foreach (TraitTreeNode node in profile.values)
					LogTraitTree(node);
			}

			if (profile.needs != null && profile.personality.Length > 0)
			{
				Log.Debug("TestPersonalityInsightsV3", "Needs trait tree");
				foreach (TraitTreeNode node in profile.needs)
					LogTraitTree(node);
			}

			if (profile.behavior != null && profile.behavior.Length > 0)
			{
				Log.Debug("TestPersonalityInsightsV3", "Behavior tree");
				foreach (BehaviorNode behavior in profile.behavior)
				{
					Log.Debug("TestPersonalityInsightsV3", "trait_id: {0}", behavior.trait_id);
					Log.Debug("TestPersonalityInsightsV3", "name: {0}", behavior.name);
					Log.Debug("TestPersonalityInsightsV3", "category: {0}", behavior.category);
					Log.Debug("TestPersonalityInsightsV3", "percentage: {0}", behavior.percentage.ToString());
					Log.Debug("TestPersonalityInsightsV3", "----------------");
				}
			}

			if (profile.consumption_preferences != null && profile.consumption_preferences.Length > 0)
			{
				Log.Debug("TestPersonalityInsightsV3", "ConsumptionPreferencesCategories");
				foreach (ConsumptionPreferencesCategoryNode categoryNode in profile.consumption_preferences)
					LogConsumptionPreferencesCategory(categoryNode);
			}
		}
	}

	private void LogTraitTree(TraitTreeNode traitTreeNode)
	{
		Log.Debug("TestPersonalityInsightsV3", "trait_id: {0} | name: {1} | category: {2} | percentile: {3} | raw_score: {4}",
			string.IsNullOrEmpty(traitTreeNode.trait_id) ? "null" : traitTreeNode.trait_id,
			string.IsNullOrEmpty(traitTreeNode.name) ? "null" : traitTreeNode.name,
			string.IsNullOrEmpty(traitTreeNode.category) ? "null" : traitTreeNode.category,
			string.IsNullOrEmpty(traitTreeNode.percentile.ToString()) ? "null" : traitTreeNode.percentile.ToString(),
			string.IsNullOrEmpty(traitTreeNode.raw_score.ToString()) ? "null" : traitTreeNode.raw_score.ToString());

		if (traitTreeNode.children != null && traitTreeNode.children.Length > 0)
			foreach (TraitTreeNode childNode in traitTreeNode.children)
				LogTraitTree(childNode);
	}

	private void LogConsumptionPreferencesCategory(ConsumptionPreferencesCategoryNode categoryNode)
	{
		Log.Debug("TestPersonalityInsightsV3", "consumption_preference_category_id: {0} | name: {1}", categoryNode.consumption_preference_category_id, categoryNode.name);

		foreach (ConsumptionPreferencesNode preferencesNode in categoryNode.consumption_preferences)
			Log.Debug("TestPersonalityInsightsV3", "\t consumption_preference_id: {0} | name: {1} | score: {2}",
				string.IsNullOrEmpty(preferencesNode.consumption_preference_id) ? "null" : preferencesNode.consumption_preference_id,
				string.IsNullOrEmpty(preferencesNode.name) ? "null" : preferencesNode.name,
				string.IsNullOrEmpty(preferencesNode.score.ToString()) ? "null" : preferencesNode.score.ToString());
	}
```

### Document Conversion
The IBM Watson™ [Document conversion][document_conversion] service converts a single HTML, PDF, or Microsoft Word™ document into a normalized HTML, plain text, or a set of JSON-formatted Answer units that can be used with other Watson services. Carefully inspect output to make sure that it contains all elements and metadata required by the security standards of you or your organization.

#### Converting Documents
Convert a single document

```cs
private DocumentConversion m_DocumentConversion = new DocumentConversion();

	void Start ()
    {
        LogSystem.InstallDefaultReactors(); LogSystem.InstallDefaultReactors();
        string examplePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/watson_beats_jeopardy.html";

        if (!m_DocumentConversion.ConvertDocument(OnConvertDocument, examplePath, ConversionTarget.NORMALIZED_TEXT))
            Log.Debug("ExampleDocumentConversion", "Document conversion failed!");
    }

    private void OnConvertDocument(ConvertedDocument documentConversionResponse, string data)
    {
        if (documentConversionResponse != null)
        {
            if(!string.IsNullOrEmpty(documentConversionResponse.media_type_detected))
                Log.Debug("ExampleDocumentConversion", "mediaTypeDetected: {0}", documentConversionResponse.media_type_detected);
            if (!string.IsNullOrEmpty(documentConversionResponse.source_document_id))
                Log.Debug("ExampleDocumentConversion", "mediaTypeDetected: {0}", documentConversionResponse.source_document_id);
            if(!string.IsNullOrEmpty(documentConversionResponse.timestamp))
                Log.Debug("ExampleDocumentConversion", "mediaTypeDetected: {0}", documentConversionResponse.timestamp);
            if (documentConversionResponse.metadata != null && documentConversionResponse.metadata.Length > 0)
            {
                Log.Debug("ExampleDocumentConversion", "mediaTypeDetected: {0}", documentConversionResponse.metadata.Length);
                foreach (Metadata metadata in documentConversionResponse.metadata)
                    Log.Debug("ExampleDocumentConversion", "metadata | name: {0}, content: {1}", metadata.name, metadata.content);
            }
            if (documentConversionResponse.answer_units != null && documentConversionResponse.answer_units.Length > 0)
            {
                Log.Debug("ExampleDocumentConversion", "mediaTypeDetected: {0}", documentConversionResponse.answer_units.Length);
                foreach (AnswerUnit answerUnit in documentConversionResponse.answer_units)
                {
                Log.Debug("ExampleDocumentConversion", "answerUnit | type: {0}, title: {1}, parent_id: {2}, id: {3}, direction: {4}", answerUnit.type, answerUnit.title, answerUnit.parent_id, answerUnit.id, answerUnit.direction);
                    if (answerUnit.content != null && answerUnit.content.Length > 0)
                        foreach (Content content in answerUnit.content)
                            Log.Debug("ExampleDocumentConversion", "content | mediaType: {0}, text: {1}", content.media_type, content.text);
                }
            }

            if (!string.IsNullOrEmpty(documentConversionResponse.htmlContent))
                Log.Debug("ExampleDocumentConversion", "HTMLContent: {0}", documentConversionResponse.htmlContent);
            if (!string.IsNullOrEmpty(documentConversionResponse.textContent))
                Log.Debug("ExampleDocumentConversion", "TextContent: {0}", documentConversionResponse.textContent);
        }
    }
```


### AlchemyData News
Use the [AlchemyData News][alchemyData_news] service to provide news and blog content enriched with natural language processing to allow for highly targeted search and trend analysis. Now you can query the world's news sources and blogs like a database.

#### Getting News
AlchemyData News indexes 250k to 300k English language news and blog articles every day with historical search available for the past 60 days. You can query the News API directly with no need to acquire, enrich and store the data yourself - enabling you to go beyond simple keyword-based searches. You can request which fields to return and filter fields by string. All available fields can be accessed using Fields constants in the AlchemyData News Data Model.

```
private AlchemyAPI m_AlchemyAPI = new AlchemyAPI();

void Start()
{
	string[] returnFields = {Fields.ENRICHED_URL_ENTITIES, Fields.ENRICHED_URL_KEYWORDS};
	Dictionary<string, string> queryFields = new Dictionary<string, string>();
	queryFields.Add(Fields.ENRICHED_URL_RELATIONS_RELATION_SUBJECT_TEXT, "Obama");
	queryFields.Add(Fields.ENRICHED_URL_CLEANEDTITLE, "Washington");

	if (!m_AlchemyAPI.GetNews(OnGetNews, returnFields, queryFields))
        Log.Debug("ExampleAlchemyData", "Failed to get news!");
}

void OnGetNews(NewsResponse newsData, string data)
{
	//	Access requested fields
	if(newsData != null)
		Log.Debug("ExampleAlchemyData", "status: {0}", newsData.status);
}
```

### Retrieve and Rank
The IBM Watson™ [Retrieve and Rank][retrieve_and_rank] service combines two information retrieval components in a single service: the power of Apache Solr and a sophisticated machine learning capability. This combination provides users with more relevant results by automatically reranking them by using these machine learning algorithms.

####  Getting clusters
Retrieves the list of Solr clusters for the service instance.

```cs
void Start()
{
  Log.Debug("ExampleRetrieveAndRank", "Attempting to get clusters!");
  if (!m_RetrieveAndRank.GetClusters(OnGetClusters))
    Log.Debug("ExampleRetrieveAndRank", "Failed to get clusters!");
}

private void OnGetClusters(SolrClusterListResponse resp, string data)
{
    if (resp != null)
    {
        foreach (SolrClusterResponse cluster in resp.clusters)
            Log.Debug("ExampleRetrieveAndRank", "OnGetClusters | cluster name: {0}, size: {1}, ID: {2}, status: {3}.", cluster.cluster_name, cluster.cluster_size, cluster.solr_cluster_id, cluster.solr_cluster_status);
    }
    else
    {
        Log.Debug("ExampleRetrieveAndRank", "OnGetClusters | Get Cluster Response is null!");
    }
}
```

####  Creating a cluster
Provisions a Solr cluster asynchronously. When the operation is successful, the status of the cluster is set to NOT_AVAILABLE. The status must be READY before you can use the cluster.

```cs
void Start()
{
  Log.Debug("ExampleRetrieveAndRank", "Attempting to create cluster!");
  if (!m_RetrieveAndRank.CreateCluster(OnCreateCluster, "unity-test-cluster", "1"))
      Log.Debug("ExampleRetrieveAndRank", "Failed to create cluster!");
}

private void OnCreateCluster(SolrClusterResponse resp, string data)
{
    if (resp != null)
    {
        Log.Debug("ExampleRetrieveAndRank", "OnCreateClusters | name: {0}, size: {1}, ID: {2}, status: {3}.", resp.cluster_name, resp.cluster_size, resp.solr_cluster_id, resp.solr_cluster_status);
    }
    else
    {
        Log.Debug("ExampleRetrieveAndRank", "OnCreateClusters | Get Cluster Response is null!");
    }
}
```

####  Deleting a cluster
Stops and deletes a Solr Cluster asynchronously.

```cs
void Start()
{
  string clusterToDelete = "<cluster-to-delete>";
  Log.Debug("ExampleRetrieveAndRank", "Attempting to delete cluster {0}!", clusterToDelete);
  if (!m_RetrieveAndRank.DeleteCluster(OnDeleteCluster, clusterToDelete))
      Log.Debug("ExampleRetrieveAndRank", "Failed to delete cluster!");
}

private void OnDeleteCluster(bool success, string data)
{
    if (success)
    {
        Log.Debug("ExampleRetrieveAndRank", "OnDeleteCluster | Success!");
    }
    else
    {
        Log.Debug("ExampleRetrieveAndRank", "OnDeleteCluster | Failure!");
    }
}

```

####  Get a cluster
Returns status and other information about a cluster.

```cs
void Start()
{
  Log.Debug("ExampleRetrieveAndRank", "Attempting to get cluster {0}!", <testClusterID>);
  if (!m_RetrieveAndRank.GetCluster(OnGetCluster, testClusterID))
      Log.Debug("ExampleRetrieveAndRank", "Failed to get cluster!");
}

private void OnGetCluster(SolrClusterResponse resp, string data)
{
    if (resp != null)
    {
        Log.Debug("ExampleRetrieveAndRank", "OnGetClusters | name: {0}, size: {1}, ID: {2}, status: {3}.", resp.cluster_name, resp.cluster_size, resp.solr_cluster_id, resp.solr_cluster_status);
    }
    else
    {
        Log.Debug("ExampleRetrieveAndRank", "OnGetClusters | Get Cluster Response is null!");
    }
}
```

####  Listing cluster configs
Retrieves all configurations for a cluster.

```cs
void Start()
{
  Log.Debug("ExampleRetrieveAndRank", "Attempting to get cluster configs for {0}!", <testClusterID>);
  if (!m_RetrieveAndRank.GetClusterConfigs(OnGetClusterConfigs, <testClusterID>))
      Log.Debug("ExampleRetrieveAndRank", "Failed to get cluster configs!");
}

private void OnGetClusterConfigs(SolrConfigList resp, string data)
{
    if (resp != null)
    {
        if (resp.solr_configs.Length == 0)
            Log.Debug("ExampleRetrieveAndRank", "OnGetClusterConfigs | no cluster configs!");

        foreach (string config in resp.solr_configs)
            Log.Debug("ExampleRetrieveAndRank", "OnGetClusterConfigs | solr_config: " + config);
    }
    else
    {
        Log.Debug("ExampleRetrieveAndRank", "OnGetClustersConfigs | Get Cluster Configs Response is null!");
    }
}
```

####  Deleting a cluster config
Deletes the configuration for a cluster. Before you delete the configuration, delete any collections that point to it.

```cs
void Start()
{
  string clusterConfigToDelete = "test-config";
  Log.Debug("ExampleRetrieveAndRank", "Attempting to delete cluster {0} config {1}!", <testClusterID>, clusterConfigToDelete);
  if (!m_RetrieveAndRank.DeleteClusterConfig(OnDeleteClusterConfig, <testClusterID>, clusterConfigToDelete))
      Log.Debug("ExampleRetriveAndRank", "Failed to delete cluster config {0}", clusterConfigToDelete);
}

private void OnDeleteClusterConfig(bool success, string data)
{
    if (success)
    {
        Log.Debug("ExampleRetrieveAndRank", "OnDeleteClusterConfig | Success!");
    }
    else
    {
        Log.Debug("ExampleRetrieveAndRank", "OnDeleteClusterConfig | Failure!");
    }
}
```

####  Getting a cluster config
Retrieves the configuration for a cluster by its name.

```cs
void Start()
{
  Log.Debug("ExampleRetrieveAndRank", "Attempting to get cluster {0} config {1}!", <testClusterID>, <testClusterConfigName>);
  if (!m_RetrieveAndRank.GetClusterConfig(OnGetClusterConfig, <testClusterID>, <testClusterConfigName>))
      Log.Debug("ExampleRetrieveAndRank", "Failed to get cluster config {0}!", <testClusterConfigName>);
}

private void OnGetClusterConfig(byte[] respData, string data)
{
    if(respData != null)
    {
        Log.Debug("ExampleRetrieveAndRank", "OnGetClusterConfig | success!");
        string currentDirectory = Application.dataPath;
        var path = EditorUtility.SaveFilePanel("Save Config", currentDirectory, "config", "zip");
        if (!string.IsNullOrEmpty(path))
        {
            currentDirectory = Path.GetDirectoryName(path);
            m_RetrieveAndRank.SaveConfig(OnSaveConfig, respData, path, data);
        }
    }
    else
        Log.Debug("ExampleRetrieveAndRank", "OnGetClusterConfig | respData is null!");
}

private void OnSaveConfig(bool success, string data)
{
    if (success)
        Log.Debug("ExampleRetrieveAndRank", "OnSaveConfig | success!");
    else
        Log.Debug("ExampleRetrieveAndRank", "OnSaveConfig | fail!");
}
```

####  Uploading a cluster config
Uploads a zip file containing the configuration files for your Solr collection. The zip file must include schema.xml, solrconfig.xml, and other files you need for your configuration. Configuration files on the zip file's path are not uploaded. The request fails if a configuration with the same name exists. To update an existing config, use the [Solr configuration API](https://cwiki.apache.org/confluence/display/solr/Config+API).

```cs
void Start()
{
  Log.Debug("ExampleRetrieveAndRank", "Attempting to upload cluster {0} config {1}!", <testClusterID>, <testClusterConfigName>);
  if (!m_RetrieveAndRank.UploadClusterConfig(OnUploadClusterConfig, <testClusterID>, <testClusterConfigName>, <testClusterConfigPath>))
      Log.Debug("ExampleRetrieveAndRank", "Failed to upload cluster config {0}!", <testClusterConfigPath>);
}

private void OnUploadClusterConfig(UploadResponse resp, string data)
{
    if (resp != null)
    {
        Log.Debug("ExampleRetrieveAndRank", "OnUploadClusterConfig | Success! {0}, {1}", resp.message, resp.statusCode);
    }
    else
    {
        Log.Debug("ExampleRetrieveAndRank", "OnUploadClusterConfig | Failure!");
    }
}
```

####  List collection request
An example of a method that forwards to the [Solr Collections API](https://cwiki.apache.org/confluence/display/solr/Collections+API). This Retrieve and Rank resource improves error handling and resiliency of the Solr Collections API.

```cs
void Start()
{
  Log.Debug("ExampleRetrieveAndRank", "Attempting to list collections!");
  if (!m_RetrieveAndRank.ForwardCollectionRequest(OnGetCollections, <testClusterID>, CollectionsAction.LIST))
      Log.Debug("ExampleRetrieveAndRank", "Failed to get collections!");
}

private void OnGetCollections(CollectionsResponse resp, string data)
{
    if(resp != null)
    {
        if(resp.responseHeader != null)
            Log.Debug("ExampleRetrieveAndRank", "OnGetCollections | status: {0}, QTime: {1}.", resp.responseHeader.status, resp.responseHeader.QTime);
        if(resp.collections != null)
        {
            if (resp.collections.Length == 0)
                Log.Debug("ExampleRetrieveAndRank", "OnGetCollections | There are no collections!");
            else
                foreach (string collection in resp.collections)
                    Log.Debug("ExampleRetrieveAndRank", "\tOnGetCollections | collection: {0}", collection);
        }
    }
    else
    {
        Log.Debug("ExampleRetrieveAndRank", "OnGetCollections | GetCollections Response is null!");
    }
}
```

####  Create Collection request
An example of a method that forwards to the [Solr Collections API](https://cwiki.apache.org/confluence/display/solr/Collections+API). This Retrieve and Rank resource improves error handling and resiliency of the Solr Collections API.

```cs
void Start()
{
  Log.Debug("ExampleRetrieveAndRank", "Attempting to create collection!");
  if (!m_RetrieveAndRank.ForwardCollectionRequest(OnGetCollections, <testClusterID>, CollectionsAction.CREATE, "TestCollectionToDelete", <testClusterConfigName>))
      Log.Debug("ExampleRetrieveAndRank", "Failed to create collections!");
}

private void OnGetCollections(CollectionsResponse resp, string data)
{
    if(resp != null)
    {
        if(resp.responseHeader != null)
            Log.Debug("ExampleRetrieveAndRank", "OnGetCollections | status: {0}, QTime: {1}.", resp.responseHeader.status, resp.responseHeader.QTime);
        if(resp.collections != null)
        {
            if (resp.collections.Length == 0)
                Log.Debug("ExampleRetrieveAndRank", "OnGetCollections | There are no collections!");
            else
                foreach (string collection in resp.collections)
                    Log.Debug("ExampleRetrieveAndRank", "\tOnGetCollections | collection: {0}", collection);
        }
    }
    else
    {
        Log.Debug("ExampleRetrieveAndRank", "OnGetCollections | GetCollections Response is null!");
    }
}
```

####  Delete Collection request
An example of a method that forwards to the [Solr Collections API](https://cwiki.apache.org/confluence/display/solr/Collections+API). This Retrieve and Rank resource improves error handling and resiliency of the Solr Collections API.

```cs
void Start()
{
  Log.Debug("ExampleRetrieveAndRank", "Attempting to delete collection!");
  if (!m_RetrieveAndRank.ForwardCollectionRequest(OnGetCollections, <testClusterID>, CollectionsAction.DELETE, "TestCollectionToDelete"))
      Log.Debug("ExampleRetrieveAndRank", "Failed to delete collections!");
}

private void OnGetCollections(CollectionsResponse resp, string data)
{
    if(resp != null)
    {
        if(resp.responseHeader != null)
            Log.Debug("ExampleRetrieveAndRank", "OnGetCollections | status: {0}, QTime: {1}.", resp.responseHeader.status, resp.responseHeader.QTime);
        if(resp.collections != null)
        {
            if (resp.collections.Length == 0)
                Log.Debug("ExampleRetrieveAndRank", "OnGetCollections | There are no collections!");
            else
                foreach (string collection in resp.collections)
                    Log.Debug("ExampleRetrieveAndRank", "\tOnGetCollections | collection: {0}", collection);
        }
    }
    else
    {
        Log.Debug("ExampleRetrieveAndRank", "OnGetCollections | GetCollections Response is null!");
    }
}
```

####  Index documents
Adds content to a Solr index so you can search it.

An example of a method that forwards to Solr. For more information about indexing, see [Indexing and Basic Data Operations](https://cwiki.apache.org/confluence/display/solr/Indexing+and+Basic+Data+Operations) in the Apache Solr Reference.

You must commit your documents to the index to search for them. For more information about when to commit, see [UpdateHandlers in SolrConfig](https://cwiki.apache.org/confluence/display/solr/UpdateHandlers+in+SolrConfig) in the Solr Reference.

```cs
void Start()
{
  Log.Debug("ExampleRetrieveAndRank", "Attempting to index documents!");
  if (!m_RetrieveAndRank.IndexDocuments(OnIndexDocuments, <indexDataPath>, <testClusterID>, <testCollectionName>))
      Log.Debug("ExampleRetrieveAndRank", "Failed to index documents!");
}

private void OnIndexDocuments(IndexResponse resp, string data)
{
    if(resp != null)
    {
        if (resp.responseHeader != null)
            Log.Debug("ExampleRetrieveAndRank", "OnIndexDocuments | status: {0}, QTime: {1}", resp.responseHeader.status, resp.responseHeader.QTime);
        else
            Log.Debug("ExampleRetrieveAndRank", "OnIndexDocuments | Response header is null!");
    }
    else
    {
        Log.Debug("ExampleRetrieveAndRank", "OnIndexDocuments | response is null!");
    }
}
```

####  Standard Search and Ranked Search
Forwards to the Solr [standard query parser](https://cwiki.apache.org/confluence/display/solr/The+Standard+Query+Parser).

```cs
void Start()
{
  Log.Debug("ExampleRetrieveAndRank", "Attempting to search!");
  string[] fl = { "title", "id", "body", "author", "bibliography" };
  if (!m_RetrieveAndRank.Search(OnSearch, <testClusterID>, <testCollectionName>, <testQuery>, fl))
      Log.Debug("ExampleRetrieveAndRank", "Failed to search!");

  Log.Debug("ExampleRetrieveAndRank", "Attempting to search!");
  string[] fl = { "title", "id", "body", "author", "bibliography" };
  if (!m_RetrieveAndRank.Search(OnSearch, <testClusterID>, <testCollectionName>, <testQuery>, fl, true, <rankerID>))
      Log.Debug("ExampleRetrieveAndRank", "Failed to search!");
}

private void OnSearch(SearchResponse resp, string data)
{
    if(resp != null)
    {
        if(resp.responseHeader != null)
        {
            Log.Debug("ExampleRetrieveAndRank", "Search | status: {0}, QTime: {1}.", resp.responseHeader.status, resp.responseHeader.QTime);
            if (resp.responseHeader._params != null)
                Log.Debug("ExampleRetrieveAndRank", "\tSearch | params.q: {0}, params.fl: {1}, params.wt: {2}.", resp.responseHeader._params.q, resp.responseHeader._params.fl, resp.responseHeader._params.wt);
            else
                Log.Debug("ExampleRetrieveAndRank", "Search | responseHeader.params is null!");
        }
        else
        {
            Log.Debug("ExampleRetrieveAndRank", "Search | response header is null!");
        }

        if (resp.response != null)
        {
            Log.Debug("ExampleRetrieveAndRank", "Search | numFound: {0}, start: {1}.", resp.response.numFound, resp.response.start);
            if(resp.response.docs != null)
            {
                if (resp.response.docs.Length == 0)
                    Log.Debug("ExampleRetrieveAndRank", "Search | There are no docs!");
                else
                    foreach (Doc doc in resp.response.docs)
                    {
                        Log.Debug("ExampleRetrieveAndRank", "\tSearch | id: {0}.", doc.id);

                        if (doc.title != null)
                        {
                            if (doc.title.Length == 0)
                                Log.Debug("ExampleRetrieveAndRank", "Search | There are no title");
                            else
                                foreach (string s in doc.title)
                                    Log.Debug("ExampleRetrieveAndRank", "\tSearch | title: {0}.", s);
                        }
                        else
                        {
                            Log.Debug("ExampleRetrieveAndRank", "Search | title is null");
                        }

                        if (doc.author != null)
                        {
                            if (doc.author.Length == 0)
                                Log.Debug("ExampleRetrieveAndRank", "Search | There are no authors");
                            else
                                foreach (string s in doc.author)
                                    Log.Debug("ExampleRetrieveAndRank", "\tSearch | Author: {0}.", s);
                        }
                        else
                        {
                            Log.Debug("ExampleRetrieveAndRank", "Search | Authors is null");
                        }

                        if (doc.body != null)
                        {
                            if (doc.body.Length == 0)
                                Log.Debug("ExampleRetrieveAndRank", "Search | There are no body");
                            else
                                foreach (string s in doc.body)
                                    Log.Debug("ExampleRetrieveAndRank", "\tSearch | body: {0}.", s);
                        }
                        else
                        {
                            Log.Debug("ExampleRetrieveAndRank", "Search | Body is null");
                        }

                        if (doc.bibliography != null)
                        {
                            if (doc.bibliography.Length == 0)
                                Log.Debug("ExampleRetrieveAndRank", "Search | There are no bibliographies");
                            else
                                foreach (string s in doc.bibliography)
                                    Log.Debug("ExampleRetrieveAndRank", "\tSearch | bibliography: {0}.", s);
                        }
                        else
                        {
                            Log.Debug("ExampleRetrieveAndRank", "Search | Bibliography is null");
                        }
                    }
            }
            else
            {
                Log.Debug("ExampleRetrieveAndRank", "Search | docs are null!");
            }
        }
        else
        {
            Log.Debug("ExampleRetrieveAndRank", "Search | response is null!");
        }
    }
    else
    {
        Log.Debug("ExampleRetrieveAndRank", "Search response is null!");
    }
}
```

####  Getting rankers
Retrieves the list of rankers for the service instance.

```cs
void Start()
{
  Log.Debug("ExampleRetrieveAndRank", "Attempting to get rankers!");
  if (!m_RetrieveAndRank.GetRankers(OnGetRankers))
      Log.Debug("ExampleRetrieveAndRank", "Failed to get rankers!");
}

private void OnGetRankers(ListRankersPayload resp, string data)
{
    if (resp != null)
    {
        if (resp.rankers.Length == 0)
            Log.Debug("ExampleRetrieveAndRank", "OnGetRankers | no rankers!");
        foreach (RankerInfoPayload ranker in resp.rankers)
            Log.Debug("ExampleRetrieveAndRank", "OnGetRankers | ranker name: {0}, ID: {1}, created: {2}, url: {3}.", ranker.name, ranker.ranker_id, ranker.created, ranker.url);
    }
    else
    {
        Log.Debug("ExampleRetrieveAndRank", "OnGetRankers | Get Ranker Response is null!");
    }
}
```

####  Creating a ranker
Sends data to create and train a ranker and returns information about the new ranker.

When the operation is successful, the status of the ranker is set to Training. The status must be Available before you can use the ranker.

```cs
void Start()
{
  Log.Debug("ExampleRetrieveAndRank", "Attempting to create rankers!");
  if (!m_RetrieveAndRank.CreateRanker(OnCreateRanker, <testRankerTrainingPath>, "testRanker"))
      Log.Debug("ExampleRetrieveAndRank", "Failed to create ranker!");
}

private void OnCreateRanker(RankerStatusPayload resp, string data)
{
	if (resp != null)
	{
		Log.Debug("ExampleRetrieveAndRank", "OnCreateRanker | ID: {0}, url: {1}, name: {2}, created: {3}, status: {4}, statusDescription: {5}.", resp.ranker_id, resp.url, resp.name, resp.created, resp.status, resp.status_description);
	}
	else
	{
		Log.Debug("ExampleRetrieveAndRank", "OnCreateRanker | Get Cluster Response is null!");
	}
}
```

####  Rank
Returns the top answer and a list of ranked answers with their ranked scores and confidence values. Use the [Get information about a ranker](http://www.ibm.com/watson/developercloud/retrieve-and-rank/api/v1/#get_status) method to retrieve the status.

Use this method to return answers when you train the ranker with custom features. However, in most cases, you can use the [Search and rank](http://www.ibm.com/watson/developercloud/retrieve-and-rank/api/v1/#query_ranker) method.

```cs
void Start()
{
  Log.Debug("ExampleRetrieveAndRank", "Attempting to rank!");
  if (!m_RetrieveAndRank.Rank(OnRank, <testRankerID>, <testAnswerDataPath>))
      Log.Debug("ExampleRetriveAndRank", "Failed to rank!");
}

private void OnRank(RankerOutputPayload resp, string data)
{
	if (resp != null)
	{
		Log.Debug("ExampleRetrieveAndRank", "OnRank | ID: {0}, url: {1}, name: {2}, top_answer: {3}.", resp.ranker_id, resp.url, resp.name, resp.top_answer);
		if (resp.answers != null)
			if (resp.answers.Length == 0)
			{
				Log.Debug("ExampleRetrieveAndRank", "\tThere are no answers!");
			}
			else
			{
				foreach (RankedAnswer answer in resp.answers)
					Log.Debug("ExampleRetrieveAndRank", "\tOnRank | answerID: {0}, score: {1}, confidence: {2}.", answer.answer_id, answer.score, answer.confidence);
			}
	}
	else
	{
		Log.Debug("ExampleRetrieveAndRank", "OnRank | Rank response is null!");
	}
}
```

####  Deleting rankers
Deletes a ranker.

```cs
void Start()
{
  Log.Debug("ExampleRetriveAndRank", "Attempting to delete ranker {0}!", <rankerToDelete>);
  if (!m_RetrieveAndRank.DeleteRanker(OnDeleteRanker, <rankerToDelete>))
      Log.Debug("ExampleRetrieveAndRank", "Failed to delete ranker {0}!", <rankerToDelete>);
}

private void OnDeleteRanker(bool success, string data)
{
	if (success)
	{
		Log.Debug("ExampleRetrieveAndRank", "OnDeleteRanker | Success!");
	}
	else
	{
		Log.Debug("ExampleRetrieveAndRank", "OnDeleteRanker | Failure!");
	}
}
```

####  Getting ranker info
Returns status and other information about a ranker.

```cs
void Start()
{
  Log.Debug("ExampleRetrieveAndRank", "Attempting to get Ranker Info!");
  if (!m_RetrieveAndRank.GetRanker(OnGetRanker, <testRankerID>))
      Log.Debug("ExampleRetrieveAndRank", "Failed to get ranker!");
}

private void OnGetRanker(RankerStatusPayload resp, string data)
{
	if(resp != null)
	{
		Log.Debug("ExampleRetrieveAndRank", "GetRanker | ranker_id: {0}, url: {1}, name: {2}, created: {3}, status: {4}, status_description: {5}.", resp.ranker_id, resp.url, resp.name, resp.created, resp.status, resp.status_description);
	}
	else
	{
		Log.Debug("ExampleRetrieveAndRank", "GetRanker | GetRanker response is null!");
	}
}
```


## Developing a basic application in one minute
You can quickly develop a basic application that uses the Speech to Text service and the Natural Language Classifier service by using the prefabs that come with the SDK. Ensure that you prepare the test data before you complete the the following steps:

  1. Create a new scene and drag the following prefabs from **Assets -> Watson -> Prefabs**, and drop them in the Hierarchy tab:

    * MicWidget
    * SpeechToTextWidget
    * Natural Language Classifier Widget
    * ClassDisplayWidget

  2. Select the **Natural Language Classifier Widget**.
  5. In the **Classifier Name** field in the Inspector tab, specify `TestNaturalLanguageClassifier`.
  6. In the Natural Language Classifier Editor, expand the **Test Natural Language Classifier** , expand the classes, and determine which questions about the weather to ask to test the classifier.
  7. Run the application.
  8. Say your questions into the microphone to test the MicWidget, the SpeechToTextWidget, and the NaturalLanguageClassifierWidget.

## Documentation
To read the documentation you need to have a **chm reader** installed. Open the documentation by selecting API Reference the Watson menu (**Watson -> API Reference**).

## Questions

If you are having difficulties using the APIs or have a question about the IBM Watson Services, please ask a question on
[dW Answers](https://developer.ibm.com/answers/questions/ask/?topics=watson)
or [Stack Overflow](http://stackoverflow.com/questions/ask?tags=ibm-watson).

## Open Source @ IBM
Find more open source projects on the [IBM Github Page](http://ibm.github.io/).

## License
This library is licensed under Apache 2.0. Full license text is available in [LICENSE](LICENSE).

## Contributing
See [CONTRIBUTING.md](.github/CONTRIBUTING.md).

[wdc]: http://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/
[wdc_unity_sdk]: https://github.com/watson-developer-cloud/unity-sdk
[latest_release]: https://github.com/watson-developer-cloud/unity-sdk/releases/latest
[bluemix_registration]: http://bluemix.net/registration
[get_unity]: https://unity3d.com/get-unity

[speech_to_text]: http://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/doc/speech-to-text/
[text_to_speech]: http://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/doc/text-to-speech/
[language_translator]: http://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/doc/language-translator/
[dialog]: http://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/doc/dialog/
[natural_language_classifier]: http://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/doc/nl-classifier/

[alchemy_language]: http://www.alchemyapi.com/products/alchemylanguage
[alchemyData_news]: http://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/alchemy-data-news.html
[sentiment_analysis]: http://www.alchemyapi.com/products/alchemylanguage/sentiment-analysis
[tone_analyzer]: http://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/doc/tone-analyzer/
[tradeoff_analytics]: http://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/doc/tradeoff-analytics/
[conversation]:http://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/doc/conversation/
[visual_recognition]: http://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/visual-recognition/api/v3/
[personality_insights]: http://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/personality-insights/api/v2/
[conversation_tooling]: https://www.ibmwatsonconversation.com
[retrieve_and_rank]: http://www.ibm.com/watson/developercloud/retrieve-and-rank/api/v1/
[document_conversion]: http://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/document-conversion/api/v1/
[expressive_ssml]: http://www.ibm.com/watson/developercloud/doc/text-to-speech/http.shtml#expressive
[ssml]: http://www.ibm.com/watson/developercloud/doc/text-to-speech/SSML.shtml

[dialog_service]: http://www.ibm.com/watson/developercloud/doc/dialog/
[dialog_migration]: https://www.ibm.com/watson/developercloud/doc/conversation/migration.shtml
[conversation_service]: https://www.ibm.com/watson/developercloud/doc/conversation/
