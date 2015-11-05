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

namespace IBM.Watson.Widgets.Question
{
	/// <summary>
	/// Handles all Location Facet functionality.
	/// </summary>
    public class Location : Base
    {
        [SerializeField]
        private Text m_LocationText;

        private string m_LocationString;
        public string LocationString
        {
            get { return m_LocationString; }
            set
            {
                m_LocationString = value;
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
				UpdateMap ();
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
				UpdateMap ();
			}
		}

		/// <summary>
		/// Set LocationString from data.
		/// </summary>
        override public void Init()
        {
			base.Init ();

            LocationString = m_Question.QuestionData.Location;
            UpdateLocation();
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
    }
}
