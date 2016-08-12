Change Log
==========
## Version 0.8.0
_2016-08-12_

* Fixed: Removed tag stripping from `ToSpeech()` method of the `TextToSpeech` service.
* New: The `Conversation` service now accepts full conversation payload in overloaded `Message()` method.
* Fixed: Removed `SetLastWriteTime()` for Android platform  in the data cache because of a known android issue. 
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
