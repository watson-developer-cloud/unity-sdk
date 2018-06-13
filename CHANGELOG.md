Change Log
==========
## Version 2.3.0
_2018-05-31_
* New: IAM support in Visual Recognition.
* Fixed: Use non-region specific url for IamUrl.
* Fixed: Authenticating with apikey sets the service url to `gateway-a`.
* New: Abstract `DeleteUserData()` for Speech to Text and Text to Speech.

## Version 2.2.3
_2018-05-21_
* New: Abstract `DeleteUserData()` for Assistant, Conversation and Discovery ([#387](https://github.com/watson-developer-cloud/unity-sdk/pull/387)).
* Fixed: Replaced `ToUnixTimeInSeconds()` with dotnet 3.5 equivalent ([#382](https://github.com/watson-developer-cloud/unity-sdk/issues/382), [#386](https://github.com/watson-developer-cloud/unity-sdk/pull/386)).
* New: Get credentials for testing from internal github repo ([#4333](https://github.ibm.com/Watson/developer-experience/issues/4333), [#364](https://github.com/watson-developer-cloud/unity-sdk/pull/364)).
* New: Warn user to `InstallDefaultReactors()` ([#328](https://github.com/watson-developer-cloud/unity-sdk/issues/328), [#380](https://github.com/watson-developer-cloud/unity-sdk/pull/380)).
* New: Revised WSConnector for TLS 1.2 support in upcoming Unity release ([#4230](https://github.ibm.com/Watson/developer-experience/issues/4230), [#379](https://github.com/watson-developer-cloud/unity-sdk/pull/379)).
* New: Added support for IAM authentication ([#4291](https://github.ibm.com/Watson/developer-experience/issues/4291), [#377](https://github.com/watson-developer-cloud/unity-sdk/pull/377)).

## Version 2.2.2
* New: Send custom request headers and access response headers via `customData` object ([#4310](https://github.ibm.com/Watson/developer-experience/issues/4310), [#362](https://github.com/watson-developer-cloud/unity-sdk/pull/362)).
* Fixed: Unresolved errors hidden by `#ifdef` ([#368](https://github.com/watson-developer-cloud/unity-sdk/issues/368), [#369](https://github.com/watson-developer-cloud/unity-sdk/pull/369))

## Version 2.2.1
_2018-04-12_
* Fixed: Serialization/Deserialization of generic object in the `assistant` service([361](https://github.com/watson-developer-cloud/unity-sdk/issues/361), [363](https://github.com/watson-developer-cloud/unity-sdk/pull/363)).

## Version 2.2.0
_2018-04-09_
* New: Updated Visual Recognition with Core ML support ([4182](https://zenhub.innovate.ibm.com/app/workspace/o/watson/developer-experience/issues/4182), [357](https://github.com/watson-developer-cloud/unity-sdk/pull/357)).
* New: Abstract Classify Collections operation in Natural Language Classifier ([4223](https://github.ibm.com/Watson/developer-experience/issues/4223), [355](https://github.com/watson-developer-cloud/unity-sdk/pull/355))
* Removed: Tradeoff Analytics ([352](https://github.com/watson-developer-cloud/unity-sdk/pull/352))
* Removed: Retrieve and Rank ([352](https://github.com/watson-developer-cloud/unity-sdk/pull/352))
* Removed: Document Conversion ([352](https://github.com/watson-developer-cloud/unity-sdk/pull/352))
* Removed: Alchemy API ([350](https://github.com/watson-developer-cloud/unity-sdk/pull/350))
* Fixed: Added `result_index` to Speech to Text response ([347](https://github.com/watson-developer-cloud/unity-sdk/issues/347), [349](https://github.com/watson-developer-cloud/unity-sdk/pull/349))
* Enhanced: Made credentials, url and versionDates accessible in the inspector in all examples ([342](https://github.com/watson-developer-cloud/unity-sdk/issues/342), [351](https://github.com/watson-developer-cloud/unity-sdk/pull/351))
* Fixed: Created new branch for asset store releases with better documentation ([341](https://github.com/watson-developer-cloud/unity-sdk/issues/341), [asset-store-release](https://github.com/watson-developer-cloud/unity-sdk/tree/asset-store-release)) and list of files to remove from an asset store release ([Wiki](https://github.com/watson-developer-cloud/unity-sdk/wiki/Asset-Store-Release))
* Fixed: Platform specific compilation for Unity Web Request ([335](https://github.com/watson-developer-cloud/unity-sdk/issues/335), [346](https://github.com/watson-developer-cloud/unity-sdk/pull/346))
* Fixed: Speech to Text - Do not send keywords if they are not provided ([336](https://github.com/watson-developer-cloud/unity-sdk/issues/336), [340](https://github.com/watson-developer-cloud/unity-sdk/pull/340)).

## Version 2.1.0
_2018-03-06_
New: Abstract Watson Assistant service.
New: External credentials in integration tests.
New: Documentation on publishing a release.
Fixed: Visual Recognition Classify method sends byte[] data in form data instead of body data.
Fixed: Redirect Speech to Text streaming requests to TLS 1.0 streaming endpoint.
Deprecated: Document Conversion - Please use Discovery.
Removed: Language Translation - Please use Langauge Translator.

## Version 2.0.0
_2017-11-20_ MAJOR RELEASE, BREAKING CHANGES
* New: Implemented error callbacks in each call
* New: Implemented generic type success callbacks
* New: Implemented `Dictionary<string, object>` to hold custom data for each call
* New: Support for `Hololens`
* New: Abstracted custom acoustic models for `SpeechToText`
* New: Addition of streaming example where the sample is split up to improve latency
* New: Transition to dll for `WebSocketSharp`
* New: Added support for decoding unicode characters in `TextToSpeech`
* Fixed: Transition `Delete` methods to `UnityWebRequest`
* Fixed: Improvements to `SpeechToText` streaming
* Fixed: `SpeechToText` streaming parameters
* Fixed: Improvements to `ExampleStreaming`
* Fixed: `SpeechToText` custom corpus
* Fixed: Allow empty string to be sent when invoking `Message` from the `Conversation` service
* Fixed: Standardized `Debug.Log` output throughout SDK
* Fixed: Fix all integration tests

## Version 1.0.0
_2017-08-31_ MAJOR RELEASE, BREAKING CHANGES

* New: Abstracted `Natural Language Understanding` service.
* New: Removed higher level `Widget` architecture - Will create a separate package to add in this functionality.
* New: Removed dependency on `config.json` file for credentials.
* New: Removed the `Configuration Editor`.
* New: Removed the `Natural Language Classifier Editor`.
* New: Added `KeywordSpotting` functionality to `Speech to Text`.
* New: Support authentication tokens.
* New: Remove `Touchscript` and `Watson Camera` scripts.
* New: Revised all services to directly take credentials.
* New: Data models to support credentials through `VCAP_SERVICES`.
* Fixed: Updated examples, tests and ReadMe.md files.
* Fixed: Updated 3rd party plugins.
* Fixed: Removed deprecated services.


## Version 0.13.0
_2017-01-25_

* New: Abstracted `Discovery` service.
* Fix: Updated TouchScript plugin.
* New: Updated builds to use Unity 5.5.
* Fix: TextToSpeech Widget now has multi output.
* Fix: Added custom scenes in build editor.

## Version 0.13.0
_2016-12-02_

* Fix: Increased conversation version.
* Fix: Fixed infinite loop in Conversation service example.

## Version 0.12.0
_2016-11-04_

* New: Added streaming `SpeechToText` example.
* New: Abstraction for `Personality Insights V3`

## Version 0.11.0
_2016-10-27_

* New: Abstracted `Speech to Text` customization methods.

## Version 0.10.0
_2016-09-23_

* New: Added `similarity search` to the `Visual Recognition` service.
* Fix: `Touch Widget` improvements.
* Fix: Disabled 3rd Party plugin warnings.
* Fix: Removed `Conversation` Message overload method that takes only input and conversationID.
* Fix: Rewrote `Conversation` example script to show how to create MessageRequest object.

## Version 0.9.0
_2016-08-26_

* Deprecated: Retired `Dialog` service.
* Fix: `ExampleLanguageTranslation` now is using `LanguageTranslator` before `LanguageTranslation` service goes live.
* Fix: Abstracted custom voice model methods in `Text to Speech` service.
* Fix: Error when pasting credentials from the new IBM Cloud site into the `Config Editor`.
* New: Added `CameraWidget` and `CameraDisplayWidget` to get video from device camera.
* New: Added test scene for using the device camera with the `Visual Recognition` service.

## Version 0.8.0
_2016-08-12_

* Fix: Removed tag stripping from `ToSpeech()` method of the `TextToSpeech` service.
* New: The `Conversation` service now accepts full conversation payload in overloaded `Message()` method.
* Fix: Removed `SetLastWriteTime()` for Android platform  in the data cache because of a known android issue.
```https://code.google.com/p/android/issues/detail?id=15480```


## Version 0.7.0
_2016-07-29_

* New: Visual Recognition: Added retraining functionality.
* New: Visual Recognition: Use byteArray data to classify, detect faces and recognize text.
* Fix: Updated integration testing.


## Version 0.6.1
_2016-07-17_

* Fix: Updated documentation

## Version 0.6.0

_2016-07-15_

* New: Added `Document Conversion` abstraction
* New: Added `AlchemyData News` abstraction
* New: Added `Retrieve and Rank` abstraction
* New: Added `Conversation` abstraction
* Fix: Added `LanguageTranslation` and `LanguageTranslator`

## Version 0.5.0

_2016-06-24_

 * New: Added `Alchemy Language` abstraction
 * New: Added `Personality Insights` abstraction
 * Fix: Added `Tone Analyzer` to the Configuration Editor
 * Fix: Added `Tradeoff Analytics` to the Configuration Editor
 * Fix: Added `Conversation` to the Configuration Editor
 * Fix: Added `Personality Insights` to the Configuration Editor
 * Fix: Added `Alchemy Language` to the Configuration Editor
 * Fix: Added `Visual Recognition` to the Configuration Editor

## Version 0.4.0

_2016-06-09_

 * New: Added `Tone Analyzer v3` abstraction
 * New: Added `Tradeoff Analytics` abstraction
 * New: Added `Conversation` abstraction
 * New: Added `Visual Recognition v3` abstraction
 * Fix: Creating test project dynamically for Travis CL integration
 * Fix: Refactored Language Translation to Language Translator
 * Fix: Widget examples sprite references were disconnected

## Version 0.3.0

_2016-04-29_

 * Fix: Make SDK application agnostic
 * Fix: Rewrite README
 * New: Added example code snippets showing how to access low level services
 * Fix: Restructured SDK to put non-core components in Examples
 * Fix: Fixed several usability issues
 * Fix: Revised several aspects of the SDK to match the formats of other WDC SDKs
