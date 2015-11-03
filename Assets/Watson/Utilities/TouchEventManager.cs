using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;
using IBM.Watson.Logging;

namespace IBM.Watson.Utilities
{
	[RequireComponent (typeof (TapGesture))]
	public class TouchEventManager : MonoBehaviour {



		#region Public Types
		public delegate void TapEventDelegate(TapGesture tapGesture, Transform hitTransform);
		public delegate void DragEventDelegate(ScreenTransformGesture transformGesture);
		#endregion

		public class TouchEventData
		{
			private Collider m_Collider;
			private Collider[] m_ColliderList;
			private GameObject m_GameObject;
			private TapEventDelegate m_tapEventCallback;
			private DragEventDelegate m_dragEventCallback;
			private bool m_isInside;
			private int m_SortingLayer;

			public Collider Collider { get { return m_Collider; } }
			public bool IsInside{ get { return m_isInside; } }
			public TapEventDelegate TapCallback{ get { return m_tapEventCallback; } }
			public DragEventDelegate DragCallback{ get { return m_dragEventCallback; } }
			public int SortingLayer{ get { return m_SortingLayer; } }

			public TouchEventData(Collider collider, TapEventDelegate callback, int sortingLayer, bool isInside){
				m_Collider = collider;
				m_tapEventCallback = callback;
				m_SortingLayer = sortingLayer;
				m_isInside = isInside;
			}

			public TouchEventData(GameObject gameObject, DragEventDelegate callback, int sortingLayer, bool isInside){
				m_GameObject = gameObject;
				m_ColliderList = gameObject.GetComponentsInChildren<Collider>();
				m_dragEventCallback = callback;
				m_SortingLayer = sortingLayer;
				m_isInside = isInside;
			}

			public override bool Equals (object obj)
			{
				bool isEqual = false;
				TouchEventData touchEventData = obj as TouchEventData;
				if (touchEventData != null) 
				{
					isEqual = (touchEventData.Collider == this.Collider && 
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

		}

		#region Private Data
		private UnityEngine.Camera m_mainCamera;

		private bool m_Active = true;
		private bool m_UpdateActivate = true;
		private Dictionary<int, List<TouchEventData>> m_TapEvents = new Dictionary<int, List<TouchEventData>>();
		private Dictionary<int, List<TouchEventData>> m_DragEvents = new Dictionary<int, List<TouchEventData>>();
		#endregion

		#region Serialized Private 
		[SerializeField]
		private TapGesture m_TapGesture;
		[SerializeField]
		private ScreenTransformGesture m_OneFingerManipulationGesture;
		[SerializeField]
		private ScreenTransformGesture m_TwoFingerMoveGesture;
		#endregion

		#region Public Properties
		/// <summary>
		/// Set/Get the active state of this manager.
		/// </summary>
		public bool Active { get { return m_Active; } set { m_UpdateActivate = value; } }

		private static TouchEventManager sm_Instance = null;
		/// <summary>
		/// The current instance of the TouchEventManager.
		/// </summary>
		public static TouchEventManager Instance { get { if(sm_Instance != null) return sm_Instance; return Singleton<TouchEventManager>.Instance; } }
		#endregion

		#region OnEnable / OnDisable

		void Awake(){
			sm_Instance = this;
		}

		private void OnEnable()
		{
			m_mainCamera = UnityEngine.Camera.main;
			m_TapGesture.Tapped += TapGesture_Tapped;

			m_OneFingerManipulationGesture.Transformed += OneFingerManipulationTransformedHandler;
			m_TwoFingerMoveGesture.Transformed += TwoFingerTransformedHandler;
		}
		
		private void OnDisable()
		{
			m_TapGesture.Tapped += TapGesture_Tapped;

			m_OneFingerManipulationGesture.Transformed += OneFingerManipulationTransformedHandler;
			m_TwoFingerMoveGesture.Transformed += TwoFingerTransformedHandler;
		}


		#endregion

		#region OneFinger Events - Register / UnRegister / Call

		public bool RegisterDragEvent(GameObject gameObjectToDrag, DragEventDelegate callback, int numberOfFinger = 1, int SortingLayer = 0, bool isDragInside = true)
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
		
		public bool UnregisterDragEvent(GameObject gameObjectToDrag, DragEventDelegate callback, int numberOfFinger = 1, int SortingLayer = 0, bool isDragInside = true)
		{
			bool success = false;
			
			if (m_DragEvents.ContainsKey (numberOfFinger)) 
			{
				success = m_DragEvents[numberOfFinger].Remove(  new TouchEventData(gameObjectToDrag, callback, SortingLayer, isDragInside) );
			} 
			
			return success;
		}

		private void OneFingerManipulationTransformedHandler(object sender, System.EventArgs e)
		{
			Log.Status ("TouchEventManager", "oneFingerManipulationTransformedHandler: {0}", m_OneFingerManipulationGesture.DeltaPosition);
			if (m_Active) {
				TouchEventData dragEventToFire = null;
					
				foreach (var kp in m_DragEvents) {
					if (kp.Key == 1) {

						for (int i = 0; i < kp.Value.Count; ++i) {
							TouchEventData dragEventData = kp.Value [i];
								
							if (dragEventData.DragCallback == null) {
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

				if(dragEventToFire != null && dragEventToFire.DragCallback != null)
					dragEventToFire.DragCallback(m_OneFingerManipulationGesture);

			}
			
			
			// update our active flag AFTER we check the active flag, this prevents
			// us from responding the key events during the same frame as we activate
			// this manager.
			m_Active = m_UpdateActivate;
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
							
							if (dragEventData.DragCallback == null) {
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
				
				if(dragEventToFire != null && dragEventToFire.DragCallback != null)
					dragEventToFire.DragCallback(m_TwoFingerMoveGesture);
				
			}

			// update our active flag AFTER we check the active flag, this prevents
			// us from responding the key events during the same frame as we activate
			// this manager.
			m_Active = m_UpdateActivate;
		}


		#endregion

		#region TapEvents - Register / UnRegister / Call 

		public bool RegisterTapEvent(GameObject gameObjectToTouch, TapEventDelegate callback, int SortingLayer = 0, bool isTapInside = true)
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

		public bool UnregisterTapEvent(GameObject gameObjectToTouch, TapEventDelegate callback, int SortingLayer = 0, bool isTapInside = true)
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
				TouchEventData tapEventToFire = null;

				foreach (var kp in m_TapEvents)
				{
				
					Ray rayForTab = m_mainCamera.ScreenPointToRay(m_TapGesture.ScreenPosition);
					RaycastHit hit;
					bool isHitOnLayer = Physics.Raycast(rayForTab, out hit, Mathf.Infinity, 1 << kp.Key);


					for (int i = 0; i < kp.Value.Count; ++i)
					{
						TouchEventData tapEventData = kp.Value[i];

						if (tapEventData.TapCallback == null)
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

				if(tapEventToFire != null && tapEventToFire.TapCallback != null)
					tapEventToFire.TapCallback(m_TapGesture, tapEventToFire.Collider.transform);

			}

			// update our active flag AFTER we check the active flag, this prevents
			// us from responding the key events during the same frame as we activate
			// this manager.
			m_Active = m_UpdateActivate;
		}

		#endregion
			
	}

}
