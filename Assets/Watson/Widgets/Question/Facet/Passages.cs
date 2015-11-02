using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace IBM.Watson.Widgets.Question
{
	/// <summary>
	/// Handles all Passages Facet functionality. 
	/// </summary>
	public class Passages : Base {
		private float m_RectTransformPosX = 9f;
		private float m_RectTransformPosY = -77f;
		private float m_RectTransformPosZ = 375f;
		private float m_RectTransformZSpacing = -50f;

		[SerializeField]
		private GameObject m_PassageItemPrefab;

		[SerializeField]
		private RectTransform m_PassageCanvasRectTransform;

		private List<PassageItem> m_PassageItems = new List<PassageItem>();
		
		/// <summary>
		/// Dynamically creates Passage Items based on data.
		/// </summary>
		override public void Init()
		{
			base.Init ();

			for(int i = 0; i < m_Question.QuestionData.AnswerDataObject.answers[0].evidence.Length; i++) {
				GameObject PassageItemGameObject = Instantiate(m_PassageItemPrefab, new Vector3(m_RectTransformPosX, m_RectTransformPosY, m_RectTransformPosZ + m_RectTransformZSpacing * i), Quaternion.identity) as GameObject;
				RectTransform PassageItemRectTransform = PassageItemGameObject.GetComponent<RectTransform>();
				PassageItemRectTransform.SetParent(m_PassageCanvasRectTransform, false);
				PassageItem PassageItem = PassageItemGameObject.GetComponent<PassageItem>();
				m_PassageItems.Add(PassageItem);
				PassageItem.PassageString = m_Question.QuestionData.AnswerDataObject.answers[0].evidence[i].passage;
				PassageItem.Confidence = m_Question.QuestionData.AnswerDataObject.answers[0].confidence;
			}
		}
		
		/// <summary>
		/// Clears dynamically generated Facet Elements when a question is answered. Called from Question Widget.
		/// </summary>
		override protected void Clear()
		{
			while (m_PassageItems.Count != 0)
			{
				Destroy(m_PassageItems[0].gameObject);
				m_PassageItems.Remove(m_PassageItems[0]);
			}
		}
	}
}