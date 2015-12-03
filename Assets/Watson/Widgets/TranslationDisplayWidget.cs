/**
* Copyright 2015 IBM Corp. All Rights Reserved.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/

using UnityEngine;
using UnityEngine.UI;
using IBM.Watson.DataModels;
using IBM.Watson.Logging;
using IBM.Watson.Services.v1;
using IBM.Watson.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;

namespace IBM.Watson.Widgets
{

    /// <summary>
    /// Translation widget to handle translation service calls
    /// </summary>
	public class TranslationDisplayWidget : Widget
    {
		#region Private Data
		private Translate m_Translate = new Translate();

		[SerializeField]
		private InputField m_Input = null;
		[SerializeField]
		private InputField m_Output = null;
		[SerializeField]
		private Dropdown m_DropDownSourceLanguage = null;
		[SerializeField]
		private Dropdown m_DropDownTargetLanguage = null;
		[SerializeField]
		private TextToSpeechWidget m_TextToSpeechToChangeVoice = null;
		[SerializeField]
		private Button m_TextToSpeechButton = null;

		private Dictionary<string, string> m_LanguageToIdentify = new Dictionary<string, string> ();
		private Dictionary<string, List<string>> m_LanguageToTranslate = new Dictionary<string, List<string>> ();
		private string[] m_LanguagesFrom = null;
		//private string[] m_LanguagesTo = null;

		private string m_FromDetectedLanguage = null;
		private string m_DetectLanguage = "DETECT";
		private string m_DetectLanguageName = "Detect Language";
		private string m_DetectedLanguageNameFormat = "Detected: {0}";
		private string m_DefaultLanguageFromTranslate = "DETECT";
		private string m_DefaultLanguageToTranslate = "en";
		private string m_DefaultDomainToUse = "conversational";
		private float m_ThresholdTimeToTranslate = 1.0f; //in each second there can be a translate request not more than it. 
		private float m_TimeLastTranslateRequest = 0.0f;
		private bool m_IsThereTranslationWaiting = false;

		private int m_ThresholdCharacterCountToStopIdentifyLanguage = 10; //after 10 character it stops calling identify language
		private float m_ThresholdTimeToIdentify = 1.0f; //in each second there can be a translate request not more than it. 
		private float m_TimeLastIdentifyRequest = 0.0f;
		private bool m_IsThereIdentifynWaiting = false;
		#endregion

        #region Widget interface
        protected override string GetName()
        {
			return "Translation";
        }
        #endregion

		#region Public Members

		public string FromLanguage{
			get{
				string valueToReturn = null;

				if(m_DropDownSourceLanguage != null && m_LanguageToTranslate.Keys != null && m_LanguageToTranslate.Keys.Count > m_DropDownSourceLanguage.value && m_DropDownSourceLanguage.value >= 0){
					valueToReturn = m_LanguagesFrom[m_DropDownSourceLanguage.value];
				}

				return valueToReturn;
			}
		}

		public string ToLanguage{
			get{
				string valueToReturn = null;
				if(m_DropDownTargetLanguage != null && m_LanguageToTranslate != null && FromLanguage != null && m_LanguageToTranslate.ContainsKey(FromDetectedLanguage) && m_LanguageToTranslate[FromDetectedLanguage].Count > m_DropDownTargetLanguage.value && m_DropDownTargetLanguage.value >= 0){
					valueToReturn = m_LanguageToTranslate[FromDetectedLanguage][m_DropDownTargetLanguage.value];
				}
				return valueToReturn;
			}
		}

		public string FromDetectedLanguage{
			get{
				string valueToReturn = null;
				if(m_DropDownSourceLanguage != null && !string.IsNullOrEmpty(m_FromDetectedLanguage) && m_DropDownSourceLanguage.value == 0){
					valueToReturn = m_FromDetectedLanguage;
				}
				if(string.IsNullOrEmpty(valueToReturn))
					valueToReturn = FromLanguage;

				return valueToReturn;
			}
		}

		public bool IsFromLanguageToDetect{
			get{
				return string.Equals(FromLanguage, m_DetectLanguage);
			}
		}

		public bool IsFromLanguageToDetectedAlready{
			get{
				return !string.Equals(FromDetectedLanguage, m_DetectLanguage);
			}
		}

		public bool IsSameLanguage{	//No need to call translate
			get{
				return String.Equals(FromDetectedLanguage, ToLanguage);
			}
		}

		#endregion

		void OnEnable()
		{
			Log.Status ("TranslationWidget", "OnEnable");
			//UnityEngine.Debug.LogWarning("TranslationWidget - OnEnable");
			m_Translate.GetLanguages( OnGetLanguagesAndGetModelsAfter );
		}
		
		void OnDisable()
		{

		}

		void Update(){
			if (m_IsThereTranslationWaiting && Time.time - m_TimeLastTranslateRequest > m_ThresholdTimeToTranslate) {
				m_IsThereTranslationWaiting = false;
				OnTranslation();
			}

			if (m_IsThereIdentifynWaiting && Time.time - m_TimeLastIdentifyRequest > m_ThresholdTimeToIdentify) {
				m_IsThereIdentifynWaiting = false;
				IdentifyLanguageFromInputTextIfNeeded();
			}
		}

		/// <summary>
		/// Button event handler.
		/// </summary>
		public void OnTranslation()
		{
			if (m_Translate != null) {
				if(FromLanguage != null && ToLanguage != null){
					if (m_Input != null) {
						Translate(m_Input.text);
						IdentifyLanguageFromInputTextIfNeeded();

					}
					else{
						Log.Error("TranslationWidget", "OnTranslation - Input is null");
					}
				}
				else{
					Log.Error("TranslationWidget", "OnTranslation - From language and To Language should be set!");
				}

			} else {
				Log.Error("TranslationWidget", "OnTranslation - Translate object is null");
			}
		}

		private void IdentifyLanguageFromInputTextIfNeeded(){
			if(IsFromLanguageToDetect && !string.IsNullOrEmpty(m_Input.text) && ( !IsFromLanguageToDetectedAlready || m_Input.text.Length < m_ThresholdCharacterCountToStopIdentifyLanguage) ){	//Identify and translate. If already detected then check the length of the char to no to detect one more time.

				if(Time.time - m_TimeLastIdentifyRequest > m_ThresholdTimeToIdentify){
					m_TimeLastIdentifyRequest = Time.time;
					m_Translate.Identify(m_Input.text, OnIdentifyAndTranslateFromLanguage );
					m_IsThereIdentifynWaiting = false;
				}
				else{
					m_IsThereIdentifynWaiting = true;
				}

			}

		}
		
		private void Translate(string text){
			if (!string.IsNullOrEmpty (text)) {
				if (IsSameLanguage || (IsFromLanguageToDetect && !IsFromLanguageToDetectedAlready)) {	//same language or it is still not detected!
					SetOutput(text);
				} else {
					if(Time.time - m_TimeLastTranslateRequest > m_ThresholdTimeToTranslate){
						m_TimeLastTranslateRequest = Time.time;
						m_Translate.GetTranslation (m_Input.text, FromDetectedLanguage, ToLanguage, OnGetTranslation);
						m_IsThereTranslationWaiting = false;
					}
					else{
						m_IsThereTranslationWaiting = true;
					}
				}
			} else {
				//translation text is empty - clear output text
				SetOutput("");
			}


		}

		private void SetOutput(string text){
			if (m_Output != null) {
				m_Output.text = text;
			}
		}

		private void OnGetTranslation( Translations translation )
		{
			if ( translation != null && translation.translations.Length > 0 ){
				//Log.Status( "TranslationWidget", "OnGetTranslation - Translation: {0}", translation.translations[0].translation );
				SetOutput( translation.translations[0].translation );
			}
		}

		private void OnGetLanguagesAndGetModelsAfter( Languages languages )
		{
			if (languages != null && languages.languages.Length > 0) {
				Log.Status( "TranslationWidget", "OnGetLanguagesAndGetModelsAfter as {0}", languages.languages.Length );
				m_LanguageToIdentify.Clear();

				foreach (var lang in languages.languages) {
					m_LanguageToIdentify[lang.language] = lang.name;
				}

				m_LanguageToIdentify[m_DetectLanguage] = m_DetectLanguageName;

				m_Translate.GetModels( OnGetModels );	//To fill dropdown with models to use in Translation

			} else {
				Log.Error("TranslationWidget", "OnGetLanguages - There is no language to translate. Check the connections and service of Translation Service.");
			}

		}

		private void OnIdentifyAndTranslateFromLanguage(string lang){
			//Log.Status( "TranslationWidget", "OnIdentifyAndTranslateFromLanguage as {0}", lang );

			//Identified language is different then current language
			if(!string.Equals(lang, FromDetectedLanguage)){

				for (int i = 0; m_LanguagesFrom != null && i < m_LanguagesFrom.Length; i++) {
					if(String.Equals(m_LanguagesFrom[i], lang)){
						m_FromDetectedLanguage = lang;
						m_DropDownSourceLanguage.captionText.text = string.Format(m_DetectedLanguageNameFormat, m_LanguageToIdentify[lang]);
						m_DropDownSourceLanguage.options[0].text = string.Format(m_DetectedLanguageNameFormat, m_LanguageToIdentify[lang]);
						ResetTargetLanguageDropDown();
						ResetVoiceForTargetLanguage();
						Translate(m_Input.text);
						break;
					}
				}
			}


		}

		private IEnumerator EnableInteractiveDropDown(){
			yield return  null;
			m_DropDownSourceLanguage.interactable = true;
		}

		private void OnGetModels( TranslationModels models )
		{
			Log.Status( "TranslationWidget", "OnGetModels as {0}", models.models.Length );

			if ( models != null && models.models.Length > 0)
			{
				m_LanguageToTranslate.Clear();
				List<string> listLanguages = new List<string> ();	//From - To language list to use in translation

				m_DropDownSourceLanguage.options.Clear ();
				m_DropDownTargetLanguage.options.Clear ();
				int defaultInitialValueFromLanguage = 0;
				int defaultInitialValueToLanguage = 0;

				//Adding initial language as detected!
				listLanguages.Add (m_DetectLanguage);
				m_LanguageToTranslate.Add(m_DetectLanguage, new List<string>());

				foreach( var model in models.models )
				{
					if(string.Equals( model.domain, m_DefaultDomainToUse)){

						if(m_LanguageToTranslate.ContainsKey(model.source)){
							if(!m_LanguageToTranslate[model.source].Contains(model.target))
								m_LanguageToTranslate[model.source].Add (model.target);
						}
						else{
							m_LanguageToTranslate.Add(model.source, new List<string>());
							m_LanguageToTranslate[model.source].Add (model.target);
						}

						if(!listLanguages.Contains(model.source)){
							listLanguages.Add(model.source);
						}
						if(!listLanguages.Contains(model.target)){
							listLanguages.Add(model.target);
						}

						if(!m_LanguageToTranslate[m_DetectLanguage].Contains(model.target))
							m_LanguageToTranslate[m_DetectLanguage].Add (model.target);
					}

					//Log.Status( "TestTranslate", "ModelID: {0}, Source: {1}, Target: {2}, Domain: {3}", model.model_id, model.source, model.target, model.domain );
				}

				//Adding all languages to SourceLanguage dropdown
				foreach (string itemLanguage in listLanguages) {
					if(m_LanguageToIdentify.ContainsKey(itemLanguage)){
						m_DropDownSourceLanguage.options.Add (new Dropdown.OptionData (m_LanguageToIdentify[itemLanguage]));
					}

					if(String.Equals(m_DefaultLanguageFromTranslate, itemLanguage)){
						defaultInitialValueFromLanguage = m_DropDownSourceLanguage.options.Count - 1; 
					}

				}
				m_LanguagesFrom = listLanguages.ToArray ();
				
				m_DropDownSourceLanguage.value = defaultInitialValueFromLanguage;

			}
		}

		public void DropDownSourceValueChanged(){

			if(m_DropDownSourceLanguage.value != 0){
				m_DropDownSourceLanguage.options[0].text = m_LanguageToIdentify[m_DetectLanguage];
				m_FromDetectedLanguage = null;
			}

			ResetTargetLanguageDropDown ();
			ResetVoiceForTargetLanguage ();
			OnTranslation ();
		}

		public void DropDownTargetValueChanged(){
			OnTranslation ();
			ResetVoiceForTargetLanguage ();
		}

		public void ResetTargetLanguageDropDown(){
			if (FromLanguage != null) {

				//Add all languages, because first item is detect language
				if(m_LanguageToTranslate != null && string.Equals(m_DetectLanguage, FromDetectedLanguage)){
					string languageToPrevious = ToLanguage;
					m_DropDownTargetLanguage.options.Clear ();
					m_DropDownTargetLanguage.value = -1;
					int defaultInitialValueToLanguage = 0;

					foreach (string itemLanguage in m_LanguageToTranslate[m_DetectLanguage]) {
						if(string.Equals(itemLanguage, m_DetectLanguage))
						   continue;

						m_DropDownTargetLanguage.options.Add (new Dropdown.OptionData (m_LanguageToIdentify[itemLanguage]));
						
						if(String.Equals(m_DefaultLanguageToTranslate, itemLanguage)){
							defaultInitialValueToLanguage = m_DropDownTargetLanguage.options.Count - 1; 
						}
					}

					m_DropDownTargetLanguage.captionText.text = m_DropDownTargetLanguage.options[0].text;
					m_DropDownTargetLanguage.value = defaultInitialValueToLanguage;

				}
				//Add target language corresponding source language
				else if(m_LanguageToTranslate != null && m_LanguageToTranslate.ContainsKey(FromDetectedLanguage) && m_LanguageToTranslate[FromDetectedLanguage] != null){
					string languageToPrevious = ToLanguage;
					m_DropDownTargetLanguage.options.Clear ();
					m_DropDownTargetLanguage.value = -1;
					int defaultInitialValueToLanguage = 0;

					foreach (string itemLanguage in m_LanguageToTranslate[FromDetectedLanguage]) {
						if(string.Equals(itemLanguage, m_DetectLanguage))
							continue;

						m_DropDownTargetLanguage.options.Add (new Dropdown.OptionData (m_LanguageToIdentify[itemLanguage]));

						if(String.Equals(m_DefaultLanguageToTranslate, itemLanguage)){
							defaultInitialValueToLanguage = m_DropDownTargetLanguage.options.Count - 1; 
						}
					}

					m_DropDownTargetLanguage.captionText.text = m_DropDownTargetLanguage.options[0].text;
					m_DropDownTargetLanguage.value = defaultInitialValueToLanguage;

				}
				else{
					Log.Error("TranslationWidget", "ResetTargetLanguageDropDown - There is invalid condition. ");
				}

			} else {
				Log.Error("TranslationWidget", "ResetTargetLanguageDropDown - Source language has not been set.");
			}
		}

		private void ResetVoiceForTargetLanguage(){
			if (m_TextToSpeechToChangeVoice != null) {
				bool enableSpeechButton = true;
				if(string.Equals( ToLanguage, "en")){
					m_TextToSpeechToChangeVoice.Voice = TextToSpeech.VoiceType.en_US_Michael;
				}
				else if(string.Equals( ToLanguage, "de")){
					m_TextToSpeechToChangeVoice.Voice = TextToSpeech.VoiceType.de_DE_Dieter;
				}
				else if(string.Equals( ToLanguage, "es")){
					m_TextToSpeechToChangeVoice.Voice = TextToSpeech.VoiceType.es_ES_Enrique;
				}
				else if(string.Equals( ToLanguage, "fr")){
					m_TextToSpeechToChangeVoice.Voice = TextToSpeech.VoiceType.fr_FR_Renee;
				}
				else if(string.Equals( ToLanguage, "it")){
					m_TextToSpeechToChangeVoice.Voice = TextToSpeech.VoiceType.it_IT_Francesca;
				}
				else if(string.Equals( ToLanguage, "ja")){
					m_TextToSpeechToChangeVoice.Voice = TextToSpeech.VoiceType.ja_JP_Emi;
				}
				else{
					//disable Speaking button because there is no proper voice to talk
					enableSpeechButton = false;
				}

				if(m_TextToSpeechButton != null)
					m_TextToSpeechButton.interactable = enableSpeechButton;
			}
		}


    }



}