using UnityEngine;
using System.Collections;
using IBM.Watson.Utilities;
using IBM.Watson.Logging;

public class BehaviourManager {

	public enum BehaviourType{
		Idle = 0,
		Listening,
		Thinking,
		Speaking,
		DidntUnderstand //Embarrassed
	}

	private BehaviourType m_currentBehaviour = BehaviourType.Idle;
	public BehaviourType currentBehaviour{
		get{
			return m_currentBehaviour;
		}
		set{
			ChangeBehaviour(value);
		}
	}

	private Color m_currentBehaviourColor = Color.white;
	public Color currentBehaviourColor{
		get{

			switch (currentBehaviour) {
				case BehaviourType.Idle:
					m_currentBehaviourColor =  new Color(241 / 255.0f, 241 / 255.0f, 242 / 255.0f);	break;
				case BehaviourType.Listening:
					m_currentBehaviourColor =  new Color(0 / 255.0f, 166 / 255.0f, 160 / 255.0f);	break;
				case BehaviourType.Thinking:
					m_currentBehaviourColor =  new Color(238 / 255.0f, 62 / 255.0f, 150 / 255.0f);	break;
				case BehaviourType.Speaking:
					m_currentBehaviourColor =  new Color(140 / 255.0f, 198 / 255.0f, 63 / 255.0f);	break;
				case BehaviourType.DidntUnderstand:
					m_currentBehaviourColor =  new Color(243 / 255.0f, 137 / 255.0f, 175 / 255.0f);	break;
				default:
					Log.Error("BehaviourManager", "BehaviourType is not defined for color!");
					m_currentBehaviourColor = Color.white;
					break;
				}
			return m_currentBehaviourColor;
		}
	}

	public void ChangeBehaviour(BehaviourType behaviourType){
		m_currentBehaviour = behaviourType;
		//TODO: Sent event to change behaviour!

	}
}
