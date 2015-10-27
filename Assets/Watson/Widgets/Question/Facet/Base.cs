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
using IBM.Watson.Widgets;
using IBM.Watson.Services.v1;

namespace IBM.Watson.Widgets.Question.Facet
{
	public class Base: MonoBehaviour
	{
		/// <summary>
		/// Holds a reference to the Avatar from the Question Widget.
		/// </summary>
		/// <value>The Avatar.</value>
		public AvatarWidget m_Avatar { get; set; }

		/// <summary>
		/// Holds a reference to the Questions from the Question Widget.
		/// </summary>
		private ITM.Questions _m_Questions;
		public ITM.Questions m_Questions
		{
			get { return _m_Questions; }
			set
			{
				_m_Questions = value;
				OnQuestionData ();
			}
		}

		/// <summary>
		/// Holds a reference to to the Answers from the Question Widget.
		/// </summary>
		private ITM.Answers _m_Answers;
		public ITM.Answers m_Answers
		{
			get { return _m_Answers; }
			set
			{
				_m_Answers = value;
				OnAnswerData ();
			}
		}

		/// <summary>
		/// Holds a reference to the Parse Data from the Question Widget.
		/// </summary>
		private ITM.ParseData _m_ParseData;
		public ITM.ParseData m_ParseData
		{
			get { return _m_ParseData; }
			set
			{
				_m_ParseData = value;
				OnParseData ();
			}
		}

		public virtual void Init() {}
		protected virtual void Show() {}
		protected virtual void Hide() {}

		/// <summary>
		/// Fired when Parse Data is set.
		/// </summary>
		protected virtual void OnParseData() {}

		/// <summary>
		/// Fired when Answer Data is set.
		/// </summary>
		protected virtual void OnAnswerData() {}

		/// <summary>
		/// Fired when Question Data is set.
		/// </summary>
		protected virtual void OnQuestionData() {}
	}
}