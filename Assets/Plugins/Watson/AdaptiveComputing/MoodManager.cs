using UnityEngine;
using System.Collections;

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

	public void ChangeMood(MoodType moodType){
		m_currentMood = moodType;
		//TODO: Sent event to change mood!
	}
}
