using UnityEngine;
using System.Collections;
using IBM.Watson.Logging;
using IBM.Watson.Utilities;

namespace IBM.Watson.Widgets.Avatar
{
    public class AvatarModelManager : MonoBehaviour
    {
        #region Private Variables
        protected AvatarWidget m_AvatarWidgetAttached;
        #endregion

        #region OnEnable / OnDisable / OnApplicationQuit / Awake

        public virtual void OnEnable()
        {
            EventManager.Instance.RegisterEventReceiver(Constants.Event.ON_CHANGE_AVATAR_MOOD_FINISH, OnChangedMood);
            EventManager.Instance.RegisterEventReceiver(Constants.Event.ON_CHANGE_AVATAR_STATE_FINISH, OnChangedBehavior);
        }

        public virtual void OnDisable()
        {
            EventManager.Instance.UnregisterEventReceiver(Constants.Event.ON_CHANGE_AVATAR_MOOD_FINISH, OnChangedMood);
            EventManager.Instance.UnregisterEventReceiver(Constants.Event.ON_CHANGE_AVATAR_STATE_FINISH, OnChangedBehavior);
        }

        public virtual void Awake()
        {
            m_AvatarWidgetAttached = this.transform.GetComponentInParent<AvatarWidget>();
            if (m_AvatarWidgetAttached == null)
            {
                Log.Error("AvatarModelManager", "There is no Avatar Widget on any parent.");
                this.enabled = false;
            }
        }

        #endregion

        #region Changing Mood / Avatar State

        public virtual void OnChangedMood(System.Object[] args)
        {
            if (args.Length == 0 || args.Length == 1)
            {
                ChangeToColor(m_AvatarWidgetAttached.MoodColor, m_AvatarWidgetAttached.MoodSpeedModifier);
            }
            else if (args.Length == 2)
            {
                AvatarWidget avatarWidget = args[0] as AvatarWidget;
                if (avatarWidget != null)
                {
                    if (avatarWidget == m_AvatarWidgetAttached)
                    {
                        ChangeToColor(m_AvatarWidgetAttached.MoodColor, m_AvatarWidgetAttached.MoodSpeedModifier);
                    }
                    else
                    {
                        //do nothing because the Avatar object is not our attached avatar.
                    }
                }
                else
                {
                    Log.Error("AvatarModelManager", "OnChangeMood the argument is not AvatarWidget object");
                }
            }
            else
            {
                Log.Error("AvatarModelManager", "OnChangeMood the argument length is undefined {0}", args.Length);
            }
        }

        public virtual void OnChangedBehavior(System.Object[] args)
        {
            if (args.Length == 0 || args.Length == 1)
            {
                ChangeToColor(m_AvatarWidgetAttached.BehaviourColor, m_AvatarWidgetAttached.MoodSpeedModifier);
            }
            else if (args.Length == 2)
            {
                AvatarWidget avatarWidget = args[0] as AvatarWidget;
                if (avatarWidget != null)
                {
                    if (avatarWidget == m_AvatarWidgetAttached)
                    {
                        ChangeToColor(m_AvatarWidgetAttached.BehaviourColor, m_AvatarWidgetAttached.MoodSpeedModifier);
                    }
                    else
                    {
                        //do nothing because the Avatar object is not our attached avatar.
                    }
                }
                else
                {
                    Log.Error("AvatarModelManager", "OnChangeBehavior the argument is not AvatarWidget object");
                }
            }
            else
            {
                Log.Error("AvatarModelManager", "OnChangedBehavior the argument length is undefined {0}", args.Length);
            }
        }

        public virtual void ChangeToColor(Color colorToChange, float speedModifier) { }

        #endregion
    }

}