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
using System.Collections.Generic;
using TouchScript.Gestures;
using IBM.Watson.DeveloperCloud.Logging;

namespace IBM.Watson.DeveloperCloud.Utilities
{
    /// <summary>
    /// Touch Event Manager for all touch events. 
    /// Each element can register their touch related functions using this manager. 
    /// </summary>
	[RequireComponent (typeof (TapGesture))]
	public class TouchEventManager : MonoBehaviour {

//		#region Public Types
//		public delegate void TapEventDelegate(TapGesture tapGesture, Transform hitTransform);
//		public delegate void DragEventDelegate(ScreenTransformGesture transformGesture);
//		#endregion

        /// <summary>
        /// Touch Event Data holds all touch related event data for registering and unregistering events via Touch Event Manager.
        /// </summary>
		public class TouchEventData
		{
			private Collider m_Collider;
            private Collider2D m_Collider2D;
			private Collider[] m_ColliderList;
            private Collider2D[] m_Collider2DList;
			private GameObject m_GameObject;
			private Constants.Event m_tapEventCallback;
			private Constants.Event m_dragEventCallback;
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
            /// If it is tap event (or one time action event) we are returning the collider of the event.
            /// </summary>
            public Collider2D Collider2D { get { return m_Collider2D; } }
            /// <summary>
            /// If there is a drag event (or continues action) we are holding game object and all colliders inside that object
            /// </summary>
			public Collider[] ColliderList { get {  if(m_ColliderList == null && m_Collider != null) m_ColliderList = new Collider[]{m_Collider}; return m_ColliderList; } }
            /// <summary>
            /// If there is a drag event (or continues action) we are holding game object and all colliders inside that object
            /// </summary>
            public Collider2D[] ColliderList2D { get {  if(m_Collider2DList == null && m_Collider2D != null) m_Collider2DList = new Collider2D[]{m_Collider2D}; return m_Collider2DList; } }
            /// <summary>
            /// If the touch event has happened inside of that object (collider) we will fire that event. Otherwise, it is considered as outside
            /// </summary>
            public bool IsInside{ get { return m_isInside; } }
            /// <summary>
            /// Tap Delegate to call
            /// </summary>
			public Constants.Event TapCallback{ get { return m_tapEventCallback; } }
            /// <summary>
            /// Drag Delegate to call
            /// </summary>
			public Constants.Event DragCallback{ get { return m_dragEventCallback; } }
            /// <summary>
            /// Greater sorting layer is higher importance level. 
            /// </summary>
			public int SortingLayer{ get { return m_SortingLayer; } }
			/// <summary>
			/// Gets a value indicating whether this instance can drag object.
			/// </summary>
			/// <value><c>true</c> if this instance can drag object; otherwise, <c>false</c>.</value>
            public bool CanDragObject{ get { return GameObjectAttached != null && ((ColliderList != null && ColliderList.Length > 0) || (ColliderList2D != null && ColliderList2D.Length > 0)); } }

            /// <summary>
            /// Touch event constructor for Tap Event registration. 
            /// </summary>
            /// <param name="collider">Collider of the object to tap</param>
            /// <param name="callback">Callback for Tap Event. After tapped, callback will be invoked</param>
            /// <param name="sortingLayer">Sorting level in order to sort the event listeners</param>
            /// <param name="isInside">Whether the tap is inside the object or not</param>
			public TouchEventData(Collider collider, Constants.Event callback, int sortingLayer, bool isInside){
				m_Collider = collider;
                m_Collider2D = null;
				m_ColliderList = null;
                m_Collider2DList = null;
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
			public TouchEventData(GameObject gameObject, Constants.Event callback, int sortingLayer, bool isInside){
				m_GameObject = gameObject;
				m_ColliderList = null;
                if(gameObject != null){
                    m_ColliderList = gameObject.GetComponentsInChildren<Collider>();
                    m_Collider2DList = gameObject.GetComponentsInChildren<Collider2D>();
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
			public bool HasTouchedOn(Transform hitTransform){
				bool hasTouchedOn = false;
				if (ColliderList != null) 
				{
					foreach (Collider itemCollider in ColliderList) {
						if(itemCollider.transform == hitTransform){
							hasTouchedOn = true;
							break;
						}
					}
			
				}

                if (!hasTouchedOn && ColliderList2D != null) 
                {
                    foreach (Collider2D itemCollider in ColliderList2D) {
                        if(itemCollider.transform == hitTransform){
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
			public override bool Equals (object obj)
			{
				bool isEqual = false;
				TouchEventData touchEventData = obj as TouchEventData;
				if (touchEventData != null) 
				{
					isEqual = 
                        (touchEventData.Collider == this.Collider &&
                        touchEventData.Collider2D == this.Collider2D &&
                        touchEventData.GameObjectAttached == this.GameObjectAttached &&
                        touchEventData.IsInside == this.IsInside && 
						touchEventData.SortingLayer == this.SortingLayer &&
						touchEventData.DragCallback == this.DragCallback &&
						touchEventData.TapCallback == this.TapCallback);
				} 
				else 
				{
					isEqual = base.Equals (obj);
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

		void Awake(){
			sm_Instance = this;
		}

		private void OnEnable()
		{
			m_mainCamera = UnityEngine.Camera.main;
            if (m_TapGesture != null)
			    m_TapGesture.Tapped += TapGesture_Tapped;
            if ( m_DoubleTapGesture != null )
                m_DoubleTapGesture.Tapped += DoubleTapGesture_Tapped;
            if ( m_ThreeTapGesture != null )
			    m_ThreeTapGesture.Tapped += ThreeTapGesture_Tapped;
            if ( m_OneFingerMoveGesture != null )
			    m_OneFingerMoveGesture.Transformed += OneFingerTransformedHandler;
            if ( m_TwoFingerMoveGesture != null )
			    m_TwoFingerMoveGesture.Transformed += TwoFingerTransformedHandler;
            if ( m_PressGesture != null)
                m_PressGesture.Pressed += PressGesturePressed;
            if ( m_ReleaseGesture != null )
                m_ReleaseGesture.Released += ReleaseGestureReleased;
        }

        private void OnDisable()
		{
            if ( m_TapGesture != null )
			    m_TapGesture.Tapped -= TapGesture_Tapped;
            if ( m_DoubleTapGesture != null )
                m_DoubleTapGesture.Tapped -= DoubleTapGesture_Tapped;
            if ( m_ThreeTapGesture != null )
			    m_ThreeTapGesture.Tapped -= ThreeTapGesture_Tapped;
            if ( m_OneFingerMoveGesture != null )
			    m_OneFingerMoveGesture.Transformed -= OneFingerTransformedHandler;
            if( m_TwoFingerMoveGesture != null )
			    m_TwoFingerMoveGesture.Transformed -= TwoFingerTransformedHandler;
            if ( m_PressGesture != null )
                m_PressGesture.Pressed -= PressGesturePressed;
            if ( m_ReleaseGesture != null )
                m_ReleaseGesture.Released -= ReleaseGestureReleased;
        }

		/// <summary>
		/// Gets the main camera.
		/// </summary>
		/// <value>The main camera.</value>
        public UnityEngine.Camera MainCamera
        {
            get
            {
                if(m_mainCamera == null)
                    m_mainCamera = UnityEngine.Camera.main;

                return m_mainCamera;
            }
        }


		#endregion

		#region OneFinger Events - Register / UnRegister / Call

        /// <summary>
        /// Register Drag Event to given call back with given parameters
        /// </summary>
        /// <param name="gameObjectToDrag">GameObject to drag</param>
        /// <param name="callback">Callback to invoke while the object is dragging</param>
        /// <param name="numberOfFinger">Number of fingers working on dragging</param>
        /// <param name="SortingLayer">Sorting layer to determine the corresponding drag event listener</param>
        /// <param name="isDragInside">Not Applicable</param>
        /// <returns>Success result about registration</returns>
		public bool RegisterDragEvent(GameObject gameObjectToDrag, Constants.Event callback, int numberOfFinger = 1, int SortingLayer = 0, bool isDragInside = true)
		{
			if (m_DragEvents.ContainsKey (numberOfFinger)) 
			{
				m_DragEvents[numberOfFinger].Add( new TouchEventData(gameObjectToDrag, callback, SortingLayer, isDragInside));
			} 
			else 
			{
				m_DragEvents[numberOfFinger] = new List<TouchEventData>() {  new TouchEventData(gameObjectToDrag, callback, SortingLayer, isDragInside) };
			}
			
			return true;
		}

        /// <summary>
        /// Unregister Drag Event to given call back with given parameters
        /// </summary>
        /// <param name="gameObjectToDrag">GameObject to drag</param>
        /// <param name="callback">Callback to invoke while the object is dragging</param>
        /// <param name="numberOfFinger">Number of fingers working on dragging</param>
        /// <param name="SortingLayer">Sorting layer to determine the corresponding drag event listener</param>
        /// <param name="isDragInside">Not Applicable</param>
        /// <returns>Success result about unregistration</returns>
		public bool UnregisterDragEvent(GameObject gameObjectToDrag, Constants.Event callback, int numberOfFinger = 1, int SortingLayer = 0, bool isDragInside = true)
		{
			bool success = false;
			
			if (m_DragEvents.ContainsKey (numberOfFinger)) 
			{
				success = m_DragEvents[numberOfFinger].Remove(  new TouchEventData(gameObjectToDrag, callback, SortingLayer, isDragInside) );
			} 
			
			return success;
		}

		private void OneFingerTransformedHandler(object sender, System.EventArgs e)
		{
			//Log.Status ("TouchEventManager", "oneFingerManipulationTransformedHandler: {0}", m_OneFingerMoveGesture.DeltaPosition);
			if (m_Active) {
				TouchEventData dragEventToFire = null;
					
				Ray rayForDrag = MainCamera.ScreenPointToRay(m_OneFingerMoveGesture.ScreenPosition);
                RaycastHit hit;
                RaycastHit2D hit2D;


				foreach (var kp in m_DragEvents) {
					if (kp.Key == 1) {

						for (int i = 0; i < kp.Value.Count; ++i) {
							TouchEventData dragEventData = kp.Value [i];
								
							if (dragEventData.DragCallback == Constants.Event.NONE) {
								Log.Warning ("TouchEventManager", "Removing invalid event receiver from OneFingerDrag");
								kp.Value.RemoveAt (i--);
								continue;
							}

							bool hasDragOnObject = false;
							//If we can drag the object, we should check that whether there is a raycast or not!
							if(dragEventData.CanDragObject){
								bool isHitOnLayer = Physics.Raycast(rayForDrag, out hit, Mathf.Infinity, 1 << dragEventData.GameObjectAttached.layer);
                                Transform hitTransform = null;

                                if (isHitOnLayer)
                                {
                                    hitTransform = hit.transform;
                                }
                                else
                                {
                                    hit2D = Physics2D.Raycast(rayForDrag.origin, rayForDrag.direction, Mathf.Infinity, 1 << dragEventData.GameObjectAttached.layer);
                                    if (hit2D.collider != null)
                                    {
                                        isHitOnLayer = true;
                                        hitTransform = hit2D.transform;
                                    }
                                }
                                    

                                if(isHitOnLayer && dragEventData.HasTouchedOn(hitTransform) && dragEventData.IsInside){
									hasDragOnObject = true;
								}
                                else if (!isHitOnLayer && !dragEventData.IsInside)
                                {
                                    hasDragOnObject = true;
                                }
                                else
                                {
									//do nothing - we were checking that draggable object that we touched!
								}

							}

							if( hasDragOnObject || !dragEventData.CanDragObject){
								//They are all fullscreen drags!
								if (dragEventToFire == null) 
								{
									dragEventToFire = dragEventData;
								} 
								else 
								{
									if (dragEventData.SortingLayer > dragEventToFire.SortingLayer || (dragEventToFire.SortingLayer == dragEventData.SortingLayer && !dragEventToFire.IsInside)) {
										dragEventToFire = dragEventData;
									} else {
										//do nothing
									}
								}
							}


						}
					}

				}

				if(dragEventToFire != null && dragEventToFire.DragCallback != Constants.Event.NONE)
					EventManager.Instance.SendEvent(dragEventToFire.DragCallback, m_OneFingerMoveGesture);
					//dragEventToFire.DragCallback(m_OneFingerManipulationGesture);

                EventManager.Instance.SendEvent(Constants.Event.ON_DRAG_ONE_FINGER_FULLSCREEN, m_OneFingerMoveGesture);
			}
			
		}

		private void TwoFingerTransformedHandler(object sender, System.EventArgs e)
		{
			//Log.Status ("TouchEventManager", "TwoFingerTransformedHandler: {0}", m_TwoFingerMoveGesture.DeltaPosition);
			if (m_Active) {
				TouchEventData dragEventToFire = null;

				foreach (var kp in m_DragEvents) {
					if (kp.Key == 2) {
						
						for (int i = 0; i < kp.Value.Count; ++i) {
							TouchEventData dragEventData = kp.Value [i];
							
							if (dragEventData.DragCallback == Constants.Event.NONE) {
								Log.Warning ("TouchEventManager", "Removing invalid event receiver from TwoFingerDrag");
								kp.Value.RemoveAt (i--);
								continue;
							}
							
							if (dragEventToFire == null) {
								dragEventToFire = dragEventData;
							} else {
								if (dragEventData.SortingLayer > dragEventToFire.SortingLayer || 
								    (dragEventToFire.SortingLayer == dragEventData.SortingLayer && !dragEventToFire.IsInside)) {
									
									dragEventToFire = dragEventData;
								} else {
									//do nothing
								}
							}
						}
					}
					
				}
				
				if(dragEventToFire != null && dragEventToFire.DragCallback != Constants.Event.NONE)
					EventManager.Instance.SendEvent(dragEventToFire.DragCallback, m_TwoFingerMoveGesture);
					//dragEventToFire.DragCallback(m_TwoFingerMoveGesture);
				
                EventManager.Instance.SendEvent(Constants.Event.ON_DRAG_TWO_FINGER_FULLSCREEN, m_TwoFingerMoveGesture);
			}
		}


		#endregion

		#region TapEvents - Register / UnRegister / Call 

        /// <summary>
        /// Register tap event to given callback
        /// </summary>
        /// <param name="gameObjectToTouch">Game object to tap on</param>
        /// <param name="callback">Callback to call after tapped on object (or outside of the object)</param>
        /// <param name="SortingLayer">Sorting layer to determine the corresponding tap object</param>
        /// <param name="isTapInside">Whether to tap on object or outside the object</param>
		/// <param name="layerMask">Layer mask to tap. Default is the gameObjectToTouch's layer</param>
        /// <returns></returns>
		public bool RegisterTapEvent(GameObject gameObjectToTouch, Constants.Event callback, int SortingLayer = 0, bool isTapInside = true, LayerMask layerMask = default(LayerMask))
		{
			Collider[] colliderList = gameObjectToTouch.GetComponentsInChildren<Collider>();

			if(colliderList != null){
				foreach (Collider itemCollider in colliderList) 
				{
					int layerMaskAsKey = (layerMask != default(LayerMask))? layerMask.value : (1 << gameObjectToTouch.layer);
					
					if (m_TapEvents.ContainsKey (layerMaskAsKey)) 
					{
						m_TapEvents[layerMaskAsKey].Add( new TouchEventData(itemCollider, callback, SortingLayer, isTapInside));
					} 
					else 
					{
						m_TapEvents[layerMaskAsKey] = new List<TouchEventData>() {  new TouchEventData(itemCollider, callback, SortingLayer, isTapInside) };
					}
				}
			}
			else{
				Log.Warning("TouchEventManager","There is no collider of given gameobjectToTouch");
			}


			return true;
		}

        /// <summary>
        ///  Unregister tap event to given callback
        /// </summary>
        /// <param name="gameObjectToTouch">Game object to tap on</param>
        /// <param name="callback">Callback to call after tapped on object (or outside of the object)</param>
        /// <param name="SortingLayer">Sorting layer to determine the corresponding tap object</param>
        /// <param name="isTapInside">Whether to tap on object or outside the object</param>
		/// <param name="layerMask">Layer mask to tap. Default is the gameObjectToTouch's layer</param>
        /// <returns></returns>
		public bool UnregisterTapEvent(GameObject gameObjectToTouch, Constants.Event callback, int SortingLayer = 0, bool isTapInside = true, LayerMask layerMask = default(LayerMask))
		{
			bool success = false;

			int layerMaskAsKey = (layerMask != default(LayerMask))? layerMask.value : (1 << gameObjectToTouch.layer);

			if (m_TapEvents.ContainsKey (layerMaskAsKey)) 
			{
				success = true;
				Collider[] colliderList = gameObjectToTouch.GetComponentsInChildren<Collider>();
				foreach (Collider itemCollider in colliderList) 
				{
					success &= m_TapEvents[layerMaskAsKey].Remove( new TouchEventData(itemCollider, callback, SortingLayer, isTapInside) );
				}
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
				RaycastHit hit = default(RaycastHit);

				foreach (var kp in m_TapEvents)
				{
				
					Ray rayForTab = MainCamera.ScreenPointToRay(m_TapGesture.ScreenPosition);

					bool isHitOnLayer = Physics.Raycast(rayForTab, out hit, Mathf.Infinity, kp.Key);

					for (int i = 0; i < kp.Value.Count; ++i)
					{
						TouchEventData tapEventData = kp.Value[i];

						if (tapEventData.TapCallback == Constants.Event.NONE)
						{
							Log.Warning("TouchEventManager", "Removing invalid event receiver from TapEventList");
							kp.Value.RemoveAt(i--);
							continue;
						}

						if(isHitOnLayer && hit.collider.transform == tapEventData.Collider.transform && tapEventData.IsInside){
							//Tapped inside the object
							if(tapEventToFire == null)
							{
								tapEventToFire = tapEventData;
							}
							else
							{
								if(tapEventData.SortingLayer > tapEventToFire.SortingLayer || 
								   (tapEventToFire.SortingLayer == tapEventData.SortingLayer && !tapEventToFire.IsInside))
								{
									tapEventToFire = tapEventData;
								}
								else
								{
									//do nothing
								}
							}

						}
						else if( (!isHitOnLayer || hit.collider.transform != tapEventData.Collider.transform) && !tapEventData.IsInside){
							//Tapped outside the object
							if(tapEventToFire == null)
							{
								tapEventToFire = tapEventData;
							}
							else
							{
								if(tapEventData.SortingLayer > tapEventToFire.SortingLayer)
								{
									tapEventToFire = tapEventData;
								}
								else
								{
									//do nothing
								}
							}
						}
						else{
							//do nothing
						}
					}

				}

				if(tapEventToFire != null && tapEventToFire.TapCallback != Constants.Event.NONE)
					EventManager.Instance.SendEvent(tapEventToFire.TapCallback, m_TapGesture, hit);
					//tapEventToFire.TapCallback(m_TapGesture, tapEventToFire.Collider.transform);

                EventManager.Instance.SendEvent(Constants.Event.ON_TAP_ONE, m_TapGesture);
			}
            
		}

        #endregion

        #region Double TapEvents - Register / UnRegister / Call 

        /// <summary>
        /// Register tap event to given callback
        /// </summary>
        /// <param name="gameObjectToTouch">Game object to tap on</param>
        /// <param name="callback">Callback to call after tapped on object (or outside of the object)</param>
        /// <param name="SortingLayer">Sorting layer to determine the corresponding tap object</param>
        /// <param name="isTapInside">Whether to tap on object or outside the object</param>
        /// <param name="layerMask">Layer mask to tap. Default is the gameObjectToTouch's layer</param>
        /// <returns></returns>
        public bool RegisterDoubleTapEvent(GameObject gameObjectToTouch, Constants.Event callback, int SortingLayer = 0, bool isDoubleTapInside = true, LayerMask layerMask = default(LayerMask))
        {
            Collider[] colliderList = gameObjectToTouch.GetComponentsInChildren<Collider>();

            if(colliderList != null){
                foreach (Collider itemCollider in colliderList) 
                {
                    int layerMaskAsKey = (layerMask != default(LayerMask))? layerMask.value : (1 << gameObjectToTouch.layer);

                    if (m_DoubleTapEvents.ContainsKey (layerMaskAsKey)) 
                    {
                        m_DoubleTapEvents[layerMaskAsKey].Add( new TouchEventData(itemCollider, callback, SortingLayer, isDoubleTapInside));
                    } 
                    else 
                    {
                        m_DoubleTapEvents[layerMaskAsKey] = new List<TouchEventData>() {  new TouchEventData(itemCollider, callback, SortingLayer, isDoubleTapInside) };
                    }
                }
            }
            else{
                Log.Warning("TouchEventManager","There is no collider of given gameobjectToTouch");
            }


            return true;
        }

        /// <summary>
        ///  Unregister tap event to given callback
        /// </summary>
        /// <param name="gameObjectToTouch">Game object to tap on</param>
        /// <param name="callback">Callback to call after tapped on object (or outside of the object)</param>
        /// <param name="SortingLayer">Sorting layer to determine the corresponding tap object</param>
        /// <param name="isDoubleTapInside">Whether to tap on object or outside the object</param>
        /// <param name="layerMask">Layer mask to tap. Default is the gameObjectToTouch's layer</param>
        /// <returns></returns>
        public bool UnregisterDoubleTapEvent(GameObject gameObjectToTouch, Constants.Event callback, int SortingLayer = 0, bool isDoubleTapInside = true, LayerMask layerMask = default(LayerMask))
        {
            bool success = false;

            int layerMaskAsKey = (layerMask != default(LayerMask))? layerMask.value : (1 << gameObjectToTouch.layer);

            if (m_DoubleTapEvents.ContainsKey (layerMaskAsKey)) 
            {
                success = true;
                Collider[] colliderList = gameObjectToTouch.GetComponentsInChildren<Collider>();
                foreach (Collider itemCollider in colliderList) 
                {
                    success &= m_DoubleTapEvents[layerMaskAsKey].Remove( new TouchEventData(itemCollider, callback, SortingLayer, isDoubleTapInside) );
                }
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
                RaycastHit hit = default(RaycastHit);

                foreach (var kp in m_DoubleTapEvents)
                {

                    Ray rayForTab = MainCamera.ScreenPointToRay(m_DoubleTapGesture.ScreenPosition);

                    bool isHitOnLayer = Physics.Raycast(rayForTab, out hit, Mathf.Infinity, kp.Key);

                    for (int i = 0; i < kp.Value.Count; ++i)
                    {
                        TouchEventData tapEventData = kp.Value[i];

                        if (tapEventData.TapCallback == Constants.Event.NONE)
                        {
                            Log.Warning("TouchEventManager", "Removing invalid event receiver from TapEventList");
                            kp.Value.RemoveAt(i--);
                            continue;
                        }

                        if(isHitOnLayer && hit.collider.transform == tapEventData.Collider.transform && tapEventData.IsInside){
                            //Tapped inside the object
                            if(tapEventToFire == null)
                            {
                                tapEventToFire = tapEventData;
                            }
                            else
                            {
                                if(tapEventData.SortingLayer > tapEventToFire.SortingLayer || 
                                    (tapEventToFire.SortingLayer == tapEventData.SortingLayer && !tapEventToFire.IsInside))
                                {
                                    tapEventToFire = tapEventData;
                                }
                                else
                                {
                                    //do nothing
                                }
                            }

                        }
                        else if( (!isHitOnLayer || hit.collider.transform != tapEventData.Collider.transform) && !tapEventData.IsInside){
                            //Tapped outside the object
                            if(tapEventToFire == null)
                            {
                                tapEventToFire = tapEventData;
                            }
                            else
                            {
                                if(tapEventData.SortingLayer > tapEventToFire.SortingLayer)
                                {
                                    tapEventToFire = tapEventData;
                                }
                                else
                                {
                                    //do nothing
                                }
                            }
                        }
                        else{
                            //do nothing
                        }
                    }

                }

                if(tapEventToFire != null && tapEventToFire.TapCallback != Constants.Event.NONE)
                    EventManager.Instance.SendEvent(tapEventToFire.TapCallback, m_DoubleTapGesture, hit);
                //tapEventToFire.TapCallback(m_DoubleTapGesture, tapEventToFire.Collider.transform);

                EventManager.Instance.SendEvent(Constants.Event.ON_TAP_DOUBLE, m_DoubleTapGesture);
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
				EventManager.Instance.SendEvent(Constants.Event.ON_TAP_THREETIMES, m_ThreeTapGesture);
			}
		}

		#endregion

        #region PressGesture Events -  Call - There is no registration is sends automatically the press event
        
        private void PressGesturePressed(object sender, System.EventArgs e)
        {
            #if ENABLE_DEBUGGING
            Log.Debug("TouchEventManager", "PressGesturePressed: {0} - {1}", m_PressGesture.ScreenPosition, m_PressGesture.NumTouches);
            #endif

            EventManager.Instance.SendEvent(Constants.Event.ON_TOUCH_PRESSED_FULLSCREEN, m_PressGesture);
        }
        
        #endregion

        #region ReleaseGesture Events - Call - There is no registration is sends automatically the release event
        
        private void ReleaseGestureReleased(object sender, System.EventArgs e)
        {
            EventManager.Instance.SendEvent(Constants.Event.ON_TOUCH_RELEASED_FULLSCREEN, m_ReleaseGesture);
        }
        


        #endregion
    }

}
