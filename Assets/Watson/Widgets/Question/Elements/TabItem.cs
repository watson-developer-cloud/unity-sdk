using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace IBM.Watson.Widgets.Question
{
	/// <summary>
	/// Controls TabItem view.
	/// </summary>
	public class TabItem : MonoBehaviour {
		/// <summary>
		/// The m_ confidence text.
		/// </summary>
		[SerializeField]
		private Text m_ConfidenceText;


		private double m_Confidence;
		public double Confidence
		{
			get { return m_Confidence; }
			set
			{
				m_Confidence = value;
				UpdateConfidence();
			}
		}

		/// <summary>
		/// Update the confidence view.
		/// </summary>
		private void UpdateConfidence()
		{
			float confidence = (float)Confidence * 100;
			m_ConfidenceText.text = confidence.ToString("f1");
		}
	}
}