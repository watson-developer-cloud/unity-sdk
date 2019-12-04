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
