using UnityEngine;
using System.Collections;
using IBM.Watson.Utilities;
using IBM.Watson.Logging;

namespace IBM.Watson.AdaptiveComputing
{
	public enum BehaviorType{
		Idle = 0,
		Listening,
		Thinking,
		Speaking,
		DidntUnderstand //Embarrassed
	}

	public class BehaviorManager : MonoBehaviour {

        void OnApplicationQuit()
        {
            DestroyImmediate( gameObject );
        }
		void OnEnable(){
			EventManager.Instance.RegisterEventReceiver (EventManager.onBehaviorChange, OnChangeBehavior);
		}
		void OnDisable(){
			EventManager.Instance.UnregisterEventReceiver (EventManager.onBehaviorChange, OnChangeBehavior);
		}
		void OnApplicationQuit() {
			DestroyImmediate (gameObject);
		}

		public static BehaviorManager Instance { get { return Singleton<BehaviorManager>.Instance; } }

		private BehaviorType m_currentBehavior = BehaviorType.Idle;
		public BehaviorType currentBehavior{
			get{
				return m_currentBehavior;
			}
			set{
				ChangeBehaviour(value);
			}
		}


		private Color m_currentBehaviourColor = Color.white;
		public Color currentBehaviourColor{
			get{

				switch (currentBehavior) {
					case BehaviorType.Idle:
						m_currentBehaviourColor =  new Color(241 / 255.0f, 241 / 255.0f, 242 / 255.0f);	break;
					case BehaviorType.Listening:
						m_currentBehaviourColor =  new Color(0 / 255.0f, 166 / 255.0f, 160 / 255.0f);	break;
					case BehaviorType.Thinking:
						m_currentBehaviourColor =  new Color(238 / 255.0f, 62 / 255.0f, 150 / 255.0f);	break;
					case BehaviorType.Speaking:
						m_currentBehaviourColor =  new Color(140 / 255.0f, 198 / 255.0f, 63 / 255.0f);	break;
					case BehaviorType.DidntUnderstand:
						m_currentBehaviourColor =  new Color(243 / 255.0f, 137 / 255.0f, 175 / 255.0f);	break;
					default:
						Log.Error("BehaviourManager", "BehaviourType is not defined for color!");
						m_currentBehaviourColor = Color.white;
						break;
					}
				return m_currentBehaviourColor;
			}
		}

		BehaviorType[] m_behaviorTypeList = null;
		public BehaviorType[] behaviorTypeList{
			get{
				if(m_behaviorTypeList == null){
					m_behaviorTypeList = new BehaviorType[]{BehaviorType.Idle, BehaviorType.Listening, BehaviorType.Thinking, BehaviorType.Speaking, BehaviorType.DidntUnderstand};
				}
				return m_behaviorTypeList;
			}
		}

		public void ChangeBehaviour(BehaviorType behaviourType){
			m_currentBehavior = behaviourType;
			EventManager.Instance.SendEvent (EventManager.onBehaviorChangeFinish);
		}

		public void OnChangeBehavior(System.Object[] args){
			if(args.Length == 1){
				ChangeBehaviour( (BehaviorType) args[0]);
			}
		}
	}
}
