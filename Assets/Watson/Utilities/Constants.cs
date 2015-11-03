using UnityEngine;
using System.Collections;

namespace IBM.Watson.Utilities
{
    public static class Constants
    {
        /// <summary>
        /// All Keycodes used via keyboard listed here.
        /// </summary>
        public static class KeyCodes
        {
			//Debug Mode Key Pressed to use the keys given below
			public const KeyModifiers MODIFIER_KEY = KeyModifiers.CONTROL;

            //Debug Mode Keyboard keycodes
            public const KeyCode CHANGE_QUALITY = KeyCode.Q;
            public const KeyCode CHANGE_MOOD = KeyCode.M;

            //Cube actions keycodes
            public const KeyCode CUBE_TO_FOLD = KeyCode.F;
            public const KeyCode CUBE_TO_UNFOLD = KeyCode.U;
            public const KeyCode CUBE_TO_FOCUS = KeyCode.Z;
            public const KeyCode CUBE_TO_UNFOCUS = KeyCode.X;
			public const KeyCode CUBE_TO_ROTATE_OR_PAUSE = KeyCode.R;

			//Application - General
			public const KeyCode APPLICATION_PAUSE = KeyCode.Space;

			public const KeyCode QUESTION_WAKEUP = KeyCode.W;
        }

        /// <summary>
        /// All Event related constants listed here. Exp. Event names to call.
        /// </summary>
        public static class Event
        {
            /// <summary>
            /// This is event is sent when the user enters a command in the debug console.
            /// </summary>
            public const string ON_DEBUG_COMMAND = "ON_DEBUG_COMMAND";
            public const string ON_DEBUG_MESSAGE = "ON_DEBUG_MESSAGE";

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

			//Animation Related ones
			public const string ON_ANIMATION_STOP = "ON_ANIMATION_STOP";
			public const string ON_ANIMATION_PAUSE = "ON_ANIMATION_PAUSE";
			public const string ON_ANIMATION_RESUME = "ON_ANIMATION_RESUME";
			public const string ON_ANIMATION_SPEED_UP = "ON_ANIMATION_SPEED_UP";
			public const string ON_ANIMATION_SPEED_DOWN = "ON_ANIMATION_SPEED_DOWN";
			public const string ON_ANIMATION_SPEED_DEFAULT = "ON_ANIMATION_SPEED_DEFAULT";
         
        }

        /// <summary>
        /// All constant path variables liste here. Exp. Configuration file
        /// </summary>
        public static class Path
        {
            public const string CONFIG_FILE = "/Config.json";
        }

        /// <summary>
        /// All resources (files names under resource directory) used in the SDK listed here. Exp. Watson Logo
        /// </summary>
        public static class Resources
        {
            public const string WATSON_ICON = "WatsonSpriteIcon_32x32";
            public const string WATSON_LOGO = "WatsonSpriteLogo_506x506";

        }

        /// <summary>
        /// All string variables or string formats used in the SDK listed here. Exp. Quality Debug Format = Quality {0}
        /// </summary>
        public static class String
        {
            public const string DEBUG_DISPLAY_QUALITY = "Quality: {0}";
            public const string DEBUG_DISPLAY_AVATAR_MOOD = "Behavior:{0} Mood: {1}";
        }


    }
}


