## [5.1.1](https://github.com/watson-developer-cloud/unity-sdk/compare/v5.1.0...v5.1.1) (2021-06-16)


### Bug Fixes

* **discovery-v1:** add DateTime format ([f83b6ad](https://github.com/watson-developer-cloud/unity-sdk/commit/f83b6ad710510c6b07d3ef83522dfd094c390d9d))
* **discovery-v2:** add enrichment to the form-data ([acfd7b5](https://github.com/watson-developer-cloud/unity-sdk/commit/acfd7b5e4307af3d600f3e928e0f1a18905bab97))
* **visual-recognition-v4:** add DateTime format ([3ea83df](https://github.com/watson-developer-cloud/unity-sdk/commit/3ea83dfde0419a2a088f5f0048bf31b6bd6a6374))

# [5.1.0](https://github.com/watson-developer-cloud/unity-sdk/compare/v5.0.2...v5.1.0) (2021-06-10)


### Features

* **generation:** generated using api def sdk-2021-05-11-rerelease and gen 3.31.0 ([44079dd](https://github.com/watson-developer-cloud/unity-sdk/commit/44079dd184f4bd0c5f24dc5dccba98eb655078c1))

## [5.0.2](https://github.com/watson-developer-cloud/unity-sdk/compare/v5.0.1...v5.0.2) (2021-03-05)


### Bug Fixes

* deprecate compare and comply message ([f05490d](https://github.com/watson-developer-cloud/unity-sdk/commit/f05490db5d4e5d171e71eee66cb2ea927279654d))
* test semantic release ([489fa33](https://github.com/watson-developer-cloud/unity-sdk/commit/489fa337f7f8a3b4377b78252582b2bd63a1c13f))

## [5.0.1](https://github.com/watson-developer-cloud/unity-sdk/compare/v5.0.0...v5.0.1) (2020-12-21)


### Bug Fixes

* **Assistant:** node dialog respose should have agent props ([e0ead99](https://github.com/watson-developer-cloud/unity-sdk/commit/e0ead9909b7144d074e1dddc57176ad7309a676a))

# [5.0.0](https://github.com/watson-developer-cloud/unity-sdk/compare/v4.8.0...v5.0.0) (2020-12-10)


### Features

* **AssistantV1:** Add support for BulkClassify ([20e42e3](https://github.com/watson-developer-cloud/unity-sdk/commit/20e42e3c26d16ed3257874e3244c5ccf4d83a0b0))
* **AssistantV2:** Add support for BulkClassify and refactor context skill system ([52f0cf9](https://github.com/watson-developer-cloud/unity-sdk/commit/52f0cf940865fc28cbe14833c69bed8293149a1a))
* **CompareComply:** add support for Modification in TypeLabel ([bd6d619](https://github.com/watson-developer-cloud/unity-sdk/commit/bd6d619e73f9e50873767457406b782d694b7098))
* **DiscoveryV2:** Add support for AnalyzeDocuments ([fa97def](https://github.com/watson-developer-cloud/unity-sdk/commit/fa97def7cf4bac9468050a7c9b6db064647fedd0))
* regenrate services using current API def ([30eb7de](https://github.com/watson-developer-cloud/unity-sdk/commit/30eb7debb1d46fec979a603810459c324087b53b))
* **regeneration:** regenerated with generator 3.21.0 and api def sdk-major-release-2020 ([5de83ec](https://github.com/watson-developer-cloud/unity-sdk/commit/5de83ec432d1878d3f9edb98f47689a79f49a3bb))
* **TextToSpeechV1:** add support for CustomModel and CustomModels ([235aef2](https://github.com/watson-developer-cloud/unity-sdk/commit/235aef25527060e89c40d4e4fa93c1aaa0660796))
* **VisualRecognition:** Add deprecation warning and change starte and endtimes to DateTime ([d99b69a](https://github.com/watson-developer-cloud/unity-sdk/commit/d99b69add5d6d42ed733ccde176615f79e788491))


### BREAKING CHANGES

* **VisualRecognition:** change startTime and encTime to DateTime for GetTrainingUsave for
VisualRecognitionV4
* **TextToSpeechV1:** Moved from VoiceModel and VoiceModels to CustomModel and CustomModels
* **AssistantV2:** MessageContextSkill

# [4.8.0](https://github.com/watson-developer-cloud/unity-sdk/compare/v4.7.1...v4.8.0) (2020-08-25)


### Features

* **AssistantV1:** change default URL ([21283bc](https://github.com/watson-developer-cloud/unity-sdk/commit/21283bc78c3ff14432f4cf7a409f3b7bc8f6c125))
* **AssistantV2:** add support for ListLogs and DeleteUserData ([369e7fd](https://github.com/watson-developer-cloud/unity-sdk/commit/369e7fd7414f1d4d825bc2e60b5c7d72af8d7dab))
* regenerate services and add new default URLs ([e484113](https://github.com/watson-developer-cloud/unity-sdk/commit/e484113de45dccd7623f1616dc3b04eaa4b6d292))
* **DiscoveryV2:** add new apis for projects, enrichments and collections ([abc28ba](https://github.com/watson-developer-cloud/unity-sdk/commit/abc28bad54e43397d4ae08d04c45c5ccc2bebb64))
* **LanguageTranslatorV3:** add support for ListLanguages ([0570cdb](https://github.com/watson-developer-cloud/unity-sdk/commit/0570cdba89631679d8a3412120c1881138b9b3fd))

## [4.7.1](https://github.com/watson-developer-cloud/unity-sdk/compare/v4.7.0...v4.7.1) (2020-06-17)


### Bug Fixes

* **TTS:** update TTS example to play music and using normal synthesize ([9006680](https://github.com/watson-developer-cloud/unity-sdk/commit/90066801dca6200e7dcc802c421ce2e94e50f26e))

# [4.7.0](https://github.com/watson-developer-cloud/unity-sdk/compare/v4.6.1...v4.7.0) (2020-06-03)


### Features

* **AssistantV1:** add support for spelling suggestions ([7e5ea7c](https://github.com/watson-developer-cloud/unity-sdk/commit/7e5ea7c0a91284adc47b17f8df6a9e697e0b3985))
* **AssistantV2:** add support for stateless messages ([9998de9](https://github.com/watson-developer-cloud/unity-sdk/commit/9998de97ef415ed2ffaf49ce1643fa9deb5df430))
* **VisualRecognitionV4:** add support for download model file ([f9dede3](https://github.com/watson-developer-cloud/unity-sdk/commit/f9dede3172cc0f6e2b8b1948576382aaeccb30d0))
* Regenerate the services based on current API def ([deb0f5e](https://github.com/watson-developer-cloud/unity-sdk/commit/deb0f5ec536bac0ac2dbf951c2c2cd61be4cfefb))

## [4.6.1](https://github.com/watson-developer-cloud/unity-sdk/compare/v4.6.0...v4.6.1) (2020-05-22)


### Bug Fixes

* **Speech to Text:** Revise SpeechRecognitionAlternative model ([01f0627](https://github.com/watson-developer-cloud/unity-sdk/commit/01f06279a242d740b4abe5256449a6dac900740e))

# [4.6.0](https://github.com/watson-developer-cloud/unity-sdk/compare/v4.5.0...v4.6.0) (2020-04-24)


### Features

* **AssistantV1:** add support for runtime entity alternatives ([cd3a592](https://github.com/watson-developer-cloud/unity-sdk/commit/cd3a592dbe9cd016a7ddf579c719cf8430993366))
* **LanguageTranslator:** add support for language detection ([ccca1da](https://github.com/watson-developer-cloud/unity-sdk/commit/ccca1da16685d26d1f1a52f445941b0409555db4))
* **SpeechToText:** add support for speech detector sensitivity and background supression ([4e0e358](https://github.com/watson-developer-cloud/unity-sdk/commit/4e0e35816a7bbb187cd751759da4f1eb4a30be74))
* regenerate services based on current api def ([53460d2](https://github.com/watson-developer-cloud/unity-sdk/commit/53460d215f512b15e742bf26bd5586bd68af4e25))

# [4.5.0](https://github.com/watson-developer-cloud/unity-sdk/compare/v4.4.0...v4.5.0) (2020-02-25)


### Features

* **TextToSpeech:** add websocket support for text to speech ([42a28d6](https://github.com/watson-developer-cloud/unity-sdk/commit/42a28d6fa72a609208e1bd6b157e5b0c428b455b))

# [4.4.0](https://github.com/watson-developer-cloud/unity-sdk/compare/v4.3.0...v4.4.0) (2020-02-13)


### Features

* **Assistant:** add support for include audit and append ([cb1f3cc](https://github.com/watson-developer-cloud/unity-sdk/commit/cb1f3cc2bffba864333326b976c3168d8c19845d))
* **VisualRecognitionV4:** add support for object metadata ([7a4c8bb](https://github.com/watson-developer-cloud/unity-sdk/commit/7a4c8bb503ef342c3305e17283769c6e9a173605))
* Regenerate service using current api defs ([0b0c0a4](https://github.com/watson-developer-cloud/unity-sdk/commit/0b0c0a40ca892bef0b70b2833ff87dd7b8cab9af))

# [4.3.0](https://github.com/watson-developer-cloud/unity-sdk/compare/v4.2.1...v4.3.0) (2020-01-23)


### Features

* **TextToSpeech:** add example for text to speech synthesize ([38666a5](https://github.com/watson-developer-cloud/unity-sdk/commit/38666a52f30088afa33dfd5dfe9cbb67961362dd))

## [4.2.1](https://github.com/watson-developer-cloud/unity-sdk/compare/v4.2.0...v4.2.1) (2020-01-17)


### Bug Fixes

* **NaturalLanguageUnderstanding:** add Model param back to CategoriesOptions ([657acae](https://github.com/watson-developer-cloud/unity-sdk/commit/657acaefc7d1041b0fee2783759dbe7035fa29fa))

# [4.2.0](https://github.com/watson-developer-cloud/unity-sdk/compare/v4.1.1...v4.2.0) (2020-01-16)


### Bug Fixes

* fix copyright dates ([f48d19f](https://github.com/watson-developer-cloud/unity-sdk/commit/f48d19f3836b0d8446870a1a96be56c33ecfdd9d))


### Features

* **SpeechToText:** add support for endOfPhraseSilenceTime and splitTranscriptAtPhraseEnd ([0433887](https://github.com/watson-developer-cloud/unity-sdk/commit/0433887566283fa794491276a4a385db22a6a51e))
* regenrate all of the services with current api defs ([c090ef5](https://github.com/watson-developer-cloud/unity-sdk/commit/c090ef5f0e09767bf9cc1f15d3c3e97db258539f))
* **SpeechToText:** Add support for EndOfPhraseSilenceTime and SplitTranscriptAtPhraseEnd to STT web ([a009df5](https://github.com/watson-developer-cloud/unity-sdk/commit/a009df5b712f24ab285891e1e3af7a284acbfb44))

## [4.1.1](https://github.com/watson-developer-cloud/unity-sdk/compare/v4.1.0...v4.1.1) (2019-12-04)


### Bug Fixes

* fix examples to always set service url ([13b37ee](https://github.com/watson-developer-cloud/unity-sdk/commit/13b37eee23e374598796d7be31f55bdbce08395d))

# [4.1.0](https://github.com/watson-developer-cloud/unity-sdk/compare/v4.0.1...v4.1.0) (2019-11-27)


### Features

* **Discovery:** use json sub types in query aggregation ([0936857](https://github.com/watson-developer-cloud/unity-sdk/commit/0936857832d82dc44ebc945d5c3512a215e7a303))
* **DiscoveryV2:** add support for discovery v2 ([f8e626d](https://github.com/watson-developer-cloud/unity-sdk/commit/f8e626d7e467bb952b8e4d2f91e92ab5ca2130a5))
* **regerate:** regenerate sdk using current api defs ([4e8d68b](https://github.com/watson-developer-cloud/unity-sdk/commit/4e8d68b1d7cdacc7282a1cde80f64cb110ca4716))
* **Services:** move Connector to base service to support unit testing ([196c86a](https://github.com/watson-developer-cloud/unity-sdk/commit/196c86ac3330b9f22b23065672b8643867c4fbea))
* **VisualRecognitionV4:** add support to get training usage data ([41d8d8b](https://github.com/watson-developer-cloud/unity-sdk/commit/41d8d8be06dcba6c9b8878fcc1a9fa362e0b032a))

## [4.0.1](https://github.com/watson-developer-cloud/unity-sdk/compare/v4.0.0...v4.0.1) (2019-10-22)


### Bug Fixes

* **STT:** wait for audio to load when sending keep alive message ([8082b33](https://github.com/watson-developer-cloud/unity-sdk/commit/8082b3354ce76d220e6432378307db1bc0cd3141))

# [4.0.0](https://github.com/watson-developer-cloud/unity-sdk/compare/v3.5.0...v4.0.0) (2019-10-04)


### Features

* **additionalProps:** add support for dynamic additional properties for models ([1dcc5d2](https://github.com/watson-developer-cloud/unity-sdk/commit/1dcc5d2))
* **compare-comply:** add ContractCurrentcies model ([57677db](https://github.com/watson-developer-cloud/unity-sdk/commit/57677db))
* add meta file for vis rec v4 ([12194cf](https://github.com/watson-developer-cloud/unity-sdk/commit/12194cf))
* **Discovery:** add suggested query to query response ([3f582d1](https://github.com/watson-developer-cloud/unity-sdk/commit/3f582d1))
* **Examples:** add examples for NLU, LT and Discovery ([2f220b8](https://github.com/watson-developer-cloud/unity-sdk/commit/2f220b8))
* **regenerate:** regenerate services to include model and connector changes ([eaec06c](https://github.com/watson-developer-cloud/unity-sdk/commit/eaec06c))
* **regenerate:** regenerate services using current api def ([affd1f9](https://github.com/watson-developer-cloud/unity-sdk/commit/affd1f9))
* **regerate:** regenerate services for pre-release ([4d2a36f](https://github.com/watson-developer-cloud/unity-sdk/commit/4d2a36f))
* **SetServiceUrl:** provide setServiceUrl method and use serviceUrl instead of Url ([a6df1b8](https://github.com/watson-developer-cloud/unity-sdk/commit/a6df1b8))
* **VisualRecognitionV4:** add support for visual recognition v4 ([b47e990](https://github.com/watson-developer-cloud/unity-sdk/commit/b47e990))


### BREAKING CHANGES

* **additionalProps:** add support for dynamic additional properties for models

# [3.5.0](https://github.com/watson-developer-cloud/unity-sdk/compare/v3.4.1...v3.5.0) (2019-08-22)


### Features

* **compare-comply:** add ContractCurrentcies model ([57677db](https://github.com/watson-developer-cloud/unity-sdk/commit/57677db))

## [3.4.1](https://github.com/watson-developer-cloud/unity-sdk/compare/v3.4.0...v3.4.1) (2019-08-05)


### Bug Fixes

* **build:** make assembly definitions compatible to unity 2018 and 2019 ([a6064b1](https://github.com/watson-developer-cloud/unity-sdk/commit/a6064b1))

# [3.4.0](https://github.com/watson-developer-cloud/unity-sdk/compare/v3.3.0...v3.4.0) (2019-07-25)


### Features

* **NaturalLanguageUnderstanding:** add examples for NaturalLanguageUnderstanding ([4bf9c54](https://github.com/watson-developer-cloud/unity-sdk/commit/4bf9c54))
* **regenerate:** updates for regular release 4 ([10c4529](https://github.com/watson-developer-cloud/unity-sdk/commit/10c4529))

# [3.3.0](https://github.com/watson-developer-cloud/unity-sdk/compare/v3.2.0...v3.3.0) (2019-06-26)


### Features

* **icp4d:** add support for icp4d ([ba8044d](https://github.com/watson-developer-cloud/unity-sdk/commit/ba8044d))

# [3.2.0](https://github.com/watson-developer-cloud/unity-sdk/compare/v3.1.0...v3.2.0) (2019-06-14)


### Bug Fixes

* **Version:** Manually bumpversion to 3.1.0 ([0330079](https://github.com/watson-developer-cloud/unity-sdk/commit/0330079))


### Features

* **regenerate:** updates based on latest API definitions ([dc40499](https://github.com/watson-developer-cloud/unity-sdk/commit/dc40499))
* **Regeneration:** Regenerated SDK based on the latest API definitions ([192c13b](https://github.com/watson-developer-cloud/unity-sdk/commit/192c13b))

# [3.1.0](https://github.com/watson-developer-cloud/unity-sdk/compare/v3.0.0...v3.1.0) (2019-04-01)


### Bug Fixes

* **ExampleAssistantV1:** Added missing param names ([5fac56c](https://github.com/watson-developer-cloud/unity-sdk/commit/5fac56c))


### Features

* **Core:** Removed Core submodule ([2eab8e7](https://github.com/watson-developer-cloud/unity-sdk/commit/2eab8e7))

# [3.0.0](https://github.com/watson-developer-cloud/unity-sdk/compare/v2.15.3...v3.0.0) (2019-03-29)


### Bug Fixes

* **Speech to Text V1:** Parsing response to double because of casting issues ([b89709f](https://github.com/watson-developer-cloud/unity-sdk/commit/b89709f))


### Features

* Added WatsonResponse and WatsonError ([f16890a](https://github.com/watson-developer-cloud/unity-sdk/commit/f16890a))
* Created a commmon class to get default headers ([1be31bf](https://github.com/watson-developer-cloud/unity-sdk/commit/1be31bf))
* Generated services ([d8244f8](https://github.com/watson-developer-cloud/unity-sdk/commit/d8244f8))
* Regenerated the SDK based on the lateset definitions and unity generator ([ed1acb9](https://github.com/watson-developer-cloud/unity-sdk/commit/ed1acb9))
* Removed customData from operations ([d1bbee8](https://github.com/watson-developer-cloud/unity-sdk/commit/d1bbee8))
* Removed customData from response, added response json to response object ([e78754e](https://github.com/watson-developer-cloud/unity-sdk/commit/e78754e))
* Separating core from sdk ([53e8b1e](https://github.com/watson-developer-cloud/unity-sdk/commit/53e8b1e))
* **All services:** Exploded body parameters in service methods ([714b31e](https://github.com/watson-developer-cloud/unity-sdk/commit/714b31e))
* Simplified namespaces ([3798616](https://github.com/watson-developer-cloud/unity-sdk/commit/3798616))
* **Assistant V2:** Deserializing Dictionaries ([add8b46](https://github.com/watson-developer-cloud/unity-sdk/commit/add8b46))
* **Core:** Added submodule for core class ([4c18480](https://github.com/watson-developer-cloud/unity-sdk/commit/4c18480))
* **Core:** Separated core from the services ([f5f4791](https://github.com/watson-developer-cloud/unity-sdk/commit/f5f4791))
* **Regeneration:** Regenerated all services using API definition commit 0baaf120beb3852ab9557e700f5 ([7d60d7f](https://github.com/watson-developer-cloud/unity-sdk/commit/7d60d7f))
* **Regeneration:** Regenerated SDK with the latest API definitions ([9bdcfce](https://github.com/watson-developer-cloud/unity-sdk/commit/9bdcfce))
* **Regeneration:** This PR adds the newest generated code using API definition commit `0baaf120beb3 ([85cf79c](https://github.com/watson-developer-cloud/unity-sdk/commit/85cf79c))
* **Serialization:** Transitioned from FullSerializer to Json.net ([9ceeb61](https://github.com/watson-developer-cloud/unity-sdk/commit/9ceeb61))


### BREAKING CHANGES

* **Regeneration:** Regenerated SDK
