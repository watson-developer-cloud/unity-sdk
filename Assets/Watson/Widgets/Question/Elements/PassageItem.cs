using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace IBM.Watson.Widgets.Question
{
	/// <summary>
	/// Controls PassageItem view.
	/// </summary>
	public class PassageItem : MonoBehaviour {
		/// <summary>
		/// The passage text.
		/// </summary>
		[SerializeField]
		private Text m_PassageText;

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

		/// <summary>
		/// Update the passage view.
		/// </summary>
		private void UpdatePassage()
		{
			//	TODO format passage
			m_PassageText.text = Passage;
		}
	}
}