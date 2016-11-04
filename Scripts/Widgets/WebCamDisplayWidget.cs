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
using IBM.Watson.DeveloperCloud.DataTypes;
using System;
using UnityEngine.UI;
using IBM.Watson.DeveloperCloud.Logging;

#pragma warning disable 0414

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
    [SerializeField]
    private RawImage m_RawImage;
    [SerializeField]
    private Material m_Material;

    private WebCamTexture m_WebCamTexture;
    private int m_RequestedWidth;
    private int m_RequestedHeight;
    private int m_RequestedFPS;
    #endregion

    #region Constants
    #endregion

    #region Public Properties
    /// <summary>
    /// The Raw Image displaying the WebCam stream in UI.
    /// </summary>
    public RawImage RawImage
    {
      get { return m_RawImage; }
      set { m_RawImage = value; }
    }
    /// <summary>
    /// The Material displaying the WebCam stream on Geometry.
    /// </summary>
    public Material Material
    {
      get { return m_Material; }
    }
    #endregion

    #region Public Functions
    #endregion

    #region Widget Interface
    protected override string GetName()
    {
      return "WebCamDisplay";
    }
    #endregion

    #region Private Functions
    #endregion

    #region Event Handlers
    private void OnWebCamTexture(Data data)
    {
      Log.Debug("WebCamDisplayWidget", "OnWebCamTexture()");
      if (Material == null && RawImage == null)
        throw new ArgumentNullException("A Material or RawImage is required to display WebCamTexture");

      WebCamTextureData webCamTextureData = (WebCamTextureData)data;
      m_WebCamTexture = webCamTextureData.CamTexture;
      m_RequestedWidth = webCamTextureData.RequestedWidth;
      m_RequestedHeight = webCamTextureData.RequestedHeight;
      m_RequestedFPS = webCamTextureData.RequestedFPS;

      if (Material != null)
        Material.mainTexture = m_WebCamTexture;

      if (RawImage != null)
      {
        RawImage.texture = m_WebCamTexture;
        RawImage.material.mainTexture = m_WebCamTexture;
      }
      if (!m_WebCamTexture.isPlaying)
        m_WebCamTexture.Play();
    }
    #endregion
  }
}
