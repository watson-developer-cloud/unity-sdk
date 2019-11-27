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
  * [Discovery v2 only on CP4D](#discovery-v2-only-on-cp4d)
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
* If using Unity 2018.2 or later you'll need to set **Scripting Runtime Version** and **Api Compatibility Level** in Build Settings to **.NET 4.x equivalent**. We need to access security options to enable TLS 1.2. 

> Updating MacOS to Mojave may disable microphone for Unity. If you are running into this issue please see these [guidelines](https://support.apple.com/en-us/HT209175) to configure microphone for Mojave. 


## Getting the Watson SDK and adding it to Unity
You can get the latest SDK release by clicking [here][latest_release_sdk]. **You will also need to download the latest release of the IBM Unity SDK Core by clicking [here][latest_release_core].**

### Installing the SDK source into your Unity project
Move the **`unity-sdk`** and **`unity-sdk-core`** directories into the **`Assets`** directory of your Unity project. _Optional: rename the SDK directory from `unity-sdk` to `Watson` and the Core directory from `unity-sdk-core` to `IBMSdkCore`_.

## Discovery v2 only on CP4D

Discovery v2 is only available on Cloud Pak for Data.

## Configuring your service credentials
To create instances of Watson services and their credentials, follow the steps below.

**Note:** Service credentials are different from your IBM Cloud account username and password.

1. Determine which services to configure.
1. If you have configured the services already, complete the following steps. Otherwise, go to step 3.
    1. Log in to IBM Cloud at https://cloud.ibm.com.
    1. Click the service you would like to use.
    1. Click **Service credentials**.
    1. Click **View credentials** to access your credentials.
1. If you need to configure the services that you want to use, complete the following steps.
    1. Log in to IBM Cloud at https://cloud.ibm.com.
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
* [Assistant V1](/Scripts/Services/Assistant/V1)
* [Assistant V2](/Scripts/Services/Assistant/V2)
* [Compare Comply V1](/Scripts/Services/CompareComply/V1)
* [Conversation](/Scripts/Services/Conversation/V1) (deprecated - Use Assistant V1 or Assistant V2)
* [Discovery](/Scripts/Services/Discovery/V1)
* [Language Translator V3](/Scripts/Services/LanguageTranslator/V3)
* [Natural Language Classifier](/Scripts/Services/NaturalLanguageClassifier/V2)
* [Natural Language Understanding](/Scripts/Services/NaturalLanguageUnderstanding/V1)
* [Personality Insights](/Scripts/Services/PersonalityInsights/V3)
* [Speech to Text](/Scripts/Services/SpeechToText/V1)
* [Text to Speech](/Scripts/Services/TextToSpeech/V1)
* [Tone Analyzer](/Scripts/Services/ToneAnalyzer/V3)
* [Visual Recognition](/Scripts/Services/VisualRecognition/V3)

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
- Use the access token if you want to manage the lifecycle yourself. For details, see [Authenticating with IAM tokens](https://cloud.ibm.com/docs/services/watson?topic=watson-iam). If you want to switch to API key, in a coroutine, override your stored IAM credentials with an IAM API key and yield until the credentials object `HasIamTokenData()` returns `true`.

#### Supplying the IAM API key
```cs
Authenticator authenticator;
AssistantService assistant;
string versionDate = "<service-version-date>";

IEnumerator TokenExample()
{
    //  Create authenticator using the IAM token options
    authenticator = new IamAuthenticator(apikey: "<iam-api-key>");
    while (!authenticator.CanAuthenticate())
        yield return null;

    assistant = new AssistantService(versionDate, authenticator);
    assistant.SetServiceUrl("<service-url>");
    assistant.ListWorkspaces(callback: OnListWorkspaces);
}

private void OnListWorkspaces(DetailedResponse<WorkspaceCollection> response, IBMError error)
{
    Log.Debug("OnListWorkspaces()", "Response: {0}", response.Response);
}
```

#### Supplying the access token
```cs
Authenticator authenticator;
AssistantService assistant;
string versionDate = "<service-version-date>";

void TokenExample()
{
    //  Create authenticator using the Bearer Token
    authenticator = new BearerTokenAuthenticator("<bearer-token>");

    assistant = new AssistantService(versionDate, authenticator);
    assistant.SetServiceUrl("<service-url>");
    assistant.ListWorkspaces(callback: OnListWorkspaces);
}

private void OnListWorkspaces(DetailedResponse<WorkspaceCollection> response, IBMError error)
{
    Log.Debug("OnListWorkspaces()", "Response: {0}", response.Response);
}
```

### Username and password
```cs
Authenticator authenticator;
AssistantService assistant;
string versionDate = "<service-version-date>";

void UsernamePasswordExample()
{
    Authenticator authenticator = new BasicAuthenticator("<username>", "<password>", "<url>");
    assistant = new AssistantService(versionDate, authenticator);
    assistant.SetServiceUrl("<service-url>");
}
```

### Supplying authenticator

There are two ways to supply the authenticator you found above to the SDK for authentication.

#### Credential file (easier!)

With a credential file, you just need to put the file in the right place and the SDK will do the work of parsing it and authenticating. You can get this file by clicking the **Download** button for the credentials in the **Manage** tab of your service instance.

The file downloaded will be called `ibm-credentials.env`. This is the name the SDK will search for and **must** be preserved unless you want to configure the file path (more on that later). The SDK will look for your `ibm-credentials.env` file in the following places (in order):

- Your system's home directory
- The top-level directory of the project you're using the SDK in

As long as you set that up correctly, you don't have to worry about setting any authentication options in your code. So, for example, if you created and downloaded the credential file for your Discovery instance, you just need to do the following:

```cs
public IEnumerator ExampleAutoService()
{
    Assistant assistantService = new Assistant("2019-04-03");

    //  Wait for authorization token
    while (!assistantService.Authenticator.CanAuthenticate())
        yield return null;

    var listWorkspacesResult = assistantService.ListWorkspaces();
}
```

And that's it!

If you're using more than one service at a time in your code and get two different `ibm-credentials.env` files, just put the contents together in one `ibm-credentials.env` file and the SDK will handle assigning authenticator to their appropriate services.

If you would like to configure the location/name of your credential file, you can set an environment variable called `IBM_CREDENTIALS_FILE`. **This will take precedence over the locations specified above.** Here's how you can do that:

```bash
export IBM_CREDENTIALS_FILE="<path>"
```

where `<path>` is something like `/home/user/Downloads/<file_name>.env`.

#### Manually

If you'd prefer to set authentication values manually in your code, the SDK supports that as well. The way you'll do this depends on what type of authenticator your service instance gives you.

## Callbacks
A success callback is required. You can specify the return type in the callback.
```cs
AssistantService assistant;
string assistantVersionDate = "<assistant-version-date>";
Authenticator assistantAuthenticator;
string workspaceId = "<workspaceId>";

DiscoveryService discovery;
string discoveryVersionDate = "<discovery-version-date>";
Authenticator discoveryAuthenticator;

private void Example()
{
    assistant = new AssistantService(assistantVersionDate, assistantAuthenticator);
    assistant.SetServiceUrl("<service-url>");

    discovery = new DiscoveryService(discoveryVersionDate, discoveryAuthenticator);
    discovery.SetServiceUrl("<service-url>");

    //  Call with sepcific callbacks
    assistant.Message(
        callback: OnMessage,
        workspaceId: workspaceId
    );

    discovery.ListEnvironments(
        callback: OnGetEnvironments
    );
}

private void OnMessage(DetailedResponse<MessageResponse> response, IBMError error)
{
    Log.Debug("ExampleCallback.OnMessage()", "Response received: {0}", response.Response);
}

private void OnGetEnvironments(DetailedResponse<ListEnvironmentsResponse> response, IBMError error)
{
    Log.Debug("ExampleCallback.OnGetEnvironments()", "Response received: {0}", response.Response);
}
```

Since the success callback signature is generic and the failure callback always has the same signature, you can use a single set of callbacks to handle multiple calls.
```cs
AssistantService assistant;
string assistantVersionDate = "<assistant-version-date>";
Authenticator assistantAuthenticator;
string workspaceId = "<workspaceId>";

DiscoveryService discovery;
string discoveryVersionDate = "<discovery-version-date>";
Authenticator discoveryAuthenticator;

private void Example()
{
    assistant = new AssistantService(assistantVersionDate, assistantAuthenticator);
    assistant.SetServiceUrl("<service-url>");

    //  Call with generic callbacks
    JObject input = new JObject();
    input.Add("text", "");
    assistant.Message(
        callback: OnSuccess,
        workspaceId: workspaceId,
        input: input
    );

    discovery = new DiscoveryService(discoveryVersionDate, discoveryAuthenticator);
    discovery.SetServiceUrl("<service-url>");

    discovery.ListEnvironments(
        callback: OnSuccess
    );
}

//  Generic success callback
private void OnSuccess<T>(DetailedResponse<T> resp, IBMError error)
{
    Log.Debug("ExampleCallback.OnSuccess()", "Response received: {0}", resp.Response);
}
```

You can also use an anonymous callback
```cs
AssistantService assistant;
string assistantVersionDate = "<assistant-version-date>";
Authenticator assistantAuthenticator;
string workspaceId = "<workspaceId>";

private void Example()
{
    assistant = new AssistantService(assistantVersionDate, assistantAuthenticator);

    assistant.ListWorkspaces(
        callback: (DetailedResponse<WorkspaceCollection> response, IBMError error) =>
        {
            Log.Debug("ExampleCallback.OnSuccess()", "ListWorkspaces result: {0}", response.Response);
        },
        pageLimit: 1,
        includeCount: true,
        sort: "-name",
        includeAudit: true
    );
    assistant.SetServiceUrl("<service-url>");
}
```

You can check the `error` response to see if there was an error in the call.
```cs
AssistantService assistant;
string assistantVersionDate = "<assistant-version-date>";
Authenticator assistantAuthenticator;
string workspaceId = "<workspaceId>";

private void Example()
{
    assistant = new AssistantService(versionDate, authenticator);

    assistant.Message(OnMessage, workspaceId);
}

private void OnMessage(DetailedResponse<MessageResponse> response, IBMError error)
{
    if (error == null)
    {
        Log.Debug("ExampleCallback.OnMessage()", "Response received: {0}", response.Response);
    }
    else
    {
        Log.Debug("ExampleCallback.OnMessage()", "Error received: {0}, {1}, {3}", error.StatusCode, error.ErrorMessage, error.Response);
    }
}
```

## Custom Request Headers
You can send custom request headers by adding them to the service.

```cs
AssistantService assistant;
string assistantVersionDate = "<assistant-version-date>";
Authenticator assistantAuthenticator;
string workspaceId = "<workspaceId>";

void Example()
{
    assistant = new AssistantService(assistantVersionDate, assistantAuthenticator);

    //  Add custom header to the REST call
    assistant.WithHeader("X-Watson-Metadata", "customer_id=some-assistant-customer-id");
    assistant.Message(OnSuccess, "<workspace-id>");
}

private void OnSuccess(DetailedResponse<MessageResponse> response, IBMError error)
{
    Log.Debug("ExampleCallback.OnMessage()", "Response received: {0}", response.Response);
}
```

## Response Headers
You can get response headers in the `headers` object in the DetailedResponse.

```cs
AssistantService assistant;
string assistantVersionDate = "<assistant-version-date>";
Authenticator assistantAuthenticator;
string workspaceId = "<workspaceId>";

void Example()
{
    assistant = new AssistantService(assistantVersionDate, assistantAuthenticator);

    assistant.Message(OnMessage, "<workspace-id>");
}

private void OnMessage(DetailedResponse<MessageResponse> response, IBMError error)
{
    //  List all headers in the response headers object
    foreach (KeyValuePair<string, object> kvp in response.Headers)
    {
        Log.Debug("ExampleCustomHeader.OnMessage()", "{0}: {1}", kvp.Key, kvp.Value);
    }
}
```

## TLS 1.0 support
Watson services have upgraded their hosts to TLS 1.2. The Dallas location has a TLS 1.0 endpoint that works for streaming. To stream in other regions, use Unity 2018.2 and set **Scripting Runtime Version** in Build Settings to `.NET 4.x equivalent`. To support Speech to Text in earlier versions of Unity, create the instance in the Dallas location.

## Disabling SSL verification
You can disable SSL verifciation when making a service call.
```cs
AssistantService assistant;
string assistantVersionDate = "<assistant-version-date>";
Authenticator assistantAuthenticator;
string workspaceId = "<workspaceId>";

void Example()
{
    authenticator.DisableSslVerification = true;
    assistant = new AssistantService(assistantVersionDate, assistantAuthenticator);

    //  disable ssl verification
    assistant.DisableSslVerification = true;
}
```

## IBM Cloud Pak for Data(ICP4D)
If your service instance is of ICP4D, below are two ways of initializing the assistant service.

#### 1) Supplying the `username`, `password`, `icp4d_url` and `authentication_type`

The SDK will manage the token for the user

```cs
    CloudPakForDataAuthenticator authenticator = new CloudPakForDataAuthenticator("<url>", "<username>", "<password>");
    while(!authenticator.CanAuthenticate())
    {
        yield return null;
    }
    service = new AssistantService(versionDate, authenticator);
```

#### 2) Supplying the access token

```cs
    BearerTokenAuthenticator = new BearerTokenAuthenticator("<accessToken>");
    while(!authenticator.CanAuthenticate())
    {
        yield return null;
    }
    service = new AssistantService(versionDate, authenticator);
```

## IBM Cloud Private
The Watson Unity SDK does not support IBM Cloud Private because connection via proxy is not supported in UnityWebRequest. 

## Documentation
Documentation can be found [here][documentation]. You can also access the documentation by selecting API Reference the Watson menu (**Watson -> API Reference**).

## Getting started videos
You can view Getting Started videos for the IBM Watson SDK for Unity on [YouTube](https://www.youtube.com/watch?v=sNPsdUWSi34&list=PLZDyxLlNKRY9b2vurEhkSoNWZN5c5l4Nr).

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

## Featured projects
We'd love to highlight cool open-source projects that use this SDK! If you'd like to get your project added to the list, feel free to make an issue linking us to it.

[wdc]: https://www.ibm.com/watson/developer/
[wdc_unity_sdk]: https://github.com/watson-developer-cloud/unity-sdk
[latest_release_sdk]: https://github.com/watson-developer-cloud/unity-sdk/releases/latest
[latest_release_core]: https://github.com/IBM/unity-sdk-core/releases/latest
[get_unity]: https://unity3d.com/get-unity
[documentation]: https://watson-developer-cloud.github.io/unity-sdk/
[ibm-cloud-onboarding]: https://cloud.ibm.com/registration?target=/developer/watson&cm_sp=WatsonPlatform-WatsonServices-_-OnPageNavLink-IBMWatson_SDKs-_-Unity
[watson-dashboard]: https://cloud.ibm.com/
