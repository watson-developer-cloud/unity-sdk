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
* @author Taj Santiago
*/

namespace IBM.Watson.Widgets.Question
{
	using UnityEngine;
	using System.Collections;
	using IBM.Watson.Logging;

	public class SideProjection : MonoBehaviour {
		[SerializeField]
		private RenderTexture m_RenderTexture;

		private Camera m_OrthographicCamera;

		void Awake()
		{
			if(!m_RenderTexture)
			{
				Log.Warning("SideProjection", "Render texture is not set on " + gameObject.name + " - disabling projection.");
				gameObject.SetActive(false);
				return;
			}

			m_OrthographicCamera = this.transform.GetComponentInChildren<Camera>();
			m_OrthographicCamera.targetTexture = m_RenderTexture;
		}
	}
}