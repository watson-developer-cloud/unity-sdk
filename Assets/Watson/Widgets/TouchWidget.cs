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

using IBM.Watson.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IBM.Watson.Widgets
{

    /// <summary>
    /// This widget class maps Touch events to a SerializedDelegate.
    /// </summary>
    public class TouchWidget : Widget
    {
        #region Widget interface
        protected override string GetName()
        {
            return "Touch";
        }
        #endregion
        
        [Serializable]
        private class TapEventMapping
        {
            public GameObject m_TapObject = null;
            public bool m_TapOnObject = true;
            public int m_SortingLayer = 0;
            public SerializedDelegate m_Callback = new SerializedDelegate(typeof(TouchEventManager.TapEventDelegate));
        };

        [Serializable]
        private class FullScreenDragEventMapping
        {
            public int m_NumberOfFinger = 1;
            public int m_SortingLayer = 0;
            public SerializedDelegate m_Callback = new SerializedDelegate(typeof(TouchEventManager.DragEventDelegate));
        };

        [SerializeField]
        private List<TapEventMapping> m_TapMappings = new List<TapEventMapping>();

        [SerializeField]
        private List<FullScreenDragEventMapping> m_FullScreenDragMappings = new List<FullScreenDragEventMapping>();

        private void OnEnable()
        {
            foreach (var mapping in m_TapMappings)
            {
                TouchEventManager.Instance.RegisterTapEvent(mapping.m_TapObject, mapping.m_Callback.ResolveDelegate() as TouchEventManager.TapEventDelegate, mapping.m_SortingLayer, mapping.m_TapOnObject);
            }

            foreach (var mapping in m_FullScreenDragMappings)
            {
                TouchEventManager.Instance.RegisterDragEvent(mapping.m_Callback.TargetGameObject, mapping.m_Callback.ResolveDelegate() as TouchEventManager.DragEventDelegate, mapping.m_NumberOfFinger, mapping.m_SortingLayer);
            }
        }

        private void OnDisable()
        {
            foreach (var mapping in m_TapMappings)
            {
                TouchEventManager.Instance.UnregisterTapEvent(mapping.m_TapObject, mapping.m_Callback.ResolveDelegate() as TouchEventManager.TapEventDelegate, mapping.m_SortingLayer, mapping.m_TapOnObject);
            }

            foreach (var mapping in m_FullScreenDragMappings)
            {
                TouchEventManager.Instance.UnregisterDragEvent(mapping.m_Callback.TargetGameObject, mapping.m_Callback.ResolveDelegate() as TouchEventManager.DragEventDelegate, mapping.m_NumberOfFinger, mapping.m_SortingLayer);
            }
        }
    }

}