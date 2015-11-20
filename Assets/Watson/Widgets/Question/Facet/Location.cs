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

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using IBM.Watson.Logging;
using IBM.Watson.Utilities;
using IBM.Watson.Data;

namespace IBM.Watson.Widgets.Question
{
	/// <summary>
	/// Handles all Location Facet functionality.
	/// </summary>
    public class Location : Facet
    {
        [SerializeField]
        private Text m_LocationText;

		[SerializeField]
		private RectTransform m_MapDotRectTransform;

        private string m_LocationString;
        public string LocationString
        {
            get { return m_LocationString; }
            set
            {
                m_LocationString = value;
                if (m_LocationText != null )
					UpdateLocation();
            }
        }

		/// <summary>
		/// Location latitude. Updates the dot on map texture
		/// </summary>
		private float m_Latitude = 0f;
		public float Latitude
		{
			get { return m_Latitude; }
			set
			{
				m_Latitude = value;
			}
		}

		/// <summary>
		/// Location longitude. Updates the dot on map texture
		/// </summary>
		private float m_Longitude = 0f;
		public float Longitude
		{
			get { return m_Longitude; }
			set
			{
				m_Longitude = value;
			}
		}

		private string m_LocationData = null;
		
		private void OnEnable()
		{
			EventManager.Instance.RegisterEventReceiver( Constants.Event.ON_QUESTION_LOCATION, OnLocationData );
		}
		
		private void OnDisable()
		{
			EventManager.Instance.UnregisterEventReceiver( Constants.Event.ON_QUESTION_LOCATION, OnLocationData );
		}

        /// <summary>
        /// Update the Location view.
        /// </summary>
        private void UpdateLocation()
        {
            m_LocationText.text = LocationString;
        }

		/// <summary>
		/// Updates the map based on Latitude and Longitude.
		/// </summary>
		private void UpdateMap()
		{

		}

		/// <summary>
		/// Callback for Location data.
		/// </summary>
		/// <param name="args">Arguments.</param>
		private void OnLocationData( object [] args )
		{
            if ( Focused )
            {
			    m_LocationData = args != null && args.Length > 0 ? args[0] as string : null;
			    LocationString = m_LocationData;
		    }
        }
    }
}
