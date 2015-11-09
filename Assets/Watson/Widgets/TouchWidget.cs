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
using IBM.Watson.Logging;
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
			public Constants.Event m_Callback = Constants.Event.NONE;
        };

        [Serializable]
        private class FullScreenDragEventMapping
        {
			[Tooltip("If there is no drag layer object set, it uses FullScreen")]
			public GameObject m_DragLayerObject = null;
            public int m_NumberOfFinger = 1;
            public int m_SortingLayer = 0;
			public Constants.Event m_Callback = Constants.Event.NONE;
        };

        [SerializeField]
        private List<TapEventMapping> m_TapMappings = new List<TapEventMapping>();

        [SerializeField]
        private List<FullScreenDragEventMapping> m_FullScreenDragMappings = new List<FullScreenDragEventMapping>();

        private void OnEnable()
        {
			if (TouchEventManager.Instance == null) 
			{
				Log.Error ("TouchWidget", "There should be TouchEventManager in the scene! No TouchEventManager found.");
				return;
			}

            foreach (var mapping in m_TapMappings)
            {
                TouchEventManager.Instance.RegisterTapEvent(mapping.m_TapObject, mapping.m_Callback, mapping.m_SortingLayer, mapping.m_TapOnObject);
            }

            foreach (var mapping in m_FullScreenDragMappings)
            {
				TouchEventManager.Instance.RegisterDragEvent(mapping.m_DragLayerObject, mapping.m_Callback, mapping.m_NumberOfFinger, mapping.m_SortingLayer);
            }
        }

        private void OnDisable()
        {
			if (TouchEventManager.Instance == null) 
			{
				Log.Error ("TouchWidget", "There should be TouchEventManager in the scene! No TouchEventManager found.");
				return;
			}

            foreach (var mapping in m_TapMappings)
            {
                TouchEventManager.Instance.UnregisterTapEvent(mapping.m_TapObject, mapping.m_Callback, mapping.m_SortingLayer, mapping.m_TapOnObject);
            }

            foreach (var mapping in m_FullScreenDragMappings)
            {
				TouchEventManager.Instance.UnregisterDragEvent(mapping.m_DragLayerObject, mapping.m_Callback, mapping.m_NumberOfFinger, mapping.m_SortingLayer);
            }
        }
    }

}