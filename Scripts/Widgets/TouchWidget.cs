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

using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Logging;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace IBM.Watson.DeveloperCloud.Widgets
{

  /// <summary>
  /// This widget class maps Touch events to a SerializedDelegate.
  /// </summary>
  public class TouchWidget : Widget
  {
    #region Widget interface
    /// <exclude />
    protected override string GetName()
    {
      return "Touch";
    }
    #endregion

    #region Private Data
    [Serializable]
    public class TapEventMapping
    {
      public GameObject m_TapObject = null;
      public bool m_TapOnObject = true;
      public int m_SortingLayer = 0;
      public LayerMask m_LayerMask = default(LayerMask);
      public string m_Callback = "";
    };

    [Serializable]
    public class FullScreenDragEventMapping
    {
      [Tooltip("If there is no drag layer object set, it uses FullScreen")]
      public GameObject m_DragLayerObject = null;
      public int m_NumberOfFinger = 1;
      public int m_SortingLayer = 0;
      public bool m_IsDragInside = true;
      public string m_Callback = "";
    };

    [SerializeField]
    private List<TapEventMapping> m_TapMappings = new List<TapEventMapping>();

    [SerializeField]
    private List<FullScreenDragEventMapping> m_FullScreenDragMappings = new List<FullScreenDragEventMapping>();
    #endregion

    #region Public Members

    /// <summary>
    /// Gets or sets the tap mappings.
    /// </summary>
    /// <value>The tap mappings.</value>
    public List<TapEventMapping> TapMappings
    {
      get
      {
        return m_TapMappings;
      }
      set
      {
        m_TapMappings = value;
      }
    }

    /// <summary>
    /// Gets or sets the full screen drag mappings.
    /// </summary>
    /// <value>The full screen drag mappings.</value>
    public List<FullScreenDragEventMapping> FullScreenDragMappings
    {
      get
      {
        return m_FullScreenDragMappings;
      }
      set
      {
        m_FullScreenDragMappings = value;
      }
    }

    #endregion

    #region Event Handlers
    private void OnEnable()
    {
      if (TouchEventManager.Instance == null)
      {
        Log.Error("TouchWidget", "There should be TouchEventManager in the scene! No TouchEventManager found.");
        return;
      }

      foreach (var mapping in m_TapMappings)
      {
        if (!string.IsNullOrEmpty(mapping.m_Callback))
        {
          TouchEventManager.Instance.RegisterTapEvent(mapping.m_TapObject, mapping.m_Callback, mapping.m_SortingLayer, mapping.m_TapOnObject, mapping.m_LayerMask);
        }
        else
        {
          Log.Warning("TouchWidget", "Callback function needs to be defined to register TouchWidget - Tap");
        }
      }

      foreach (var mapping in m_FullScreenDragMappings)
      {
        if (!string.IsNullOrEmpty(mapping.m_Callback))
        {
          TouchEventManager.Instance.RegisterDragEvent(mapping.m_DragLayerObject, mapping.m_Callback, mapping.m_NumberOfFinger, mapping.m_SortingLayer, isDragInside: mapping.m_IsDragInside);
        }
        else
        {
          Log.Warning("TouchWidget", "Callback function needs to be defined to register TouchWidget - Drag");
        }
      }
    }

    private void OnDisable()
    {
      if (TouchEventManager.Instance == null)
      {
        //Log.Error ("TouchWidget", "There should be TouchEventManager in the scene! No TouchEventManager found.");
        return;
      }

      foreach (var mapping in m_TapMappings)
      {
        if (!string.IsNullOrEmpty(mapping.m_Callback))
        {
          TouchEventManager.Instance.UnregisterTapEvent(mapping.m_TapObject, mapping.m_Callback, mapping.m_SortingLayer, mapping.m_TapOnObject, mapping.m_LayerMask);
        }
        else
        {
          Log.Warning("TouchWidget", "Callback function needs to be defined to unregister TouchWidget - Tap");
        }
      }

      foreach (var mapping in m_FullScreenDragMappings)
      {
        if (!string.IsNullOrEmpty(mapping.m_Callback))
        {
          TouchEventManager.Instance.UnregisterDragEvent(mapping.m_DragLayerObject, mapping.m_Callback, mapping.m_NumberOfFinger, mapping.m_SortingLayer, isDragInside: mapping.m_IsDragInside);
        }
        else
        {
          Log.Warning("TouchWidget", "Callback function needs to be defined to unregister TouchWidget - Drag");
        }


      }
    }
    #endregion
  }

}