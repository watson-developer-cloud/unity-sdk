using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using IBM.Watson.Utilities;
using IBM.Watson.Widgets;
using IBM.Watson.Logging;

namespace IBM.Watson.Debug
{
    /// <summary>
    /// Debug mode for Avatar Mood. Mood can be changed via keyboard on Debug mode.
    /// </summary>
    [RequireComponent(typeof(Text))]
    public class AvatarMood : MonoBehaviour
    {

        #region Private Variable

        private AvatarWidget.MoodType[] m_MoodTypeList = null;
        private int m_CurrentMoodIndex = 0;
        private Text m_Text;
        private AvatarWidget m_AvatarWidget;

        #endregion

        #region All private functions. OnEnable / OnDisable / Awake / Start / Update / Update Avatar Mood Settings

        void OnEnable()
        {
            EventManager.Instance.RegisterEventReceiver(Constants.Event.ON_CHANGE_AVATAR_MOOD, ChangeMood);
            EventManager.Instance.RegisterEventReceiver(Constants.Event.ON_CHANGE_AVATAR_MOOD_FINISH, UpdateMood);
        }

        void OnDisable()
        {
            EventManager.Instance.UnregisterEventReceiver(Constants.Event.ON_CHANGE_AVATAR_MOOD, ChangeMood);
            EventManager.Instance.UnregisterEventReceiver(Constants.Event.ON_CHANGE_AVATAR_MOOD_FINISH, UpdateMood);
        }


        // Use this for initialization
        void Awake()
        {
            m_Text = GetComponent<Text>();
            m_AvatarWidget = GameObject.FindObjectOfType<AvatarWidget>();
            if(m_AvatarWidget != null)
            {
                m_CurrentMoodIndex = (int)m_AvatarWidget.Mood;
                m_MoodTypeList = m_AvatarWidget.MoodTypeList;
            }
            else
            {
                Log.Error("AvatarMood", "AvatarWidget couldn't find in the scene.");
                this.enabled = false;
            }
        }

        void Start()
        {
            EventManager.Instance.SendEvent(Constants.Event.ON_CHANGE_AVATAR_MOOD_FINISH, m_CurrentMoodIndex);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(Constants.KeyCodes.CHANGE_MOOD))
            {
                if (m_AvatarWidget != null)
                    EventManager.Instance.SendEvent(Constants.Event.ON_CHANGE_AVATAR_MOOD, m_AvatarWidget, (m_CurrentMoodIndex + 1) % m_MoodTypeList.Length);
                else
                    EventManager.Instance.SendEvent(Constants.Event.ON_CHANGE_AVATAR_MOOD, (m_CurrentMoodIndex + 1) % m_MoodTypeList.Length);
            }
        }

        void UpdateMood(System.Object[] args)
        {
            if (m_AvatarWidget != null)
                m_Text.text = string.Format(Constants.String.DEBUG_DISPLAY_AVATAR_MOOD, m_AvatarWidget.State, m_AvatarWidget.Mood);
            else
                m_Text.text = string.Format(Constants.String.DEBUG_DISPLAY_AVATAR_MOOD, m_MoodTypeList[m_CurrentMoodIndex].ToString());

        }

        void ChangeMood(System.Object[] args)
        {
            if (args.Length == 0)
            {
                m_CurrentMoodIndex = (m_CurrentMoodIndex + 1) % m_MoodTypeList.Length;
                EventManager.Instance.SendEvent(Constants.Event.ON_CHANGE_AVATAR_MOOD_FINISH, m_CurrentMoodIndex);
            }
            else if (args.Length == 1)
            {
                int.TryParse(args[0].ToString(), out m_CurrentMoodIndex);
                EventManager.Instance.SendEvent(Constants.Event.ON_CHANGE_AVATAR_MOOD_FINISH, m_CurrentMoodIndex);
            }
            else if (args.Length == 2)
            {
                AvatarWidget avatarWidget = args[0] as AvatarWidget;
                if (avatarWidget != null)
                {
                    int.TryParse(args[1].ToString(), out m_CurrentMoodIndex);
                    EventManager.Instance.SendEvent(Constants.Event.ON_CHANGE_AVATAR_MOOD_FINISH, avatarWidget, m_CurrentMoodIndex);
                }
                else
                {
                    Log.Error("AvatarMood", "Change Mood has invalid object type in arguments");
                }
            }
            else
            {
                Log.Error("AvatarMood", "Change Mood has undefined number of arguments {0}", args.Length);
            }

        }

        #endregion
    }
}
