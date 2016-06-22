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
* [Developing a basic application in one minute](#developing-a-basic-application-in-one-minute)
* [Documentation](#documentation)
* [License](#license)
* [Contributing](#contributing)

## Before you begin
Ensure that you have the following prerequisites:
* An IBM Bluemix account. If you don't have one, [sign up][bluemix_registration].
* [Unity][get_unity]. You win! You can use the **free** Personal edition.
* Change the build settings in Unity (**File > Build Settings**) to PC, Mac & Linux Standalone. Click PC, Mac & Linux Standalone and click the Switch Platform button.

## Getting the Watson SDK and adding it to Unity
You can get the latest SDK release by clicking [here][latest_release].

### Installing the SDK source into your Unity project
Move the `unity-sdk` directory into the Assets directory of the Unity project. **Rename the SDK directory from `unity-sdk` to 'Watson'.**

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
Use the [Text to Speech][text_to_speech] service to get the available voices to synthesize.

```cs
TextToSpeech m_TextToSpeech = new TextToSpeech();
string m_TestString = "Hello! This is Text to Speech!";

void Start ()
{
  m_TextToSpeech.Voice = VoiceType.en_GB_Kate;
  m_TextToSpeech.ToSpeech(m_TestString, HandleToSpeechCallback);
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
Converse with Watson using the [Dialog][dialog] service. Upload a dialog by following instructions on [Uploading Dialogs](#uploading-dialogs) and replace the DialogID in m_DialogID below with the uploaded dialog's DialogID.

```cs
private Dialog m_Dialog = new Dialog();
private string m_DialogID = <DialogID>;

void Start ()
{
  Debug.Log("User: 'Hello'");
  m_Dialog.Converse(m_DialogID, "Hello", OnConverse);
}

private void OnConverse(ConverseResponse resp)
{
  foreach (string r in resp.response)
    Debug.Log("Watson: " + r);
}
```
#### Uploading dialogs
You can upload dialogs by using the Dialog Editor.
  1. Click **Watson -> Dialog Editor**. The Dialog Editor window is displayed.
  2. Specify a **unique** name for the dialog in the **Name** field.
  3. Click **Upload**.
  4. Navigate to the dialog file to be uploaded, and click **Open**.

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
With the IBM Watson™ [Conversation][conversation] service you can create cognitive agents - virtual agents that combine machine learning, natural language understanding, and integrated dialog scripting tools to provide outstanding customer engagements.

```cs
private Conversation m_Conversation = new Conversation();
private string m_WorkspaceID = "car_demo_1";
private string m_Input = "Can you unlock the door?";

void Start () {
	Debug.Log("User: " + m_Input);
	m_Conversation.Message(m_WorkspaceID, m_Input, OnMessage);
}

void OnMessage (DataModels.MessageResponse resp)
{
	foreach(DataModels.MessageIntent mi in resp.intents)
		Debug.Log("intent: " + mi.intent + ", confidence: " + mi.confidence);

	Debug.Log("response: " + resp.output.text);
}
```


### Visual Recognition
Use the [Visual Recognition][visual_recognition] service to classify an image against a default or custom trained classifier. In addition, the service can detect faces and text in an image. Instead of credentials, the Visual Recognition key ("VISUAL\_RECOGNITION\_API\_KEY") must be set as a variable in the Advanced Mode of the Config Editor (**Watson -> Configuration Editor**). The ServiceID (VisualRecognitionV3) and endpoint URL (https://gateway-a.watsonplatform.net/visual-recognition/api) must also be added manually.

![visual-recognition0](http://g.recordit.co/Qke2gKfaKJ.gif)

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
Train a new classifier by uploading image data. Two compressed zip files containing at least two positive example files or one positive and one negative example file. The prefix of the positive example file is used as the classname for the new classifier ("<Class Name>_positive_examples"). Negative examples zip must be named "negative_examples". After a successful call, training the classifier takes a few minutes.

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
### Personality Insights
The IBM Watson™ [Personality Insights][personality_insights] service enables applications to derive insights from social media, enterprise data, or other digital communications. The service uses linguistic analytics to infer individuals' intrinsic personality characteristics, including Big Five, Needs, and Values, from digital communications such as email, text messages, tweets, and forum posts. The service can automatically infer, from potentially noisy social media, portraits of individuals that reflect their personality characteristics. 

```cs
PersonalityInsights m_personalityInsights = new PersonalityInsights();

void Start () {
	string dataPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/personalityInsights.json";
	
	if(!m_personalityInsights.GetProfile(OnGetProfile, dataPath, DataModels.ContentType.TEXT_PLAIN, DataModels.Language.ENGLISH))
            Log.Debug("ExamplePersonalityInsights", "Failed to get profile!");
}

private void OnGetProfile(DataModels.Profile profile, string data)
{
    Log.Debug("ExamplePersonalityInsights", "data: {0}", data);
    if(profile != null)
    {
        if(!string.IsNullOrEmpty(profile.id))
            Log.Debug("ExamplePersonalityInsights", "id: {0}", profile.id);
        if(!string.IsNullOrEmpty(profile.source))
            Log.Debug("ExamplePersonalityInsights", "source: {0}", profile.source);
        if(!string.IsNullOrEmpty(profile.processed_lang))
            Log.Debug("ExamplePersonalityInsights", "proccessed_lang: {0}", profile.processed_lang);
        if(!string.IsNullOrEmpty(profile.word_count))
            Log.Debug("ExamplePersonalityInsights", "word_count: {0}", profile.word_count);
        if(!string.IsNullOrEmpty(profile.word_count_message))
            Log.Debug("ExamplePersonalityInsights", "word_count_message: {0}", profile.word_count_message);

        if(profile.tree != null)
        {
            LogTraitTree(profile.tree);
        }
    }
    else
    {
        Log.Debug("ExamplePersonalityInsights", "Failed to get profile!");
    }
}

private void LogTraitTree(DataModels.TraitTreeNode traitTreeNode)
{
    if(!string.IsNullOrEmpty(traitTreeNode.id))
        Log.Debug("ExamplePersonalityInsights", "id: {0}", traitTreeNode.id);
    if(!string.IsNullOrEmpty(traitTreeNode.name))
        Log.Debug("ExamplePersonalityInsights", "name: {0}", traitTreeNode.name);
    if(!string.IsNullOrEmpty(traitTreeNode.category))
        Log.Debug("ExamplePersonalityInsights", "category: {0}", traitTreeNode.category);
    if(!string.IsNullOrEmpty(traitTreeNode.percentage))
        Log.Debug("ExamplePersonalityInsights", "percentage: {0}", traitTreeNode.percentage);
    if(!string.IsNullOrEmpty(traitTreeNode.sampling_error))
        Log.Debug("ExamplePersonalityInsights", "sampling_error: {0}", traitTreeNode.sampling_error);
    if(!string.IsNullOrEmpty(traitTreeNode.raw_score))
        Log.Debug("ExamplePersonalityInsights", "raw_score: {0}", traitTreeNode.raw_score);
    if(!string.IsNullOrEmpty(traitTreeNode.raw_sampling_error))
        Log.Debug("ExamplePersonalityInsights", "raw_sampling_error: {0}", traitTreeNode.raw_sampling_error);
    if(traitTreeNode.children != null && traitTreeNode.children.Length > 0)
        foreach(DataModels.TraitTreeNode childNode in traitTreeNode.children)
            LogTraitTree(childNode);
}
```




### Alchemy Language
Use the [Alchemy Language][alchemy_language] service to extract semantic meta-data from content such as information on people, places, companies, topics, facts, relationships, authors and languages. Instead of credentials, the Alchemy API Key ("ALCHEMY\_API\_KEY") must be set as a variable in the Advanced Mode of the Config Editor (**Watson -> Configuration Editor**). The ServiceID (AlchemyLanguageV1) and endpoint URL (https://gateway-a.watsonplatform.net) must also be added manually.

![alchemy-language0](http://g.recordit.co/xkGArdMVbC.gif)

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

## Developing a basic application in one minute
You can quickly develop a basic application that uses the Speech to Text service and the Natural Language Classifier service by using the prefabs that come with the SDK. Ensure that you prepare the test data before you complete the the following steps:
  1. Create a new scene and drag the following prefabs from **Assets -> Watson -> Prefabs**, and drop them in the Hierarchy tab:
    * MicWidget
    * SpeechToTextWidget
    * Natural Language Classifier Widget
    * ClassDisplayWidget
  2. Select the **Natural Language Classifier Widget**.
  5. In the **Classifier Name** field in the Inspector tab, specify 'TestNaturalLanguageClassifier'.
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
[latest_release]: https://github.com/watson-developer-cloud/unity-sdk/archive/0.3.0.zip
[bluemix_registration]: http://bluemix.net/registration
[get_unity]: https://unity3d.com/get-unity

[speech_to_text]: http://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/doc/speech-to-text/
[text_to_speech]: http://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/doc/text-to-speech/
[language_translator]: http://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/doc/language-translator/
[dialog]: http://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/doc/dialog/
[natural_language_classifier]: http://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/doc/nl-classifier/

[alchemy_language]: http://www.alchemyapi.com/products/alchemylanguage
[sentiment_analysis]: http://www.alchemyapi.com/products/alchemylanguage/sentiment-analysis
[tone_analyzer]: http://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/doc/tone-analyzer/
[tradeoff_analytics]: http://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/doc/tradeoff-analytics/
[conversation]:http://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/doc/conversation/
[visual_recognition]: http://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/visual-recognition/api/v3/
[personality_insights]: http://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/personality-insights/api/v2/
