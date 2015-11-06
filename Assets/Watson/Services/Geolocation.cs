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

namespace IBM.Watson.Services
{
	using UnityEngine;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Text;
	using IBM.Watson.Connection;
	using IBM.Watson.Utilities;
	using IBM.Watson.Logging;
	using MiniJSON;
	using FullSerializer;

	/// <summary>
	/// This class converts location data into latitude and longitude
	/// to be displayed in the cube location map.
	/// </summary>
	public class Geolocation {
		private string m_GeocodeURL = "http://geocode.arcgis.com/arcgis/rest/services/World/GeocodeServer/find?f=pjson&text=";

		public delegate void OnGetLocation(GeolocationData data);
		private static fsSerializer sm_Serializer = new fsSerializer();

		/// <summary>
		/// Sets up and calls ERSI geolocation api.
		/// </summary>
		/// <returns><c>true</c>, if location was gotten, <c>false</c> otherwise.</returns>
		/// <param name="location">Location.</param>
		/// <param name="token">Token.</param>
		/// <param name="callback">Callback.</param>
		public bool getLocation(string location, string token, OnGetLocation callback)
		{
			GeolocationRequest req = new GeolocationRequest();
			req.Location = location;
			req.Token = token;
			req.Callback = callback;
			req.OnResponse = GetGeolocationResponse;

			RESTConnector connector = new RESTConnector();
			connector.URL = m_GeocodeURL + WWW.EscapeURL(location);
			Log.Debug("Geolocation", "Sending geolocationRequest: " + connector.URL);

			return connector.Send(req);
		}

		/// <summary>
		/// Response for REST call. Converts response data into GeolocationData object.
		/// </summary>
		/// <param name="req">Req.</param>
		/// <param name="resp">Resp.</param>
		private void GetGeolocationResponse(RESTConnector.Request req, RESTConnector.Response resp)
		{
			GeolocationRequest geoReq = req as GeolocationRequest;
			if (geoReq == null)
				throw new WatsonException("Wrong type of request object.");
			
			Log.Debug( "Geolocation", "Request completed in {0} seconds.", resp.ElapsedTime );

			if(resp.Success)
			{
				GeolocationData geolocationData = new GeolocationData();

				fsData data = null;
				fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
				if (!r.Succeeded)
					throw new WatsonException(r.FormattedMessages);

				object obj = geolocationData;
				r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
				if (!r.Succeeded)
					throw new WatsonException(r.FormattedMessages);

				if (((GeolocationRequest)req).Callback != null)
					((GeolocationRequest)req).Callback(resp.Success ? geolocationData : null);
			}
			else
			{
				Log.Debug("Geolocation", "Fail: " + resp.Error);
			}
		}

		/// <summary>
		/// Private Request object that holds data specific to the getLocation request.
		/// </summary>
		private class GeolocationRequest : RESTConnector.Request
		{
			public string Location { get; set; }
			public string Token { get; set; }
			public OnGetLocation Callback { get; set; }
		}

		public class GeolocationData
		{
			public spatialReference spatialReference { get; set; }
			public locations[] locations { get; set; }
		};


		public class spatialReference
		{
			public int wkid { get; set; }
			public int latestWkid { get; set; }
		}

		public class locations
		{
			public string name { get; set; }
			public extent extent { get; set; }
			public feature feature { get; set; }
		}

		public class extent
		{
			public float xmin { get; set; }
			public float ymin { get; set; }
			public float xmax { get; set; }
			public float ymax { get; set; }
		}

		public class feature
		{
			public geometry geometry { get; set; }
			public attributes attributes { get; set; }
		}

		public class geometry
		{
			public float x { get; set; }
			public float y { get; set; }
		};

		public class attributes
		{
			public float Score { get; set; }
			public string Addr_Type { get; set; }
		}

	}
}