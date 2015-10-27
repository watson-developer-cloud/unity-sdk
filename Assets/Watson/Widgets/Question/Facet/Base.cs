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
		public AvatarWidget Avatar { get; set; }

		/// <summary>
		/// Holds a reference to the Questions from the Question Widget.
		/// </summary>
		private ITM.Questions _Questions;
		public ITM.Questions Questions
		{
			get { return _Questions; }
			set
			{
				_Questions = value;
				OnQuestionData ();
			}
		}

		/// <summary>
		/// Holds a reference to to the Answers from the Question Widget.
		/// </summary>
		private ITM.Answers _Answers;
		public ITM.Answers Answers
		{
			get { return _Answers; }
			set
			{
				_Answers = value;
				OnAnswerData ();
			}
		}

		/// <summary>
		/// Holds a reference to the Parse Data from the Question Widget.
		/// </summary>
		private ITM.ParseData _ParseData;
		public ITM.ParseData ParseData
		{
			get { return _ParseData; }
			set
			{
				_ParseData = value;
				OnParseData ();
			}
		}

		public virtual void Init() {}
		protected virtual void Show() {}
		protected virtual void Hide() {}

		/// <summary>
		/// Clears dynamically generated Facet Elements when a question is answered. Called from Question Widget.
		/// </summary>
		public virtual void Clear() {}

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