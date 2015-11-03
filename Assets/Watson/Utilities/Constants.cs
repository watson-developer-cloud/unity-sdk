/**
* Copyright 2015 IBM Corp. All Rights Reserved.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
* @author Dogukan Erenel (derenel@us.ibm.com)
*/

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
			public const KeyModifiers MODIFIER_KEY = KeyModifiers.SHIFT;

            //Cube actions keycodes
            public const KeyCode CUBE_TO_FOLD = KeyCode.F;
            public const KeyCode CUBE_TO_UNFOLD = KeyCode.U;
            public const KeyCode CUBE_TO_FOCUS = KeyCode.Z;
            public const KeyCode CUBE_TO_UNFOCUS = KeyCode.X;
			public const KeyCode CUBE_TO_ROTATE_OR_PAUSE = KeyCode.R;

        }

        /// <summary>
        /// All constant event names used in SDK and Applications listed here. 
        /// </summary>
        public enum Event
        {
            /// <summary>
            /// Event to open debug console
            /// </summary>
            ON_DEBUG_COMMAND,
            /// <summary>
            /// Event to send debug message
            /// </summary>
            ON_DEBUG_MESSAGE,
            /// <summary>
            /// Event to change Avatar mood
            /// </summary>
            ON_CHANGE_AVATAR_MOOD,
            /// <summary>
            /// Event after Avatar mood change
            /// </summary>
            ON_CHANGE_AVATAR_MOOD_FINISH,
            /// <summary>
            /// Event to change Avatar state
            /// </summary>
            ON_CHANGE_AVATAR_STATE,
            /// <summary>
            /// Event after Avatar state change
            /// </summary>
            ON_CHANGE_AVATAR_STATE_FINISH,
            /// <summary>
            /// Event to change quality settings
            /// </summary>
            ON_CHANGE_QUALITY,
            /// <summary>
            /// Event after Quality Settings change
            /// </summary>
            ON_CHANGE_QUALITY_FINISH,
            /// <summary>
            /// Event on Question Cube state change
            /// </summary>
            ON_CHANGE_STATE_QUESTIONCUBE_ANIMATION,
            /// <summary>
            /// Event to Stop the all animations
            /// </summary>
            ON_ANIMATION_STOP,
            /// <summary>
            /// Event to pause the all animations playing
            /// </summary>
            ON_ANIMATION_PAUSE,
            /// <summary>
            /// Event to resume the paused animations
            /// </summary>
            ON_ANIMATION_RESUME,
            /// <summary>
            /// Event to speed-up the animations
            /// </summary>
            ON_ANIMATION_SPEED_UP,
            /// <summary>
            /// Event to speed-down the animations
            /// </summary>
            ON_ANIMATION_SPEED_DOWN,
            /// <summary>
            /// Event to set the default speed on animations
            /// </summary>
            ON_ANIMATION_SPEED_DEFAULT
        }

        /// <summary>
        /// All constant path variables liste here. Exp. Configuration file
        /// </summary>
        public static class Path
        {

        }

        /// <summary>
        /// All resources (files names under resource directory) used in the SDK listed here. Exp. Watson Logo
        /// </summary>
        public static class Resources
        {

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


