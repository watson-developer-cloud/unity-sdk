using UnityEngine;
using System.Collections;
using IBM.Watson.Logging;
using IBM.Watson.Utilities;

public class MoodManager{

	public enum MoodType
	{
		Idle = 0,	//Bored
		Interested,
		Urgent,
		Upset,
		Shy
	}

	private MoodType m_currentMood = MoodType.Idle;
	public MoodType currentMood{
		get{
			return m_currentMood;
		}
		set{
			ChangeMood(value);
		}
	}

	private Color m_currentMoodColor = Color.white;
	public Color currentMoodColor{
		get{
			
			switch (currentMood) {
			case MoodType.Idle:
				m_currentMoodColor =  new Color(241 / 255.0f, 241 / 255.0f, 242 / 255.0f);	break;
			case MoodType.Interested:
				m_currentMoodColor =  new Color(131 / 255.0f, 209 / 255.0f, 245 / 255.0f);	break;
			case MoodType.Urgent:
				m_currentMoodColor =  new Color(241 / 255.0f, 241 / 255.0f, 242 / 255.0f);	break;
			case MoodType.Upset:
				m_currentMoodColor =  new Color(217 / 255.0f, 24 / 255.0f, 45 / 255.0f);	break;
			case MoodType.Shy:
				m_currentMoodColor =  new Color(243 / 255.0f, 137 / 255.0f, 175 / 255.0f);	break;
			default:
				Log.Error("MoodManager", "MoodType is not defined for color!");
				m_currentMoodColor = Color.white;
				break;
			}
			return m_currentMoodColor;
		}
	}

	public void ChangeMood(MoodType moodType){
		m_currentMood = moodType;
		//TODO: Sent event to change mood!
	}
}
