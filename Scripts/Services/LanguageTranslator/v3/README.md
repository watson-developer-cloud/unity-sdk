# Language Translator V3

[Language Translator][language_translator] translates text from one language to another. The service offers multiple domain-specific models that you can customize based on your unique terminology and language. Use Language Translator to take news from across the globe and present it in your language, communicate with your customers in their own language, and more.

## Usage
Select a domain, then identify or select the language of text, and then translate the text from one supported language to another.

### List models
Lists available models for language translation with option to filter by source or by target language.
```cs
private void GetModels()
{
  if (!_languageTranslator.GetModels(OnGetModels, OnFail))
    Log.Debug("ExampleLanguageTranslatorV3.GetModels()", "Failed to get models.");
}

private void OnGetModels(TranslationModels models, Dictionary<string, object> customData)
{
  Log.Debug("ExampleLanguageTranslatorV3.OnGetModels()", "Language Translator - Get models response: {0}", customData["json"].ToString());
}
```





### Create a model
Uploads a TMX glossary file on top of a domain to customize a translation model. Depending on the size of the file, training can range from minutes for a glossary to several hours for a large parallel corpus. Glossary files must be less than 10 MB. The cumulative file size of all uploaded glossary and corpus files is limited to 250 MB.
```cs
private void CreateModel()
{
  if (!_languageTranslator.CreateModel(OnCreateModel, OnFail, <base-model-name>, <custom-model-name>, <glossary-filepath>))
    Log.Debug("ExampleLanguageTranslatorV3.CreateModel()", "Failed to create model.");
}

private void OnCreateModel(TranslationModel model, Dictionary<string, object> customData)
{
  Log.Debug("ExampleLanguageTranslatorV3.OnCreateModel()", "Language Translator - Create model response: {0}", customData["json"].ToString());
}
```





### Get a model details
Returns information, including training status, about a specified translation model.
```cs
private void GetModel()
{
  if (!_languageTranslator.GetModel(OnGetModel, OnFail, <custom-language-model-id>))
    Log.Debug("ExampleLanguageTranslatorV3.GetModel()", "Failed to get model.");
}

private void OnGetModel(TranslationModel model, Dictionary<string, object> customData)
{
  Log.Debug("ExampleLanguageTranslatorV3.OnGetModel()", "Language Translator - Get model response: {0}", customData["json"].ToString());
}
```





### Delete a model
Deletes trained translation models.
```cs
private void DeleteModel()
{
  if (!_languageTranslator.DeleteModel(OnDeleteModel, OnFail, <custom-language-model-id>))
    Log.Debug("ExampleLanguageTranslatorV3.DeleteModel()", "Failed to delete model.");
}

private void OnDeleteModel(bool success, Dictionary<string, object> customData)
{
  Log.Debug("ExampleLanguageTranslatorV3.OnDeleteModel()", "Language Translator - Delete model response: success: {0}", success);
}
```





### Translate
Translates input text from the source language to the target language.
```cs
private void Translate()
{
  if (!_languageTranslator.GetTranslation(OnGetTranslation, OnFail, <text-to-translate>, <from-language>, <to-language>))
    Log.Debug("ExampleLanguageTranslatorV3.Translate()", "Failed to translate.");
}

private void OnGetTranslation(Translations translation, Dictionary<string, object> customData)
{
  Log.Debug("ExampleLanguageTranslatorV3.OnGetTranslation()", "Langauge Translator - Translate Response: {0}", customData["json"].ToString());
}
```




### Identify language
Identify the language in which a text is written.
```cs
private void Identify()
{
  if (!_languageTranslator.Identify(OnIdentify, OnFail, <text-to-identify>))
    Log.Debug("ExampleLanguageTranslatorV3.Identify()", "Failed to identify language.");
}

private void OnIdentify(string lang, Dictionary<string, object> customData)
{
  Log.Debug("ExampleLanguageTranslatorV3.OnIdentify()", "Language Translator - Identify response: {0}", customData["json"].ToString());
}
```





### Identifiable languages
Return the list of languages it can detect.
```cs
private void GetLanguages()
{
  if (!_languageTranslator.GetLanguages(OnGetLanguages, OnFail))
    Log.Debug("ExampleLanguageTranslatorV3.GetLanguages()", "Failed to get languages.");
}

private void OnGetLanguages(Languages languages, Dictionary<string, object> customData)
{
  Log.Debug("ExampleLanguageTranslatorV3.OnGetLanguages()", "Language Translator - Get languages response: {0}", customData["json"].ToString());
}
```





[language_translator]: https://www.ibm.com/watson/services/language-translator/
