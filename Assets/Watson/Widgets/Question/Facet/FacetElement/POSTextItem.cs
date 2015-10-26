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
* @author Taj Santiago
*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace IBM.Watson.Widgets.Question.Facet.FacetElement
{
	public class POSTextItem : MonoBehaviour
		{
		[SerializeField]
		private Text m_POSTextField;
		
		private string _m_POSWord;
		public string m_POSWord
		{
			get { return _m_POSWord; }
			set 
			{
				_m_POSWord = value;
				UpdatePOSTextField();
			}
		}

		/// <summary>
		/// Update the POS view.
		/// </summary>
		private void UpdatePOSTextField()
		{
			m_POSTextField.text = m_POSWord;
		}
	}
}
