using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace IBM.Watson.Widgets.Question
{
	/// <summary>
	/// Handles all Passages Facet functionality. 
	/// </summary>
	public class Passages : Base {

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
			
//			for (int i = 0; i < m_Question.QuestionData.AnswerDataObject.answers[0].features.Length; i++)
//			{
//				GameObject featureItemGameObject = Instantiate(m_PassageItemPrefab, new Vector3(95f, -i * 50f - 150f, 0f), Quaternion.identity) as GameObject;
//				RectTransform featureItemRectTransform = featureItemGameObject.GetComponent<RectTransform>();
//				featureItemRectTransform.SetParent(m_PassageCanvasRectTransform, false);
//				FeatureItem featureItem = featureItemGameObject.GetComponent<FeatureItem>();
//				m_PassageItems.Add(featureItem);
//				featureItem.FeatureString = m_Question.QuestionData.AnswerDataObject.answers[0].features[i].displayLabel;
//				featureItem.FeatureIndex = m_Question.QuestionData.AnswerDataObject.answers[0].features[i].weightedScore;
//			}
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