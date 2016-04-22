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
*/

using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

namespace IBM.Watson.DeveloperCloud.Utilities
{
    /// <summary>
    /// This class wraps all constants.
    /// </summary>
    public static class Constants
    {

        /// <summary>
        /// All constant event names used in SDK and Applications listed here. 
        /// </summary>
        public enum Event
        {
//            /// <summary>
//            /// Invalid event type.
//            /// </summary>
//            NONE = -1,
//
//            #region Debug
//            /// <summary>
//            /// Send a debug command.
//            /// </summary>
//            ON_DEBUG_COMMAND = 0,
//            /// <summary>
//            /// TOggle the debug console on or off.
//            /// </summary>
//            ON_DEBUG_TOGGLE,
//            /// <summary>
//            /// BEgin editing a command in the debug console.
//            /// </summary>
//            ON_DEBUG_BEGIN_COMMAND,
//            /// <summary>
//            /// Event to send debug message
//            /// </summary>
//            ON_DEBUG_MESSAGE,
//            /// <summary>
//            /// Event to change quality settings
//            /// </summary>
//            ON_CHANGE_QUALITY,
//            /// <summary>
//            /// Event after Quality Settings change
//            /// </summary>
//            ON_CHANGE_QUALITY_FINISH,
//            /// <summary>
//            /// Event to close the application
//            /// </summary>
//            ON_APPLICATION_QUIT,
//			/// <summary>
//			/// Event to make application idle
//			/// </summary>
//			ON_APPLICATION_TO_BECOME_IDLE,
//			/// <summary>
//			/// Event after appliation became idle
//			/// </summary>
//			ON_APPLICATION_BECAME_IDLE,
//            /// <summary>
//            /// AFter toggle the debug console on or off.
//            /// </summary>
//            ON_DEBUG_TOGGLE_FINISH,
//            /// <summary>
//            /// This is sent to provide additional info about a error that is ocurring.
//            /// </summary>
//            ON_ERROR_INFO,
//            #endregion
//
//            #region Animation / Camera
//            /// <summary>
//            /// Event to Stop the all animations
//            /// </summary>
//            ON_ANIMATION_STOP = 100,
//            /// <summary>
//            /// Event to pause the all animations playing
//            /// </summary>
//            ON_ANIMATION_PAUSE,
//            /// <summary>
//            /// Event to resume the paused animations
//            /// </summary>
//            ON_ANIMATION_RESUME,
//            /// <summary>
//            /// Event to speed-up the animations
//            /// </summary>
//            ON_ANIMATION_SPEED_UP,
//            /// <summary>
//            /// Event to speed-down the animations
//            /// </summary>
//            ON_ANIMATION_SPEED_DOWN,
//            /// <summary>
//            /// Event to set the default speed on animations
//            /// </summary>
//            ON_ANIMATION_SPEED_DEFAULT,
//            /// <summary>
//            /// Event to drag camera with two finger to zoom and pan
//            /// </summary>
//            ON_CAMERA_DRAG_TWO_FINGER,
//            /// <summary>
//            /// Event to zoom-in camera
//            /// </summary>
//            ON_CAMERA_ZOOM_IN,
//            /// <summary>
//            /// Event to zoom-out camera
//            /// </summary>
//            ON_CAMERA_ZOOM_OUT,
//            /// <summary>
//            /// Event to move the camera left
//            /// </summary>
//            ON_CAMERA_MOVE_LEFT,
//            /// <summary>
//            /// Event to move the camera right
//            /// </summary>
//            ON_CAMERA_MOVE_RIGHT,
//            /// <summary>
//            /// Event to move the camera up
//            /// </summary>
//            ON_CAMERA_MOVE_UP,
//            /// <summary>
//            /// Event to move the camera down
//            /// </summary>
//            ON_CAMERA_MOVE_DOWN,
//            /// <summary>
//            /// Event to set Antialiasing on / off
//            /// </summary>
//            ON_CAMERA_SET_ANTIALIASING,
//            /// <summary>
//            /// Event to set depth of field on / off
//            /// </summary>
//            ON_CAMERA_SET_DEPTHOFFIELD,
//            /// <summary>
//            /// Event to set interactivity enable / disable on camera
//            /// </summary>
//            ON_CAMERA_SET_INTERACTIVITY,
//            /// <summary>
//            /// Event to set default position of camera
//            /// </summary>
//            ON_CAMERA_RESET,
//            #endregion
//
//			#region NLC
//			/// <summary>
//			/// 
//			/// </summary>
//			ON_CLASSIFY_FAILURE = 200,
//			/// <summary>
//			/// Sent when any classify result is made.
//			/// </summary>
//			ON_CLASSIFY_RESULT,
//			#endregion
//
//			#region Input - Touch
//			/// <summary>
//			/// Event if there is touch on fullscreen pressed - It is called for each touch
//			/// </summary>
//			ON_TOUCH_PRESSED_FULLSCREEN = 300,
//            /// <summary>
//            /// Event if 
//            /// </summary>
//            ON_TOUCH_RELEASED_FULLSCREEN,
//            /// <summary>
//            /// Five finger tap event
//            /// </summary>
//            ON_TOUCH_FIVE_FINGER,
//            /// <summary>
//            /// Event while user is speaking
//            /// </summary>
//            ON_USER_SPEAKING,
//			/// <summary>
//			/// Event fired after any keyboard down
//			/// </summary>
//			ON_KEYBOARD_ANYKEY_DOWN,
//			/// <summary>
//			/// Event fired after user is tapping three times - used for some hot keys like open / close debug info etc.
//			/// </summary>
//			ON_TAP_THREETIMES,
//			/// <summary>
//			/// Event fired after three tap on bottom left - hot corner
//			/// </summary>
//			ON_TAP_THREETIMES_BOTTOM_LEFT,
//			/// <summary>
//			/// Event fired after three tap on bottom right - hot corner
//			/// </summary>
//			ON_TAP_THREETIMES_BOTTOM_RIGHT,
//			/// <summary>
//			/// Event fired after three tap on top left - hot corner
//			/// </summary>
//			ON_TAP_THREETIMES_TOP_LEFT,
//			/// <summary>
//			/// Event fired after three tap on top right - hot corner
//			/// </summary>
//			ON_TAP_THREETIMES_TOP_RIGHT,
//			/// <summary>
//			/// Event fired to open virtual keyboard
//			/// </summary>
//			ON_VIRTUAL_KEYBOARD_TOGGLE,
//			/// <summary>
//			/// Event fired after three tap on middle left - hot corner
//			/// </summary>
//			ON_TAP_THREETIMES_MIDDLE_LEFT,
//			/// <summary>
//			/// Event fired after three tap on middle right - hot corner
//			/// </summary>
//			ON_TAP_THREETIMES_MIDDLE_RIGHT,
//			/// <summary>
//			/// Event fired after three tap on middle bottom - hot corner
//			/// </summary>
//			ON_TAP_THREETIMES_MIDDLE_BOTTOM,
//			/// <summary>
//			/// Event fired after three tap on middle top - hot corner
//			/// </summary>
//			ON_TAP_THREETIMES_MIDDLE_TOP,
//            /// <summary>
//            /// Event fired when a single tap occur
//            /// </summary>
//            ON_TAP_ONE,
//            /// /// <summary>
//            /// Event fired when a double tap occur
//            /// </summary>
//            ON_TAP_DOUBLE,
//            /// <summary>
//            /// Event fired when one finger fullscreen drag occur
//            /// </summary>
//            ON_DRAG_ONE_FINGER_FULLSCREEN,
//            /// <summary>
//            /// Event fired when two finger fullscreen drag occur
//            /// </summary>
//            ON_DRAG_TWO_FINGER_FULLSCREEN,
//            /// <summary>
//            /// Event fired when there is long press with one finger
//            /// </summary>
//            ON_LONG_PRESS_ONE_FINGER,
//            /// <summary>
//            /// Event fired when user hit Tab key from keyboard
//            /// </summary>
//            ON_KEYBOARD_TAB,
//            /// <summary>
//            /// Event fired when user hit Return key from keyboard
//            /// </summary>
//            ON_KEYBOARD_RETURN,
//            /// <summary>
//            /// Event fired when user hit Escape key from keyboard
//            /// </summary>
//            ON_KEYBOARD_ESCAPE,
//            /// <summary>
//            /// Event fired when user hit back quote key from keyboard (for open console)
//            /// </summary>
//            ON_KEYBOARD_BACKQUOTE,
//            #endregion
//
//            #region User Actions
//            /// <summary>
//            /// Event fired to user logout
//            /// </summary>
//            USER_TO_LOGOUT = 400
//            #endregion
        }

        /// <summary>
        /// All constant path variables liste here. Exp. Configuration file
        /// </summary>
        public static class Path
        {
            /// <summary>
            /// Configuration file name.
            /// </summary>
            public const string CONFIG_FILE = "/Config.json";
        }

        /// <summary>
        /// All resources (files names under resource directory) used in the SDK listed here. Exp. Watson Logo
        /// </summary>
        public static class Resources
        {
            /// <summary>
            /// Watson icon.
            /// </summary>
            public const string WATSON_ICON = "WatsonSpriteIcon_32x32";
            /// <summary>
            /// Watson logo.
            /// </summary>
            public const string WATSON_LOGO = "WatsonSpriteLogo_506x506";
        }

        /// <summary>
        /// All string variables or string formats used in the SDK listed here. Exp. Quality Debug Format = Quality {0}
        /// </summary>
        public static class String
        {
            /// <exclude />
            public const string DEBUG_DISPLAY_QUALITY = "Quality: {0}";
		}
    }
}


