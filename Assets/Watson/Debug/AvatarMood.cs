using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using IBM.Watson.AdaptiveComputing;
using IBM.Watson.Utilities;
using IBM.Watson.Widgets;

namespace IBM.Watson.Debug
{
    /// <summary>
    /// Debug mode for Avatar Mood. Mood can be changed via keyboard on Debug mode.
    /// </summary>
    [RequireComponent(typeof(Text))]
    public class AvatarMood : MonoBehaviour
    {

        #region Private Variable

        private MoodType[] moodTypeList = null;
        private int m_currentMoodIndex = 0;
        private Text m_Text;

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
            //AvatarWidget avatarWidget = GameObject.FindObjectOfType<AvatarWidget>();
           
            //TODO: Avatar Widget with Mood Managment integration
            m_currentMoodIndex = (int)MoodManager.Instance.currentMood;
            moodTypeList = MoodManager.Instance.moodTypeList;
            m_Text = GetComponent<Text>();
        }

        void Start()
        {
            EventManager.Instance.SendEvent(Constants.Event.ON_CHANGE_AVATAR_MOOD_FINISH, m_currentMoodIndex);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(Constants.KeyCodes.CHANGE_MOOD))
            {
                EventManager.Instance.SendEvent(Constants.Event.ON_CHANGE_AVATAR_MOOD, (m_currentMoodIndex + 1) % moodTypeList.Length);
            }
        }

        void UpdateMood(System.Object[] args)
        {
            m_Text.text = string.Format(Constants.String.DEBUG_DISPLAY_AVATAR_MOOD, moodTypeList[m_currentMoodIndex].ToString());
        }

        void ChangeMood(System.Object[] args)
        {
            m_currentMoodIndex = (m_currentMoodIndex + 1) % moodTypeList.Length;
            EventManager.Instance.SendEvent(Constants.Event.ON_CHANGE_AVATAR_MOOD_FINISH, m_currentMoodIndex);
        }

        #endregion
    }
}
