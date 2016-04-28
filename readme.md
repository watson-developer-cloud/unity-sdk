# Watson Developer Cloud Unity SDK

Use this SDK to build Watson-powered applications in Unity. It comes with a set of prefabs that you can use to develop a simple Watson application in just one minute.

## Table of Contents
  * [Before you begin](#before-you-begin)
  * [Getting the Watson SDK and adding it to Unity](#getting-the-watson-sdk-and-adding-it-to-unity)
    * [Installing the SDK source into your Unity project](#installing-the-sdk-source-into-your-unity-project)
        * [git clone](#git-clone)
        * [git submodule](#git-submodule)
        * [zip archive](#zip-archive)
* [Configuring your service credentials](#configuring-your-service-credentials)
* [Preparing the test data for developing a basic application](#preparing-the-test-data-for-developing-a-basic-application)
* [Developing a basic application in one minute](#developing-a-basic-application-in-one-minute)
* [Dialog and classifier management](#dialog-and-classifier-management)
    * [Uploading dialogs](#uploading-dialogs)
    * [Managing classifiers](#managing-classifiers)
        * [Importing files for existing classifiers](#importing-files-for-existing-classifiers)
        * [Creating new classifiers](#creating-new-classifiers)
        * [Editing and training classifiers](#editing-and-training-classifiers)
* [IBM Watson Services](#ibm-watson-services)
    * [Speech to Text](#speech-to-text)
    * [Text to Speech](#text-to-speech)
    * [Language Translation](#language-translation)
    * [Dialog](#dialog)
    * [Natural Language Classifier](#natural-language-classifier)
* [Documentation](#documentation)
* [License](#license)
* [Contributing](#contributing)

## Before you begin
Ensure that you have the following prerequisites:
* An IBM Bluemix account. If you don't have one, [sign up][bluemix_registration].
* [Unity][get_unity]. You win! You can use the **free** Personal edition.
* Change the build settings to PC, Mac & Linux Standalone. Click File > Build Settings > PC, Mac & Linux Standalone and click the Switch Platform button.

## Getting the Watson SDK and adding it to Unity
You can get the SDK from the [github repository](wdc_unity_sdk).

### Installing the SDK source into your Unity project
    1. Clone the following Git repository into the Assets directory within your current Unity project using one of the following methods.

#### git clone 
```sh
git clone git@github.com:watson-developer-cloud/unity-sdk.git Assets/Watson/
```

#### git submodule
```sh
git submodule add https://github.com/watson-developer-cloud/unity-sdk.git Assets/Watson/
```

#### zip archive
    1. Click on the "Download Zip" button from the [github repository](wdc_unity_sdk). 
    2. Extract the zip file into your Assets directory.
    3. Rename the directory from "unity-sdk" to "Watson".


## Configuring your service credentials
1. Determine which services to configure.
2. If you have configured the services already, complete the following steps. Otherwise, go to step 3.
      1. Navigate to the **Dashboard** on your Bluemix account.
      2. Click the **tile** for a service.
      3. Click **Service Credentials**.
      4. Copy the content in the **Service Credentials** field, and paste it in the credentials field in the Config Editor (**Watson -> Config Editor**) in Unity.
      5. Click **Apply Credentials**.
      6. Repeat steps 1 - 5 for each service you want to use.
3. If you need to configure the services that you want to use, complete the following steps.
      1. In the Config Editor (**Watson -> Config Editor**), click the **Configure** button beside the service to register. The service window is displayed.
      2. Click **Create**.
      3. Click **Service Credentials**.
      4. Copy the content in the **Service Credentials** field, and paste it in the empty credentials field in the **Config Editor** in Unity.
      5. Click **Apply Credentials**.
      6. Repeat steps 1 - 5 for each service you want to use.
4. Click **Save**, and close the Config Editor.

## Preparing the test data for developing a basic application
The SDK contains a Test Natural Language Classifier, which contains classes for temperature and conditions. Before you develop a sample application in the next section, train the classifier on the test data.

1. Open the Natural Language Classifier Editor by clicking **Watson -> Natural Language Classifier Editor**.
2. Locate the Test Natural Language Classifier, and click **Train**. The training process begins. The process lasts a few minutes.
3. To check the status of the training process, click **Refresh**. When the status changes from Training to Available, the process is finished, and you can begin to develop a basic application that uses the Natural Language Classifier, as described in the next section.

## Developing a basic application in one minute
You can quickly develop a basic application that uses the Speech to Text service and the Natural Language Classifier service by using the prefabs that come with the SDK. Ensure that you prepare the test data before you complete the the following steps:
1. Create a new scene and drag the following prefabs from **Assets -> Watson -> Prefabs**, and drop them in the Hierarchy tab:
    * MicWidget
    * SpeechToTextWidget
    * Natural Language Classifier Widget
    * ClassDisplayWidget
2. Select the **Natural Language Classifier Widget**.
5. In the **Classifier Name** field in the Inspector tab, specify 'TestNaturalLanguageClassifier'.
6. In the Natural Language Classifier Editor, expand the **Test Natural Languge Classifier** , expand the classes, and determine which questions about the weather to ask to test the classifier.
7. Run the application.
8. Say your questions into the microphone to test the MicWidget, the SpeechToTextWidget, and the NaturalLanguageClassifierWidget.

## Dialog and classifier management
The SDK contains editors for managing your dialogs and classifiers.

### Uploading dialogs
You can upload dialogs by using the Dialog Editor.
1. Click **Watson -> Dialog Editor**. The Dialog Editor window is displayed.
2. Specify a **unique** name for the dialog in the **Name** field.
3. Click **Upload**.
4. Navigate to the dialog file to be uploaded, and click **Open**.

### Managing classifiers
You can use the Natural Language Classifier Editor to import and export classifier files, and create new classifiers and edit them.

#### Importing files for existing classifiers
1. Click **Watson -> Natural Language Classifier Editor**. The Natural Language Classifier Editor window is displayed.
2. Click **Import**.
3. Navigate to the '.csv' file to import, and click **Open**. The file is imported.
4. Click **Train**.

#### Creating new classifiers
1. Click **Watson -> Natural Language Classifier Editor**. The Natural Language Classifier Editor window is displayed.
2. In the **Name** field, specify a name for the classifier.
3. Click **Create**.

#### Editing and training classifiers
1. Click **Watson -> Natural Language Classifier Editor**. The Natural Language Classifier Editor window is displayed.
2. Expand the classifier.
3. To create a new class, specity a name for the new class in the empty field, and click **Add Class**.
4. To add a phrase to the class, specify a phrase in the empty field, and click **Add Phrase**.
5. Click **Train**.


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

### Language Translation
Select a domain, then identify or select the language of text, and then translate the text from one supported language to another.  
Example: Ask how to get to the disco in Spanish using [Language Translation][language_translation] service.

```cs
private LanguageTranslation m_Translate = new LanguageTranslation();
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

### Natural Language Classifier
Use [Natural Language Classifier][natural_language_classifier] service to create a classifier instance by providing a set of representative strings and a set of one or more correct classes for each as training. Then use the trained classifier to classify your new question for best matching answers or to retrieve next actions for your application. Create and train a Natural Language Classifier instance by following the instructions in [Creating new classifiers](#creating-new-classifiers) and [Editing and training classifiers](#editing-and-training-classifiers). Replace the ClassifierID below with the Natural Language Classifier instance's ClassifierID. 

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

## Documentation
To read the documentation you need to have a **chm reader** installed. Open the documentation by selcting API Reference the Watson menu (Watson -> API Reference).

## Questions

If you are having difficulties using the APIs or have a question about the IBM Watson Services, please ask a question on
[dW Answers](https://developer.ibm.com/answers/questions/ask/?topics=watson)
or [Stack Overflow](http://stackoverflow.com/questions/ask?tags=ibm-watson).

## Open Source @ IBM
Find more open source projects on the [IBM Github Page](http://ibm.github.io/)

## License
This library is licensed under Apache 2.0. Full license text is
available in [LICENSE](LICENSE).

## Contributing
See [CONTRIBUTING.md](.github/CONTRIBUTING.md).

[wdc]: http://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/
[wdc_unity_sdk]: https://github.com/watson-developer-cloud/unity-sdk
[bluemix_registration]: https://apps.admin.ibmcloud.com/manage/trial/bluemix.html?cm_mmc=WatsonDeveloperCloud-_-LandingSiteGetStarted-_-x-_-CreateAnAccountOnBluemixCLI
[get_unity]: https://unity3d.com/get-unity

[speech_to_text]: http://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/doc/speech-to-text/
[text_to_speech]: http://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/doc/text-to-speech/
[language_translation]: http://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/doc/language-translation/
[dialog]: http://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/doc/dialog/
[natural_language_classifier]: http://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/doc/nl-classifier/

[alchemy_language]: http://www.alchemyapi.com/products/alchemylanguage
[sentiment_analysis]: http://www.alchemyapi.com/products/alchemylanguage/sentiment-analysis
[tone_analyzer]: http://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/doc/tone-analyzer/
[tradeoff_analytics]: http://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/doc/tradeoff-analytics/
