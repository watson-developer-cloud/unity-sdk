using UnityEngine;
using System.Collections;

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

	private Color m_currentBehaviourColor = null;
	public Color currentBehaviourColor{
		get{
			switch (currentBehaviour) {
			case BehaviourType.Idle:
				m_currentBehaviourColor =  new Color(241 / 255.0f, 241 / 255.0f, 242 / 255.0f);	break;
			case BehaviourType.Listening:
				m_currentBehaviourColor =  new Color(131 / 255.0f, 209 / 255.0f, 245 / 255.0f);	break;
			case BehaviourType.Thinking:
				m_currentBehaviourColor =  new Color(241 / 255.0f, 241 / 255.0f, 242 / 255.0f);	break;
			case BehaviourType.Speaking:
				m_currentBehaviourColor =  new Color(217 / 255.0f, 24 / 255.0f, 45 / 255.0f);	break;
			case BehaviourType.DidntUnderstand:
				m_currentBehaviourColor =  new Color(243 / 255.0f, 137 / 255.0f, 175 / 255.0f);	break;
			default:
			break;
			}

		}
	}

	public void ChangeBehaviour(BehaviourType behaviourType){
		m_currentBehaviour = behaviourType;
		//TODO: Sent event to change behaviour!
	}
}
