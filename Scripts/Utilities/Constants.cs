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
            /// <summary>
            /// Invalid event type.
            /// </summary>
            NONE = -1,

            #region Debug
            /// <summary>
            /// Send a debug command.
            /// </summary>
            ON_DEBUG_COMMAND = 0,
            /// <summary>
            /// TOggle the debug console on or off.
            /// </summary>
            ON_DEBUG_TOGGLE,
            /// <summary>
            /// BEgin editing a command in the debug console.
            /// </summary>
            ON_DEBUG_BEGIN_COMMAND,
            /// <summary>
            /// Event to send debug message
            /// </summary>
            ON_DEBUG_MESSAGE,
            /// <summary>
            /// Event to change quality settings
            /// </summary>
            ON_CHANGE_QUALITY,
            /// <summary>
            /// Event after Quality Settings change
            /// </summary>
            ON_CHANGE_QUALITY_FINISH,
            /// <summary>
            /// Event to close the application
            /// </summary>
            ON_APPLICATION_QUIT,
			/// <summary>
			/// Event to make application idle
			/// </summary>
			ON_APPLICATION_TO_BECOME_IDLE,
			/// <summary>
			/// Event after appliation became idle
			/// </summary>
			ON_APPLICATION_BECAME_IDLE,
            /// <summary>
            /// AFter toggle the debug console on or off.
            /// </summary>
            ON_DEBUG_TOGGLE_FINISH,
            /// <summary>
            /// This is sent to provide additional info about a error that is ocurring.
            /// </summary>
            ON_ERROR_INFO,
            #endregion

            #region Avatar 
            /// <summary>
            /// Event to change Avatar mood
            /// </summary>
            ON_CHANGE_AVATAR_MOOD = 100,
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
			/// Event to avatar move down
			/// </summary>
			ON_AVATAR_MOVE_DOWN,
			/// <summary>
			/// The event to move the avatar initial position
			/// </summary>
			ON_AVATAR_MOVE_DEFAULT,
			/// <summary>
			/// Event to stop avatar move up / down
			/// </summary>
			ON_AVATAR_STOP_MOVE,
			/// <summary>
			/// The event to stop avatar rotation
			/// </summary>
			ON_AVATAR_STOP_ROTATIOM,
            /// <summary>
            /// Event when avatar is speaking. 
            /// </summary>
            ON_AVATAR_SPEAKING,
            /// <summary>
            /// Event toggle of top light avatar
            /// </summary>
            ON_AVATAR_TOP_LIGHT_TOGGLE,
            /// <summary>
            /// Event to wake up avatar
            /// </summary>
            ON_AVATAR_TO_WAKE_UP,
            /// <summary>
            /// Tap on avatar on question flow mode
            /// </summary>
            ON_AVATAR_TAP_SINGLE,
            /// <summary>
            /// Event fired when there is long press with one finger on Avatar object
            /// </summary>
            ON_AVATAR_LONG_PRESS_ONE_FINGER,
            /// <summary>
            /// Drag on avatar on question flow mode
            /// </summary>
            ON_AVATAR_DRAG_ONE_FINGER,
            /// <summary>
            /// Event to move Avatar move down away
            /// </summary>
            ON_AVATAR_MOVE_DOWN_AWAY,
            /// <summary>
            /// Event fired after avatar close caption
            /// </summary>
            CLOSECAPTION_AVATAR_MESSAGE,
            #endregion

            #region Question
            /// <summary>
            /// Event on Question Cube state change
            /// </summary>
            ON_CHANGE_STATE_QUESTIONCUBE_ANIMATION = 200,
			/// <summary>
			/// Event on Question tap inside question cube
			/// </summary>
			ON_QUESTIONCUBE_TAP_INSIDE,
			/// <summary>
			/// Event on Question tap outside question cube
			/// </summary>
			ON_QUESTIONCUBE_TAP_OUTSIDE,
			/// <summary>
			/// Event on Question cube drag via fullscreen
			/// </summary>
			ON_QUESTIONCUBE_DRAG_ONE_FINGER_FULLSCREEN,
			/// <summary>
			/// Event on Question cube drag via object dragging
			/// </summary>
			ON_QUESTIONCUBE_DRAG_ONE_FINGER_OBJECT,
            /// <summary>
            /// Event sent when a question is asked from a given location.
            /// </summary>
            ON_QUESTION_LOCATION,
            /// <summary>
            /// Event sent when a question is being answered, this is sent after the QuestionCube has been instanced
            /// </summary>
            ON_QUESTION,
            /// <summary>
            /// Event sent containing the answer results.
            /// </summary>
            ON_QUESTION_ANSWERS,
            /// <summary>
            /// Event sent containing the parse results.
            /// </summary>
            ON_QUESTION_PARSE,
            /// <summary>
            /// Event sent with the pipeline after question has been asked.
            /// </summary>
            ON_QUESTION_PIPELINE,
            /// <summary>
            /// Event sent to cancel the current question.
            /// </summary>
            ON_QUESTION_CANCEL,
			/// <summary>
			/// Event sent containing personal information.
			/// </summary>
			ON_QUESTION_PERSONAL_INFO,
            /// <summary>
            /// Event fired when question flow to start
            /// </summary>
            QUESTION_FLOW_TO_RESTART,
            /// <summary>
            /// Event fired when question flow to stop
            /// </summary>
            QUESTION_FLOW_TO_STOP,
            /// <summary>
            /// Event to immerse features
            /// </summary>
            ON_QUESTION_IMMERSE_FEATURES,
            /// <summary>
            /// Event fired when location with ship tracking information
            /// </summary>
            ON_QUESTION_LOCATION_SHIPTRACKING,
			/// <summary>
			/// Event fired when location with procurement information
			/// </summary>
			ON_QUESTION_PROCUREMENT,
            #endregion

            #region Animation / Camera
            /// <summary>
            /// Event to Stop the all animations
            /// </summary>
            ON_ANIMATION_STOP = 300,
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
            ON_ANIMATION_SPEED_DEFAULT,
            /// <summary>
            /// Event to drag camera with two finger to zoom and pan
            /// </summary>
            ON_CAMERA_DRAG_TWO_FINGER,
            /// <summary>
            /// Event to zoom-in camera
            /// </summary>
            ON_CAMERA_ZOOM_IN,
            /// <summary>
            /// Event to zoom-out camera
            /// </summary>
            ON_CAMERA_ZOOM_OUT,
            /// <summary>
            /// Event to move the camera left
            /// </summary>
            ON_CAMERA_MOVE_LEFT,
            /// <summary>
            /// Event to move the camera right
            /// </summary>
            ON_CAMERA_MOVE_RIGHT,
            /// <summary>
            /// Event to move the camera up
            /// </summary>
            ON_CAMERA_MOVE_UP,
            /// <summary>
            /// Event to move the camera down
            /// </summary>
            ON_CAMERA_MOVE_DOWN,
            /// <summary>
            /// Event to set Antialiasing on / off
            /// </summary>
            ON_CAMERA_SET_ANTIALIASING,
            /// <summary>
            /// Event to set depth of field on / off
            /// </summary>
            ON_CAMERA_SET_DEPTHOFFIELD,
            /// <summary>
            /// Event to set interactivity enable / disable on camera
            /// </summary>
            ON_CAMERA_SET_INTERACTIVITY,
            #endregion

            #region NLC

            /// <summary>
            /// 
            /// </summary>
            ON_CLASSIFY_FAILURE = 400,
            /// <summary>
            /// 
            /// </summary>
            ON_CLASSIFY_QUESTION,
            /// <summary>
            /// 
            /// </summary>
            ON_CLASSIFY_DIALOG,
            /// <summary>
            /// 
            /// </summary>
            ON_COMMAND_WAKEUP,
            /// <summary>
            /// 
            /// </summary>
            ON_COMMAND_SLEEP,
            /// <summary>
            /// 
            /// </summary>
            ON_COMMAND_DEBUGON,
            /// <summary>
            /// 
            /// </summary>
            ON_COMMAND_DEBUGOFF,
            /// <summary>
            /// 
            /// </summary>
            ON_COMMAND_FOLD,
            /// <summary>
            /// 
            /// </summary>
            ON_COMMAND_UNFOLD,
            /// <summary>
            /// 
            /// </summary>
            ON_COMMAND_ANSWERS,
            /// <summary>
            /// 
            /// </summary>
            ON_COMMAND_EVIDENCE,
            /// <summary>
            /// 
            /// </summary>
            ON_COMMAND_CHAT,
            /// <summary>
            /// 
            /// </summary>
            ON_COMMAND_PARSE,
            /// <summary>
            /// 
            /// </summary>
            ON_COMMAND_LOCATION,
            /// <summary>
            /// Sent when any classify result is made.
            /// </summary>
            ON_CLASSIFY_RESULT,
            /// <summary>
            /// Show next item , it depends on context
            /// </summary>
            ON_COMMAND_NEXT,
            /// <summary>
            /// Show previous item - it depends on context
            /// </summary>
            ON_COMMAND_PREVIOUS,
            /// <summary>
            /// To scroll up
            /// </summary>
            ON_COMMAND_SCROLL_UP,
            /// <summary>
            /// To scroll down
            /// </summary>
            ON_COMMAND_SCROLL_DOWN,
            /// <summary>
            /// To repeat last action (ex. more zoom-in)
            /// </summary>
            ON_COMMAND_REPEAT_LAST_ACTION,
            /// <summary>
            /// Show first item, it depends on context
            /// </summary>
            ON_COMMAND_FIRST,
            /// <summary>
            /// Show last item, it depends on context
            /// </summary>
            ON_COMMAND_LAST,
            /// <summary>
            /// Cancel command
            /// </summary>
            ON_COMMAND_CANCEL,
            /// <summary>
            /// On classify the question as personnel information related
            /// </summary>
            ON_CLASSIFY_PERSONNEL,
            /// <summary>
            /// On Classify the question as ship tracking question
            /// </summary>
            ON_CLASSIFY_SHIP_TRACKING,
			/// <summary>
			/// On Classify the question as procurement question
			/// </summary>
			ON_CLASSIFY_PROCUREMENT,
            /// <summary>
            /// 
            /// </summary>
            ON_COMMAND_PROCUREMENT,
			/// <summary>
			/// 
			/// </summary>
			ON_COMMAND_PROCUREMENT_LIST,
			#endregion

			#region Input - Touch
			/// <summary>
			/// Event if there is touch on fullscreen pressed - It is called for each touch
			/// </summary>
			ON_TOUCH_PRESSED_FULLSCREEN = 500,
            /// <summary>
            /// Event if 
            /// </summary>
            ON_TOUCH_RELEASED_FULLSCREEN,
            /// <summary>
            /// Five finger tap event
            /// </summary>
            ON_TOUCH_FIVE_FINGER,
            /// <summary>
            /// Event while user is speaking
            /// </summary>
            ON_USER_SPEAKING,
			/// <summary>
			/// Event fired after any keyboard down
			/// </summary>
			ON_KEYBOARD_ANYKEY_DOWN,
			/// <summary>
			/// Event fired after user is tapping three times - used for some hot keys like open / close debug info etc.
			/// </summary>
			ON_TAP_THREETIMES,
			/// <summary>
			/// Event fired after three tap on bottom left - hot corner
			/// </summary>
			ON_TAP_THREETIMES_BOTTOM_LEFT,
			/// <summary>
			/// Event fired after three tap on bottom right - hot corner
			/// </summary>
			ON_TAP_THREETIMES_BOTTOM_RIGHT,
			/// <summary>
			/// Event fired after three tap on top left - hot corner
			/// </summary>
			ON_TAP_THREETIMES_TOP_LEFT,
			/// <summary>
			/// Event fired after three tap on top right - hot corner
			/// </summary>
			ON_TAP_THREETIMES_TOP_RIGHT,
			/// <summary>
			/// Event fired to open virtual keyboard
			/// </summary>
			ON_VIRTUAL_KEYBOARD_TOGGLE,
			/// <summary>
			/// Event fired after three tap on middle left - hot corner
			/// </summary>
			ON_TAP_THREETIMES_MIDDLE_LEFT,
			/// <summary>
			/// Event fired after three tap on middle right - hot corner
			/// </summary>
			ON_TAP_THREETIMES_MIDDLE_RIGHT,
			/// <summary>
			/// Event fired after three tap on middle bottom - hot corner
			/// </summary>
			ON_TAP_THREETIMES_MIDDLE_BOTTOM,
			/// <summary>
			/// Event fired after three tap on middle top - hot corner
			/// </summary>
			ON_TAP_THREETIMES_MIDDLE_TOP,
            /// <summary>
            /// Event fired when a single tap occur
            /// </summary>
            ON_TAP_ONE,
            /// /// <summary>
            /// Event fired when a double tap occur
            /// </summary>
            ON_TAP_DOUBLE,
            /// <summary>
            /// Event fired when one finger fullscreen drag occur
            /// </summary>
            ON_DRAG_ONE_FINGER_FULLSCREEN,
            /// <summary>
            /// Event fired when two finger fullscreen drag occur
            /// </summary>
            ON_DRAG_TWO_FINGER_FULLSCREEN,
            /// <summary>
            /// Event fired when there is long press with one finger
            /// </summary>
            ON_LONG_PRESS_ONE_FINGER,
            /// <summary>
            /// Event fired when user hit Tab key from keyboard
            /// </summary>
            ON_KEYBOARD_TAB,
            /// <summary>
            /// Event fired when user hit Return key from keyboard
            /// </summary>
            ON_KEYBOARD_RETURN,
            /// <summary>
            /// Event fired when user hit Escape key from keyboard
            /// </summary>
            ON_KEYBOARD_ESCAPE,
            /// <summary>
            /// Event fired when user hit back quote key from keyboard (for open console)
            /// </summary>
            ON_KEYBOARD_BACKQUOTE,
            #endregion

            #region Map Interactions
            /// <summary>
            /// Event fired when one finger dragging on Map
            /// </summary>
            ON_MAP_DRAG_ONE_FINGER = 600,
			/// <summary>
			/// Event fired when one finger Tap on Map
			/// </summary>
			ON_MAP_TAP_ONE,
			/// <summary>
			/// Event fired when Double tap on Map
			/// </summary>
			ON_MAP_TAP_DOUBLE,
            /// <summary>
            /// Event fired when two finger dragging on Map - to zoom
            /// </summary>
            ON_MAP_DRAG_TWO_FINGER,
            #endregion

            #region Update
            /// <summary>
            /// Event fire to confirm an update
            /// </summary>
            CONFIRM_UPDATE = 700,
            /// <summary>
            /// Event fire to dismiss the update 
            /// </summary>
            DISMISS_UPDATE,
            /// <summary>
            /// Event fire to check for update
            /// </summary>
            CHECK_FOR_UPDATES,
            /// <summary>
            /// Event fired after getting new version number from backend
            /// </summary>
            CHECK_FOR_UPDATES_FINISH,
            #endregion

			#region Feature Visualization
			/// <summary>
			/// Event fired when tapping question in feature visulaztion.
			/// </summary>
			FEATURE_VISUALIZATION_TAP_QUESTION = 800,
			/// <summary>
			/// Event fired when tapping answer in feature visulaztion.
			/// </summary>
			FEATURE_VISUALIZATION_TAP_ANSWER,
			/// <summary>
			/// Event fired when tapping feature in feature visulaztion
			/// </summary>
			FEATURE_VISUALIZATION_TAP_FEATURE,
			/// <summary>
			/// Event fired when tapping feature render texture
			/// </summary>
			FEATURE_VISUALIZATION_TAP_RENDERTEXTURE,
			/// <summary>
			/// Event fired when tapping feature header from the evidence facet
			/// </summary>
			FEATURE_VISUALIZATION_SHOW_FEATURE_VISUALIZATION,
			/// <summary>
			/// Event sent when feature visualization data is received.
			/// </summary>
			ON_QUESTION_FEATURE_VISUALIZATION,
			#endregion

            #region User Actions
            /// <summary>
            /// Event fired to user logout
            /// </summary>
            USER_TO_LOGOUT = 900,
            /// <summary>
            /// Event fired after user close caption
            /// </summary>
            CLOSECAPTION_USER_MESSAGE,
            /// <summary>
            /// Event fired to set ON for close caption system
            /// </summary>
            CLOSECAPTION_ON,
            /// <summary>
            /// Event fired to set OFF for close caption system
            /// </summary>
            CLOSECAPTION_OFF,
            /// <summary>
            /// Event fired to toggle the close caption system
            /// </summary>
            CLOSECAPTION_TOGGLE,
            /// <summary>
            /// To show system health panel
            /// </summary>
            SHOW_SYSTEM_HEALTH,
            /// <summary>
            /// To show system log panel
            /// </summary>
            SHOW_SYSTEM_LOG,
            /// <summary>
            /// To show system log panel
            /// </summary>
            SHOW_APPLICATION_ABOUT,
            #endregion

			#region Answer selection
			/// <summary>
			/// Answer or passage selection
			/// </summary>
			SELECT_ANSWER = 950
			#endregion

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

            /// <summary>
            /// Cache folder to customize a parent folder for cache directory
            /// </summary>
            public static string CACHE_FOLDER = "";   //It needs to start with / 

            /// <summary>
            /// Log folder to customize a parent folder for logs
            /// </summary>
            public static string LOG_FOLDER = "";    //It needs to start with / 
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
            /// <exclude />
            public const string DEBUG_DISPLAY_AVATAR_MOOD = "Behavior:{0} Mood: {1}";
			/// <exclude />
			public const string KEY_CONFIG_CLIENT_NAME = "CLIENT_NAME";
			public const string CLIENT_TEXTURE_LOGO = "{0}/Logo/{0}_LOGO";

            public const string DATE_LOCAL_FORMAT = "";
        }

    }
}


