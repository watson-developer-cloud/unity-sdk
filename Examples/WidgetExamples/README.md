# Widget Examples

These example scenes show how to create a simple application by combining Widget functionality.

## Usage

### ExampleDialog
Open the ExampleDialog scene. In the ExampleDialog GameObject click on the DialogWidget GameObject. The dialog file to be uploaded is specified in the Inspector under "Auto Upload Dialog". Specify a **unique** name for the dialog instance under "Dialog Name". The upload will result in an error if a unique name is not provided. At runtime, the Widgets will automatically connect their inputs and outputs resulting in a simple dialog application using [Speech to Text][speech_to_text] and [Dialog][dialog].

Be sure a microphone is connected and enabled on your computer. Play the scene in the Unity editor and say "Hello" to Watson.

### ExampleLanguageTranslation
Open the ExampleLanguageTranslation scene. At runtime, the Widgets will automatically connect their inputs and outputs resulting in a simple language translation application using [Speech to Text][speech_to_text], [Language Translation][language_translation] and [Text to Speech][text_to_speech]. 

Be sure a microphone is connected and enabled on your computer. Play the scene in the Unity editor and speak to Watson. Alternatively, phrases can be typed into the top field for translation. The [Language Translation][language_translation] service will auto detect language and translate the phrased to Spanish by default. Other languages are listed in the dropdowns above each field.

[speech_to_text]: http://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/doc/speech-to-text/
[text_to_speech]: http://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/doc/text-to-speech/
[language_translation]: http://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/doc/language-translation/
[dialog]: http://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/doc/dialog/
[natural_language_classifier]: http://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/doc/nl-classifier/