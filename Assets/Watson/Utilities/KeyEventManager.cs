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
* @author Richard Lyle (rolyle@us.ibm.com)
*/


using System.Collections.Generic;
using UnityEngine;

namespace IBM.Watson.Utilities
{
    /// <summary>
    /// Key press modifiers
    /// </summary>
    public enum KeyModifiers
    {
        NONE = 0x0,
        SHIFT = 0x1,
        CONTROL = 0x2,
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

        #region Public Types
        /// <summary>
        /// Key press delegate callback.
        /// </summary>
        public delegate void KeyEventDelegate();

        /// <summary>
        /// This data class holds the data for a registered key press event
        /// </summary>
        public class KeyEvent
        {
            /// <summary>
            /// The name of the event to send to the EventManager singleton.
            /// </summary>
            public string m_SendEvent;
            /// <summary>
            /// Callback to invoke when the key is pressed.
            /// </summary>
            public KeyEventDelegate m_Delegate;
        };
        #endregion

        #region Private Data
        private bool m_Active = true;
        private bool m_UpdateActivate = true;
        private Dictionary<int, KeyEvent> m_KeyEvents = new Dictionary<int, KeyEvent>();
        #endregion

        #region Public Properties
        /// <summary>
        /// Set/Get the active state of this manager.
        /// </summary>
        public bool Active { get { return m_Active; } set { m_UpdateActivate = value; } }
        /// <summary>
        /// The current instance of the DebugConsole.
        /// </summary>
        public static KeyEventManager Instance { get { return Singleton<KeyEventManager>.Instance; } }
        #endregion

        #region Public Functions
        /// <summary>
        /// Register a key event.
        /// </summary>
        /// <param name="key">The KeyCode of the key.</param>
        /// <param name="modifiers">KeyCode modifiers</param>
        /// <param name="ke">The KeyEvent object.</param>
        /// <returns>True is returned on success.</returns>
        public bool RegisterKeyEvent(KeyCode key, KeyModifiers modifiers, KeyEvent ke)
        {
            int code = ((int)key) | (((int)modifiers) << MODIFIER_SHIFT_BITS);
            if (m_KeyEvents.ContainsKey(code))
                return false;
            m_KeyEvents[code] = ke;
            return true;
        }
        /// <summary>
        /// Invoke a callback when a key is released.
        /// </summary>
        /// <param name="key">The KeyCode of the key.</param>
        /// <param name="callback">The delegate to invoke.</param>
        /// <param name="modifiers">Additional keys that must be down as well to fire the event.</param>
        /// <returns>True is returned on success.</returns>
        public bool RegisterKeyEvent(KeyCode key, KeyEventDelegate callback, KeyModifiers modifiers = KeyModifiers.NONE)
        {
            return RegisterKeyEvent(key, modifiers, new KeyEvent() { m_Delegate = callback }) ;
        }
        /// <summary>
        /// Send a event when a key is released. 
        /// </summary>
        /// <param name="key">The KeyCode of the key.</param>
        /// <param name="eventName">The event to send when the key is released.</param>
        /// <param name="modifiers">Additional keys that must be down as well to fire the event.</param>
        /// <returns>True is returned on success.</returns>
        public bool RegisterKeyEvent(KeyCode key, string eventName, KeyModifiers modifiers = KeyModifiers.NONE )
        {
            return RegisterKeyEvent(key, modifiers, new KeyEvent() { m_SendEvent = eventName });
        }
        /// <summary>
        /// Unregister a key event.
        /// </summary>
        /// <param name="key">The KeyCode to unregister.</param>
        /// <param name="modifiers">Additional keys that must be down as well to fire the event.</param>
        /// <param name="callback">If provided, then the key will be unregistered only the callback matches the existing registration.</param>
        /// <returns>True is returned on success.</returns>
		public bool UnregisterKeyEvent(KeyCode key, KeyEventDelegate callback = null, KeyModifiers modifiers = KeyModifiers.NONE)
        {
            int code = ((int)key) | (((int)modifiers) << MODIFIER_SHIFT_BITS);
            if ( callback != null && m_KeyEvents.ContainsKey( code ) && m_KeyEvents[code].m_Delegate != callback )
                return false;

            return m_KeyEvents.Remove(code);
        }

        #endregion

        private void Update()
        {
            if (m_Active)
            {
                foreach (var kp in m_KeyEvents)
                {
                    KeyCode key = (KeyCode)(kp.Key & KEYCODE_MASK);

                    if (Input.GetKeyDown(key))
                    {
                        bool bFireEvent = true;

                        int modifiers = kp.Key >> MODIFIER_SHIFT_BITS;
                        if (modifiers != 0)
                        {
                            if ( (modifiers & (int)KeyModifiers.SHIFT) != 0 
                                && !Input.GetKey( KeyCode.RightShift ) && !Input.GetKey( KeyCode.LeftShift ) )
                            {
                                bFireEvent = false;
                            }
                            if ( (modifiers & (int)KeyModifiers.CONTROL) != 0 
                                && !Input.GetKey( KeyCode.RightControl ) && !Input.GetKey( KeyCode.LeftControl ) )
                            {
                                bFireEvent = false;
                            }
                            if ( (modifiers & (int)KeyModifiers.ALT) != 0 
                                && !Input.GetKey( KeyCode.RightAlt ) && !Input.GetKey( KeyCode.LeftAlt ) )
                            {
                                bFireEvent = false;
                            }
                        }

                        if (bFireEvent)
                        {
                            if (!string.IsNullOrEmpty(kp.Value.m_SendEvent))
                                EventManager.Instance.SendEvent(kp.Value.m_SendEvent);
                            if (kp.Value.m_Delegate != null)
                                kp.Value.m_Delegate();
                        }
                    }
                }
            }

            // update our active flag AFTER we check the active flag, this prevents
            // us from responding the key events during the same frame as we activate
            // this manager.
            m_Active = m_UpdateActivate;
        }

        private void OnApplicationQuit()
        {
            Destroy(gameObject);
        }
    }
}
