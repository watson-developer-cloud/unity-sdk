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


using System;
using System.Reflection;
using UnityEngine;

namespace IBM.Watson.DeveloperCloud.Utilities
{
    /// <summary>
    /// This class allows for a delegate to be serialized for a component and method 
    /// on a given GameObject.
    /// </summary>
    [Serializable]
    public class SerializedDelegate
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="delegateType">The delegate type.</param>
        public SerializedDelegate(Type delegateType)
        {
            DelegateType = delegateType;
        }

        /// <summary>
        /// The delegate type of the method.
        /// </summary>
        public Type DelegateType { get; private set; }

        [SerializeField]
        GameObject _target = null;
        [SerializeField]
        string _component = null;
        [SerializeField]
        string _method = null;

        /// <summary>
        /// Target Game Object to invoke the callback under selected component
        /// </summary>
        public GameObject TargetGameObject { get { return _target; } }

        /// <summary>
        /// This resolves the actual delegate for invoke.
        /// </summary>
        /// <returns>Returns a delegate or null if the delegate can't be resolved.</returns>
        public Delegate ResolveDelegate()
        {
            if (_target == null)
                return null;
            Component component = _target.GetComponent(_component);
            if (component == null)
                return null;
#if NETFX_CORE
            MethodInfo info = component.GetType().GetMethod(_method, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
            if (info == null)
                return null;

            return info.CreateDelegate(DelegateType, component);
#else
            MethodInfo info = component.GetType().GetMethod(_method, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod);
            if (info == null)
                return null;

            return Delegate.CreateDelegate(DelegateType, component, info);
#endif
        }
    }
}
