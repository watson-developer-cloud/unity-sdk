using UnityEngine;
using UnityEngine.UI;
using IBM.Watson.Utilities;

namespace IBM.Watson.Debug
{
    /// <summary>
    /// Debug mode for different Quality Settings.
    /// </summary>
    [RequireComponent(typeof(Text))]
    public class Quality : MonoBehaviour
    {

        #region Private Variables
        private string[] m_qualitySettingsNames = null;
        private int m_currentQualityIndex = 0;
        private Text m_Text;
        #endregion

        #region All private functions. OnEnable / OnDisable / Awake / Start / Update / Update Quality Settings

        void OnEnable()
        {
            EventManager.Instance.RegisterEventReceiver(Constants.Event.ON_CHANGE_QUALITY, ChangeQualitySettings);
            EventManager.Instance.RegisterEventReceiver(Constants.Event.ON_CHANGE_QUALITY_FINISH, UpdateQualitySettings);
        }

        void OnDisable()
        {
            EventManager.Instance.UnregisterEventReceiver(Constants.Event.ON_CHANGE_QUALITY, ChangeQualitySettings);
            EventManager.Instance.UnregisterEventReceiver(Constants.Event.ON_CHANGE_QUALITY_FINISH, UpdateQualitySettings);
        }
        
        void Awake()
        {
            m_currentQualityIndex = QualitySettings.GetQualityLevel();
            m_qualitySettingsNames = QualitySettings.names;
            m_Text = GetComponent<Text>();
        }

        void Start()
        {
            EventManager.Instance.SendEvent(Constants.Event.ON_CHANGE_QUALITY_FINISH);
        }
        
        void Update()
        {
            if (Input.GetKeyDown(Constants.KeyCodes.CHANGE_QUALITY))
            {
                EventManager.Instance.SendEvent(Constants.Event.ON_CHANGE_QUALITY);
            }
        }

        void ChangeQualitySettings(System.Object[] args)
        {
            m_currentQualityIndex = (m_currentQualityIndex + 1) % m_qualitySettingsNames.Length;
            QualitySettings.SetQualityLevel(m_currentQualityIndex, true);
            EventManager.Instance.SendEvent(Constants.Event.ON_CHANGE_QUALITY_FINISH);
        }

        void UpdateQualitySettings(System.Object[] args)
        {
            m_Text.text = string.Format(Constants.String.DEBUG_DISPLAY_QUALITY, m_qualitySettingsNames[m_currentQualityIndex]);
        }

        #endregion
    }
}
