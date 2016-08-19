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
using System;
using IBM.Watson.DeveloperCloud.DataTypes;
using IBM.Watson.DeveloperCloud.Logging;

#pragma warning disable 414

namespace IBM.Watson.DeveloperCloud.Widgets
{
    public class WebCamWidget : Widget
    {
        #region Inputs
        [SerializeField]
        private Input m_DisableInput = new Input("Disable", typeof(DisableWebCamData), "OnDisableInput");
        #endregion

        #region Outputs
        //  Byte[]
        //  WebCamTexture
        #endregion

        #region Private Data
        private bool m_Active = false;
        private bool m_Disabled = false;

        [SerializeField]
        private int m_RequestedWidth;
        [SerializeField]
        private int m_RequestedHeight;
        [SerializeField]
        private int m_RequestedFPS;
        [SerializeField]
        private float m_SendInterval;
        
        #endregion

        #region Public Properties
        /// <summary>
        /// True if microphone is disabled, false if enabled.
        /// </summary>
        public bool Disable
        {
            get { return m_Disabled; }
            set
            {
                if (m_Disabled != value)
                {
                    m_Disabled = value;
                    if (m_Active && !m_Disabled)
                        StartRecording();
                    else
                        StopRecording();
                }
            }
        }

        public WebCamDevice[] Devices
        {
            get { return WebCamTexture.devices; }
        }


        #endregion

        #region EventHandlers
        protected override void Start()
        {
            base.Start();
            Application.RequestUserAuthorization(UserAuthorization.WebCam);
        }

        private void OnDisableInput(Data data)
        {
            Disable = ((DisableWebCamData)data).Boolean;
        }
        #endregion

        #region Widget Interface
        protected override string GetName()
        {
            return "WebCam";
        }
        #endregion

        #region Recording Functions
        private void StartRecording()
        {
            Log.Debug("WebCamWidget", "StartRecording();");
        }

        private void StopRecording()
        {
            Log.Debug("WebCamWidget", "StopRecording();");
        }
        #endregion
    }
}
