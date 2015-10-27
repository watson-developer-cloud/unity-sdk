using UnityEngine;
using System.Collections;

namespace IBM.Watson.Utilities
{
    public class Constants
    {
        /// <summary>
        /// All Keycodes used via keyboard listed here.
        /// </summary>
        public class KeyCodes
        {
            //Debug Mode Keyboard keycodes
            public const KeyCode CHANGE_QUALITY = KeyCode.Q;
            public const KeyCode CHANGE_MOOD = KeyCode.M;

            //Cube actions keycodes
            public const KeyCode CUBE_TO_FOLD = KeyCode.F;
            public const KeyCode CUBE_TO_UNFOLD = KeyCode.U;
            public const KeyCode CUBE_TO_FOCUS = KeyCode.Z;
            public const KeyCode CUBE_TO_UNFOCUS = KeyCode.X;
        }

        /// <summary>
        /// All Event related constants listed here. Exp. Event names to call.
        /// </summary>
        public class Event
        {
            //Avatar Mood Changes
            public const string ON_CHANGE_AVATAR_MOOD = "ON_CHANGE_AVATAR_MOOD";
            public const string ON_CHANGE_AVATAR_MOOD_FINISH = "ON_CHANGE_AVATAR_MOOD_FINISH";

            //Avatar Mood Changes
            public const string ON_CHANGE_AVATAR_STATE = "ON_CHANGE_AVATAR_STATE";
            public const string ON_CHANGE_AVATAR_STATE_FINISH = "ON_CHANGE_AVATAR_STATE_FINISH";

            //Quality Settings
            public const string ON_CHANGE_QUALITY = "ON_CHANGE_QUALITY";
            public const string ON_CHANGE_QUALITY_FINISH = "ON_CHANGE_QUALITY_FINISH";

            //Question Widget State Change
            public const string ON_CHANGE_STATE_QUESTIONCUBE_ANIMATION = "ON_CHANGE_STATE_QUESTIONCUBE_ANIMATION";
         
        }

        /// <summary>
        /// All constant path variables liste here. Exp. Configuration file
        /// </summary>
        public class Path
        {

        }

        /// <summary>
        /// All resources (files names under resource directory) used in the SDK listed here. Exp. Watson Logo
        /// </summary>
        public class Resources
        {

        }

        /// <summary>
        /// All string variables or string formats used in the SDK listed here. Exp. Quality Debug Format = Quality {0}
        /// </summary>
        public class String
        {
            public const string DEBUG_DISPLAY_QUALITY = "Quality: {0}";
            public const string DEBUG_DISPLAY_AVATAR_MOOD = "State:{0} \nMood: {1}";
        }


    }
}


