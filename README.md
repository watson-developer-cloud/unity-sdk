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
private string m_PharseToTranslate = "How do I get to the disco?";

void Start ()
{
  Debug.Log("English Phrase to translate: " + m_PharseToTranslate);
  m_Translate.GetTranslation(m_PharseToTranslate, "en", "es", OnGetTranslation);
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
