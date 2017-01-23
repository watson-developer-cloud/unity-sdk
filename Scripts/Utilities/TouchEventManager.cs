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

// uncomment to enable debugging
//#define ENABLE_DEBUGGING

using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using TouchScript.Gestures;
using IBM.Watson.DeveloperCloud.Logging;

namespace IBM.Watson.DeveloperCloud.Utilities
{
  /// <summary>
  /// Touch Event Manager for all touch events. 
  /// Each element can register their touch related functions using this manager. 
  /// </summary>
  [RequireComponent(typeof(TapGesture))]
  public class TouchEventManager : MonoBehaviour
  {

    /// <summary>
    /// Touch Event Data holds all touch related event data for registering and unregistering events via Touch Event Manager.
    /// </summary>
    public class TouchEventData
    {
      private Collider m_Collider;
      private Collider2D m_Collider2D;
      private RectTransform m_RectTransform;
      private Collider[] m_ColliderList;
      private Collider2D[] m_Collider2DList;
      private RectTransform[] m_RectTransformList;
      private GameObject m_GameObject;
      private string m_tapEventCallback;
      private string m_dragEventCallback;
      private bool m_isInside;
      private int m_SortingLayer;

      /// <summary> 
      /// Game Object related with touch event
      /// </summary>
      public GameObject GameObjectAttached { get { return m_GameObject; } }
      /// <summary>
      /// If it is tap event (or one time action event) we are returning the collider of the event.
      /// </summary>
      public Collider Collider { get { return m_Collider; } }
      /// <summary>
      /// Gets the collider2 d.
      /// </summary>
      /// <value>The collider2 d.</value>
      public Collider2D Collider2D { get { return m_Collider2D; } }
      /// <summary>
      /// Gets the rect transform.
      /// </summary>
      /// <value>The rect transform.</value>
      public RectTransform RectTransform { get { return m_RectTransform; } }
      /// <summary>
      /// If there is a drag event (or continues action) we are holding game object and all colliders inside that object
      /// </summary>
      public Collider[] ColliderList { get { if (m_ColliderList == null && m_Collider != null) m_ColliderList = new Collider[] { m_Collider }; return m_ColliderList; } }
      /// <summary>
      /// If there is a drag event (or continues action) we are holding game object and all colliders inside that object
      /// </summary>
      public Collider2D[] ColliderList2D { get { if (m_Collider2DList == null && m_Collider2D != null) m_Collider2DList = new Collider2D[] { m_Collider2D }; return m_Collider2DList; } }
      /// <summary>
      /// Gets the rect transform list.
      /// </summary>
      /// <value>The rect transform list.</value>
      public RectTransform[] RectTransformList { get { if (m_RectTransformList == null && m_RectTransform != null) m_RectTransformList = new RectTransform[] { m_RectTransform }; return m_RectTransformList; } }

      /// <summary>
      /// If the touch event has happened inside of that object (collider) we will fire that event. Otherwise, it is considered as outside
      /// </summary>
      public bool IsInside { get { return m_isInside; } }
      /// <summary>
      /// Tap Delegate to call
      /// </summary>
      public string TapCallback { get { return m_tapEventCallback; } }
      /// <summary>
      /// Drag Delegate to call
      /// </summary>
      public string DragCallback { get { return m_dragEventCallback; } }
      /// <summary>
      /// Greater sorting layer is higher importance level. 
      /// </summary>
      public int SortingLayer { get { return m_SortingLayer; } }
      /// <summary>
      /// Gets a value indicating whether this instance can drag object.
      /// </summary>
      /// <value><c>true</c> if this instance can drag object; otherwise, <c>false</c>.</value>
      public bool CanDragObject { get { return GameObjectAttached != null && ((ColliderList != null && ColliderList.Length > 0) || (ColliderList2D != null && ColliderList2D.Length > 0) || (RectTransformList != null && RectTransformList.Length > 0)); } }

      /// <summary>
      /// Touch event constructor for Tap Event registration. 
      /// </summary>
      /// <param name="collider">Collider of the object to tap</param>
      /// <param name="callback">Callback for Tap Event. After tapped, callback will be invoked</param>
      /// <param name="sortingLayer">Sorting level in order to sort the event listeners</param>
      /// <param name="isInside">Whether the tap is inside the object or not</param>
      public TouchEventData(Collider collider, string callback, int sortingLayer, bool isInside)
      {
        m_Collider = collider;
        m_Collider2D = null;
        m_RectTransform = null;
        m_ColliderList = null;
        m_Collider2DList = null;
        m_RectTransformList = null;
        m_tapEventCallback = callback;
        m_SortingLayer = sortingLayer;
        m_isInside = isInside;
      }

      /// <summary>
      /// Touch event constructor for 2D Tap Event registration. 
      /// </summary>
      /// <param name="collider">Collider of the object to tap</param>
      /// <param name="callback">Callback for Tap Event. After tapped, callback will be invoked</param>
      /// <param name="sortingLayer">Sorting level in order to sort the event listeners</param>
      /// <param name="isInside">Whether the tap is inside the object or not</param>
      public TouchEventData(Collider2D collider, string callback, int sortingLayer, bool isInside)
      {
        m_Collider = null;
        m_Collider2D = collider;
        m_RectTransform = null;
        m_ColliderList = null;
        m_Collider2DList = null;
        m_RectTransformList = null;
        m_tapEventCallback = callback;
        m_SortingLayer = sortingLayer;
        m_isInside = isInside;
      }

      /// <summary>
      /// Initializes a new instance of the
      /// <see cref="IBM.Watson.DeveloperCloud.Utilities.TouchEventManager+TouchEventData"/> class.
      /// </summary>
      /// <param name="rectTransform">Rect transform.</param>
      /// <param name="callback">Callback.</param>
      /// <param name="sortingLayer">Sorting layer.</param>
      /// <param name="isInside">If set to <c>true</c> is inside.</param>
      public TouchEventData(RectTransform rectTransform, string callback, int sortingLayer, bool isInside)
      {
        m_Collider = null;
        m_Collider2D = null;
        m_RectTransform = rectTransform;
        m_ColliderList = null;
        m_Collider2DList = null;
        m_RectTransformList = null;
        m_tapEventCallback = callback;
        m_SortingLayer = sortingLayer;
        m_isInside = isInside;
      }

      /// <summary>
      /// Touch event constructor for Drag Event registration. 
      /// </summary>
      /// <param name="gameObject">Gameobject to drag</param>
      /// <param name="callback">Callback for Drag event. After dragging started, callback will be invoked until drag will be finished</param>
      /// <param name="sortingLayer">Sorting level in order to sort the event listeners</param>
      /// <param name="isInside"></param>
      public TouchEventData(GameObject gameObject, string callback, int sortingLayer, bool isInside)
      {
        m_GameObject = gameObject;
        m_ColliderList = null;
        if (gameObject != null)
        {
          m_ColliderList = gameObject.GetComponentsInChildren<Collider>(includeInactive: true);
          m_Collider2DList = gameObject.GetComponentsInChildren<Collider2D>(includeInactive: true);
          m_RectTransformList = gameObject.GetComponentsInChildren<RectTransform>(includeInactive: true);
        }
        m_dragEventCallback = callback;
        m_SortingLayer = sortingLayer;
        m_isInside = isInside;
      }

      /// <summary>
      /// Determines whether this instance has touched on the specified hitTransform.
      /// </summary>
      /// <returns><c>true</c> if this instance has touched on the specified hitTransform; otherwise, <c>false</c>.</returns>
      /// <param name="hitTransform">Hit transform.</param>
      public bool HasTouchedOn(Transform hitTransform)
      {
        bool hasTouchedOn = false;
        if (ColliderList != null)
        {
          foreach (Collider itemCollider in ColliderList)
          {
            if (itemCollider.transform == hitTransform && itemCollider.gameObject.activeSelf)
            {
              hasTouchedOn = true;
              break;
            }
          }
        }

        if (!hasTouchedOn && ColliderList2D != null)
        {
          foreach (Collider2D itemCollider in ColliderList2D)
          {
            if (itemCollider.transform == hitTransform && itemCollider.gameObject.activeSelf)
            {
              hasTouchedOn = true;
              break;
            }
          }
        }

        if (!hasTouchedOn && RectTransformList != null)
        {
          foreach (RectTransform itemRectTransform in RectTransformList)
          {
            if (itemRectTransform.transform == hitTransform && itemRectTransform.gameObject.activeSelf)
            {
              hasTouchedOn = true;
              break;
            }
          }
        }

        return hasTouchedOn;
      }

      /// <summary>
      /// To check equality of the same Touch Event Data
      /// </summary>
      /// <param name="obj">Object to check equality</param>
      /// <returns>True if objects are equal</returns>
      public override bool Equals(object obj)
      {
        bool isEqual = false;
        TouchEventData touchEventData = obj as TouchEventData;
        if (touchEventData != null)
        {
          isEqual =
              (touchEventData.Collider == this.Collider &&
                  touchEventData.Collider2D == this.Collider2D &&
                  touchEventData.RectTransform == this.RectTransform &&
                  touchEventData.GameObjectAttached == this.GameObjectAttached &&
                  touchEventData.IsInside == this.IsInside &&
                  touchEventData.SortingLayer == this.SortingLayer &&
                  touchEventData.DragCallback == this.DragCallback &&
                  touchEventData.TapCallback == this.TapCallback);
        }
        else
        {
          isEqual = base.Equals(obj);
        }

        return isEqual;
      }

      /// <summary>
      /// Returns the hash code
      /// </summary>
      /// <returns>Default hash code coming from base class</returns>
      public override int GetHashCode()
      {
        return base.GetHashCode();
      }

      /// <summary>
      /// Returns a <see cref="System.String"/> that represents the current <see cref="IBM.Watson.DeveloperCloud.Utilities.TouchEventManager+TouchEventData"/>.
      /// </summary>
      /// <returns>A <see cref="System.String"/> that represents the current <see cref="IBM.Watson.DeveloperCloud.Utilities.TouchEventManager+TouchEventData"/>.</returns>
      public override string ToString()
      {
        return string.Format("[TouchEventData: GameObjectAttached={0}, Collider={1}, Collider2D={2}, RectTransform={3}, ColliderList={4}, ColliderList2D={5}, RectTransformList={6}, IsInside={7}, TapCallback={8}, DragCallback={9}, SortingLayer={10}, CanDragObject={11}]", GameObjectAttached, Collider, Collider2D, RectTransform, ColliderList, ColliderList2D, RectTransformList, IsInside, TapCallback, DragCallback, SortingLayer, CanDragObject);
      }

    }

    #region Private Data
    private UnityEngine.Camera m_mainCamera;

    private bool m_Active = true;
    private Dictionary<int, List<TouchEventData>> m_TapEvents = new Dictionary<int, List<TouchEventData>>();
    private Dictionary<int, List<TouchEventData>> m_DoubleTapEvents = new Dictionary<int, List<TouchEventData>>();
    private Dictionary<int, List<TouchEventData>> m_DragEvents = new Dictionary<int, List<TouchEventData>>();
    #endregion

    #region Serialized Private 
    [SerializeField]
    private TapGesture m_TapGesture;
    [SerializeField]
    private TapGesture m_DoubleTapGesture;
    [SerializeField]
    private TapGesture m_ThreeTapGesture;
    [SerializeField]
    private ScreenTransformGesture m_OneFingerMoveGesture;
    [SerializeField]
    private ScreenTransformGesture m_TwoFingerMoveGesture;
    [SerializeField]
    private PressGesture m_PressGesture;
    [SerializeField]
    private ReleaseGesture m_ReleaseGesture;
    [SerializeField]
    private LongPressGesture m_LongPressGesture;

    #endregion

    #region Public Properties
    /// <summary>
    /// Set/Get the active state of this manager.
    /// </summary>
    public bool Active { get { return m_Active; } set { m_Active = value; } }

    private static TouchEventManager sm_Instance = null;
    /// <summary>
    /// The current instance of the TouchEventManager.
    /// </summary>
    public static TouchEventManager Instance { get { return sm_Instance; } }
    #endregion

    #region Awake / OnEnable / OnDisable

    void Awake()
    {
      sm_Instance = this;
    }

    private void OnEnable()
    {
      m_mainCamera = UnityEngine.Camera.main;
      if (m_TapGesture != null)
        m_TapGesture.Tapped += TapGesture_Tapped;
      if (m_DoubleTapGesture != null)
        m_DoubleTapGesture.Tapped += DoubleTapGesture_Tapped;
      if (m_ThreeTapGesture != null)
        m_ThreeTapGesture.Tapped += ThreeTapGesture_Tapped;
      if (m_OneFingerMoveGesture != null)
        m_OneFingerMoveGesture.Transformed += OneFingerTransformedHandler;
      if (m_TwoFingerMoveGesture != null)
        m_TwoFingerMoveGesture.Transformed += TwoFingerTransformedHandler;
      if (m_PressGesture != null)
        m_PressGesture.Pressed += PressGesturePressed;
      if (m_ReleaseGesture != null)
        m_ReleaseGesture.Released += ReleaseGestureReleased;
      if (m_LongPressGesture != null)
        m_LongPressGesture.LongPressed += LongPressGesturePressed;

    }

    private void OnDisable()
    {
      if (m_TapGesture != null)
        m_TapGesture.Tapped -= TapGesture_Tapped;
      if (m_DoubleTapGesture != null)
        m_DoubleTapGesture.Tapped -= DoubleTapGesture_Tapped;
      if (m_ThreeTapGesture != null)
        m_ThreeTapGesture.Tapped -= ThreeTapGesture_Tapped;
      if (m_OneFingerMoveGesture != null)
        m_OneFingerMoveGesture.Transformed -= OneFingerTransformedHandler;
      if (m_TwoFingerMoveGesture != null)
        m_TwoFingerMoveGesture.Transformed -= TwoFingerTransformedHandler;
      if (m_PressGesture != null)
        m_PressGesture.Pressed -= PressGesturePressed;
      if (m_ReleaseGesture != null)
        m_ReleaseGesture.Released -= ReleaseGestureReleased;
      if (m_LongPressGesture != null)
        m_LongPressGesture.LongPressed -= LongPressGesturePressed;

      if (m_DragEvents != null)
        m_DragEvents.Clear();
      if (m_TapEvents != null)
        m_TapEvents.Clear();
      if (m_DoubleTapEvents != null)
        m_DoubleTapEvents.Clear();
    }

    /// <summary>
    /// Gets the main camera.
    /// </summary>
    /// <value>The main camera.</value>
    public UnityEngine.Camera MainCamera
    {
      get
      {
        if (m_mainCamera == null || !m_mainCamera.transform.CompareTag("MainCamera"))
          m_mainCamera = UnityEngine.Camera.main;

        return m_mainCamera;
      }
    }


    #endregion

    #region OneFinger Events - Register / UnRegister / Call
    /// <summary>
    /// Registers the drag event.
    /// </summary>
    /// <returns><c>true</c>, if drag event was registered, <c>false</c> otherwise.</returns>
    /// <param name="gameObjectToDrag">Game object to drag. If it is null then fullscreen drag is registered. </param>
    /// <param name="callback">Callback.</param>
    /// <param name="numberOfFinger">Number of finger.</param>
    /// <param name="SortingLayer">Sorting layer.</param>
    /// <param name="isDragInside">If set to <c>true</c> is drag inside.</param>
    public bool RegisterDragEvent(GameObject gameObjectToDrag, string callback, int numberOfFinger = 1, int SortingLayer = 0, bool isDragInside = true)
    {
      bool success = false;

      if (!string.IsNullOrEmpty(callback))
      {
        if (m_DragEvents.ContainsKey(numberOfFinger))
        {
          m_DragEvents[numberOfFinger].Add(new TouchEventData(gameObjectToDrag, callback, SortingLayer, isDragInside));
        }
        else
        {
          m_DragEvents[numberOfFinger] = new List<TouchEventData>() { new TouchEventData(gameObjectToDrag, callback, SortingLayer, isDragInside) };
        }

        success = true;
      }
      else
      {
        Log.Warning("TouchEventManager", "There is no callback for drag event registration");
      }

      return success;
    }

    /// <summary>
    /// Unregisters the drag event.
    /// </summary>
    /// <returns><c>true</c>, if drag event was unregistered, <c>false</c> otherwise.</returns>
    /// <param name="gameObjectToDrag">Game object to drag.</param>
    /// <param name="callback">Callback.</param>
    /// <param name="numberOfFinger">Number of finger.</param>
    /// <param name="SortingLayer">Sorting layer.</param>
    /// <param name="isDragInside">If set to <c>true</c> is drag inside.</param>
    public bool UnregisterDragEvent(GameObject gameObjectToDrag, string callback, int numberOfFinger = 1, int SortingLayer = 0, bool isDragInside = true)
    {
      bool success = false;

      if (!string.IsNullOrEmpty(callback))
      {
        if (m_DragEvents.ContainsKey(numberOfFinger))
        {
          int numberOfRemovedCallbacks = m_DragEvents[numberOfFinger].RemoveAll(
              e =>
              e.GameObjectAttached == gameObjectToDrag &&
              e.DragCallback == callback &&
              e.SortingLayer == SortingLayer &&
              e.IsInside == isDragInside);

          success &= (numberOfRemovedCallbacks > 0);
        }
      }
      else
      {
        Log.Warning("TouchEventManager", "There is no callback for drag event unregistration");
      }
      return success;
    }


    private void OneFingerTransformedHandler(object sender, System.EventArgs e)
    {
      RaycastHit hitToFire3D = default(RaycastHit);
      RaycastHit2D hitToFire2D = default(RaycastHit2D);
#if UNITY_4_6 || UNITY_5 || UNITY_5_3_OR_NEWER
      RaycastResult hitToFire2DEventSystem = default(RaycastResult);
#endif

      //Log.Status ("TouchEventManager", "oneFingerManipulationTransformedHandler: {0}", m_OneFingerMoveGesture.DeltaPosition);
      if (m_Active)
      {
        TouchEventData dragEventToFire = null;
        Vector3 oneFingerScreenPosition = m_OneFingerMoveGesture.ScreenPosition;
        Ray rayForDrag = MainCamera.ScreenPointToRay(oneFingerScreenPosition);

        foreach (var kp in m_DragEvents)
        {
          if (kp.Key == 1)
          {
            //Adding Variables for 3D Touch Check
            Transform hitTransform3D = null;
            RaycastHit hit3D = default(RaycastHit);
            bool isHitOnLayer3D = Physics.Raycast(rayForDrag, out hit3D, Mathf.Infinity, kp.Key);
            if (isHitOnLayer3D)
            {
              hitTransform3D = hit3D.collider.transform;
            }

            //Adding Variables for 2D Touch Check for 2d Colliders
            Transform hitTransform2D = null;
            RaycastHit2D hit2D = Physics2D.Raycast(rayForDrag.origin, rayForDrag.direction, Mathf.Infinity, kp.Key);
            bool isHitOnLayer2D = false;
            if (hit2D.collider != null)
            {
              isHitOnLayer2D = true;
              hitTransform2D = hit2D.collider.transform;
            }

#if UNITY_4_6 || UNITY_5 || UNITY_5_3_OR_NEWER
            //Adding Variables for Event.System Touch for UI Elements
            Transform hitTransform2DEventSystem = null;
            bool isHitOnLayer2DEventSystem = false;
            RaycastResult hit2DEventSystem = default(RaycastResult);
            if (EventSystem.current != null)
            {
              PointerEventData pointerEventForTap = new PointerEventData(EventSystem.current);
              pointerEventForTap.position = oneFingerScreenPosition;
              List<RaycastResult> raycastResultListFor2DEventSystem = new List<RaycastResult>();
              EventSystem.current.RaycastAll(pointerEventForTap, raycastResultListFor2DEventSystem);
              foreach (RaycastResult itemRaycastResult in raycastResultListFor2DEventSystem)
              {

                isHitOnLayer2DEventSystem = kp.Value.Exists(element => (element.GameObjectAttached != null && element.GameObjectAttached.activeSelf && element.GameObjectAttached.layer == itemRaycastResult.gameObject.layer));
                if (isHitOnLayer2DEventSystem)
                {
                  hit2DEventSystem = itemRaycastResult;
                  hitTransform2DEventSystem = itemRaycastResult.gameObject.transform;
                  break;
                }
              }
            }
#endif

            for (int i = 0; i < kp.Value.Count; ++i)
            {
              TouchEventData dragEventData = kp.Value[i];

              if (kp.Value[i].ColliderList == null && kp.Value[i].ColliderList2D == null && kp.Value[i].RectTransformList == null)
              {
                Log.Warning("TouchEventManager", "Removing invalid collider event receiver from OneFingerDrag");
                kp.Value.RemoveAt(i--);
                continue;
              }

              if (string.IsNullOrEmpty(dragEventData.DragCallback))
              {
                Log.Warning("TouchEventManager", "Removing invalid event receiver from OneFingerDrag");
                kp.Value.RemoveAt(i--);
                continue;
              }

              //If we can drag the object, we should check that whether there is a raycast or not!

              //3d Hit Check
              if (dragEventData.ColliderList != null && dragEventData.ColliderList.Length > 0)
              {
                if (dragEventData.IsInside && isHitOnLayer3D && Array.Exists(dragEventData.ColliderList, element => element.transform == hitTransform3D && element.gameObject.activeSelf))
                {
                  //Tapped inside the object
                  if (dragEventToFire == null)
                  {
                    dragEventToFire = dragEventData;
                    hitToFire3D = hit3D;
                    hitToFire2D = default(RaycastHit2D);
                    hitToFire2DEventSystem = default(RaycastResult);
#if ENABLE_DEBUGGING
                                        Log.Debug("TouchEventManager", "Inside - OneFingerDrag Event Found 3D. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform3D, dragEventData.DragCallback, dragEventData.SortingLayer, dragEventData.IsInside);
#endif
                  }
                  else
                  {
                    if (dragEventData.SortingLayer > dragEventToFire.SortingLayer ||
                        (dragEventToFire.SortingLayer == dragEventData.SortingLayer && !dragEventToFire.IsInside))
                    {
                      dragEventToFire = dragEventData;
                      hitToFire3D = hit3D;
                      hitToFire2D = default(RaycastHit2D);
                      hitToFire2DEventSystem = default(RaycastResult);

#if ENABLE_DEBUGGING
                                            Log.Debug("TouchEventManager", "Inside - OneFingerDrag Found 3D. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform3D, dragEventData.DragCallback, dragEventData.SortingLayer, dragEventData.IsInside);
#endif
                    }
                    else
                    {
                      //do nothing
                    }
                  }

                }
                else if (!dragEventData.IsInside && (!isHitOnLayer3D || !Array.Exists(dragEventData.ColliderList, element => element.transform == hitTransform3D && element.gameObject.activeSelf)))
                {
                  //Tapped outside the object
                  if (dragEventToFire == null)
                  {
                    dragEventToFire = dragEventData;
                    hitToFire3D = hit3D;
                    hitToFire2D = default(RaycastHit2D);
                    hitToFire2DEventSystem = default(RaycastResult);

#if ENABLE_DEBUGGING
                                        Log.Debug("TouchEventManager", "Outside - OneFingerDrag Event Found 3D - outside. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform3D, dragEventData.DragCallback, dragEventData.SortingLayer, dragEventData.IsInside);
#endif
                  }
                  else
                  {
                    if (dragEventData.SortingLayer > dragEventToFire.SortingLayer ||
                        (dragEventToFire.SortingLayer == dragEventData.SortingLayer && !dragEventToFire.IsInside))
                    {
                      dragEventToFire = dragEventData;
                      hitToFire3D = hit3D;
                      hitToFire2D = default(RaycastHit2D);
                      hitToFire2DEventSystem = default(RaycastResult);

#if ENABLE_DEBUGGING
                                            Log.Debug("TouchEventManager", "Outside - OneFingerDrag Event Found 3D - outside. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform3D, dragEventData.DragCallback, dragEventData.SortingLayer, dragEventData.IsInside);
#endif
                    }
                    else
                    {
                      //do nothing
                    }
                  }
                }
                else
                {
                  //do nothing
                }
              }

              //2d Hit Check
              if (dragEventData.ColliderList2D != null && dragEventData.ColliderList2D.Length > 0)
              {
                if (dragEventData.IsInside && isHitOnLayer2D && Array.Exists(dragEventData.ColliderList2D, element => element.transform == hitTransform2D && element.gameObject.activeSelf))
                {
                  //Tapped inside the object
                  if (dragEventToFire == null)
                  {
                    dragEventToFire = dragEventData;
                    hitToFire3D = default(RaycastHit);
                    hitToFire2D = hit2D;
                    hitToFire2DEventSystem = default(RaycastResult);

#if ENABLE_DEBUGGING
                                        Log.Debug("TouchEventManager", "Inside - OneFingerDrag Event Found 2D. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform2D, dragEventData.DragCallback, dragEventData.SortingLayer, dragEventData.IsInside);
#endif
                  }
                  else
                  {
                    if (dragEventData.SortingLayer > dragEventToFire.SortingLayer ||
                        (dragEventToFire.SortingLayer == dragEventData.SortingLayer && !dragEventToFire.IsInside))
                    {
                      dragEventToFire = dragEventData;
                      hitToFire3D = default(RaycastHit);
                      hitToFire2D = hit2D;
                      hitToFire2DEventSystem = default(RaycastResult);

#if ENABLE_DEBUGGING
                                            Log.Debug("TouchEventManager", "Inside - OneFingerDrag Event Found 2D. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform2D, dragEventData.DragCallback, dragEventData.SortingLayer, dragEventData.IsInside);
#endif
                    }
                    else
                    {
                      //do nothing
                    }
                  }

                }
                else if (!dragEventData.IsInside && (!isHitOnLayer2D || !Array.Exists(dragEventData.ColliderList2D, element => element.transform == hitTransform2D && element.gameObject.activeSelf)))
                {
                  //Tapped outside the object
                  if (dragEventToFire == null)
                  {
                    dragEventToFire = dragEventData;
                    hitToFire3D = default(RaycastHit);
                    hitToFire2D = hit2D;
                    hitToFire2DEventSystem = default(RaycastResult);

#if ENABLE_DEBUGGING
                                        Log.Debug("TouchEventManager", "Outside - OneFingerDrag Event Found 2D. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform2D, dragEventData.DragCallback, dragEventData.SortingLayer, dragEventData.IsInside);
#endif
                  }
                  else
                  {
                    if (dragEventData.SortingLayer > dragEventToFire.SortingLayer)
                    {
                      dragEventToFire = dragEventData;
                      hitToFire3D = default(RaycastHit);
                      hitToFire2D = hit2D;
                      hitToFire2DEventSystem = default(RaycastResult);

#if ENABLE_DEBUGGING
                                            Log.Debug("TouchEventManager", "Outside - OneFingerDrag Event Found 2D. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform2D, dragEventData.DragCallback, dragEventData.SortingLayer, dragEventData.IsInside);
#endif
                    }
                    else
                    {
                      //do nothing
                    }
                  }
                }
                else
                {
                  //do nothing
                }
              }

#if UNITY_4_6 || UNITY_5 || UNITY_5_3_OR_NEWER
              //2D UI Hit Check using EventSystem
              if (dragEventData.RectTransformList != null && dragEventData.RectTransformList.Length > 0)
              {
                if (dragEventData.IsInside && isHitOnLayer2DEventSystem && Array.Exists(dragEventData.RectTransformList, element => element.transform == hitTransform2DEventSystem && element.gameObject.activeSelf))
                {
                  //Tapped inside the object
                  if (dragEventToFire == null)
                  {
                    dragEventToFire = dragEventData;
                    hitToFire3D = default(RaycastHit);
                    hitToFire2D = default(RaycastHit2D);
                    hitToFire2DEventSystem = hit2DEventSystem;

#if ENABLE_DEBUGGING
                                        Log.Debug("TouchEventManager", "Inside - OneFingerDrag Event Found 2D Event System. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform2DEventSystem, dragEventData.DragCallback, dragEventData.SortingLayer, dragEventData.IsInside);
#endif
                  }
                  else
                  {
                    if (dragEventData.SortingLayer > dragEventToFire.SortingLayer ||
                        (dragEventToFire.SortingLayer == dragEventData.SortingLayer && !dragEventToFire.IsInside))
                    {
                      dragEventToFire = dragEventData;
                      hitToFire3D = default(RaycastHit);
                      hitToFire2D = default(RaycastHit2D);
                      hitToFire2DEventSystem = hit2DEventSystem;

#if ENABLE_DEBUGGING
                                            Log.Debug("TouchEventManager", "Inside - OneFingerDrag Event Found 2D Event System. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform2DEventSystem, dragEventData.DragCallback, dragEventData.SortingLayer, dragEventData.IsInside);
#endif
                    }
                    else
                    {
                      //do nothing
                    }
                  }

                }
                else if (!dragEventData.IsInside && (!isHitOnLayer2DEventSystem || !Array.Exists(dragEventData.RectTransformList, element => element.transform == hitTransform2DEventSystem && element.gameObject.activeSelf)))
                {
                  //Tapped outside the object
                  if (dragEventToFire == null)
                  {
                    dragEventToFire = dragEventData;
                    hitToFire3D = default(RaycastHit);
                    hitToFire2D = default(RaycastHit2D);
                    hitToFire2DEventSystem = hit2DEventSystem;

#if ENABLE_DEBUGGING
                                        Log.Debug("TouchEventManager", "Outside - OneFingerDrag Event Found 2D Event System. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform2DEventSystem, dragEventData.DragCallback, dragEventData.SortingLayer, dragEventData.IsInside);
#endif
                  }
                  else
                  {
                    if (dragEventData.SortingLayer > dragEventToFire.SortingLayer)
                    {
                      dragEventToFire = dragEventData;
                      hitToFire3D = default(RaycastHit);
                      hitToFire2D = default(RaycastHit2D);
                      hitToFire2DEventSystem = hit2DEventSystem;

#if ENABLE_DEBUGGING
                                            Log.Debug("TouchEventManager", "Outside - OneFingerDrag Event Found 2D Event System. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform2DEventSystem, dragEventData.DragCallback, dragEventData.SortingLayer, dragEventData.IsInside);
#endif
                    }
                    else
                    {
                      //do nothing
                    }
                  }
                }
                else
                {
                  //do nothing
                }
              }
#endif
            }
          }
        }

        if (dragEventToFire != null)
          EventManager.Instance.SendEvent(dragEventToFire.DragCallback, m_OneFingerMoveGesture, hitToFire3D, hitToFire2D, hitToFire2DEventSystem);

        EventManager.Instance.SendEvent("OnDragOneFingerFullscreen", m_OneFingerMoveGesture);
      }
    }

    private void TwoFingerTransformedHandler(object sender, System.EventArgs e)
    {
      RaycastHit hitToFire3D = default(RaycastHit);
      RaycastHit2D hitToFire2D = default(RaycastHit2D);
#if UNITY_4_6 || UNITY_5 || UNITY_5_3_OR_NEWER
      RaycastResult hitToFire2DEventSystem = default(RaycastResult);
#endif

      //Log.Status ("TouchEventManager", "TwoFingerTransformedHandler: {0}", m_TwoFingerMoveGesture.DeltaPosition);
      if (m_Active)
      {
        TouchEventData dragEventToFire = null;
        Vector3 twoFingerScreenPosition = m_TwoFingerMoveGesture.ScreenPosition;
        Ray rayForDrag = MainCamera.ScreenPointToRay(twoFingerScreenPosition);


        foreach (var kp in m_DragEvents)
        {
          if (kp.Key == 2)
          {
            //Adding Variables for 3D Touch Check
            Transform hitTransform3D = null;
            RaycastHit hit3D = default(RaycastHit);
            bool isHitOnLayer3D = Physics.Raycast(rayForDrag, out hit3D, Mathf.Infinity, kp.Key);
            if (isHitOnLayer3D)
            {
              hitTransform3D = hit3D.collider.transform;
            }

            //Adding Variables for 2D Touch Check for 2d Colliders
            Transform hitTransform2D = null;
            RaycastHit2D hit2D = Physics2D.Raycast(rayForDrag.origin, rayForDrag.direction, Mathf.Infinity, kp.Key);
            bool isHitOnLayer2D = false;
            if (hit2D.collider != null)
            {
              isHitOnLayer2D = true;
              hitTransform2D = hit2D.collider.transform;
            }

#if UNITY_4_6 || UNITY_5 || UNITY_5_3_OR_NEWER
            //Adding Variables for Event.System Touch for UI Elements
            Transform hitTransform2DEventSystem = null;
            bool isHitOnLayer2DEventSystem = false;
            RaycastResult hit2DEventSystem = default(RaycastResult);
            if (EventSystem.current != null)
            {
              PointerEventData pointerEventForTap = new PointerEventData(EventSystem.current);
              pointerEventForTap.position = twoFingerScreenPosition;
              List<RaycastResult> raycastResultListFor2DEventSystem = new List<RaycastResult>();
              EventSystem.current.RaycastAll(pointerEventForTap, raycastResultListFor2DEventSystem);
              foreach (RaycastResult itemRaycastResult in raycastResultListFor2DEventSystem)
              {

                isHitOnLayer2DEventSystem = kp.Value.Exists(element => (element.GameObjectAttached != null && element.GameObjectAttached.layer == itemRaycastResult.gameObject.layer));
                if (isHitOnLayer2DEventSystem)
                {
                  hit2DEventSystem = itemRaycastResult;
                  hitTransform2DEventSystem = itemRaycastResult.gameObject.transform;
                  break;
                }
              }
            }
#endif

            for (int i = 0; i < kp.Value.Count; ++i)
            {
              TouchEventData dragEventData = kp.Value[i];

              if (kp.Value[i].ColliderList == null && kp.Value[i].ColliderList2D == null && kp.Value[i].RectTransformList == null)
              {
                Log.Warning("TouchEventManager", "Removing invalid collider event receiver from TwoFingerDrag");
                kp.Value.RemoveAt(i--);
                continue;
              }

              if (string.IsNullOrEmpty(dragEventData.DragCallback))
              {
                Log.Warning("TouchEventManager", "Removing invalid event receiver from TwoFingerDrag");
                kp.Value.RemoveAt(i--);
                continue;
              }

              //If we can drag the object, we should check that whether there is a raycast or not!

              //3d Hit Check
              if (dragEventData.ColliderList != null && dragEventData.ColliderList.Length > 0)
              {
                if (dragEventData.IsInside && isHitOnLayer3D && Array.Exists(dragEventData.ColliderList, element => element.transform == hitTransform3D && element.gameObject.activeSelf))
                {
                  //Tapped inside the object
                  if (dragEventToFire == null)
                  {
                    dragEventToFire = dragEventData;
                    hitToFire3D = hit3D;
                    hitToFire2D = default(RaycastHit2D);
                    hitToFire2DEventSystem = default(RaycastResult);
#if ENABLE_DEBUGGING
                                        Log.Debug("TouchEventManager", "Inside - TwoFingerDrag Event Found 3D. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform3D, dragEventData.DragCallback, dragEventData.SortingLayer, dragEventData.IsInside);
#endif
                  }
                  else
                  {
                    if (dragEventData.SortingLayer > dragEventToFire.SortingLayer ||
                        (dragEventToFire.SortingLayer == dragEventData.SortingLayer && !dragEventToFire.IsInside))
                    {
                      dragEventToFire = dragEventData;
                      hitToFire3D = hit3D;
                      hitToFire2D = default(RaycastHit2D);
                      hitToFire2DEventSystem = default(RaycastResult);

#if ENABLE_DEBUGGING
                                            Log.Debug("TouchEventManager", "Inside - TwoFingerDrag Found 3D. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform3D, dragEventData.DragCallback, dragEventData.SortingLayer, dragEventData.IsInside);
#endif
                    }
                    else
                    {
                      //do nothing
                    }
                  }

                }
                else if (!dragEventData.IsInside && (!isHitOnLayer3D || !Array.Exists(dragEventData.ColliderList, element => element.transform == hitTransform3D && element.gameObject.activeSelf)))
                {
                  //Tapped outside the object
                  if (dragEventToFire == null)
                  {
                    dragEventToFire = dragEventData;
                    hitToFire3D = hit3D;
                    hitToFire2D = default(RaycastHit2D);
                    hitToFire2DEventSystem = default(RaycastResult);

#if ENABLE_DEBUGGING
                                        Log.Debug("TouchEventManager", "Outside - TwoFingerDrag Event Found 3D - outside. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform3D, dragEventData.DragCallback, dragEventData.SortingLayer, dragEventData.IsInside);
#endif
                  }
                  else
                  {
                    if (dragEventData.SortingLayer > dragEventToFire.SortingLayer ||
                        (dragEventToFire.SortingLayer == dragEventData.SortingLayer && !dragEventToFire.IsInside))
                    {
                      dragEventToFire = dragEventData;
                      hitToFire3D = hit3D;
                      hitToFire2D = default(RaycastHit2D);
                      hitToFire2DEventSystem = default(RaycastResult);

#if ENABLE_DEBUGGING
                                            Log.Debug("TouchEventManager", "Outside - TwoFingerDrag Event Found 3D - outside. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform3D, dragEventData.DragCallback, dragEventData.SortingLayer, dragEventData.IsInside);
#endif
                    }
                    else
                    {
                      //do nothing
                    }
                  }
                }
                else
                {
                  //do nothing
                }
              }

              //2d Hit Check
              if (dragEventData.ColliderList2D != null && dragEventData.ColliderList2D.Length > 0)
              {
                if (dragEventData.IsInside && isHitOnLayer2D && Array.Exists(dragEventData.ColliderList2D, element => element.transform == hitTransform2D && element.gameObject.activeSelf))
                {
                  //Tapped inside the object
                  if (dragEventToFire == null)
                  {
                    dragEventToFire = dragEventData;
                    hitToFire3D = default(RaycastHit);
                    hitToFire2D = hit2D;
                    hitToFire2DEventSystem = default(RaycastResult);

#if ENABLE_DEBUGGING
                                        Log.Debug("TouchEventManager", "Inside - TwoFingerDrag Event Found 2D. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform2D, dragEventData.DragCallback, dragEventData.SortingLayer, dragEventData.IsInside);
#endif
                  }
                  else
                  {
                    if (dragEventData.SortingLayer > dragEventToFire.SortingLayer ||
                        (dragEventToFire.SortingLayer == dragEventData.SortingLayer && !dragEventToFire.IsInside))
                    {
                      dragEventToFire = dragEventData;
                      hitToFire3D = default(RaycastHit);
                      hitToFire2D = hit2D;
                      hitToFire2DEventSystem = default(RaycastResult);

#if ENABLE_DEBUGGING
                                            Log.Debug("TouchEventManager", "Inside - TwoFingerDrag Event Found 2D. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform2D, dragEventData.DragCallback, dragEventData.SortingLayer, dragEventData.IsInside);
#endif
                    }
                    else
                    {
                      //do nothing
                    }
                  }

                }
                else if (!dragEventData.IsInside && (!isHitOnLayer2D || !Array.Exists(dragEventData.ColliderList2D, element => element.transform == hitTransform2D && element.gameObject.activeSelf)))
                {
                  //Tapped outside the object
                  if (dragEventToFire == null)
                  {
                    dragEventToFire = dragEventData;
                    hitToFire3D = default(RaycastHit);
                    hitToFire2D = hit2D;
                    hitToFire2DEventSystem = default(RaycastResult);

#if ENABLE_DEBUGGING
                                        Log.Debug("TouchEventManager", "Outside - TwoFingerDrag Event Found 2D. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform2D, dragEventData.DragCallback, dragEventData.SortingLayer, dragEventData.IsInside);
#endif
                  }
                  else
                  {
                    if (dragEventData.SortingLayer > dragEventToFire.SortingLayer)
                    {
                      dragEventToFire = dragEventData;
                      hitToFire3D = default(RaycastHit);
                      hitToFire2D = hit2D;
                      hitToFire2DEventSystem = default(RaycastResult);

#if ENABLE_DEBUGGING
                                            Log.Debug("TouchEventManager", "Outside - TwoFingerDrag Event Found 2D. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform2D, dragEventData.DragCallback, dragEventData.SortingLayer, dragEventData.IsInside);
#endif
                    }
                    else
                    {
                      //do nothing
                    }
                  }
                }
                else
                {
                  //do nothing
                }
              }

#if UNITY_4_6 || UNITY_5 || UNITY_5_3_OR_NEWER
              //2D UI Hit Check using EventSystem
              if (dragEventData.RectTransformList != null && dragEventData.RectTransformList.Length > 0)
              {
                if (dragEventData.IsInside && isHitOnLayer2DEventSystem && Array.Exists(dragEventData.RectTransformList, element => element.transform == hitTransform2DEventSystem && element.gameObject.activeSelf))
                {
                  //Tapped inside the object
                  if (dragEventToFire == null)
                  {
                    dragEventToFire = dragEventData;
                    hitToFire3D = default(RaycastHit);
                    hitToFire2D = default(RaycastHit2D);
                    hitToFire2DEventSystem = hit2DEventSystem;

#if ENABLE_DEBUGGING
                                        Log.Debug("TouchEventManager", "Inside - TwoFingerDrag Event Found 2D Event System. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform2DEventSystem, dragEventData.DragCallback, dragEventData.SortingLayer, dragEventData.IsInside);
#endif
                  }
                  else
                  {
                    if (dragEventData.SortingLayer > dragEventToFire.SortingLayer ||
                        (dragEventToFire.SortingLayer == dragEventData.SortingLayer && !dragEventToFire.IsInside))
                    {
                      dragEventToFire = dragEventData;
                      hitToFire3D = default(RaycastHit);
                      hitToFire2D = default(RaycastHit2D);
                      hitToFire2DEventSystem = hit2DEventSystem;

#if ENABLE_DEBUGGING
                                            Log.Debug("TouchEventManager", "Inside - TwoFingerDrag Event Found 2D Event System. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform2DEventSystem, dragEventData.DragCallback, dragEventData.SortingLayer, dragEventData.IsInside);
#endif
                    }
                    else
                    {
                      //do nothing
                    }
                  }

                }
                else if (!dragEventData.IsInside && (!isHitOnLayer2DEventSystem || !Array.Exists(dragEventData.RectTransformList, element => element.transform == hitTransform2DEventSystem && element.gameObject.activeSelf)))
                {
                  //Tapped outside the object
                  if (dragEventToFire == null)
                  {
                    dragEventToFire = dragEventData;
                    hitToFire3D = default(RaycastHit);
                    hitToFire2D = default(RaycastHit2D);
                    hitToFire2DEventSystem = hit2DEventSystem;

#if ENABLE_DEBUGGING
                                        Log.Debug("TouchEventManager", "Outside - TwoFingerDrag Event Found 2D Event System. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform2DEventSystem, dragEventData.DragCallback, dragEventData.SortingLayer, dragEventData.IsInside);
#endif
                  }
                  else
                  {
                    if (dragEventData.SortingLayer > dragEventToFire.SortingLayer)
                    {
                      dragEventToFire = dragEventData;
                      hitToFire3D = default(RaycastHit);
                      hitToFire2D = default(RaycastHit2D);
                      hitToFire2DEventSystem = hit2DEventSystem;

#if ENABLE_DEBUGGING
                                            Log.Debug("TouchEventManager", "Outside - TwoFingerDrag Event Found 2D Event System. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform2DEventSystem, dragEventData.DragCallback, dragEventData.SortingLayer, dragEventData.IsInside);
#endif
                    }
                    else
                    {
                      //do nothing
                    }
                  }
                }
                else
                {
                  //do nothing
                }
              }
#endif
            }
          }
        }

        if (dragEventToFire != null)
          EventManager.Instance.SendEvent(dragEventToFire.DragCallback, m_TwoFingerMoveGesture, hitToFire3D, hitToFire2D, hitToFire2DEventSystem);

        EventManager.Instance.SendEvent("OnDragTwoFingerFullscreen", m_TwoFingerMoveGesture);
      }
    }
    #endregion

    #region TapEvents - Register / UnRegister / Call 
    /// <summary>
    /// Registers the tap event. 
    /// 3d Colliders is the first priority
    /// 2d Collider is the second priority 
    /// 2d UI Event System is the third priority
    /// If object has 3d collider, and 2d collider and an UI element. 
    /// Touch is checking by priority order. 
    /// </summary>
    /// <returns><c>true</c>, if tap event was registered, <c>false</c> otherwise.</returns>
    /// <param name="gameObjectToTouch">Game object to touch.</param>
    /// <param name="callback">Callback.</param>
    /// <param name="SortingLayer">Sorting layer.</param>
    /// <param name="isTapInside">If set to <c>true</c> is tap inside.</param>
    /// <param name="layerMask">Layer mask.</param>
    public bool RegisterTapEvent(GameObject gameObjectToTouch, string callback, int SortingLayer = 0, bool isTapInside = true, LayerMask layerMask = default(LayerMask))
    {
      bool success = false;

      if (gameObjectToTouch != null)
      {
        if (!string.IsNullOrEmpty(callback))
        {
          Collider[] colliderList = gameObjectToTouch.GetComponentsInChildren<Collider>();

          if (colliderList != null && colliderList.Length > 0)
          {
            foreach (Collider itemCollider in colliderList)
            {
              int layerMaskAsKey = (layerMask != default(LayerMask)) ? layerMask.value : (1 << gameObjectToTouch.layer);

#if ENABLE_DEBUGGING
                            Log.Debug("TouchEventManager", "RegisterTapEvent for 3D. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3}",itemCollider, callback, SortingLayer, isTapInside);
#endif

              if (m_TapEvents.ContainsKey(layerMaskAsKey))
              {
                m_TapEvents[layerMaskAsKey].Add(new TouchEventData(itemCollider, callback, SortingLayer, isTapInside));
              }
              else
              {
                m_TapEvents[layerMaskAsKey] = new List<TouchEventData>() { new TouchEventData(itemCollider, callback, SortingLayer, isTapInside) };
              }
            }

            success = true;
          }
          else
          {
#if ENABLE_DEBUGGING
                        Log.Debug("TouchEventManager", "There is no 3D collider of given gameobjectToTouch");
#endif
          }

          if (!success)
          {
            Collider2D[] colliderList2D = gameObjectToTouch.GetComponentsInChildren<Collider2D>();
            if (colliderList2D != null && colliderList2D.Length > 0)
            {
              foreach (Collider2D itemCollider in colliderList2D)
              {
                int layerMaskAsKey = (layerMask != default(LayerMask)) ? layerMask.value : (1 << gameObjectToTouch.layer);

#if ENABLE_DEBUGGING
                                Log.Debug("TouchEventManager", "RegisterTapEvent For 2D. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3}",itemCollider, callback, SortingLayer, isTapInside);
#endif

                if (m_TapEvents.ContainsKey(layerMaskAsKey))
                {
                  m_TapEvents[layerMaskAsKey].Add(new TouchEventData(itemCollider, callback, SortingLayer, isTapInside));
                }
                else
                {
                  m_TapEvents[layerMaskAsKey] = new List<TouchEventData>() { new TouchEventData(itemCollider, callback, SortingLayer, isTapInside) };
                }
              }

              success = true;
            }
            else
            {
#if ENABLE_DEBUGGING
                            Log.Debug ("TouchEventManager", "There is no 2D collider of given gameobjectToTouch");
#endif
            }
          }
#if UNITY_4_6 || UNITY_5 || UNITY_5_3_OR_NEWER
          if (!success)
          {
            RectTransform[] rectTransformList = gameObjectToTouch.GetComponentsInChildren<RectTransform>(includeInactive: true);
            if (rectTransformList != null && rectTransformList.Length > 0)
            {
              foreach (RectTransform itemRectTransform in rectTransformList)
              {
                int layerMaskAsKey = (layerMask != default(LayerMask)) ? layerMask.value : (1 << itemRectTransform.gameObject.layer);

#if ENABLE_DEBUGGING
                                Log.Debug("TouchEventManager", "RegisterTapEvent For 2D Event System. itemRectTransform: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3}",itemRectTransform, callback, SortingLayer, isTapInside);
#endif

                if (m_TapEvents.ContainsKey(layerMaskAsKey))
                {
                  m_TapEvents[layerMaskAsKey].Add(new TouchEventData(itemRectTransform, callback, SortingLayer, isTapInside));
                }
                else
                {
                  m_TapEvents[layerMaskAsKey] = new List<TouchEventData>() { new TouchEventData(itemRectTransform, callback, SortingLayer, isTapInside) };
                }
              }

              success = true;
            }
            else
            {
#if ENABLE_DEBUGGING
                            Log.Debug ("TouchEventManager", "There is no Rect Transform of given gameobjectToTouch");
#endif
            }
          }
#endif

        }
        else
        {
          Log.Warning("TouchEventManager", "There is no callback for tap event registration");
        }
      }
      else
      {
        Log.Warning("TouchEventManager", "There is no gameobject for tap event registration");
      }

      return success;
    }

    /// <summary>
    /// Unregisters the tap event.
    /// </summary>
    /// <returns><c>true</c>, if tap event was unregistered, <c>false</c> otherwise.</returns>
    /// <param name="gameObjectToTouch">Game object to touch.</param>
    /// <param name="callback">Callback.</param>
    /// <param name="SortingLayer">Sorting layer.</param>
    /// <param name="isTapInside">If set to <c>true</c> is tap inside.</param>
    /// <param name="layerMask">Layer mask.</param>
    public bool UnregisterTapEvent(GameObject gameObjectToTouch, string callback, int SortingLayer = 0, bool isTapInside = true, LayerMask layerMask = default(LayerMask))
    {
      bool success = false;

      if (gameObjectToTouch != null)
      {
        if (!string.IsNullOrEmpty(callback))
        {
          int layerMaskAsKey = (layerMask != default(LayerMask)) ? layerMask.value : (1 << gameObjectToTouch.layer);

          if (m_TapEvents.ContainsKey(layerMaskAsKey))
          {
            success = true;
            Collider[] colliderList = gameObjectToTouch.GetComponentsInChildren<Collider>(includeInactive: true);
            if (colliderList != null && colliderList.Length > 0)
            {
              foreach (Collider itemCollider in colliderList)
              {
                int numberOfRemovedCallbacks = m_TapEvents[layerMaskAsKey].RemoveAll(
                                                   e =>
                    e.Collider == itemCollider &&
                                                   e.TapCallback == callback &&
                                                   e.SortingLayer == SortingLayer &&
                                                   e.IsInside == isTapInside);

                success &= (numberOfRemovedCallbacks > 0);
              }
            }
            else
            {
              success = false;
            }

            if (!success)
            {
              success = true;
              Collider2D[] colliderList2D = gameObjectToTouch.GetComponentsInChildren<Collider2D>(includeInactive: true);
              if (colliderList2D != null && colliderList2D.Length > 0)
              {
                foreach (Collider2D itemCollider2D in colliderList2D)
                {
                  int numberOfRemovedCallbacks = m_TapEvents[layerMaskAsKey].RemoveAll(
                      e =>
                      e.Collider2D == itemCollider2D &&
                      e.TapCallback == callback &&
                      e.SortingLayer == SortingLayer &&
                      e.IsInside == isTapInside);

                  success &= (numberOfRemovedCallbacks > 0);
                }
              }
              else
              {
                success = false;
              }
            }


#if UNITY_4_6 || UNITY_5 || UNITY_5_3_OR_NEWER
            if (!success)
            {
              success = true;
              RectTransform[] rectTransformList = gameObjectToTouch.GetComponentsInChildren<RectTransform>(includeInactive: true);
              if (rectTransformList != null && rectTransformList.Length > 0)
              {
                foreach (RectTransform itemRectTransform in rectTransformList)
                {
                  int numberOfRemovedCallbacks = m_TapEvents[layerMaskAsKey].RemoveAll(
                      e =>
                      e.RectTransform == itemRectTransform &&
                      e.TapCallback == callback &&
                      e.SortingLayer == SortingLayer &&
                      e.IsInside == isTapInside);

                  success &= (numberOfRemovedCallbacks > 0);
                }
              }
              else
              {
                success = false;
              }
            }
#endif
          }
        }
        else
        {
          Log.Warning("TouchEventManager", "There is no callback for tap event unregistration");
        }
      }
      else
      {
        Log.Warning("TouchEventManager", "There is no gameobject for tap event unregistration");
      }

      return success;
    }

    private void TapGesture_Tapped(object sender, System.EventArgs e)
    {
      if (m_Active)
      {
#if ENABLE_DEBUGGING
                Log.Debug("TouchEventManager", "TapGesture_Tapped: {0} - {1}", m_TapGesture.ScreenPosition, m_TapGesture.NumTouches);
#endif

        TouchEventData tapEventToFire = null;

        RaycastHit hitToFire3D = default(RaycastHit);
        RaycastHit2D hitToFire2D = default(RaycastHit2D);
#if UNITY_4_6 || UNITY_5 || UNITY_5_3_OR_NEWER
        RaycastResult hitToFire2DEventSystem = default(RaycastResult);
#endif

        foreach (var kp in m_TapEvents)
        {
          //Adding Variables for 3D Tap Check
          Ray rayForTab = MainCamera.ScreenPointToRay(m_TapGesture.ScreenPosition);
          Transform hitTransform3D = null;
          RaycastHit hit3D = default(RaycastHit);
          bool isHitOnLayer3D = Physics.Raycast(rayForTab, out hit3D, Mathf.Infinity, kp.Key);
          if (isHitOnLayer3D)
          {
            hitTransform3D = hit3D.collider.transform;
          }

          //Adding Variables for 2D Tap Check for 2d Colliders
          Transform hitTransform2D = null;
          RaycastHit2D hit2D = Physics2D.Raycast(rayForTab.origin, rayForTab.direction, Mathf.Infinity, kp.Key);
          bool isHitOnLayer2D = false;
          if (hit2D.collider != null)
          {
            isHitOnLayer2D = true;
            hitTransform2D = hit2D.collider.transform;
          }

#if UNITY_4_6 || UNITY_5 || UNITY_5_3_OR_NEWER
          //Adding Variables for Event.System Tap for UI Elements
          Transform hitTransform2DEventSystem = null;
          bool isHitOnLayer2DEventSystem = false;
          RaycastResult hit2DEventSystem = default(RaycastResult);
          if (EventSystem.current != null)
          {
            PointerEventData pointerEventForTap = new PointerEventData(EventSystem.current);
            pointerEventForTap.position = m_TapGesture.ScreenPosition;
            List<RaycastResult> raycastResultListFor2DEventSystem = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventForTap, raycastResultListFor2DEventSystem);
            foreach (RaycastResult itemRaycastResult in raycastResultListFor2DEventSystem)
            {
              LayerMask layerMaskOfItem = 1 << itemRaycastResult.gameObject.layer;
              isHitOnLayer2DEventSystem = ((layerMaskOfItem.value & kp.Key) == layerMaskOfItem.value);

              if (isHitOnLayer2DEventSystem)
              {
                hit2DEventSystem = itemRaycastResult;
                hitTransform2DEventSystem = itemRaycastResult.gameObject.transform;
                break;
              }
            }
          }
#endif


          for (int i = 0; i < kp.Value.Count; ++i)
          {
            TouchEventData tapEventData = kp.Value[i];

            if (kp.Value[i].Collider == null && kp.Value[i].Collider2D == null && kp.Value[i].RectTransform == null && kp.Value[i].RectTransformList == null)
            {
              Log.Warning("TouchEventManager", "Removing invalid collider event receiver from TapEventList from {0}", kp.Value[i].ToString());
              kp.Value.RemoveAt(i--);
              continue;
            }

            if (string.IsNullOrEmpty(tapEventData.TapCallback))
            {
              Log.Warning("TouchEventManager", "Removing invalid event receiver from TapEventList {0}", kp.Value[i]);
              kp.Value.RemoveAt(i--);
              continue;
            }

            //3d Hit Check
            if (tapEventData.Collider != null)
            {
              if (isHitOnLayer3D && hitTransform3D == tapEventData.Collider.transform && tapEventData.IsInside)
              {
                //Tapped inside the object
                if (tapEventToFire == null)
                {
                  tapEventToFire = tapEventData;
                  hitToFire3D = hit3D;
                  hitToFire2D = default(RaycastHit2D);
                  hitToFire2DEventSystem = default(RaycastResult);
#if ENABLE_DEBUGGING
                                    Log.Debug("TouchEventManager", "Tap Event Found 3D. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform3D, tapEventData.TapCallback, tapEventData.SortingLayer, tapEventData.IsInside);
#endif
                }
                else
                {
                  if (tapEventData.SortingLayer > tapEventToFire.SortingLayer ||
                      (tapEventToFire.SortingLayer == tapEventData.SortingLayer && !tapEventToFire.IsInside))
                  {
                    tapEventToFire = tapEventData;
                    hitToFire3D = hit3D;
                    hitToFire2D = default(RaycastHit2D);
                    hitToFire2DEventSystem = default(RaycastResult);

#if ENABLE_DEBUGGING
                                        Log.Debug("TouchEventManager", "Tap Event Found 3D. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform3D, tapEventData.TapCallback, tapEventData.SortingLayer, tapEventData.IsInside);
#endif
                  }
                  else
                  {
                    //do nothing
                  }
                }

              }
              else if ((!isHitOnLayer3D || hitTransform3D != tapEventData.Collider.transform) && !tapEventData.IsInside)
              {
                //Tapped outside the object
                if (tapEventToFire == null)
                {
                  tapEventToFire = tapEventData;
                  hitToFire3D = hit3D;
                  hitToFire2D = default(RaycastHit2D);
                  hitToFire2DEventSystem = default(RaycastResult);

#if ENABLE_DEBUGGING
                                    Log.Debug("TouchEventManager", "Tap Event Found 3D. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform3D, tapEventData.TapCallback, tapEventData.SortingLayer, tapEventData.IsInside);
#endif
                }
                else
                {
                  if (tapEventData.SortingLayer > tapEventToFire.SortingLayer)
                  {
                    tapEventToFire = tapEventData;
                    hitToFire3D = hit3D;
                    hitToFire2D = default(RaycastHit2D);
                    hitToFire2DEventSystem = default(RaycastResult);

#if ENABLE_DEBUGGING
                                        Log.Debug("TouchEventManager", "Tap Event Found 3D. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform3D, tapEventData.TapCallback, tapEventData.SortingLayer, tapEventData.IsInside);
#endif
                  }
                  else
                  {
                    //do nothing
                  }
                }
              }
              else
              {
                //do nothing
              }
            }

            //2d Hit Check
            if (tapEventData.Collider2D != null)
            {
              if (isHitOnLayer2D && hitTransform2D == tapEventData.Collider2D.transform && tapEventData.IsInside)
              {
                //Tapped inside the object
                if (tapEventToFire == null)
                {
                  tapEventToFire = tapEventData;
                  hitToFire3D = default(RaycastHit);
                  hitToFire2D = hit2D;
                  hitToFire2DEventSystem = default(RaycastResult);

#if ENABLE_DEBUGGING
                                    Log.Debug("TouchEventManager", "Tap Event Found 2D. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform2D, tapEventData.TapCallback, tapEventData.SortingLayer, tapEventData.IsInside);
#endif
                }
                else
                {
                  if (tapEventData.SortingLayer > tapEventToFire.SortingLayer ||
                      (tapEventToFire.SortingLayer == tapEventData.SortingLayer && !tapEventToFire.IsInside))
                  {
                    tapEventToFire = tapEventData;
                    hitToFire3D = default(RaycastHit);
                    hitToFire2D = hit2D;
                    hitToFire2DEventSystem = default(RaycastResult);

#if ENABLE_DEBUGGING
                                        Log.Debug("TouchEventManager", "Tap Event Found 2D. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform2D, tapEventData.TapCallback, tapEventData.SortingLayer, tapEventData.IsInside);
#endif
                  }
                  else
                  {
                    //do nothing
                  }
                }

              }
              else if ((!isHitOnLayer2D || hitTransform2D != tapEventData.Collider2D.transform) && !tapEventData.IsInside)
              {
                //Tapped outside the object
                if (tapEventToFire == null)
                {
                  tapEventToFire = tapEventData;
                  hitToFire3D = default(RaycastHit);
                  hitToFire2D = hit2D;
                  hitToFire2DEventSystem = default(RaycastResult);

#if ENABLE_DEBUGGING
                                    Log.Debug("TouchEventManager", "Tap Event Found 2D. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform2D, tapEventData.TapCallback, tapEventData.SortingLayer, tapEventData.IsInside);
#endif
                }
                else
                {
                  if (tapEventData.SortingLayer > tapEventToFire.SortingLayer)
                  {
                    tapEventToFire = tapEventData;
                    hitToFire3D = default(RaycastHit);
                    hitToFire2D = hit2D;
                    hitToFire2DEventSystem = default(RaycastResult);

#if ENABLE_DEBUGGING
                                        Log.Debug("TouchEventManager", "Tap Event Found 2D. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform2D, tapEventData.TapCallback, tapEventData.SortingLayer, tapEventData.IsInside);
#endif
                  }
                  else
                  {
                    //do nothing
                  }
                }
              }
              else
              {
                //do nothing
              }
            }

#if UNITY_4_6 || UNITY_5 || UNITY_5_3_OR_NEWER
            //2D UI Hit Check using EventSystem
            if (tapEventData.RectTransform != null)
            {
              if (isHitOnLayer2DEventSystem && hitTransform2DEventSystem == tapEventData.RectTransform.transform && tapEventData.IsInside)
              {
                //Tapped inside the object
                if (tapEventToFire == null)
                {
                  tapEventToFire = tapEventData;
                  hitToFire3D = default(RaycastHit);
                  hitToFire2D = default(RaycastHit2D);
                  hitToFire2DEventSystem = hit2DEventSystem;

#if ENABLE_DEBUGGING
                                    Log.Debug("TouchEventManager", "Tap Event Found 2D Event System. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform2DEventSystem, tapEventData.TapCallback, tapEventData.SortingLayer, tapEventData.IsInside);
#endif
                }
                else
                {
                  if (tapEventData.SortingLayer > tapEventToFire.SortingLayer ||
                      (tapEventToFire.SortingLayer == tapEventData.SortingLayer && !tapEventToFire.IsInside))
                  {
                    tapEventToFire = tapEventData;
                    hitToFire3D = default(RaycastHit);
                    hitToFire2D = default(RaycastHit2D);
                    hitToFire2DEventSystem = hit2DEventSystem;

#if ENABLE_DEBUGGING
                                        Log.Debug("TouchEventManager", "Tap Event Found 2D Event System. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform2DEventSystem, tapEventData.TapCallback, tapEventData.SortingLayer, tapEventData.IsInside);
#endif
                  }
                  else
                  {
                    //do nothing
                  }
                }

              }
              else if ((!isHitOnLayer2DEventSystem || hitTransform2DEventSystem != tapEventData.RectTransform.transform) && !tapEventData.IsInside)
              {
                //Tapped outside the object
                if (tapEventToFire == null)
                {
                  tapEventToFire = tapEventData;
                  hitToFire3D = default(RaycastHit);
                  hitToFire2D = default(RaycastHit2D);
                  hitToFire2DEventSystem = hit2DEventSystem;

#if ENABLE_DEBUGGING
                                    Log.Debug("TouchEventManager", "Tap Event Found 2D Event System. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform2DEventSystem, tapEventData.TapCallback, tapEventData.SortingLayer, tapEventData.IsInside);
#endif
                }
                else
                {
                  if (tapEventData.SortingLayer > tapEventToFire.SortingLayer)
                  {
                    tapEventToFire = tapEventData;
                    hitToFire3D = default(RaycastHit);
                    hitToFire2D = default(RaycastHit2D);
                    hitToFire2DEventSystem = hit2DEventSystem;

#if ENABLE_DEBUGGING
                                        Log.Debug("TouchEventManager", "Tap Event Found 2D Event System. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform2DEventSystem, tapEventData.TapCallback, tapEventData.SortingLayer, tapEventData.IsInside);
#endif
                  }
                  else
                  {
                    //do nothing
                  }
                }
              }
              else
              {
                //do nothing
              }
            }
#endif

          }
        }

        if (tapEventToFire != null)
          EventManager.Instance.SendEvent(tapEventToFire.TapCallback, m_TapGesture, hitToFire3D, hitToFire2D, hitToFire2DEventSystem);

        EventManager.Instance.SendEvent("OnSingleTap", m_TapGesture);
      }
    }
    #endregion

    #region Double TapEvents - Register / UnRegister / Call 
    /// <summary>
    /// Registers the double tap event.
    /// </summary>
    /// <returns><c>true</c>, if double tap event was registered, <c>false</c> otherwise.</returns>
    /// <param name="gameObjectToTouch">Game object to touch.</param>
    /// <param name="callback">Callback.</param>
    /// <param name="SortingLayer">Sorting layer.</param>
    /// <param name="isDoubleTapInside">If set to <c>true</c> is double tap inside.</param>
    /// <param name="layerMask">Layer mask.</param>
    public bool RegisterDoubleTapEvent(GameObject gameObjectToTouch, string callback, int SortingLayer = 0, bool isDoubleTapInside = true, LayerMask layerMask = default(LayerMask))
    {
      bool success = false;

      if (gameObjectToTouch != null)
      {
        if (!string.IsNullOrEmpty(callback))
        {
          Collider[] colliderList = gameObjectToTouch.GetComponentsInChildren<Collider>();

          if (colliderList != null && colliderList.Length > 0)
          {
            foreach (Collider itemCollider in colliderList)
            {
              int layerMaskAsKey = (layerMask != default(LayerMask)) ? layerMask.value : (1 << gameObjectToTouch.layer);

#if ENABLE_DEBUGGING
                            Log.Debug("TouchEventManager", "RegisterDoubleTapEvent for 3D. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3}",itemCollider, callback, SortingLayer, isDoubleTapInside);
#endif

              if (m_DoubleTapEvents.ContainsKey(layerMaskAsKey))
              {
                m_DoubleTapEvents[layerMaskAsKey].Add(new TouchEventData(itemCollider, callback, SortingLayer, isDoubleTapInside));
              }
              else
              {
                m_DoubleTapEvents[layerMaskAsKey] = new List<TouchEventData>() { new TouchEventData(itemCollider, callback, SortingLayer, isDoubleTapInside) };
              }
            }

            success = true;
          }
          else
          {
            Log.Warning("TouchEventManager", "There is no 3D collider of given gameobjectToTouch (double-tap)");
          }

          if (!success)
          {
            Collider2D[] colliderList2D = gameObjectToTouch.GetComponentsInChildren<Collider2D>();
            if (colliderList2D != null && colliderList2D.Length > 0)
            {
              foreach (Collider2D itemCollider in colliderList2D)
              {
                int layerMaskAsKey = (layerMask != default(LayerMask)) ? layerMask.value : (1 << gameObjectToTouch.layer);

#if ENABLE_DEBUGGING
                                Log.Debug("TouchEventManager", "RegisterDoubleTapEvent For 2D. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3}",itemCollider, callback, SortingLayer, isDoubleTapInside);
#endif

                if (m_DoubleTapEvents.ContainsKey(layerMaskAsKey))
                {
                  m_DoubleTapEvents[layerMaskAsKey].Add(new TouchEventData(itemCollider, callback, SortingLayer, isDoubleTapInside));
                }
                else
                {
                  m_DoubleTapEvents[layerMaskAsKey] = new List<TouchEventData>() { new TouchEventData(itemCollider, callback, SortingLayer, isDoubleTapInside) };
                }
              }

              success = true;
            }
            else
            {
              Log.Warning("TouchEventManager", "There is no 2D collider of given gameobjectToTouch (double-tap)");
            }
          }

#if UNITY_4_6 || UNITY_5 || UNITY_5_3_OR_NEWER
          if (!success)
          {
            RectTransform[] rectTransformList = gameObjectToTouch.GetComponentsInChildren<RectTransform>(includeInactive: true);
            if (rectTransformList != null && rectTransformList.Length > 0)
            {
              foreach (RectTransform itemRectTransform in rectTransformList)
              {
                int layerMaskAsKey = (layerMask != default(LayerMask)) ? layerMask.value : (1 << itemRectTransform.gameObject.layer);

#if ENABLE_DEBUGGING
                                Log.Debug("TouchEventManager", "RegisterDoubleTapEvent For 2D Event System. itemRectTransform: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3}",itemRectTransform, callback, SortingLayer, isDoubleTapInside);
#endif

                if (m_DoubleTapEvents.ContainsKey(layerMaskAsKey))
                {
                  m_DoubleTapEvents[layerMaskAsKey].Add(new TouchEventData(itemRectTransform, callback, SortingLayer, isDoubleTapInside));
                }
                else
                {
                  m_DoubleTapEvents[layerMaskAsKey] = new List<TouchEventData>() { new TouchEventData(itemRectTransform, callback, SortingLayer, isDoubleTapInside) };
                }
              }

              success = true;
            }
            else
            {
              Log.Warning("TouchEventManager", "There is no Rect Transform of given gameobjectToTouch (double-tap)");
            }
          }
#endif

        }
        else
        {
          Log.Warning("TouchEventManager", "There is no callback for double-tap event registration");
        }
      }
      else
      {
        Log.Warning("TouchEventManager", "There is no gameobject for double-tap event registration");
      }

      return success;
    }

    /// <summary>
    /// Unregisters the double tap event.
    /// </summary>
    /// <returns><c>true</c>, if double tap event was unregistered, <c>false</c> otherwise.</returns>
    /// <param name="gameObjectToTouch">Game object to touch.</param>
    /// <param name="callback">Callback.</param>
    /// <param name="SortingLayer">Sorting layer.</param>
    /// <param name="isDoubleTapInside">If set to <c>true</c> is double tap inside.</param>
    /// <param name="layerMask">Layer mask.</param>
    public bool UnregisterDoubleTapEvent(GameObject gameObjectToTouch, string callback, int SortingLayer = 0, bool isDoubleTapInside = true, LayerMask layerMask = default(LayerMask))
    {
      bool success = false;

      if (gameObjectToTouch != null)
      {
        if (!string.IsNullOrEmpty(callback))
        {
          int layerMaskAsKey = (layerMask != default(LayerMask)) ? layerMask.value : (1 << gameObjectToTouch.layer);

          if (m_DoubleTapEvents.ContainsKey(layerMaskAsKey))
          {
            success = true;
            Collider[] colliderList = gameObjectToTouch.GetComponentsInChildren<Collider>(includeInactive: true);
            if (colliderList != null && colliderList.Length > 0)
            {
              foreach (Collider itemCollider in colliderList)
              {
                int numberOfRemovedCallbacks = m_DoubleTapEvents[layerMaskAsKey].RemoveAll(
                    e =>
                    e.Collider == itemCollider &&
                    e.TapCallback == callback &&
                    e.SortingLayer == SortingLayer &&
                    e.IsInside == isDoubleTapInside);

                success &= (numberOfRemovedCallbacks > 0);
              }
            }
            else
            {
              success = false;
            }

            if (!success)
            {
              success = true;
              Collider2D[] colliderList2D = gameObjectToTouch.GetComponentsInChildren<Collider2D>(includeInactive: true);
              if (colliderList2D != null && colliderList2D.Length > 0)
              {
                foreach (Collider2D itemCollider2D in colliderList2D)
                {
                  int numberOfRemovedCallbacks = m_DoubleTapEvents[layerMaskAsKey].RemoveAll(
                      e =>
                      e.Collider2D == itemCollider2D &&
                      e.TapCallback == callback &&
                      e.SortingLayer == SortingLayer &&
                      e.IsInside == isDoubleTapInside);

                  success &= (numberOfRemovedCallbacks > 0);
                }
              }
              else
              {
                success = false;
              }

            }
#if UNITY_4_6 || UNITY_5 || UNITY_5_3_OR_NEWER
            if (!success)
            {
              success = true;
              RectTransform[] rectTransformList = gameObjectToTouch.GetComponentsInChildren<RectTransform>(includeInactive: true);
              if (rectTransformList != null && rectTransformList.Length > 0)
              {
                foreach (RectTransform itemRectTransform in rectTransformList)
                {
                  int numberOfRemovedCallbacks = m_DoubleTapEvents[layerMaskAsKey].RemoveAll(
                      e =>
                      e.RectTransform == itemRectTransform &&
                      e.TapCallback == callback &&
                      e.SortingLayer == SortingLayer &&
                      e.IsInside == isDoubleTapInside);

                  success &= (numberOfRemovedCallbacks > 0);
                }
              }
              else
              {
                success = false;
              }
            }
#endif
          }
        }
        else
        {
          Log.Warning("TouchEventManager", "There is no callback for double-tap event unregistration");
        }
      }
      else
      {
        Log.Warning("TouchEventManager", "There is no gameobject for double-tap event unregistration");
      }

      return success;
    }


    private void DoubleTapGesture_Tapped(object sender, System.EventArgs e)
    {
      if (m_Active)
      {
#if ENABLE_DEBUGGING
                Log.Debug("TouchEventManager", "DoubleTapGesture_Tapped: {0} - {1}", m_DoubleTapGesture.ScreenPosition, m_DoubleTapGesture.NumTouches);
#endif

        TouchEventData tapEventToFire = null;

        RaycastHit hitToFire3D = default(RaycastHit);
        RaycastHit2D hitToFire2D = default(RaycastHit2D);
#if UNITY_4_6 || UNITY_5 || UNITY_5_3_OR_NEWER
        RaycastResult hitToFire2DEventSystem = default(RaycastResult);
#endif

        foreach (var kp in m_DoubleTapEvents)
        {
          //Adding Variables for 3D Tap Check
          Ray rayForTab = MainCamera.ScreenPointToRay(m_DoubleTapGesture.ScreenPosition);
          Transform hitTransform3D = null;
          RaycastHit hit3D = default(RaycastHit);
          bool isHitOnLayer3D = Physics.Raycast(rayForTab, out hit3D, Mathf.Infinity, kp.Key);
          if (isHitOnLayer3D)
          {
            hitTransform3D = hit3D.collider.transform;
          }

          //Adding Variables for 2D Tap Check for 2d Colliders
          Transform hitTransform2D = null;
          RaycastHit2D hit2D = Physics2D.Raycast(rayForTab.origin, rayForTab.direction, Mathf.Infinity, kp.Key);
          bool isHitOnLayer2D = false;
          if (hit2D.collider != null)
          {
            isHitOnLayer2D = true;
            hitTransform2D = hit2D.collider.transform;
          }

#if UNITY_4_6 || UNITY_5 || UNITY_5_3_OR_NEWER
          //Adding Variables for Event.System Tap for UI Elements
          Transform hitTransform2DEventSystem = null;
          bool isHitOnLayer2DEventSystem = false;
          RaycastResult hit2DEventSystem = default(RaycastResult);
          if (EventSystem.current != null)
          {
            PointerEventData pointerEventForTap = new PointerEventData(EventSystem.current);
            pointerEventForTap.position = m_DoubleTapGesture.ScreenPosition;
            List<RaycastResult> raycastResultListFor2DEventSystem = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventForTap, raycastResultListFor2DEventSystem);
            foreach (RaycastResult itemRaycastResult in raycastResultListFor2DEventSystem)
            {

              LayerMask layerMaskOfItem = 1 << itemRaycastResult.gameObject.layer;
              isHitOnLayer2DEventSystem = ((layerMaskOfItem.value & kp.Key) == layerMaskOfItem.value);
              if (isHitOnLayer2DEventSystem)
              {
                hit2DEventSystem = itemRaycastResult;
                hitTransform2DEventSystem = itemRaycastResult.gameObject.transform;
                break;
              }
            }
          }
#endif


          for (int i = 0; i < kp.Value.Count; ++i)
          {
            TouchEventData tapEventData = kp.Value[i];

            if (kp.Value[i].Collider == null && kp.Value[i].Collider2D == null && kp.Value[i].RectTransform == null && kp.Value[i].RectTransformList == null)
            {
              Log.Warning("TouchEventManager", "Removing invalid collider event receiver from DoubleTapEventList");
              kp.Value.RemoveAt(i--);
              continue;
            }

            if (string.IsNullOrEmpty(tapEventData.TapCallback))
            {
              Log.Warning("TouchEventManager", "Removing invalid event receiver from DoubleTapEventList");
              kp.Value.RemoveAt(i--);
              continue;
            }

            //3d Hit Check
            if (tapEventData.Collider != null)
            {
              if (isHitOnLayer3D && hitTransform3D == tapEventData.Collider.transform && tapEventData.IsInside)
              {
                //Tapped inside the object
                if (tapEventToFire == null)
                {
                  tapEventToFire = tapEventData;
                  hitToFire3D = hit3D;
                  hitToFire2D = default(RaycastHit2D);
                  hitToFire2DEventSystem = default(RaycastResult);
#if ENABLE_DEBUGGING
                                    Log.Debug("TouchEventManager", "Double-Tap Event Found 3D. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform3D, tapEventData.TapCallback, tapEventData.SortingLayer, tapEventData.IsInside);
#endif
                }
                else
                {
                  if (tapEventData.SortingLayer > tapEventToFire.SortingLayer ||
                      (tapEventToFire.SortingLayer == tapEventData.SortingLayer && !tapEventToFire.IsInside))
                  {
                    tapEventToFire = tapEventData;
                    hitToFire3D = hit3D;
                    hitToFire2D = default(RaycastHit2D);
                    hitToFire2DEventSystem = default(RaycastResult);

#if ENABLE_DEBUGGING
                                        Log.Debug("TouchEventManager", "Double-Tap Event Found 3D. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform3D, tapEventData.TapCallback, tapEventData.SortingLayer, tapEventData.IsInside);
#endif
                  }
                  else
                  {
                    //do nothing
                  }
                }

              }
              else if ((!isHitOnLayer3D || hitTransform3D != tapEventData.Collider.transform) && !tapEventData.IsInside)
              {
                //Tapped outside the object
                if (tapEventToFire == null)
                {
                  tapEventToFire = tapEventData;
                  hitToFire3D = hit3D;
                  hitToFire2D = default(RaycastHit2D);
                  hitToFire2DEventSystem = default(RaycastResult);

#if ENABLE_DEBUGGING
                                    Log.Debug("TouchEventManager", "Double-Tap Event Found 3D. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform3D, tapEventData.TapCallback, tapEventData.SortingLayer, tapEventData.IsInside);
#endif
                }
                else
                {
                  if (tapEventData.SortingLayer > tapEventToFire.SortingLayer)
                  {
                    tapEventToFire = tapEventData;
                    hitToFire3D = hit3D;
                    hitToFire2D = default(RaycastHit2D);
                    hitToFire2DEventSystem = default(RaycastResult);

#if ENABLE_DEBUGGING
                                        Log.Debug("TouchEventManager", "Double-Tap Event Found 3D. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform3D, tapEventData.TapCallback, tapEventData.SortingLayer, tapEventData.IsInside);
#endif
                  }
                  else
                  {
                    //do nothing
                  }
                }
              }
              else
              {
                //do nothing
              }
            }

            //2d Hit Check
            if (tapEventData.Collider2D != null)
            {
              if (isHitOnLayer2D && hitTransform2D == tapEventData.Collider2D.transform && tapEventData.IsInside)
              {
                //Tapped inside the object
                if (tapEventToFire == null)
                {
                  tapEventToFire = tapEventData;
                  hitToFire3D = default(RaycastHit);
                  hitToFire2D = hit2D;
                  hitToFire2DEventSystem = default(RaycastResult);

#if ENABLE_DEBUGGING
                                    Log.Debug("TouchEventManager", "Double-Tap Event Found 2D. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform2D, tapEventData.TapCallback, tapEventData.SortingLayer, tapEventData.IsInside);
#endif
                }
                else
                {
                  if (tapEventData.SortingLayer > tapEventToFire.SortingLayer ||
                      (tapEventToFire.SortingLayer == tapEventData.SortingLayer && !tapEventToFire.IsInside))
                  {
                    tapEventToFire = tapEventData;
                    hitToFire3D = default(RaycastHit);
                    hitToFire2D = hit2D;
                    hitToFire2DEventSystem = default(RaycastResult);

#if ENABLE_DEBUGGING
                                        Log.Debug("TouchEventManager", "Double-Tap Event Found 2D. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform2D, tapEventData.TapCallback, tapEventData.SortingLayer, tapEventData.IsInside);
#endif
                  }
                  else
                  {
                    //do nothing
                  }
                }

              }
              else if ((!isHitOnLayer2D || hitTransform2D != tapEventData.Collider2D.transform) && !tapEventData.IsInside)
              {
                //Tapped outside the object
                if (tapEventToFire == null)
                {
                  tapEventToFire = tapEventData;
                  hitToFire3D = default(RaycastHit);
                  hitToFire2D = hit2D;
                  hitToFire2DEventSystem = default(RaycastResult);

#if ENABLE_DEBUGGING
                                    Log.Debug("TouchEventManager", "Double-Tap Event Found 2D. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform2D, tapEventData.TapCallback, tapEventData.SortingLayer, tapEventData.IsInside);
#endif
                }
                else
                {
                  if (tapEventData.SortingLayer > tapEventToFire.SortingLayer)
                  {
                    tapEventToFire = tapEventData;
                    hitToFire3D = default(RaycastHit);
                    hitToFire2D = hit2D;
                    hitToFire2DEventSystem = default(RaycastResult);

#if ENABLE_DEBUGGING
                                        Log.Debug("TouchEventManager", "Double-Tap Event Found 2D. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform2D, tapEventData.TapCallback, tapEventData.SortingLayer, tapEventData.IsInside);
#endif
                  }
                  else
                  {
                    //do nothing
                  }
                }
              }
              else
              {
                //do nothing
              }
            }

#if UNITY_4_6 || UNITY_5 || UNITY_5_3_OR_NEWER
            //2D UI Hit Check using EventSystem
            if (tapEventData.RectTransform != null)
            {
              if (isHitOnLayer2DEventSystem && hitTransform2DEventSystem == tapEventData.RectTransform.transform && tapEventData.IsInside)
              {
                //Tapped inside the object
                if (tapEventToFire == null)
                {
                  tapEventToFire = tapEventData;
                  hitToFire3D = default(RaycastHit);
                  hitToFire2D = default(RaycastHit2D);
                  hitToFire2DEventSystem = hit2DEventSystem;

#if ENABLE_DEBUGGING
                                    Log.Debug("TouchEventManager", "Double-Tap Event Found 2D Event System. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform2DEventSystem, tapEventData.TapCallback, tapEventData.SortingLayer, tapEventData.IsInside);
#endif
                }
                else
                {
                  if (tapEventData.SortingLayer > tapEventToFire.SortingLayer ||
                      (tapEventToFire.SortingLayer == tapEventData.SortingLayer && !tapEventToFire.IsInside))
                  {
                    tapEventToFire = tapEventData;
                    hitToFire3D = default(RaycastHit);
                    hitToFire2D = default(RaycastHit2D);
                    hitToFire2DEventSystem = hit2DEventSystem;

#if ENABLE_DEBUGGING
                                        Log.Debug("TouchEventManager", "Double-Tap Event Found 2D Event System. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform2DEventSystem, tapEventData.TapCallback, tapEventData.SortingLayer, tapEventData.IsInside);
#endif
                  }
                  else
                  {
                    //do nothing
                  }
                }

              }
              else if ((!isHitOnLayer2DEventSystem || hitTransform2DEventSystem != tapEventData.RectTransform.transform) && !tapEventData.IsInside)
              {
                //Tapped outside the object
                if (tapEventToFire == null)
                {
                  tapEventToFire = tapEventData;
                  hitToFire3D = default(RaycastHit);
                  hitToFire2D = default(RaycastHit2D);
                  hitToFire2DEventSystem = hit2DEventSystem;

#if ENABLE_DEBUGGING
                                    Log.Debug("TouchEventManager", "Double-Tap Event Found 2D Event System. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform2DEventSystem, tapEventData.TapCallback, tapEventData.SortingLayer, tapEventData.IsInside);
#endif
                }
                else
                {
                  if (tapEventData.SortingLayer > tapEventToFire.SortingLayer)
                  {
                    tapEventToFire = tapEventData;
                    hitToFire3D = default(RaycastHit);
                    hitToFire2D = default(RaycastHit2D);
                    hitToFire2DEventSystem = hit2DEventSystem;

#if ENABLE_DEBUGGING
                                        Log.Debug("TouchEventManager", "Double-Tap Event Found 2D Event System. itemCollider: {0}, callback: {1}, SortingLayer: {2}, isTapInside: {3} ",hitTransform2DEventSystem, tapEventData.TapCallback, tapEventData.SortingLayer, tapEventData.IsInside);
#endif
                  }
                  else
                  {
                    //do nothing
                  }
                }
              }
              else
              {
                //do nothing
              }
            }
#endif

          }
        }

        if (tapEventToFire != null)
          EventManager.Instance.SendEvent(tapEventToFire.TapCallback, m_DoubleTapGesture, hitToFire3D, hitToFire2D, hitToFire2DEventSystem);

        EventManager.Instance.SendEvent("OnDoubleTap", m_DoubleTapGesture);
      }
    }

    #endregion


    #region Three Tap Gesture Call

    private void ThreeTapGesture_Tapped(object sender, System.EventArgs e)
    {
      if (m_Active)
      {
#if ENABLE_DEBUGGING
                Log.Debug("TouchEventManager", "ThreeTapGesture_Tapped: {0} - {1}", m_ThreeTapGesture.ScreenPosition, m_ThreeTapGesture.NumTouches);
#endif
        EventManager.Instance.SendEvent("OnTripleTap", m_ThreeTapGesture);
      }
    }

    #endregion

    #region PressGesture Events -  Call - There is no registration is sends automatically the press event

    private void PressGesturePressed(object sender, System.EventArgs e)
    {
#if ENABLE_DEBUGGING
            Log.Debug("TouchEventManager", "PressGesturePressed: {0} - {1}", m_PressGesture.ScreenPosition, m_PressGesture.NumTouches);
#endif

      EventManager.Instance.SendEvent("OnTouchPressedFullscreen", m_PressGesture);
    }

    #endregion

    #region Long Press Gesture Events

    private void LongPressGesturePressed(object sender, System.EventArgs e)
    {
#if ENABLE_DEBUGGING
            Log.Debug("TouchEventManager", "LongPressGesturePressed: {0} - {1}", m_LongPressGesture.ScreenPosition, m_LongPressGesture.NumTouches);
#endif

      EventManager.Instance.SendEvent("OnLongPressOneFinger", m_LongPressGesture);
    }

    #endregion

    #region ReleaseGesture Events - Call - There is no registration is sends automatically the release event

    private void ReleaseGestureReleased(object sender, System.EventArgs e)
    {
#if ENABLE_DEBUGGING
            Log.Debug("TouchEventManager", "ReleaseGestureReleased: {0} - {1}", m_ReleaseGesture.ScreenPosition, m_ReleaseGesture.NumTouches);
#endif

      EventManager.Instance.SendEvent("OnTouchReleasedFullscreen", m_ReleaseGesture);
    }
    #endregion
  }

}
