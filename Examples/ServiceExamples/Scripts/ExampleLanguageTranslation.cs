using UnityEngine;
using IBM.Watson.DeveloperCloud.Services.LanguageTranslation.v1;

public class ExampleLanguageTranslation : MonoBehaviour {
	private LanguageTranslation m_Translate = new LanguageTranslation();
	private string m_PharseToTranslate = "How do I get to the disco?";
	
	void Start ()
	{
		Debug.Log("English Phrase to translate: " + m_PharseToTranslate);
		m_Translate.GetTranslation(m_PharseToTranslate, "en", "es", OnGetTranslation);
	}

	private void OnGetTranslation(Translations translation)
	{
		if (translation != null && translation.translations.Length > 0)
			Debug.Log("Spanish Translation: " + translation.translations[0].translation);
	}
}
