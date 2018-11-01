# IBM Watson SDK for Unity
[![Build Status](https://travis-ci.org/watson-developer-cloud/unity-sdk.svg?branch=develop)](https://travis-ci.org/watson-developer-cloud/unity-sdk)
[![wdc-community.slack.com](https://wdc-slack-inviter.mybluemix.net/badge.svg)](http://wdc-slack-inviter.mybluemix.net/)
[![semantic-release](https://img.shields.io/badge/%20%20%F0%9F%93%A6%F0%9F%9A%80-semantic--release-e10079.svg)](https://github.com/semantic-release/semantic-release)
[![CLA assistant](https://cla-assistant.io/readme/badge/watson-developer-cloud/unity-sdk)](https://cla-assistant.io/watson-developer-cloud/unity-sdk)

Use this SDK to build Watson-powered applications in Unity.

<details>
  <summary>Table of Contents</summary>

  * [Before you begin](#before-you-begin)
  * [Getting the Watson SDK and adding it to Unity](#getting-the-watson-sdk-and-adding-it-to-unity)
    * [Installing the SDK source into your Unity project](#installing-the-sdk-source-into-your-unity-project)
  * [Configuring your service credentials](#configuring-your-service-credentials)
  * [Authentication](#authentication)
  * [Watson Services](#watson-services)
  * [Authentication Tokens](#authentication-tokens)
  * [Documentation](#documentation)
  * [Questions](#questions)
  * [Open Source @ IBM](#open-source--ibm)
  * [License](#license)
  * [Contributing](#contributing)

</details>

## Before you begin
Ensure that you have the following prerequisites:

* You need an [IBM Cloud][ibm-cloud-onboarding] account.
* [Unity][get_unity]. You can use the **free** Personal edition.

## Configuring Unity
* Change the build settings in Unity (**File > Build Settings**) to any platform except for web player/Web GL. The IBM Watson SDK for Unity does not support Unity Web Player.
* If using Unity 2018.2 or later you'll need to set Scripting Runtime Version in Build Settings to .NET 4.x equivalent. We need to access security options to enable TLS 1.2. 

## Getting the Watson SDK and adding it to Unity
You can get the latest SDK release by clicking [here][latest_release].

### Installing the SDK source into your Unity project
Move the **`unity-sdk`** directory into the **`Assets`** directory of your Unity project. _Optional: rename the SDK directory from `unity-sdk` to `Watson`_.

## Configuring your service credentials
To create instances of Watson services and their credentials, follow the steps below.

**Note:** Service credentials are different from your IBM Cloud account username and password.

1. Determine which services to configure.
1. If you have configured the services already, complete the following steps. Otherwise, go to step 3.
    1. Log in to IBM Cloud at https://console.bluemix.net.
    1. Click the service you would like to use.
    1. Click **Service credentials**.
    1. Click **View credentials** to access your credentials.
1. If you need to configure the services that you want to use, complete the following steps.
    1. Log in to IBM Cloud at https://console.bluemix.net.
    1. Click the **Create service** button.
    1. Under **Watson**, select which service you would like to create an instance of and click that service.
    1. Give the service and credential a name. Select a plan and click the **Create** button on the bottom.
    4. Click **Service Credentials**.
    5. Click **View credentials** to access your credentials.
1. Your service credentials can be used to instantiate Watson Services within your application. Most services also support tokens which you can instantiate the service with as well.

The credentials for each service contain either a `username`, `password` and endpoint `url` **or** an `apikey` and endpoint `url`.

**WARNING:** You are responsible for securing your own credentials. Any user with your service credentials can access your service instances!

## Watson Services
To get started with the Watson Services in Unity, click on each service below to read through each of their `README.md`'s and their codes.
* [Assistant V1](/Scripts/Services/Assistant/v1)
* [Assistant V2](/Scripts/Services/Assistant/v2) (private beta)
* [Conversation](/Scripts/Services/Conversation/v1) (deprecated - Use Assistant V1 or Assistant V2)
* [Discovery](/Scripts/Services/Discovery/v1)
* [Language Translator V2](/Scripts/Services/LanguageTranslator/v2) (deprecated - Use LanguageTranslator V3)
* [Language Translator V3](/Scripts/Services/LanguageTranslator/v3)
* [Natural Language Classifier](/Scripts/Services/NaturalLanguageClassifier/v2)
* [Natural Language Understanding](/Scripts/Services/NaturalLanguageUnderstanding/v1)
* [Personality Insights](/Scripts/Services/PersonalityInsights/v3)
* [Speech to Text](/Scripts/Services/SpeechToText/v1)
* [Text to Speech](/Scripts/Services/TextToSpeech/v1)
* [Tone Analyzer](/Scripts/Services/ToneAnalyzer/v3)
* [Visual Recognition](/Scripts/Services/VisualRecognition/v3)

## Authentication
Watson services are migrating to token-based Identity and Access Management (IAM) authentication.

- With some service instances, you authenticate to the API by using **[IAM](#iam)**.
- In other instances, you authenticate by providing the **[username and password](#username-and-password)** for the service instance.

### Getting credentials
To find out which authentication to use, view the service credentials. You find the service credentials for authentication the same way for all Watson services:

1.  Go to the IBM Cloud **[Dashboard][watson-dashboard]** page.
1.  Either click an existing Watson service instance or click **Create**.
1.  Click **Show** to view your service credentials.
1.  Copy the `url` and either `apikey` or `username` and `password`.

In your code, you can use these values in the service constructor or with a method call after instantiating your service.

### IAM

Some services use token-based Identity and Access Management (IAM) authentication. IAM authentication uses a service API key to get an access token that is passed with the call. Access tokens are valid for approximately one hour and must be regenerated.

You supply either an IAM service **API key** or an **access token**:

- Use the API key to have the SDK manage the lifecycle of the access token. The SDK requests an access token, ensures that the access token is valid, and refreshes it if necessary.
- Use the access token if you want to manage the lifecycle yourself. For details, see [Authenticating with IAM tokens](https://console.bluemix.net/docs/services/watson/getting-started-iam.html). If you want to switch to API key, in a coroutine, override your stored IAM credentials with an IAM API key and yield until the credentials object `HasIamTokenData()` returns `true`.

#### Supplying the IAM API key
```cs
IEnumerator TokenExample()
{
    //  Create IAM token options and supply the apikey. IamUrl is the URL used to get the 
    //  authorization token using the IamApiKey. It defaults to https://iam.bluemix.net/identity/token
    TokenOptions iamTokenOptions = new TokenOptions()
    {
        IamApiKey = "<iam-api-key>",
        IamUrl = "<iam-url>"
    };

    //  Create credentials using the IAM token options
    _credentials = new Credentials(iamTokenOptions, "<service-url>");
    while (!_credentials.HasIamTokenData())
        yield return null;

    _assistant = new Assistant(_credentials);
    _assistant.VersionDate = "2018-02-16";
    _assistant.ListWorkspaces(OnListWorkspaces, OnFail);
}

private void OnListWorkspaces(WorkspaceCollection response, Dictionary<string, object> customData)
{
    Log.Debug("OnListWorkspaces()", "Response: {0}", customData["json"].ToString());
}

private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
{
    Log.Debug("OnFail()", "Failed: {0}", error.ToString());
}
```

#### Supplying the access token
```cs
void TokenExample()
{
    //  Create IAM token options and supply the access token.
    TokenOptions iamTokenOptions = new TokenOptions()
    {
        IamAccessToken = "<iam-access-token>"
    };

    //  Create credentials using the IAM token options
     _credentials = new Credentials(iamTokenOptions, "<service-url");

    _assistant = new Assistant(_credentials);
    _assistant.VersionDate = "2018-02-16";
    _assistant.ListWorkspaces(OnListWorkspaces, OnFail);
}

private void OnListWorkspaces(WorkspaceCollection response, Dictionary<string, object> customData)
{
    Log.Debug("OnListWorkspaces()", "Response: {0}", customData["json"].ToString());
}

private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
{
    Log.Debug("OnFail()", "Failed: {0}", error.ToString());
}
```

### Username and password
```cs
using IBM.Watson.DeveloperCloud.Services.Assistant.v1;
using IBM.Watson.DeveloperCloud.Utilities;

void Start()
{
    Credentials credentials = new Credentials(<username>, <password>, <url>);
    Assistant _assistant = new Assistant(credentials);
}
```

## Callbacks
Success and failure callbacks are required. You can specify the return type in the callback.  
```cs
private void Example()
{
    //  Call with sepcific callbacks
    assistant.Message(OnMessage, OnGetEnvironmentsFail, _workspaceId, "");
    discovery.GetEnvironments(OnGetEnvironments, OnFail);
}

//  OnMessage callback
private void OnMessage(object resp, Dictionary<string, object> customData)
{
    Log.Debug("ExampleCallback.OnMessage()", "Response received: {0}", customData["json"].ToString());
}

//  OnGetEnvironments callback
private void OnGetEnvironments(GetEnvironmentsResponse resp, Dictionary<string, object> customData)
{
    Log.Debug("ExampleCallback.OnGetEnvironments()", "Response received: {0}", customData["json"].ToString());
}

//  OnMessageFail callback
private void OnMessageFail(RESTConnector.Error error, Dictionary<string, object> customData)
{
    Log.Error("ExampleCallback.OnMessageFail()", "Error received: {0}", error.ToString());
}

//  OnGetEnvironmentsFail callback
private void OnGetEnvironmentsFail(RESTConnector.Error error, Dictionary<string, object> customData)
{
    Log.Error("ExampleCallback.OnGetEnvironmentsFail()", "Error received: {0}", error.ToString());
}
```

Since the success callback signature is generic and the failure callback always has the same signature, you can use a single set of callbacks to handle multiple calls.
```cs
private void Example()
{
    //  Call with generic callbacks
    assistant.Message(OnSuccess, OnMessageFail, "<workspace-id>", "");
    discovery.GetEnvironments(OnSuccess, OnFail);
}

//  Generic success callback
private void OnSuccess<T>(T resp, Dictionary<string, object> customData)
{
    Log.Debug("ExampleCallback.OnSuccess()", "Response received: {0}", customData["json"].ToString());
}

//  Generic fail callback
private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
{
    Log.Error("ExampleCallback.OnFail()", "Error received: {0}", error.ToString());
}
```

## Custom data
Custom data can be passed through a `Dictionary<string, object> customData` in each call. In most cases, the raw json response is returned in the customData under `"json"` entry. In cases where there is no returned json, the entry will contain the success and http response code of the call.

```cs
void Example()
{
    Dictionary<string, object> customData = new Dictionary<string, object>();
    customData.Add("foo", "bar");
    assistant.Message(OnSuccess, OnFail, "<workspace-id>", "", customData);
}

//  Generic success callback
private void OnSuccess<T>(T resp, Dictionary<string, object> customData)
{
    Log.Debug("ExampleCustomData.OnSuccess()", "Custom Data: {0}", customData["foo"].ToString());  // returns "bar"
}

//  Generic fail callback
private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
{
    Log.Error("ExampleCustomData.OnFail()", "Error received: {0}", error.ToString());  // returns error string
    Log.Debug("ExampleCustomData.OnFail()", "Custom Data: {0}", customData["foo"].ToString());  // returns "bar"
}
```

## Custom Request Headers
You can send custom request headers by adding them to the `customData` object.

```cs
void Example()
{
    //  Create customData object
    Dictionary<string, object> customData = new Dictionary<string, object>();
    //  Create a dictionary of custom headers
    Dictionary<string, string> customHeaders = new Dictionary<string, string>();
    //  Add to the header dictionary
    customHeaders.Add("X-Watson-Metadata", "customer_id=some-assistant-customer-id");
    //  Add the header dictionary to the custom data object
    customData.Add(Constants.String.CUSTOM_REQUEST_HEADERS, customHeaders);

    assistant.Message(OnSuccess, OnFail, "<workspace-id>", customData: customData);
}
```

## Response Headers
You can get responseheaders in the `customData` object in the callback.

```cs
void Example()
{
    assistant.Message(OnMessage, OnFail, "<workspace-id>");
}

private void OnMessage(object resp, Dictionary<string, object> customData)
{
    //  List all headers in the response headers object
    if (customData.ContainsKey(Constants.String.RESPONSE_HEADERS))
    {
        foreach (KeyValuePair<string, string> kvp in customData[Constants.String.RESPONSE_HEADERS] as Dictionary<string, string>)
        {
            Log.Debug("ExampleCustomHeader.OnMessage()", "{0}: {1}", kvp.Key, kvp.Value);
        }
    }
}
```

## Authentication Tokens

**Authenticating with the `X-Watson-Authorization-Token` header is deprecated. The token continues to work with Cloud Foundry services, but is not supported for services that use Identity and Access Management (IAM) authentication. For details see [Authenticating with IAM tokens](https://console.bluemix.net/docs/services/watson/getting-started-iam.html#iam) or the [README](#IAM) in the IBM Watson SDK you use.**

You use tokens to write applications that make authenticated requests to IBM Watson™ services without embedding service credentials in every call.

You can write an authentication proxy in IBM Cloud that obtains and returns a token to your client application, which can then use the token to call the service directly. This proxy eliminates the need to channel all service requests through an intermediate server-side application, which is otherwise necessary to avoid exposing your service credentials from your client application.

```cs
using IBM.Watson.DeveloperCloud.Services.Assistant.v1;
using IBM.Watson.DeveloperCloud.Utilities;

void Start()
{
    Credentials credentials = new Credentials(<service-url>)
    {
        AuthenticationToken = <authentication-token>
    };
    Assistant _assistant = new Assistant(credentials);
}
```

There is a helper class included to obtain tokens from within your Unity application.

```cs
using IBM.Watson.DeveloperCloud.Utilities;

AuthenticationToken _authenticationToken;

void Start()
{
    if (!Utility.GetToken(OnGetToken, <service-url>, <service-username>, <service-password>))
        Log.Debug("ExampleGetToken.Start()", "Failed to get token.");
}

private void OnGetToken(AuthenticationToken authenticationToken, string customData)
{
    _authenticationToken = authenticationToken;
    Log.Debug("ExampleGetToken.OnGetToken()", "created: {0} | time to expiration: {1} minutes | token: {2}", _authenticationToken.Created, _authenticationToken.TimeUntilExpiration, _authenticationToken.Token);
}
```

## Streaming outside of US South region
Watson services have upgraded their hosts to TLS 1.2. The US South region has a TLS 1.0 endpoint that will work for streaming but if you are streaming in other regions you will need to use Unity 2018.2 and set Scripting Runtime Version in Build Settings to .NET 4.x equivalent. In lower versions of Unity you will need to create the Speech to Text instance in US South.

## Documentation
Documentation can be found [here][documentation]. You can also access the documentation by selecting API Reference the Watson menu (**Watson -> API Reference**).

## Getting started videos
You can view Getting Started videos for the IBM Watson SDK for Unity on [YouTube](https://www.youtube.com/watch?v=Sa5hmukwHV8&list=PLZDyxLlNKRY8MdgiUq45cZztn_f0EVz6c).

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

[wdc]: https://www.ibm.com/watson/developer/
[wdc_unity_sdk]: https://github.com/watson-developer-cloud/unity-sdk
[latest_release]: https://github.com/watson-developer-cloud/unity-sdk/releases/latest
[get_unity]: https://unity3d.com/get-unity
[documentation]: https://watson-developer-cloud.github.io/unity-sdk/
[ibm-cloud-onboarding]: http://console.bluemix.net/registration?target=/developer/watson&cm_sp=WatsonPlatform-WatsonServices-_-OnPageNavLink-IBMWatson_SDKs-_-Unity
[watson-dashboard]: https://console.bluemix.net/dashboard/apps?category=watson
