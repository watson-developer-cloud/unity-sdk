using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;
using IBM.Watson.Logging;

namespace IBM.Watson.Utilities
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
			private Collider[] m_ColliderList;
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
            /// If there is a drag event (or continues action) we are holding game object and all colliders inside that object
            /// </summary>
            public Collider[] ColliderList { get { return m_ColliderList; } }
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
            /// Touch event constructor for Tap Event registration. 
            /// </summary>
            /// <param name="collider">Collider of the object to tap</param>
            /// <param name="callback">Callback for Tap Event. After tapped, callback will be invoked</param>
            /// <param name="sortingLayer">Sorting level in order to sort the event listeners</param>
            /// <param name="isInside">Whether the tap is inside the object or not</param>
			public TouchEventData(Collider collider, Constants.Event callback, int sortingLayer, bool isInside){
				m_Collider = collider;
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
				m_ColliderList = gameObject.GetComponentsInChildren<Collider>();
				m_dragEventCallback = callback;
				m_SortingLayer = sortingLayer;
				m_isInside = isInside;
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
		private Dictionary<int, List<TouchEventData>> m_DragEvents = new Dictionary<int, List<TouchEventData>>();
		#endregion

		#region Serialized Private 
		[SerializeField]
		private TapGesture m_TapGesture;
		[SerializeField]
		private ScreenTransformGesture m_OneFingerMoveGesture;
		[SerializeField]
		private ScreenTransformGesture m_TwoFingerMoveGesture;
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
			m_TapGesture.Tapped += TapGesture_Tapped;

			m_OneFingerMoveGesture.Transformed += OneFingerTransformedHandler;
			m_TwoFingerMoveGesture.Transformed += TwoFingerTransformedHandler;
		}
		
		private void OnDisable()
		{
			m_TapGesture.Tapped += TapGesture_Tapped;

			m_OneFingerMoveGesture.Transformed += OneFingerTransformedHandler;
			m_TwoFingerMoveGesture.Transformed += TwoFingerTransformedHandler;
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
			Log.Status ("TouchEventManager", "oneFingerManipulationTransformedHandler: {0}", m_OneFingerMoveGesture.DeltaPosition);
			if (m_Active) {
				TouchEventData dragEventToFire = null;
					
				foreach (var kp in m_DragEvents) {
					if (kp.Key == 1) {

						for (int i = 0; i < kp.Value.Count; ++i) {
							TouchEventData dragEventData = kp.Value [i];
								
							if (dragEventData.DragCallback == Constants.Event.NONE) {
								Log.Warning ("TouchEventManager", "Removing invalid event receiver from OneFingerDrag");
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
					EventManager.Instance.SendEvent(dragEventToFire.DragCallback, m_OneFingerMoveGesture);
					//dragEventToFire.DragCallback(m_OneFingerManipulationGesture);

			}
			
		}

		private void TwoFingerTransformedHandler(object sender, System.EventArgs e)
		{
			Log.Status ("TouchEventManager", "TwoFingerTransformedHandler: {0}", m_TwoFingerMoveGesture.DeltaPosition);
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
        /// <returns></returns>
		public bool RegisterTapEvent(GameObject gameObjectToTouch, Constants.Event callback, int SortingLayer = 0, bool isTapInside = true)
		{
			Collider[] colliderList = gameObjectToTouch.GetComponentsInChildren<Collider>();

			foreach (Collider itemCollider in colliderList) 
			{
				if (m_TapEvents.ContainsKey (gameObjectToTouch.layer)) 
				{
					m_TapEvents[gameObjectToTouch.layer].Add( new TouchEventData(itemCollider, callback, SortingLayer, isTapInside));
				} 
				else 
				{
					m_TapEvents[gameObjectToTouch.layer] = new List<TouchEventData>() {  new TouchEventData(itemCollider, callback, SortingLayer, isTapInside) };
				}
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
        /// <returns></returns>
		public bool UnregisterTapEvent(GameObject gameObjectToTouch, Constants.Event callback, int SortingLayer = 0, bool isTapInside = true)
		{
			bool success = false;

			if (m_TapEvents.ContainsKey (gameObjectToTouch.layer)) 
			{
				success = true;
				Collider[] colliderList = gameObjectToTouch.GetComponentsInChildren<Collider>();
				foreach (Collider itemCollider in colliderList) 
				{
					success &= m_TapEvents[gameObjectToTouch.layer].Remove( new TouchEventData(itemCollider, callback, SortingLayer, isTapInside) );
				}
			} 

			return success;
		}


		private void TapGesture_Tapped(object sender, System.EventArgs e)
		{   
            if (m_Active)
			{
                Log.Status("TouchEventManager", "TapGesture_Tapped: {0} ", m_TapGesture.ScreenPosition);

                TouchEventData tapEventToFire = null;

				foreach (var kp in m_TapEvents)
				{
				
					Ray rayForTab = m_mainCamera.ScreenPointToRay(m_TapGesture.ScreenPosition);
					RaycastHit hit;
					bool isHitOnLayer = Physics.Raycast(rayForTab, out hit, Mathf.Infinity, 1 << kp.Key);


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
					EventManager.Instance.SendEvent(tapEventToFire.TapCallback, m_TapGesture, tapEventToFire.Collider.transform);
					//tapEventToFire.TapCallback(m_TapGesture, tapEventToFire.Collider.transform);

			}
            
		}

		#endregion
			
	}

}
