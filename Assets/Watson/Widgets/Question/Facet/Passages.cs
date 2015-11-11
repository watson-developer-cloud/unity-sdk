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
using System.Collections.Generic;
using IBM.Watson.Logging;
using IBM.Watson.Utilities;
using IBM.Watson.Data;

namespace IBM.Watson.Widgets.Question
{
	/// <summary>
	/// Handles all Passages Facet functionality. 
	/// </summary>
	public class Passages : Base {
		private float m_RectTransformPosX = -555f;
		private float m_RectTransformPosY = -640.2f;
		private float m_RectTransformPosZ = 375f;
		private float m_RectTransformZSpacing = -50f;

		[SerializeField]
		private GameObject m_PassageItemPrefab;

		[SerializeField]
		private RectTransform m_PassageCanvasRectTransform;

		private List<PassageItem> m_PassageItems = new List<PassageItem>();

		private Data.XRAY.Answers m_AnswerData = null;
		
		private void OnEnable()
		{
			EventManager.Instance.RegisterEventReceiver( Constants.Event.ON_QUESTION_ANSWERS, OnAnswerData );
		}
		
		private void OnDisable()
		{
			EventManager.Instance.UnregisterEventReceiver( Constants.Event.ON_QUESTION_ANSWERS, OnAnswerData );
		}
		
		/// <summary>
		/// Dynamically creates Passage Items based on data.
		/// </summary>
		override public void Init()
		{
			Log.Debug("Passages", "m_PassageItems.count: " + m_PassageItems.Count);
			base.Init ();

			for(int i = 0; i < m_AnswerData.answers.Length ; i++) {
				Log.Debug("Passages", "adding passage " + i);
				GameObject PassageItemGameObject = Instantiate(m_PassageItemPrefab, new Vector3(m_RectTransformPosX, m_RectTransformPosY, m_RectTransformPosZ + m_RectTransformZSpacing * (m_AnswerData.answers.Length - i)), Quaternion.identity) as GameObject;
				PassageItemGameObject.name = "PassageItem_" + i.ToString("00");
				RectTransform PassageItemRectTransform = PassageItemGameObject.GetComponent<RectTransform>();
				PassageItemRectTransform.SetParent(m_PassageCanvasRectTransform, false);
				PassageItem PassageItem = PassageItemGameObject.GetComponent<PassageItem>();
				PassageItemRectTransform.pivot = Vector2.zero; 	//setting pivot as bottom left
				PassageItemRectTransform.SetAsFirstSibling();
				m_PassageItems.Add(PassageItem);
//				PassageItem.PassageString = m_Question.QuestionData.AnswerDataObject.answers[0].evidence[i].passage;
				PassageItem.PassageString = m_AnswerData.answers[i].answerText;
				PassageItem.MaxConfidence = m_AnswerData.answers[0].confidence;
				PassageItem.MinConfidence = m_AnswerData.answers[m_AnswerData.answers.Length - 1].confidence;
				PassageItem.Confidence = m_AnswerData.answers[i].confidence;
			}
		}
		
		/// <summary>
		/// Clears dynamically generated Facet Elements when a question is answered. Called from Question Widget.
		/// </summary>
		override public void Clear()
		{
			while (m_PassageItems.Count != 0)
			{
				Destroy(m_PassageItems[0].gameObject);
				m_PassageItems.Remove(m_PassageItems[0]);
			}
		}

		private void OnAnswerData( object [] args )
		{
			m_AnswerData = args != null && args.Length > 0 ? args[0] as Data.XRAY.Answers : null;
			Init ();
		}
	}
}