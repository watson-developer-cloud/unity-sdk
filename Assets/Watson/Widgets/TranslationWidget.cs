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
	public class TranslationWidget : Widget
    {
		#region Private Data
		private Translate m_Translate = new Translate();

		[SerializeField]
		private InputField m_Input = null;
		[SerializeField]
		private Dropdown m_DropDownFromLanguage = null;
		[SerializeField]
		private Dropdown m_DropDownToLanguage = null;

		private string[] m_LanguagesFrom = null;
		private string[] m_LanguagesTo = null;

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
				if(m_DropDownFromLanguage != null && m_LanguagesFrom != null && m_LanguagesFrom.Length > m_DropDownFromLanguage.value && m_DropDownFromLanguage.value >= 0){
					valueToReturn = m_LanguagesTo[m_DropDownFromLanguage.value];
				}
				return valueToReturn;
			}
		}

		public string ToLanguage{
			get{
				string valueToReturn = null;
				if(m_DropDownToLanguage != null && m_LanguagesTo != null && m_LanguagesTo.Length > m_DropDownToLanguage.value && m_DropDownToLanguage.value >= 0){
					valueToReturn = m_LanguagesTo[m_DropDownToLanguage.value];
				}
				return valueToReturn;
			}
		}

		#endregion

		private void OnEnable()
		{
			m_Translate.GetLanguages( OnGetLanguages );
		}
		
		private void OnDisable()
		{

		}

		/// <summary>
		/// Button event handler.
		/// </summary>
		public void OnTranslation()
		{
			if (m_Translate != null) {
				if(FromLanguage != null && ToLanguage != null){
					if (m_Input != null) {
						m_Translate.GetTranslation (m_Input.text, FromLanguage, ToLanguage, OnGetTranslation);
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

		private void OnGetTranslation( Translations translation )
		{
			if ( translation != null && translation.translations.Length > 0 ){
				Log.Status( "TestTranslate", "Translation: {0}", translation.translations[0].translation );
			}
			
		}

		private void OnGetLanguages( Languages languages )
		{
			if ( languages != null )
			{
				List<string> listLanguages = new List<string>();

				if(m_DropDownFromLanguage != null){
					m_DropDownFromLanguage.options.Add(new Dropdown.OptionData("Detect Language"));
					listLanguages.Add("xx");
				}
				foreach( var lang in languages.languages ){
					if(m_DropDownFromLanguage != null){
						m_DropDownFromLanguage.options.Add(new Dropdown.OptionData(lang.name));
					}
					if(m_DropDownToLanguage != null){
						m_DropDownToLanguage.options.Add(new Dropdown.OptionData(lang.name));
					}
					listLanguages.Add(lang.language);
				}

				m_LanguagesFrom = listLanguages.ToArray();
				listLanguages.RemoveAt(0);
				m_LanguagesTo = listLanguages.ToArray();
			}

		}
    }



}