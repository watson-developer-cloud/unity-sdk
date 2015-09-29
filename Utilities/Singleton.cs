#define SINGLETONS_VISIBLE

using UnityEngine;
using System;

namespace IBM.Watson.Utilities
{
    class Singleton<T> where T:class
    {
        #region Private Data
        static private T sm_Instance = null;
        #endregion

        #region Public Properties
        public static T Instance
        {
            get {
                if ( sm_Instance == null )
                    CreateInstance();
                return sm_Instance;
            }
        }
        #endregion

        #region Singleton Creation
        private static void CreateInstance()
        {
            if ( typeof(MonoBehaviour).IsAssignableFrom( typeof(T) ) )
            {
                GameObject singletonObject = new GameObject( "_" + typeof(T).Name );
#if SINGLETONS_VISIBLE
                singletonObject.hideFlags = HideFlags.DontSave;
#else
                singletonObject.hideFlags = HideFlags.HideAndDontSave;
#endif
                sm_Instance = singletonObject.AddComponent( typeof(T) ) as T;
            }
            else
            {
                sm_Instance = Activator.CreateInstance( typeof(T) ) as T;
            }

            if ( sm_Instance == null )
                throw new Exception( "Failed to create instance " + typeof(T).Name );
        }
        #endregion
    }
}
