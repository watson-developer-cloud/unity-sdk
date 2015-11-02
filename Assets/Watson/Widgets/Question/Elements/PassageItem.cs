using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace IBM.Watson.Widgets.Question
{
	/// <summary>
	/// Controls PassageItem view.
	/// </summary>
	public class PassageItem : MonoBehaviour {
		[SerializeField]
		private Text m_PassageText;

		[SerializeField]
		private TabItem m_TabItem;

		private string m_Passage;
		public string Passage
		{
			get { return m_Passage; }
			set
			{
				m_Passage = value;
				UpdatePassage();
			}
		}

		private double m_Confidence = 0f;
		public string Confidence
		{
			get { return m_Confidence; }
			set
			{
				m_Confidence = value;
				UpdateConfidence();
			}
		}

		private double m_MaxConfidence = 1f;
		public double MaxConfidence
		{
			get { return m_MaxConfidence; }
			set
			{
				m_MaxConfidence = value;
				UpdateConfidence();
			}
		}
		
		private double m_MinConfidence = 0f;
		public double MinConfidence
		{
			get { return m_MinConfidence; }
			set
			{
				m_MinConfidence = value;
				UpdateConfidence();
			}
		}

		/// <summary>
		/// Update the passage view.
		/// </summary>
		private void UpdatePassage()
		{
			//	TODO format passage
			m_PassageText.text = Passage;
		}

		/// <summary>
		/// Update the confidence tab view.
		/// </summary>
		private void UpdateConfidence()
		{

		}
	}
}