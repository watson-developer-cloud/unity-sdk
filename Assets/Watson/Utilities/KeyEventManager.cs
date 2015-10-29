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
    /// This class handles key presses and will sent events and/or invoke a delegate when a key is pressed.
    /// </summary>
    public class KeyEventManager : MonoBehaviour
    {
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
            /// <summary>
            /// Additional keys that must be down to fire this event.
            /// </summary>
            public KeyCode[] m_Modifiers;
        };
        #endregion

        #region Private Data
        private bool m_Active = true;
        private bool m_UpdateActivate = true;
        private Dictionary<KeyCode, KeyEvent> m_KeyEvents = new Dictionary<KeyCode, KeyEvent>();
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
        /// <param name="ke">The KeyEvent object.</param>
        /// <returns>True is returned on success.</returns>
        public bool RegisterKeyEvent(KeyCode key, KeyEvent ke)
        {
            if (m_KeyEvents.ContainsKey(key))
                return false;
            m_KeyEvents[key] = ke;
            return true;
        }
        /// <summary>
        /// Invoke a callback when a key is released.
        /// </summary>
        /// <param name="key">The KeyCode of the key.</param>
        /// <param name="callback">The delegate to invoke.</param>
        /// <param name="modifiers">Additional keys that must be down as well to fire the event.</param>
        /// <returns>True is returned on success.</returns>
        public bool RegisterKeyEvent(KeyCode key, KeyEventDelegate callback, KeyCode[] modifiers = null)
        {
            return RegisterKeyEvent(key, new KeyEvent() { m_Delegate = callback, m_Modifiers = modifiers });
        }
        /// <summary>
        /// Send a event when a key is released. 
        /// </summary>
        /// <param name="key">The KeyCode of the key.</param>
        /// <param name="eventName">The event to send when the key is released.</param>
        /// <param name="modifiers">Additional keys that must be down as well to fire the event.</param>
        /// <returns>True is returned on success.</returns>
        public bool RegisterKeyEvent(KeyCode key, string eventName, KeyCode[] modifiers = null)
        {
            return RegisterKeyEvent(key, new KeyEvent() { m_SendEvent = eventName, m_Modifiers = modifiers });
        }
        /// <summary>
        /// Unregister a key event.
        /// </summary>
        /// <param name="key">The KeyCode to unregister.</param>
        /// <returns>True is returned on success.</returns>
        public bool UnregisterKeyEvent(KeyCode key)
        {
            return m_KeyEvents.Remove(key);
        }

		/// <summary>
		/// Unregisters the key event.
		/// </summary>
		/// <returns><c>true</c>, if key event was unregistered, <c>false</c> otherwise.</returns>
		/// <param name="key">The KeyCode to unregister.</param>
		/// <param name="callback">The delegate to invoke.</param>
		/// <param name="modifiers">Modifiers.</param>
		public bool UnregisterKeyEvent(KeyCode key, KeyEventDelegate callback, KeyCode[] modifiers = null)
		{
			//TODO
			return true;
		}


        #endregion

        private void Update()
        {
            if (m_Active)
            {
                foreach (var kp in m_KeyEvents)
                {
                    if (Input.GetKeyDown(kp.Key))
                    {
                        bool bFireEvent = true;
                        if (kp.Value.m_Modifiers != null)
                        {
                            foreach (var mod in kp.Value.m_Modifiers)
                                if (!Input.GetKey(mod))
                                {
                                    bFireEvent = false;
                                    break;
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
