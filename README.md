# Watson Developer Cloud Unity SDK
[![Build Status](https://travis-ci.org/watson-developer-cloud/unity-sdk.svg?branch=develop)](https://travis-ci.org/watson-developer-cloud/unity-sdk)

Use this SDK to build Watson-powered applications in Unity.

## Table of Contents
* [Before you begin](#before-you-begin)
* [Getting the Watson SDK and adding it to Unity](#getting-the-watson-sdk-and-adding-it-to-unity)
  * [Installing the SDK source into your Unity project](#installing-the-sdk-source-into-your-unity-project)
* [Configuring your service credentials](#configuring-your-service-credentials)
* [Documentation](#documentation)
* [License](#license)
* [Contributing](#contributing)

## Before you begin
Ensure that you have the following prerequisites:

* An IBM Bluemix account. If you don't have one, [sign up][bluemix_registration].
* [Unity][get_unity]. You can use the **free** Personal edition.
* Change the build settings in Unity (**File > Build Settings**) to any platform except for web player. The Watson Developer Cloud Unity SDK does not support Unity Web Player.

## Getting the Watson SDK and adding it to Unity
You can get the latest SDK release by clicking [here][latest_release].

### Installing the SDK source into your Unity project
Move the **`unity-sdk`** directory into the Assets directory of the Unity project. **Rename the SDK directory from `unity-sdk` to `Watson`.**

## Configuring your service credentials
You will need the 'username' and 'password' credentials for each service. Service credentials are different from your Bluemix account username and password. 

1. Determine which services to configure.
1. If you have configured the services already, complete the following steps. Otherwise, go to step 3.
    1. Log in to Bluemix at https://bluemix.net.
    1. Click the service you would like to use.
    1. Click **Service credentials**.
    1. Click **View credentials** to access your credentials.
1. If you need to configure the services that you want to use, complete the following steps.
    1. Log in to Bluemix at https://bluemix.net.
    1. Click the **Create service** button.
    1. Under **Watson**, select which service you would like to create an instnace of and click that service.
    1. Give the service and credential a name. Select a plan and click the **Create** button on the bottom.
    4. Click **Service Credentials**.
    5. Click **View credentials** to access your credentials.
1. Your service credentials can be used to instantiate Watson Services within your application. Most services also support tokens which you can instantiate the service with as well.

**Note:** You are responsible for securing your own credentials. Any user with your service credentials can access your service instances!

## Watson Services
* [Speech to Text](/Scripts/Services/SpeechToText/v1)
* [Text to Speech](/Scripts/Services/TextToSpeech/v1)
* [Language Translator](/Scripts/Services/LanguageTranslator/v2)
* [Natural Language Classifier](/Scripts/Services/NaturalLanguageClassifier/v2)
* [Tone Analyzer](/Scripts/Services/ToneAnalyzer/v3)
* [Tradeoff Analytics](/Scripts/Services/TradeoffAnalytics/v1)
* [Conversation](/Scripts/Services/Conversation/v1)
* [Visual Recognition](/Scripts/Services/VisualRecognition/v3)
* [Alchemy Language](/Scripts/Services/AlchemyAPI/v1)
* [Personality Insights](/Scripts/Services/PersonalityInsights/v3)
* [Document Conversion](/Scripts/Services/DocumentConversion/v1)
* [Retrieve and Rank](/Scripts/Services/RetrieveAndRank/v1)
* [Discovery](/Scripts/Services/Discovery/v1)
* [Natural Language Understanding](/Scripts/Services/NaturalLanguageUnderstanding/v1)

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

[wdc]: http://www.ibm.com/watson/developercloud/
[wdc_unity_sdk]: https://github.com/watson-developer-cloud/unity-sdk
[latest_release]: https://github.com/watson-developer-cloud/unity-sdk/releases/latest
[bluemix_registration]: http://bluemix.net/registration
[get_unity]: https://unity3d.com/get-unity

[speech_to_text]: http://www.ibm.com/watson/developercloud/doc/speech-to-text/
[text_to_speech]: http://www.ibm.com/watson/developercloud/doc/text-to-speech/
[language_translator]: http://www.ibm.com/watson/developercloud/doc/language-translator/index.html
[dialog]: http://www.ibm.com/watson/developercloud/doc/dialog/
[natural_language_classifier]: http://www.ibm.com/watson/developercloud/doc/natural-language-classifier/index.html

[alchemy_language]: http://www.alchemyapi.com/products/alchemylanguage
[alchemyData_news]: http://www.ibm.com/watson/developercloud/alchemy-data-news.html
[sentiment_analysis]: http://www.alchemyapi.com/products/alchemylanguage/sentiment-analysis
[tone_analyzer]: http://www.ibm.com/watson/developercloud/doc/tone-analyzer/
[tradeoff_analytics]: http://www.ibm.com/watson/developercloud/doc/tradeoff-analytics/
[conversation]:http://www.ibm.com/watson/developercloud/doc/conversation/
[visual_recognition]: http://www.ibm.com/watson/developercloud/visual-recognition/api/v3/
[personality_insights]: http://www.ibm.com/watson/developercloud/personality-insights/api/v2/
[conversation_tooling]: https://www.ibmwatsonconversation.com
[retrieve_and_rank]: http://www.ibm.com/watson/developercloud/retrieve-and-rank/api/v1/
[discovery]: http://www.ibm.com/watson/developercloud/discovery/api/v1/
[document_conversion]: http://www.ibm.com/watson/developercloud/document-conversion/api/v1/
[expressive_ssml]: http://www.ibm.com/watson/developercloud/doc/text-to-speech/http.shtml#expressive
[ssml]: http://www.ibm.com/watson/developercloud/doc/text-to-speech/SSML.shtml
[discovery-query]: http://www.ibm.com/watson/developercloud/doc/discovery/using.shtml
[natural_language_understanding]: https://www.ibm.com/watson/developercloud/natural-language-understanding.html
[nlu_models]: https://www.ibm.com/watson/developercloud/doc/natural-language-understanding/customizing.html
[nlu_entities]: https://www.ibm.com/watson/developercloud/natural-language-understanding/api/v1/#entities
[nlu_relations]: https://www.ibm.com/watson/developercloud/natural-language-understanding/api/v1/#relations

[dialog_service]: http://www.ibm.com/watson/developercloud/doc/dialog/
[dialog_migration]: https://www.ibm.com/watson/developercloud/doc/conversation/migration.shtml
[conversation_service]: https://www.ibm.com/watson/developercloud/doc/conversation/
