using UnityEngine;
using System.Collections;
using IBM.Watson.DeveloperCloud.DataTypes;

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
namespace IBM.Watson.DeveloperCloud.Widgets
{
	/// <summary>
	/// This widget displays WebCam video.
	/// </summary>
	public class WebCamDisplayWidget : Widget
	{
		#region Inputs
		[SerializeField]
		private Input m_WebCamTextureInput = new Input("WebCamTexture", typeof(WebCamTextureData), "OnWebCamTexture");
		#endregion

		#region Outputs
		#endregion

		#region Private Data
		#endregion

		#region Constants
		#endregion

		#region Public Properties
		#endregion

		#region Public Functions
		#endregion

		#region Widget Interface
		protected override string GetName()
		{
			return "WebCamDisplay";
		}
		#endregion

		#region Event Handlers
		#endregion
	}
}
