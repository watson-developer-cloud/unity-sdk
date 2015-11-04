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
		private string m_GeocodeURL = "https://maps.googleapis.com/maps/api/geocode/json?address=";

		public delegate void OnGetLocation(GeolocationData data);
		private static fsSerializer sm_Serializer = new fsSerializer();

		public bool getLocation(string location, OnGetLocation callback)
		{
			GeolocationRequest req = new GeolocationRequest();
			req.Location = location;
			req.Callback = callback;
			req.OnResponse = GetGeolocationResponse;

			RESTConnector connector = new RESTConnector();
			connector.URL = m_GeocodeURL + WWW.EscapeURL(location);
			Log.Debug("Geolocation", "Sending geolocationRequest: " + connector.URL);

			return connector.Send(req);
		}

		private void GetGeolocationResponse(RESTConnector.Request req, RESTConnector.Response resp)
		{
			GeolocationRequest geoReq = req as GeolocationRequest;
			if (geoReq == null)
				throw new WatsonException("Wrong type of request object.");
			
			Log.Debug( "Geolocation", "Request completed in {0} seconds.", resp.ElapsedTime );



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


			if(resp.Success)
			{
				if(!geolocationData.ParseJson((IDictionary)Json.Deserialize(Encoding.UTF8.GetString(resp.Data))))
					Debug.Log("failed!!");
			}
			else
			{
				Log.Debug("Geolocation", "Fail: " + resp.Error);
			}



			Debug.Log("this object: " + obj["results"]);
		}

		/// <summary>
		/// Private Request object that holds data specific to the getLocation request.
		/// </summary>
		private class GeolocationRequest : RESTConnector.Request
		{
			public string Location { get; set; }
			public OnGetLocation Callback { get; set; }
		}

		public class GeolocationData
		{
			public AddressComponent[] AddressComponents { get; set; }
			public string FormattedAddress { get; set; }
			public Geometry Geometry { get; set; }
			public string PlaceID { get; set; }
			public string[] Types { get; set; }

			public bool ParseJson(IDictionary json)
			{
				Debug.Log("json: " + json);

				string status = (string)json["status"];
				Debug.Log("status: " + status);

//				IList iResults = (IList)json["results"];
//				Debug.Log("Formatted Address: " + iResults.IndexOf["formatted_address"]);

				try
				{
					return true;
					}
					catch (Exception e)
					{
					Log.Error("Geolocation", "Exception during parse: {0}", e.ToString());
				}

				return false;
			}
		};

		public class AddressComponent
		{
			public string LongName { get; set; }
			public string ShortName { get; set; }
			public string[] Types { get; set; }
		};

		public class Geometry
		{
			public Bounds Bounds { get; set; }
			public Location Location { get; set; }
			public string LocationType { get; set; }
			public Viewport[] Viewport { get; set; }
		};

		public class Bounds
		{
			public string BoundsString { get; set; }
			public float Latitude { get; set; }
			public float Longitude { get; set; }
		};

		public class Location
		{
			public float Latitude { get; set; }
			public float Longitude { get; set; }
		};

		public class Viewport
		{
			public string ViewportString { get; set; }
			public float Latitude { get; set; }
			public float Longitude { get; set; }
		};

	}
}