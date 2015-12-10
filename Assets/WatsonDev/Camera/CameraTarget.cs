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
using IBM.Watson.Logging;
using IBM.Watson.Utilities;
using IBM.Watson.Widgets.Question;

namespace IBM.Watson.Camera
{

	public class CameraTarget : MonoBehaviour {

		#region Private Members

		[SerializeField]
		private bool m_UseTargetPosition;
		[SerializeField]
		private bool m_UseTargetRotation;

		private UnityEngine.Camera m_WatsonCamera = null;
		private bool m_UseCustomPosition = false;
		private Vector3 m_CustomPosition = Vector3.zero;

		private bool m_UseCustomRotation = false;
		private Quaternion m_CustomRotation = Quaternion.identity;
		private bool m_UseTargetObjectToRotate = false;
		private GameObject m_CustomTargetObjectToLookAt = null;

		#endregion 


		#region Public Members

		public bool UseTargetPosition{
			get{
				return m_UseTargetPosition;
			}
		}

		public Vector3 TargetPosition{
			get{
	
				if(m_CustomPosition != Vector3.zero){
					return m_CustomPosition;
				}
				else{
					return transform.position;
				}
			}
			set{
				m_UseCustomPosition = true;
				m_CustomPosition = value;
			}
		}

		public bool UseTargetRotation{
			get{
				return m_UseTargetRotation;
			}
		}

		public Quaternion TargetRotation{
			get{
				if(m_UseCustomRotation)
				{
					return m_CustomRotation;
				}
				else if(m_UseTargetObjectToRotate)
				{
					if(TargetObject != null)
					{
						if(WatsonCamera != null)
						{
							Vector3 relativePos = TargetObject.transform.position - WatsonCamera.transform.position;
							return Quaternion.LookRotation(relativePos);
						}
						else{
							Log.Warning("CameraTarget", "WatsonCamera couldn't find");
							return Quaternion.identity;
						}
					}
					else{
						Log.Warning("CameraTarget", "TargetObject couldn't find");
						return Quaternion.identity;
					}
				}
				else{
					return transform.rotation;
				}
			}
			set{
				m_UseCustomRotation = true;
				m_CustomRotation = value;
			}
		}

		protected GameObject TargetObject{
			get{
				return m_CustomTargetObjectToLookAt;
			}
			set{
				m_UseTargetObjectToRotate = true;
				m_CustomTargetObjectToLookAt = value;
			}
		}

		protected UnityEngine.Camera WatsonCamera{
			get{
				if(m_WatsonCamera == null){
					WatsonCamera watsonCameraComponent = GameObject.FindObjectOfType<WatsonCamera>();
					if(watsonCameraComponent != null)
						m_WatsonCamera = watsonCameraComponent.GetComponent<UnityEngine.Camera>();
				}
				return m_WatsonCamera;
			}
		}

		#endregion

		#region OnStart / Update
		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}
		#endregion
	}

}
