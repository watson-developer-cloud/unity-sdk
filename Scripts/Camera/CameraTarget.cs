﻿/**
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
using IBM.Watson.DeveloperCloud.Logging;

namespace IBM.Watson.DeveloperCloud.Camera
{
  /// <summary>
  /// Camera target class to identify the camera target to follow the position and rotation
  /// </summary>
  public class CameraTarget : MonoBehaviour
  {

    #region protected Members

    protected WatsonCamera mp_WatsonCamera = null;
    protected UnityEngine.Camera mp_CameraAttached = null;
    [SerializeField]
    protected bool m_UseCustomPosition = false;
    [SerializeField]
    protected Vector3 m_CustomPosition = Vector3.zero;
    [SerializeField]
    protected Vector3 m_OffsetPosition = Vector3.zero;
    protected Quaternion m_OffsetPositionRotation = Quaternion.identity;
    [SerializeField]
    protected bool m_UseCustomRotation = false;
    protected Quaternion m_CustomRotation = Quaternion.identity;
    protected bool m_UseTargetObjectToRotate = false;
    [SerializeField]
    protected GameObject m_CustomTargetObjectToLookAt = null;
    [SerializeField]
    protected GameObject m_CameraPathRootObject = null;
    [SerializeField]
    protected float m_RatioAtCameraPath = 0.0f;
    [SerializeField]
    protected Vector3 m_DistanceFromCamera = Vector3.zero;

    protected Transform[] m_PathTransforms;

    [SerializeField]
    protected bool m_TextEnableCamera = false;
    [SerializeField]
    protected bool m_TestToMakeItCurrent = false;

    #endregion

    #region Public Members

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="IBM.Watson.DeveloperCloud.Camera.CameraTarget"/> use
    /// custom position.
    /// </summary>
    /// <value><c>true</c> if use custom position; otherwise, <c>false</c>.</value>
    public virtual bool UseCustomPosition
    {
      get
      {
        return m_UseCustomPosition;
      }
      set
      {
        m_UseCustomPosition = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="IBM.Watson.DeveloperCloud.Camera.CameraTarget"/> use
    /// custom rotation.
    /// </summary>
    /// <value><c>true</c> if use custom rotation; otherwise, <c>false</c>.</value>
    public virtual bool UseCustomRotation
    {
      get
      {
        return m_UseCustomRotation;
      }
      set
      {
        m_UseCustomRotation = value;
      }
    }

    /// <summary>
    /// Gets or sets the ratio at camera path. It is used if there is path root object assigned to the system
    /// </summary>
    /// <value>The ratio at camera path.</value>
    public virtual float RatioAtCameraPath
    {
      get
      {
        return m_RatioAtCameraPath;
      }
      set
      {
        m_RatioAtCameraPath = Mathf.Repeat(value, 1.0f);
      }
    }

    /// <summary>
    /// Gets or sets the camera path root object.
    /// </summary>
    /// <value>The camera path root object.</value>
    public virtual GameObject CameraPathRootObject
    {
      get
      {
        return m_CameraPathRootObject;
      }
      set
      {
        m_CameraPathRootObject = value;
      }
    }

    public virtual Vector3 OffsetPosition
    {
      get
      {
        return m_OffsetPosition;
      }
      set
      {
        m_OffsetPosition = value;
      }
    }

    public virtual Vector3 DistanceFromCamera
    {
      get
      {
        return m_DistanceFromCamera;
      }
      set
      {
        m_DistanceFromCamera = value;
      }
    }

    public virtual Quaternion OffsetPositionRotation
    {
      get
      {
        return m_OffsetPositionRotation;
      }
      set
      {
        m_OffsetPositionRotation = value;
      }
    }

    /// <summary>
    /// Gets or sets the target position.
    /// </summary>
    /// <value>The target position.</value>
    public virtual Vector3 TargetPosition
    {
      get
      {
        if (m_UseCustomPosition)
        {
          return m_CustomPosition;
        }
        else if (m_OffsetPosition != Vector3.zero)
        {
          return transform.position + (Quaternion.Euler(transform.rotation.eulerAngles - m_OffsetPositionRotation.eulerAngles) * m_OffsetPosition);
        }
        else
        {
          return transform.position;
        }
      }
      set
      {
        m_UseCustomPosition = true;
        m_CustomPosition = value;
      }
    }

    /// <summary>
    /// Gets or sets the target rotation.
    /// </summary>
    /// <value>The target rotation.</value>
    public virtual Quaternion TargetRotation
    {
      get
      {
        if (m_UseTargetObjectToRotate)
        {
          if (TargetObject != null)
          {
            if (CameraAttached != null)
            {
              Vector3 relativePos = TargetObject.transform.position - CameraAttached.transform.position;
              if (relativePos != Vector3.zero)
                return Quaternion.LookRotation(relativePos);
              else
                return Quaternion.identity;
            }
            else
            {
              Log.Warning("CameraTarget", "WatsonCamera couldn't find");
              return Quaternion.identity;
            }
          }
          else
          {
            Log.Warning("CameraTarget", "TargetObject couldn't find");
            return Quaternion.identity;
          }
        }
        else if (m_UseCustomRotation)
        {
          return m_CustomRotation;
        }
        else
        {
          return transform.rotation;
        }
      }
      set
      {
        m_UseCustomRotation = true;
        m_CustomRotation = value;
      }
    }

    /// <summary>
    /// Gets or sets the target object.
    /// </summary>
    /// <value>The target object.</value>
    public virtual GameObject TargetObject
    {
      get
      {
        return m_CustomTargetObjectToLookAt;
      }
      set
      {
        if (value != null)
        {
          m_UseTargetObjectToRotate = true;
          m_CustomTargetObjectToLookAt = value;
        }
        else
        {
          m_UseTargetObjectToRotate = false;
          m_CustomTargetObjectToLookAt = null;
        }
      }
    }

    /// <summary>
    /// Gets the camera attached.
    /// </summary>
    /// <value>The camera attached.</value>
    public virtual UnityEngine.Camera CameraAttached
    {
      get
      {
        if (mp_CameraAttached == null)
        {
          if (WatsonCameraAttached != null)
            mp_CameraAttached = WatsonCameraAttached.GetComponent<UnityEngine.Camera>();
        }
        return mp_CameraAttached;
      }
    }

    /// <summary>
    /// Gets the watson camera attached.
    /// </summary>
    /// <value>The watson camera attached.</value>
    public virtual WatsonCamera WatsonCameraAttached
    {
      get
      {
        if (mp_WatsonCamera == null)
        {
          //Check if is there any local camera attached
          mp_WatsonCamera = this.gameObject.GetComponent<WatsonCamera>();

          if (mp_WatsonCamera == null)
          {
            mp_WatsonCamera = GameObject.FindObjectOfType<WatsonCamera>();
          }
        }
        return mp_WatsonCamera;
      }
    }

    #endregion

    #region Set Target on Camera

    /// <summary>
    /// Sets the current target on camera.
    /// </summary>
    /// <param name="enable">If set to <c>true</c> enable.</param>
    public virtual void SetCurrentTargetOnCamera(bool enable)
    {
      if (WatsonCamera.Instance != null)
      {
        if (enable)
          WatsonCamera.Instance.CurrentCameraTarget = this;
        else
          WatsonCamera.Instance.CurrentCameraTarget = null;
      }
    }

    /// <summary>
    /// Sets the target position default.
    /// </summary>
    public virtual void SetTargetPositionDefault()
    {
      if (WatsonCamera.Instance != null && WatsonCamera.Instance.DefaultCameraTarget != null)
      {
        TargetPosition = WatsonCamera.Instance.DefaultCameraTarget.TargetPosition;
      }
    }

    /// <summary>
    /// Sets the target rotation default.
    /// </summary>
    public virtual void SetTargetRotationDefault()
    {
      if (WatsonCamera.Instance != null && WatsonCamera.Instance.DefaultCameraTarget != null)
      {
        TargetRotation = WatsonCamera.Instance.DefaultCameraTarget.TargetRotation;
      }
    }

    #endregion

    #region Update

    void Update()
    {
      if (m_TestToMakeItCurrent)
      {
        m_TestToMakeItCurrent = false;
        SetCurrentTargetOnCamera(m_TextEnableCamera);
      }
    }

    #endregion

    #region public Functions

    /// <summary>
    /// Sets the target position with offset.
    /// </summary>
    /// <param name="offsetPosition"></param>
    public virtual void SetTargetPositionWithOffset(Vector3 offsetPosition)
    {
      m_OffsetPosition = offsetPosition;
      m_OffsetPositionRotation = this.transform.rotation;
    }

    #endregion

  }

}
