# Language Translator

[Language Translator][language_translator] translates text from one language to another. The service offers multiple domain-specific models that you can customize based on your unique terminology and language. Use Language Translator to take news from across the globe and present it in your language, communicate with your customers in their own language, and more.

## Usage
Select a domain, then identify or select the language of text, and then translate the text from one supported language to another.

### Instantiating and authenticating the service
Before you can send requests to the service it must be instantiated and credentials must be set.
```cs
using IBM.Watson.DeveloperCloud.Services.LanguageTranslator.v2;
using IBM.Watson.DeveloperCloud.Utilities;

void Start()
{
    Credentials credentials = new Credentials(<username>, <password>, <url>);
    LanguageTranslator _languageTranslator = new LanguageTranslator(credentials);
}
```





### List models
Lists available models for language translation with option to filter by source or by target language.
```cs
private void GetModels()
{
  if (!_languageTranslator.GetModels(OnGetModels))
    Log.Debug("ExampleLanguageTranslator.GetModels()", "Failed to get models.");
}

private void OnGetModels(TranslationModels models, string data)
{
  Log.Debug("ExampleLanguageTranslator.OnGetModels()", "Language Translator - Get models response: {0}", data);
}
```





### Create a model
Uploads a TMX glossary file on top of a domain to customize a translation model.Depending on the size of the file, training can range from minutes for a glossary to several hours for a large parallel corpus. Glossary files must be less than 10 MB. The cumulative file size of all uploaded glossary and corpus files is limited to 250 MB.
```cs
private void CreateModel()
{
  if (!_languageTranslator.CreateModel(OnCreateModel, <base-model-name>, <custom-model-name>, <glossary-filepath>))
    Log.Debug("ExampleLanguageTranslator.CreateModel()", "Failed to create model.");
}

private void OnCreateModel(TranslationModel model, string data)
{
  Log.Debug("ExampleLanguageTranslator.OnCreateModel()", "Language Translator - Create model response: {0}", data);
}
```





### Get a model details
Returns information, including training status, about a specified translation model.
```cs
private void GetModel()
{
  if (!_languageTranslator.GetModel(OnGetModel, <custom-language-model-id>))
    Log.Debug("ExampleLanguageTranslator.GetModel()", "Failed to get model.");
}

private void OnGetModel(TranslationModel model, string data)
{
  Log.Debug("ExampleLanguageTranslator.OnGetModel()", "Language Translator - Get model response: {0}", data);
}
```





### Delete a model
Deletes trained translation models.
```cs
private void DeleteModel()
{
  if (!_languageTranslator.DeleteModel(OnDeleteModel, <custom-language-model-id>))
    Log.Debug("ExampleLanguageTranslator.DeleteModel()", "Failed to delete model.");
}

private void OnDeleteModel(bool success, string data)
{
  Log.Debug("ExampleLanguageTranslator.OnDeleteModel()", "Language Translator - Delete model response: success: {0}", success);
}
```





### Translate
Translates input text from the source language to the target language.
```cs
private void Translate()
{
  if (!_languageTranslator.GetTranslation(<text-to-translate>, <from-language>, <to-language>, OnGetTranslation))
    Log.Debug("ExampleLanguageTranslator.Translate()", "Failed to translate.");
}

private void OnGetTranslation(Translations translation, string data)
{
  Log.Debug("ExampleLanguageTranslator.OnGetTranslation()", "Langauge Translator - Translate Response: {0}", data);
}
```




### Identify language
Identify the language in which a text is written.
```cs
private void Identify()
{
  if (!_languageTranslator.Identify(OnIdentify, <text-to-identify>))
    Log.Debug("ExampleLanguageTranslator.Identify()", "Failed to identify language.");
}

private void OnIdentify(string lang, string data)
{
  Log.Debug("ExampleLanguageTranslator.OnIdentify()", "Language Translator - Identify response: {0}", data);
}
```





### Identifiable languages
Return the list of languages it can detect.
```cs
private void GetLanguages()
{
  if (!_languageTranslator.GetLanguages(OnGetLanguages))
    Log.Debug("ExampleLanguageTranslator.GetLanguages()", "Failed to get languages.");
}

private void OnGetLanguages(Languages languages, string data)
{
  Log.Debug("ExampleLanguageTranslator.OnGetLanguages()", "Language Translator - Get languages response: {0}", data);
}
```





[language_translator]: https://www.ibm.com/watson/services/language-translator/
