Change Log
==========
## Version 0.12.0
_2016-11-4_
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
* Fix: Error when pasting credentials from the new Bluemix site into the `Config Editor`.
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
