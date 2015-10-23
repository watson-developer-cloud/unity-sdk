using UnityEngine;
using System.Collections;
using IBM.Watson.Logging;
using IBM.Watson.Utilities;

namespace IBM.Watson.AdaptiveComputing
{
	public enum MoodType
	{
		Idle = 0,	//Bored
		Interested,
		Urgent,
		Upset,
		Shy
	}

	public class MoodManager : MonoBehaviour{

        public Material[] materialToChangeColor;
        private LTDescr[] m_animationsForMaterial;

        void Awake()
        {
            _instance = null;
        }
  
		void OnEnable(){
			EventManager.Instance.RegisterEventReceiver (EventManager.onMoodChange, OnChangeMood);
		}
		void OnDisable(){
			EventManager.Instance.UnregisterEventReceiver (EventManager.onMoodChange, OnChangeMood);
		}
		void OnApplicationQuit() {
			DestroyImmediate (gameObject);
		}

        private static MoodManager _instance;
		public static MoodManager Instance { get { if (_instance == null) _instance = Singleton<MoodManager>.Instance; return _instance; } }

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
					m_currentMoodColor =  new Color(221 / 255.0f, 115 / 255.0f, 28 / 255.0f);	break;
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

		
		public float currentMoodSpeedModifier{
			get{
				float value = 1.0f;
				switch (currentMood) {
				case MoodType.Idle:
					value = 1.0f;	break;
				case MoodType.Interested:
					value = 1.1f;	break;
				case MoodType.Urgent:		//Double the speed
					value = 2.0f;	break;
				case MoodType.Upset:
					value = 1.5f;	break;
				case MoodType.Shy:
					value = 0.9f;	break;
				default:
					value = 1.0f;	break;
				}
				return value;

			}
		}

		public float currentMoodTimeModifier{
			get{
				float value = currentMoodSpeedModifier;
				if(value != 0.0f)
					value = 1.0f / value;
				else 
					value = 0.00001f;

				return value;
			}
		}


		MoodType[] m_moodTypeList = null;
		public MoodType[] moodTypeList{
			get{
				if(m_moodTypeList == null){
					m_moodTypeList = new MoodType[]{MoodType.Idle, MoodType.Interested, MoodType.Urgent, MoodType.Upset, MoodType.Shy};
				}
				return m_moodTypeList;
			}
		}

		public void ChangeMood(MoodType moodType){
			m_currentMood = moodType;
			EventManager.Instance.SendEvent (EventManager.onMoodChangeFinish);

            if(materialToChangeColor != null)
            {
                if(m_animationsForMaterial == null)
                {
                    m_animationsForMaterial = new LTDescr[materialToChangeColor.Length];
                }
                else
                {
                    for (int i = 0; i < m_animationsForMaterial.Length; i++)
                    {
                        if (m_animationsForMaterial[i] != null)
                            LeanTween.cancel(m_animationsForMaterial[i].uniqueId);
                    }
                }

                for (int i = 0; i < materialToChangeColor.Length; i++)
                {
                    m_animationsForMaterial[i] = LeanTween.value(gameObject, Color.white, currentMoodColor, currentMoodTimeModifier).setLoopPingPong().setOnUpdateColor((Color a) =>
                    {

                    });
                }
            }
		}

		public void OnChangeMood(System.Object[] args){
			if(args.Length == 1){
				ChangeMood( (MoodType) args[0]);
			}
		}
	}
}
