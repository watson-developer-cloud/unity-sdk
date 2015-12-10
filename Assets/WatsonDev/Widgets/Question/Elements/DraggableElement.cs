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
using UnityEngine.EventSystems;
using IBM.Watson.Widgets.Question;
using IBM.Watson.Logging;
using IBM.Watson.Utilities;
using System;
using TouchScript.Gestures;

public class DraggableElement : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
	[SerializeField]
	private Facet m_Facet;
	[SerializeField]
	private RectTransform m_CanvasRectTransform;
	private RectTransform m_RectTransform;
//	private Vector2 m_Offset;

//	[Serializable]
//	private class FullScreenDragEventMapping
//	{
//		[Tooltip("If there is no drag layer object set, it uses FullScreen")]
//		public GameObject m_DragLayerObject = null;
//		public int m_NumberOfFinger = 1;
//		public int m_SortingLayer = 0;
//		public bool m_IsDragInside = true;
//		public Constants.Event m_Callback = Constants.Event.NONE;
//	};
	
//	[SerializeField]
//	private List<FullScreenDragEventMapping> m_FullScreenDragMappings = new List<FullScreenDragEventMapping>();

	void Awake()
	{
		m_RectTransform = gameObject.GetComponent<RectTransform>();
	}

	void OnEnable()
	{
//		if (TouchEventManager.Instance == null) 
//		{
//			Log.Error ("DraggableElement", "There should be TouchEventManager in the scene! No TouchEventManager found.");
//			return;
//		}
//
//		foreach (var mapping in m_FullScreenDragMappings)
//		{
//			TouchEventManager.Instance.RegisterDragEvent(mapping.m_DragLayerObject, mapping.m_Callback, mapping.m_NumberOfFinger, mapping.m_SortingLayer, isDragInside: mapping.m_IsDragInside);
//			EventManager.Instance.RegisterEventReceiver(mapping.m_Callback, OnParseTreeDrag);
//		}
	}

	void OnDisable()
	{
//		if (TouchEventManager.Instance == null) 
//		{
//			Log.Error ("DraggableElement", "There should be TouchEventManager in the scene! No TouchEventManager found.");
//			return;
//		}
//
//		foreach (var mapping in m_FullScreenDragMappings)
//		{
//			TouchEventManager.Instance.UnregisterDragEvent(mapping.m_DragLayerObject, mapping.m_Callback, mapping.m_NumberOfFinger, mapping.m_SortingLayer, isDragInside: mapping.m_IsDragInside);
//		}
	}
//
//	void OnParseTreeDrag(System.Object[] args = null)
//	{
//		if (args == null || args.Length == 0 || args.Length == 1)
//		{
//			Log.Debug("DraggableElement", "OnParseTreeDrag");
//		}
//		else
//		{
//			Log.Error("DraggableElement", "OnParseTreeDrag no args");
//		}
//	}
	#region IDragHandler implementation

	public void OnDrag (PointerEventData eventData)
	{
		if(m_Facet.Focused)
		{
			Vector2 tempPosition;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(m_CanvasRectTransform, eventData.position, Camera.main, out tempPosition);
			m_RectTransform.anchoredPosition = tempPosition;
		}
	}

	#endregion

	#region IBeginDragHandler implementation

	public void OnBeginDrag (PointerEventData eventData)
	{
//		RectTransformUtility.ScreenPointToLocalPointInRectangle(m_RectTransform, eventData.position, Camera.main, out m_Offset);
	}

	#endregion

	#region IEndDragHandler implementation
	public void OnEndDrag (PointerEventData eventData)
	{

	}
	#endregion

	private Vector2 GetOffset()
	{
		Vector2 touchOffset = Vector2.zero;

		return touchOffset;
	}
}
