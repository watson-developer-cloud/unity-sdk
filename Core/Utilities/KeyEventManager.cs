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

using IBM.Cloud.SDK.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IBM.Cloud.SDK
{
    /// <summary>
    /// Key press modifiers
    /// </summary>
    [Flags]
    public enum KeyModifiers
    {
        /// <summary>
        /// No key modifier down.
        /// </summary>
        NONE = 0x0,
        /// <summary>
        /// Shift key down.
        /// </summary>
        SHIFT = 0x1,
        /// <summary>
        /// Control key down.
        /// </summary>
        CONTROL = 0x2,
        /// <summary>
        /// Alt key down.
        /// </summary>
        ALT = 0x4
    };

    /// <summary>
    /// This class handles key presses and will sent events and/or invoke a delegate when a key is pressed.
    /// </summary>
    public class KeyEventManager : MonoBehaviour
    {
        /// How many bits to shift modifier up/down when mapped into the dictionary.
        private int MODIFIER_SHIFT_BITS = 10;
        private int KEYCODE_MASK = (1 << 10) - 1;

        #region Private Data
        private bool _active = true;
        private bool _updateActivate = true;
        private Dictionary<int, string> _keyEvents = new Dictionary<int, string>();
        #endregion

        #region Public Properties
        /// <summary>
        /// Set/Get the active state of this manager.
        /// </summary>
        public bool Active { get { return _active; } set { _updateActivate = value; } }
        /// <summary>
        /// The current instance of the DebugConsole.
        /// </summary>
        public static KeyEventManager Instance { get { return Singleton<KeyEventManager>.Instance; } }
        #endregion

        #region OnEnable / OnDisable - Initial keys to capture
        private void OnEnable()
        {
            KeyEventManager.Instance.RegisterKeyEvent(KeyCode.Tab, KeyModifiers.NONE, "OnKeyboardTab");
            KeyEventManager.Instance.RegisterKeyEvent(KeyCode.Return, KeyModifiers.NONE, "OnKeyboardReturn");
            KeyEventManager.Instance.RegisterKeyEvent(KeyCode.Escape, KeyModifiers.NONE, "OnKeyboardEscape");
            KeyEventManager.Instance.RegisterKeyEvent(KeyCode.BackQuote, KeyModifiers.NONE, "OnKeyboardBackquote");
        }

        private void OnDisable()
        {
            KeyEventManager.Instance.UnregisterKeyEvent(KeyCode.Tab, KeyModifiers.NONE, "OnKeyboardTab");
            KeyEventManager.Instance.UnregisterKeyEvent(KeyCode.Return, KeyModifiers.NONE, "OnKeyboardReturn");
            KeyEventManager.Instance.UnregisterKeyEvent(KeyCode.Escape, KeyModifiers.NONE, "OnKeyboardEscape");
            KeyEventManager.Instance.UnregisterKeyEvent(KeyCode.BackQuote, KeyModifiers.NONE, "OnKeyboardBackquote");
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Register a key event.
        /// </summary>
        /// <param name="key">The KeyCode of the key.</param>
        /// <param name="modifiers">KeyCode modifiers</param>
        /// <param name="eventType">The event to send.</param>
        /// <returns>True is returned on success.</returns>
        public bool RegisterKeyEvent(KeyCode key, KeyModifiers modifiers, string eventType)
        {
            int code = ((int)key) | (((int)modifiers) << MODIFIER_SHIFT_BITS);
            _keyEvents[code] = eventType;
            return true;
        }
        /// <summary>
        /// Unregister a key event.
        /// </summary>
        /// <param name="key">The KeyCode to unregister.</param>
        /// <param name="modifiers">Additional keys that must be down as well to fire the event.</param>
        /// <param name="eventType">If provided, then the key will be unregistered only the event matches the existing registration.</param>
        /// <returns>True is returned on success.</returns>
        public bool UnregisterKeyEvent(KeyCode key, KeyModifiers modifiers = KeyModifiers.NONE, string eventType = "")
        {
            int code = ((int)key) | (((int)modifiers) << MODIFIER_SHIFT_BITS);
            if (eventType != "" && _keyEvents.ContainsKey(code) && _keyEvents[code] == eventType)
                return _keyEvents.Remove(code);

            return _keyEvents.Remove(code);
        }

        #endregion

        private void Update()
        {
            if (_active)
            {
                List<string> fire = new List<string>();
                foreach (var kp in _keyEvents)
                {
                    KeyCode key = (KeyCode)(kp.Key & KEYCODE_MASK);

                    if (Input.GetKeyDown(key))
                    {
                        bool bFireEvent = true;

                        int modifiers = kp.Key >> MODIFIER_SHIFT_BITS;
                        if (modifiers != 0)
                        {
                            if ((modifiers & (int)KeyModifiers.SHIFT) != 0
                                && !Input.GetKey(KeyCode.RightShift) && !Input.GetKey(KeyCode.LeftShift))
                            {
                                bFireEvent = false;
                            }
                            if ((modifiers & (int)KeyModifiers.CONTROL) != 0
                                && !Input.GetKey(KeyCode.RightControl) && !Input.GetKey(KeyCode.LeftControl))
                            {
                                bFireEvent = false;
                            }
                            if ((modifiers & (int)KeyModifiers.ALT) != 0
                                && !Input.GetKey(KeyCode.RightAlt) && !Input.GetKey(KeyCode.LeftAlt))
                            {
                                bFireEvent = false;
                            }
                        }

                        if (bFireEvent)
                            fire.Add(kp.Value);
                    }

                    if (Input.anyKeyDown && !string.IsNullOrEmpty(Input.inputString))
                    {
                        EventManager.Instance.SendEvent("OnKeyboardAnyKeyDown", Input.inputString);
                    }
                }

                // now fire the events outside of the dictionary loop so we don't throw an exception..
                foreach (var ev in fire)
                    EventManager.Instance.SendEvent(ev);
            }

            // update our active flag AFTER we check the active flag, this prevents
            // us from responding the key events during the same frame as we activate
            // this manager.
            _active = _updateActivate;
        }

        private void OnApplicationQuit()
        {
            Destroy(gameObject);
        }
    }
}
