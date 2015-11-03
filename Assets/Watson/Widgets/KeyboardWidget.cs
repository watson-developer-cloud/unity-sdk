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

using IBM.Watson.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IBM.Watson.Widgets
{

    /// <summary>
    /// This widget class maps key events to a SerializedDelegate.
    /// </summary>
    public class KeyboardWidget : Widget
    {
        #region Widget interface
        protected override string GetName()
        {
            return "Keyboard";
        }
        #endregion

        [Serializable]
        private class Mapping
        {
            public KeyCode m_Key = (KeyCode)0;
            public KeyModifiers m_Modifiers = KeyModifiers.NONE;
            public SerializedDelegate m_Callback = new SerializedDelegate(typeof(KeyEventManager.KeyEventDelegate));
        };

        [SerializeField]
        private List<Mapping> m_Mappings = new List<Mapping>();

        private void OnEnable()
        {
            foreach (var mapping in m_Mappings)
            {
                KeyEventManager.Instance.RegisterKeyEvent(mapping.m_Key, mapping.m_Modifiers,
                   mapping.m_Callback.ResolveDelegate() as KeyEventManager.KeyEventDelegate );
            }
        }

        private void OnDisable()
        {
            foreach (var mapping in m_Mappings)
            {
                KeyEventManager.Instance.UnregisterKeyEvent(mapping.m_Key, mapping.m_Modifiers, 
                    mapping.m_Callback.ResolveDelegate() as KeyEventManager.KeyEventDelegate );
            }
        }
    }

}