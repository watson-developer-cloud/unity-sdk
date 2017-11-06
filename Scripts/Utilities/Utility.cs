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


using FullSerializer;
using IBM.Watson.DeveloperCloud.Logging;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using IBM.Watson.DeveloperCloud.Connection;
#if NETFX_CORE
using System.Reflection;
#endif

namespace IBM.Watson.DeveloperCloud.Utilities
{
    /// <summary>
    /// Utility functions.
    /// </summary>
    static public class Utility
    {
        private static fsSerializer _serializer = new fsSerializer();
        private static string _macAddress = null;

        /// <summary>
        /// This helper functions returns all Type's that inherit from the given type.
        /// </summary>
        /// <param name="type">The Type to find all types that inherit from the given type.</param>
        /// <returns>A array of all Types that inherit from type.</returns>
        public static Type[] FindAllDerivedTypes(Type type)
        {
            List<Type> types = new List<Type>();
#if NETFX_CORE
            foreach (var t in type.GetTypeInfo().Assembly.GetTypes())
            {
                if (t == type || t.GetTypeInfo().IsAbstract)
                    continue;
                if (type.IsAssignableFrom(t))
                    types.Add(t);
            }
#else
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var t in assembly.GetTypes())
                {
                    if (t == type || t.IsAbstract)
                        continue;
                    if (type.IsAssignableFrom(t))
                        types.Add(t);
                }
            }
#endif

            return types.ToArray();
        }

        /// <summary>
        /// Approximately the specified a, b and tolerance.
        /// </summary>
        /// <param name="a">The first component.</param>
        /// <param name="b">The second component.</param>
        /// <param name="tolerance">Tolerance.</param>
        public static bool Approximately(double a, double b, double tolerance = 0.0001)
        {
            return (System.Math.Abs(a - b) < tolerance);
        }

        /// <summary>
        /// Approximately the specified a, b and tolerance.
        /// </summary>
        /// <param name="a">The first component.</param>
        /// <param name="b">The second component.</param>
        /// <param name="tolerance">Tolerance.</param>
        public static bool Approximately(float a, float b, float tolerance = 0.0001f)
        {
            return (Mathf.Abs(a - b) < tolerance);
        }

        /// <summary>
        /// Approximately the specified a, b and tolerance.
        /// </summary>
        /// <param name="a">The first component.</param>
        /// <param name="b">The second component.</param>
        /// <param name="tolerance">Tolerance.</param>
        public static bool Approximately(Vector3 a, Vector3 b, float tolerance = 0.0001f)
        {
            return Approximately(a.x, b.x, tolerance) && Approximately(a.y, b.y, tolerance) && Approximately(a.z, b.z, tolerance);
        }

        /// <summary>
        /// Approximately Quaternion with the specified a, b and tolerance.
        /// </summary>
        /// <param name="a">The first component.</param>
        /// <param name="b">The second component.</param>
        /// <param name="tolerance">Tolerance.</param>
        public static bool Approximately(Quaternion a, Quaternion b, float tolerance = 0.0001f)
        {
            return
                (Approximately(a.eulerAngles.x, b.eulerAngles.x, tolerance) || Approximately((a.eulerAngles.x < 0 ? a.eulerAngles.x + 360.0f : a.eulerAngles.x), (b.eulerAngles.x < 0 ? b.eulerAngles.x + 360.0f : b.eulerAngles.x), tolerance)) &&
                    (Approximately(a.eulerAngles.y, b.eulerAngles.y, tolerance) || Approximately((a.eulerAngles.y < 0 ? a.eulerAngles.y + 360.0f : a.eulerAngles.y), (b.eulerAngles.y < 0 ? b.eulerAngles.y + 360.0f : b.eulerAngles.y), tolerance)) &&
                    (Approximately(a.eulerAngles.z, b.eulerAngles.z, tolerance) || Approximately((a.eulerAngles.z < 0 ? a.eulerAngles.z + 360.0f : a.eulerAngles.z), (b.eulerAngles.z < 0 ? b.eulerAngles.z + 360.0f : b.eulerAngles.z), tolerance));
        }

        /// <summary>
        /// Finds the object in child of parent object by name of child
        /// </summary>
        /// <returns>The object.</returns>
        /// <param name="parent">Parent Object.</param>
        /// <param name="nameChild">Name child.</param>
        public static GameObject FindObject(GameObject parent, string nameChild)
        {
            GameObject childObject = null;

            string[] childPath = nameChild.Split('/');

            for (int i = 0; i < childPath.Length; i++)
            {
                string childTransformName = childPath[i];

                Transform[] childerenTransform = parent.GetComponentsInChildren<Transform>(includeInactive: true);
                if (childerenTransform != null)
                {
                    foreach (Transform item in childerenTransform)
                    {
                        if (string.Equals(item.name, childTransformName))
                        {
                            if (i == childPath.Length - 1)
                            {
                                childObject = item.gameObject;
                            }
                            else
                            {
                                parent = item.gameObject;
                            }
                            break;
                        }
                    }
                }
            }
            return childObject;
        }

        /// <summary>
        /// Finds the objects in children of parent object by name of child
        /// </summary>
        /// <returns>The objects.</returns>
        /// <param name="parent">Parent.</param>
        /// <param name="nameChild">Name child.</param>
        /// <param name="isContains">Check string contains the name instead of equality.</param>
        /// <param name="sortByName">If true, children will be returned sorted by their name.</param>
        public static T[] FindObjects<T>(GameObject parent, string nameChild, bool isContains = false, bool sortByName = false, bool includeInactive = true) where T : Component
        {
            T[] childObjects = null;
            List<T> listGameObject = new List<T>();

            string[] childPath = nameChild.Split('/');

            for (int i = 0; i < childPath.Length; i++)
            {
                string childTransformName = childPath[i];
                T[] childerenTransform = parent.GetComponentsInChildren<T>(includeInactive: includeInactive);
                if (childerenTransform != null)
                {
                    foreach (T item in childerenTransform)
                    {
                        if ((isContains && item.name.Contains(childTransformName)) || string.Equals(item.name, childTransformName))
                        {
                            if (i == childPath.Length - 1)
                            {
                                listGameObject.Add(item);
                            }
                            else
                            {
                                parent = item.gameObject;
                            }
                        }
                    }
                }
            }

            if (listGameObject.Count > 0)
            {
                if (sortByName)
                {
                    listGameObject.Sort((x, y) => x.name.CompareTo(y.name));
                }
                childObjects = listGameObject.ToArray();
            }

            return childObjects;
        }

        /// <summary>
        /// Get the MD5 hash of a string.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string GetMD5(string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            MD5 md5 = new MD5CryptoServiceProvider();
#if NETFX_CORE
            byte[] data = Encoding.GetEncoding(0).GetBytes(s);
#else
            byte[] data = Encoding.Default.GetBytes(s);
#endif
            byte[] result = md5.ComputeHash(data);

            StringBuilder output = new StringBuilder();
            foreach (var b in result)
                output.Append(b.ToString("x2"));

            return output.ToString();
        }

        /// <summary>
        /// Removes any tags from a string. (e.g. <title></title>)
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string RemoveTags(string s, char tagStartChar = '<', char tagEndChar = '>')
        {
            if (!string.IsNullOrEmpty(s))
            {
                int tagStart = s.IndexOf(tagStartChar);
                while (tagStart >= 0)
                {
                    int tagEnd = s.IndexOf(tagEndChar, tagStart);
                    if (tagEnd < 0)
                        break;

                    string pre = tagStart > 0 ? s.Substring(0, tagStart) : string.Empty;
                    string post = tagEnd < s.Length ? s.Substring(tagEnd + 1) : string.Empty;
                    s = pre + post;

                    tagStart = s.IndexOf(tagStartChar);
                }
            }

            return s;
        }

        /// <summary>
        /// Gets the on off string.
        /// </summary>
        /// <returns>The on off string.</returns>
        /// <param name="b">If set to <c>true</c> b.</param>
        public static string GetOnOffString(bool b)
        {
            return b ? "ON" : "OFF";
        }


        /// <summary>
        /// Strips the prepending ! statment from string.
        /// </summary>
        /// <returns>The string.</returns>
        /// <param name="s">S.</param>
        public static string StripString(string s)
        {
            string[] delimiter = new string[] { "!", "! " };
            string[] newString = s.Split(delimiter, System.StringSplitOptions.None);
            if (newString.Length > 1)
            {
                return newString[1];
            }
            else
            {
                return s;
            }

        }

        /// <summary>
        /// Gets the EPOCH time in UTC time zome
        /// </summary>
        /// <returns>Double EPOCH in UTC</returns>
        public static double GetEpochUTCMilliseconds()
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (DateTime.UtcNow - epoch).TotalMilliseconds;
        }

        /// <summary>
        /// Gets the epoch UTC seconds.
        /// </summary>
        /// <returns>The epoch UTC seconds.</returns>
        public static double GetEpochUTCSeconds()
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (DateTime.UtcNow - epoch).TotalSeconds;
        }

        /// <summary>
        /// Gets the date time from epoch.
        /// </summary>
        /// <returns>The date time from epoch.</returns>
        /// <param name="epochTime">Epoch time.</param>
        /// <param name="kind">Kind.</param>
        public static DateTime GetLocalDateTimeFromEpoch(double epochTime)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            try
            {
                dateTime = dateTime.AddSeconds(epochTime).ToLocalTime();
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Log.Debug("Utility.GetLocalDateTimeFromEpoch()", "Time conversion assuming time is in Milliseconds: {0}, {1}", epochTime, ex.Message);
                dateTime = dateTime.AddMilliseconds(epochTime).ToLocalTime();
            }

            return dateTime;
        }


        /// <summary>
        /// Returns First valid Mac address of the local machine
        /// </summary>
        public static string MacAddress
        {
            get
            {
#if !NETFX_CORE
                if (string.IsNullOrEmpty(_macAddress))
                {
                    foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
                    {
                        string macAddress = adapter.GetPhysicalAddress().ToString();
                        if (!string.IsNullOrEmpty(macAddress))
                        {
                            string regex = "(.{2})(.{2})(.{2})(.{2})(.{2})(.{2})";
                            string replace = "$1:$2:$3:$4:$5:$6";
                            _macAddress = Regex.Replace(macAddress, regex, replace);

                            break;
                        }
                    }
                }
#endif

                return _macAddress;
            }
        }

        /// <summary>
        /// Converts Color32 array to Byte array.
        /// </summary>
        /// <param name="colors">Color32 array of the image.</param>
        /// <returns></returns>
        public static byte[] Color32ArrayToByteArray(Color32[] colors)
        {
            if (colors == null || colors.Length == 0)
                return null;

#if NETFX_CORE
            int lengthOfColor32 = Marshal.SizeOf<Color32>();
#else
            int lengthOfColor32 = Marshal.SizeOf(typeof(Color32));
#endif
            int length = lengthOfColor32 * colors.Length;
            byte[] bytes = new byte[length];

            GCHandle handle = default(GCHandle);
            try
            {
                handle = GCHandle.Alloc(colors, GCHandleType.Pinned);
                IntPtr ptr = handle.AddrOfPinnedObject();
                Marshal.Copy(ptr, bytes, 0, length);
            }
            finally
            {
                if (handle != default(GCHandle))
                    handle.Free();
            }

            return bytes;
        }


        #region De-Serialization

        /// <summary>
        /// Deserializes the response.
        /// </summary>
        /// <returns>The response.</returns>
        /// <param name="resp">Resp.</param>
        /// <param name="obj">Object.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T DeserializeResponse<T>(byte[] resp, object obj = null) where T : class, new()
        {
            return DeserializeResponse<T>(Encoding.UTF8.GetString(resp), obj);
        }

        /// <summary>
        /// Deserializes the response.
        /// </summary>
        /// <returns>The response.</returns>
        /// <param name="json">Json string of object</param>
        /// <param name="obj">Object.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T DeserializeResponse<T>(string json, object obj = null) where T : class, new()
        {
            try
            {
                fsData data = null;
                fsResult r = fsJsonParser.Parse(json, out data);
                if (!r.Succeeded)
                    throw new WatsonException(r.FormattedMessages);

                if (obj == null)
                    obj = new T();

                r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
                if (!r.Succeeded)
                    throw new WatsonException(r.FormattedMessages);

                return (T)obj;
            }
            catch (Exception e)
            {
                Log.Error("Utility.DeserializeResponse()", "DeserializeResponse Exception: {0}", e.ToString());
            }

            return null;
        }

        /// <summary>
        /// Hack to add a top level object to a json string because Unity does not like top level collections.
        /// </summary>
        /// <param name="json">The json string.</param>
        /// <param name="objectName">The name of the top level object.</param>
        /// <returns></returns>
        public static string AddTopLevelObjectToJson(string json, string objectName)
        {
            string convertedJson = json.Insert(1, "\"" + objectName + "\": {");
            return convertedJson.Insert(convertedJson.Length - 1, "}");
        }
        #endregion

        #region MimeTypes

        private static IDictionary<string, string> MimeTypeMappings()
        {
            var mappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                {".323", "text/h323"},
                {".3g2", "video/3gpp2"},
                {".3gp", "video/3gpp"},
                {".3gp2", "video/3gpp2"},
                {".3gpp", "video/3gpp"},
                {".7z", "application/x-7z-compressed"},
                {".aa", "audio/audible"},
                {".AAC", "audio/aac"},
                {".aaf", "application/octet-stream"},
                {".aax", "audio/vnd.audible.aax"},
                {".ac3", "audio/ac3"},
                {".aca", "application/octet-stream"},
                {".accda", "application/msaccess.addin"},
                {".accdb", "application/msaccess"},
                {".accdc", "application/msaccess.cab"},
                {".accde", "application/msaccess"},
                {".accdr", "application/msaccess.runtime"},
                {".accdt", "application/msaccess"},
                {".accdw", "application/msaccess.webapplication"},
                {".accft", "application/msaccess.ftemplate"},
                {".acx", "application/internet-property-stream"},
                {".AddIn", "text/xml"},
                {".ade", "application/msaccess"},
                {".adobebridge", "application/x-bridge-url"},
                {".adp", "application/msaccess"},
                {".ADT", "audio/vnd.dlna.adts"},
                {".ADTS", "audio/aac"},
                {".afm", "application/octet-stream"},
                {".ai", "application/postscript"},
                {".aif", "audio/aiff"},
                {".aifc", "audio/aiff"},
                {".aiff", "audio/aiff"},
                {".air", "application/vnd.adobe.air-application-installer-package+zip"},
                {".amc", "application/mpeg"},
                {".anx", "application/annodex"},
                {".apk", "application/vnd.android.package-archive" },
                {".application", "application/x-ms-application"},
                {".art", "image/x-jg"},
                {".asa", "application/xml"},
                {".asax", "application/xml"},
                {".ascx", "application/xml"},
                {".asd", "application/octet-stream"},
                {".asf", "video/x-ms-asf"},
                {".ashx", "application/xml"},
                {".asi", "application/octet-stream"},
                {".asm", "text/plain"},
                {".asmx", "application/xml"},
                {".aspx", "application/xml"},
                {".asr", "video/x-ms-asf"},
                {".asx", "video/x-ms-asf"},
                {".atom", "application/atom+xml"},
                {".au", "audio/basic"},
                {".avi", "video/x-msvideo"},
                {".axa", "audio/annodex"},
                {".axs", "application/olescript"},
                {".axv", "video/annodex"},
                {".bas", "text/plain"},
                {".bcpio", "application/x-bcpio"},
                {".bin", "application/octet-stream"},
                {".bmp", "image/bmp"},
                {".c", "text/plain"},
                {".cab", "application/octet-stream"},
                {".caf", "audio/x-caf"},
                {".calx", "application/vnd.ms-office.calx"},
                {".cat", "application/vnd.ms-pki.seccat"},
                {".cc", "text/plain"},
                {".cd", "text/plain"},
                {".cdda", "audio/aiff"},
                {".cdf", "application/x-cdf"},
                {".cer", "application/x-x509-ca-cert"},
                {".cfg", "text/plain"},
                {".chm", "application/octet-stream"},
                {".class", "application/x-java-applet"},
                {".clp", "application/x-msclip"},
                {".cmd", "text/plain"},
                {".cmx", "image/x-cmx"},
                {".cnf", "text/plain"},
                {".cod", "image/cis-cod"},
                {".config", "application/xml"},
                {".contact", "text/x-ms-contact"},
                {".coverage", "application/xml"},
                {".cpio", "application/x-cpio"},
                {".cpp", "text/plain"},
                {".crd", "application/x-mscardfile"},
                {".crl", "application/pkix-crl"},
                {".crt", "application/x-x509-ca-cert"},
                {".cs", "text/plain"},
                {".csdproj", "text/plain"},
                {".csh", "application/x-csh"},
                {".csproj", "text/plain"},
                {".css", "text/css"},
                {".csv", "text/csv"},
                {".cur", "application/octet-stream"},
                {".cxx", "text/plain"},
                {".dat", "application/octet-stream"},
                {".datasource", "application/xml"},
                {".dbproj", "text/plain"},
                {".dcr", "application/x-director"},
                {".def", "text/plain"},
                {".deploy", "application/octet-stream"},
                {".der", "application/x-x509-ca-cert"},
                {".dgml", "application/xml"},
                {".dib", "image/bmp"},
                {".dif", "video/x-dv"},
                {".dir", "application/x-director"},
                {".disco", "text/xml"},
                {".divx", "video/divx"},
                {".dll", "application/x-msdownload"},
                {".dll.config", "text/xml"},
                {".dlm", "text/dlm"},
                {".doc", "application/msword"},
                {".docm", "application/vnd.ms-word.document.macroEnabled.12"},
                {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
                {".dot", "application/msword"},
                {".dotm", "application/vnd.ms-word.template.macroEnabled.12"},
                {".dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template"},
                {".dsp", "application/octet-stream"},
                {".dsw", "text/plain"},
                {".dtd", "text/xml"},
                {".dtsConfig", "text/xml"},
                {".dv", "video/x-dv"},
                {".dvi", "application/x-dvi"},
                {".dwf", "drawing/x-dwf"},
                {".dwp", "application/octet-stream"},
                {".dxr", "application/x-director"},
                {".eml", "message/rfc822"},
                {".emz", "application/octet-stream"},
                {".eot", "application/vnd.ms-fontobject"},
                {".eps", "application/postscript"},
                {".etl", "application/etl"},
                {".etx", "text/x-setext"},
                {".evy", "application/envoy"},
                {".exe", "application/octet-stream"},
                {".exe.config", "text/xml"},
                {".fdf", "application/vnd.fdf"},
                {".fif", "application/fractals"},
                {".filters", "application/xml"},
                {".fla", "application/octet-stream"},
                {".flac", "audio/flac"},
                {".flr", "x-world/x-vrml"},
                {".flv", "video/x-flv"},
                {".fsscript", "application/fsharp-script"},
                {".fsx", "application/fsharp-script"},
                {".generictest", "application/xml"},
                {".gif", "image/gif"},
                {".gpx", "application/gpx+xml"},
                {".group", "text/x-ms-group"},
                {".gsm", "audio/x-gsm"},
                {".gtar", "application/x-gtar"},
                {".gz", "application/x-gzip"},
                {".h", "text/plain"},
                {".hdf", "application/x-hdf"},
                {".hdml", "text/x-hdml"},
                {".hhc", "application/x-oleobject"},
                {".hhk", "application/octet-stream"},
                {".hhp", "application/octet-stream"},
                {".hlp", "application/winhlp"},
                {".hpp", "text/plain"},
                {".hqx", "application/mac-binhex40"},
                {".hta", "application/hta"},
                {".htc", "text/x-component"},
                {".htm", "text/html"},
                {".html", "text/html"},
                {".htt", "text/webviewhtml"},
                {".hxa", "application/xml"},
                {".hxc", "application/xml"},
                {".hxd", "application/octet-stream"},
                {".hxe", "application/xml"},
                {".hxf", "application/xml"},
                {".hxh", "application/octet-stream"},
                {".hxi", "application/octet-stream"},
                {".hxk", "application/xml"},
                {".hxq", "application/octet-stream"},
                {".hxr", "application/octet-stream"},
                {".hxs", "application/octet-stream"},
                {".hxt", "text/html"},
                {".hxv", "application/xml"},
                {".hxw", "application/octet-stream"},
                {".hxx", "text/plain"},
                {".i", "text/plain"},
                {".ico", "image/x-icon"},
                {".ics", "application/octet-stream"},
                {".idl", "text/plain"},
                {".ief", "image/ief"},
                {".iii", "application/x-iphone"},
                {".inc", "text/plain"},
                {".inf", "application/octet-stream"},
                {".ini", "text/plain"},
                {".inl", "text/plain"},
                {".ins", "application/x-internet-signup"},
                {".ipa", "application/x-itunes-ipa"},
                {".ipg", "application/x-itunes-ipg"},
                {".ipproj", "text/plain"},
                {".ipsw", "application/x-itunes-ipsw"},
                {".iqy", "text/x-ms-iqy"},
                {".isp", "application/x-internet-signup"},
                {".ite", "application/x-itunes-ite"},
                {".itlp", "application/x-itunes-itlp"},
                {".itms", "application/x-itunes-itms"},
                {".itpc", "application/x-itunes-itpc"},
                {".IVF", "video/x-ivf"},
                {".jar", "application/java-archive"},
                {".java", "application/octet-stream"},
                {".jck", "application/liquidmotion"},
                {".jcz", "application/liquidmotion"},
                {".jfif", "image/pjpeg"},
                {".jnlp", "application/x-java-jnlp-file"},
                {".jpb", "application/octet-stream"},
                {".jpe", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".jpg", "image/jpeg"},
                {".js", "application/javascript"},
                {".json", "application/json"},
                {".jsx", "text/jscript"},
                {".jsxbin", "text/plain"},
                {".latex", "application/x-latex"},
                {".library-ms", "application/windows-library+xml"},
                {".lit", "application/x-ms-reader"},
                {".loadtest", "application/xml"},
                {".lpk", "application/octet-stream"},
                {".lsf", "video/x-la-asf"},
                {".lst", "text/plain"},
                {".lsx", "video/x-la-asf"},
                {".lzh", "application/octet-stream"},
                {".m13", "application/x-msmediaview"},
                {".m14", "application/x-msmediaview"},
                {".m1v", "video/mpeg"},
                {".m2t", "video/vnd.dlna.mpeg-tts"},
                {".m2ts", "video/vnd.dlna.mpeg-tts"},
                {".m2v", "video/mpeg"},
                {".m3u", "audio/x-mpegurl"},
                {".m3u8", "audio/x-mpegurl"},
                {".m4a", "audio/m4a"},
                {".m4b", "audio/m4b"},
                {".m4p", "audio/m4p"},
                {".m4r", "audio/x-m4r"},
                {".m4v", "video/x-m4v"},
                {".mac", "image/x-macpaint"},
                {".mak", "text/plain"},
                {".man", "application/x-troff-man"},
                {".manifest", "application/x-ms-manifest"},
                {".map", "text/plain"},
                {".master", "application/xml"},
                {".mda", "application/msaccess"},
                {".mdb", "application/x-msaccess"},
                {".mde", "application/msaccess"},
                {".mdp", "application/octet-stream"},
                {".me", "application/x-troff-me"},
                {".mfp", "application/x-shockwave-flash"},
                {".mht", "message/rfc822"},
                {".mhtml", "message/rfc822"},
                {".mid", "audio/mid"},
                {".midi", "audio/mid"},
                {".mix", "application/octet-stream"},
                {".mk", "text/plain"},
                {".mmf", "application/x-smaf"},
                {".mno", "text/xml"},
                {".mny", "application/x-msmoney"},
                {".mod", "video/mpeg"},
                {".mov", "video/quicktime"},
                {".movie", "video/x-sgi-movie"},
                {".mp2", "video/mpeg"},
                {".mp2v", "video/mpeg"},
                {".mp3", "audio/mpeg"},
                {".mp4", "video/mp4"},
                {".mp4v", "video/mp4"},
                {".mpa", "video/mpeg"},
                {".mpe", "video/mpeg"},
                {".mpeg", "video/mpeg"},
                {".mpf", "application/vnd.ms-mediapackage"},
                {".mpg", "video/mpeg"},
                {".mpp", "application/vnd.ms-project"},
                {".mpv2", "video/mpeg"},
                {".mqv", "video/quicktime"},
                {".ms", "application/x-troff-ms"},
                {".msi", "application/octet-stream"},
                {".mso", "application/octet-stream"},
                {".mts", "video/vnd.dlna.mpeg-tts"},
                {".mtx", "application/xml"},
                {".mvb", "application/x-msmediaview"},
                {".mvc", "application/x-miva-compiled"},
                {".mxp", "application/x-mmxp"},
                {".nc", "application/x-netcdf"},
                {".nsc", "video/x-ms-asf"},
                {".nws", "message/rfc822"},
                {".ocx", "application/octet-stream"},
                {".oda", "application/oda"},
                {".odb", "application/vnd.oasis.opendocument.database"},
                {".odc", "application/vnd.oasis.opendocument.chart"},
                {".odf", "application/vnd.oasis.opendocument.formula"},
                {".odg", "application/vnd.oasis.opendocument.graphics"},
                {".odh", "text/plain"},
                {".odi", "application/vnd.oasis.opendocument.image"},
                {".odl", "text/plain"},
                {".odm", "application/vnd.oasis.opendocument.text-master"},
                {".odp", "application/vnd.oasis.opendocument.presentation"},
                {".ods", "application/vnd.oasis.opendocument.spreadsheet"},
                {".odt", "application/vnd.oasis.opendocument.text"},
                {".oga", "audio/ogg"},
                {".ogg", "audio/ogg"},
                {".ogv", "video/ogg"},
                {".ogx", "application/ogg"},
                {".one", "application/onenote"},
                {".onea", "application/onenote"},
                {".onepkg", "application/onenote"},
                {".onetmp", "application/onenote"},
                {".onetoc", "application/onenote"},
                {".onetoc2", "application/onenote"},
                {".opus", "audio/ogg"},
                {".orderedtest", "application/xml"},
                {".osdx", "application/opensearchdescription+xml"},
                {".otf", "application/font-sfnt"},
                {".otg", "application/vnd.oasis.opendocument.graphics-template"},
                {".oth", "application/vnd.oasis.opendocument.text-web"},
                {".otp", "application/vnd.oasis.opendocument.presentation-template"},
                {".ots", "application/vnd.oasis.opendocument.spreadsheet-template"},
                {".ott", "application/vnd.oasis.opendocument.text-template"},
                {".oxt", "application/vnd.openofficeorg.extension"},
                {".p10", "application/pkcs10"},
                {".p12", "application/x-pkcs12"},
                {".p7b", "application/x-pkcs7-certificates"},
                {".p7c", "application/pkcs7-mime"},
                {".p7m", "application/pkcs7-mime"},
                {".p7r", "application/x-pkcs7-certreqresp"},
                {".p7s", "application/pkcs7-signature"},
                {".pbm", "image/x-portable-bitmap"},
                {".pcast", "application/x-podcast"},
                {".pct", "image/pict"},
                {".pcx", "application/octet-stream"},
                {".pcz", "application/octet-stream"},
                {".pdf", "application/pdf"},
                {".pfb", "application/octet-stream"},
                {".pfm", "application/octet-stream"},
                {".pfx", "application/x-pkcs12"},
                {".pgm", "image/x-portable-graymap"},
                {".pic", "image/pict"},
                {".pict", "image/pict"},
                {".pkgdef", "text/plain"},
                {".pkgundef", "text/plain"},
                {".pko", "application/vnd.ms-pki.pko"},
                {".pls", "audio/scpls"},
                {".pma", "application/x-perfmon"},
                {".pmc", "application/x-perfmon"},
                {".pml", "application/x-perfmon"},
                {".pmr", "application/x-perfmon"},
                {".pmw", "application/x-perfmon"},
                {".png", "image/png"},
                {".pnm", "image/x-portable-anymap"},
                {".pnt", "image/x-macpaint"},
                {".pntg", "image/x-macpaint"},
                {".pnz", "image/png"},
                {".pot", "application/vnd.ms-powerpoint"},
                {".potm", "application/vnd.ms-powerpoint.template.macroEnabled.12"},
                {".potx", "application/vnd.openxmlformats-officedocument.presentationml.template"},
                {".ppa", "application/vnd.ms-powerpoint"},
                {".ppam", "application/vnd.ms-powerpoint.addin.macroEnabled.12"},
                {".ppm", "image/x-portable-pixmap"},
                {".pps", "application/vnd.ms-powerpoint"},
                {".ppsm", "application/vnd.ms-powerpoint.slideshow.macroEnabled.12"},
                {".ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow"},
                {".ppt", "application/vnd.ms-powerpoint"},
                {".pptm", "application/vnd.ms-powerpoint.presentation.macroEnabled.12"},
                {".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
                {".prf", "application/pics-rules"},
                {".prm", "application/octet-stream"},
                {".prx", "application/octet-stream"},
                {".ps", "application/postscript"},
                {".psc1", "application/PowerShell"},
                {".psd", "application/octet-stream"},
                {".psess", "application/xml"},
                {".psm", "application/octet-stream"},
                {".psp", "application/octet-stream"},
                {".pub", "application/x-mspublisher"},
                {".pwz", "application/vnd.ms-powerpoint"},
                {".qht", "text/x-html-insertion"},
                {".qhtm", "text/x-html-insertion"},
                {".qt", "video/quicktime"},
                {".qti", "image/x-quicktime"},
                {".qtif", "image/x-quicktime"},
                {".qtl", "application/x-quicktimeplayer"},
                {".qxd", "application/octet-stream"},
                {".ra", "audio/x-pn-realaudio"},
                {".ram", "audio/x-pn-realaudio"},
                {".rar", "application/x-rar-compressed"},
                {".ras", "image/x-cmu-raster"},
                {".rat", "application/rat-file"},
                {".rc", "text/plain"},
                {".rc2", "text/plain"},
                {".rct", "text/plain"},
                {".rdlc", "application/xml"},
                {".reg", "text/plain"},
                {".resx", "application/xml"},
                {".rf", "image/vnd.rn-realflash"},
                {".rgb", "image/x-rgb"},
                {".rgs", "text/plain"},
                {".rm", "application/vnd.rn-realmedia"},
                {".rmi", "audio/mid"},
                {".rmp", "application/vnd.rn-rn_music_package"},
                {".roff", "application/x-troff"},
                {".rpm", "audio/x-pn-realaudio-plugin"},
                {".rqy", "text/x-ms-rqy"},
                {".rtf", "application/rtf"},
                {".rtx", "text/richtext"},
                {".ruleset", "application/xml"},
                {".s", "text/plain"},
                {".safariextz", "application/x-safari-safariextz"},
                {".scd", "application/x-msschedule"},
                {".scr", "text/plain"},
                {".sct", "text/scriptlet"},
                {".sd2", "audio/x-sd2"},
                {".sdp", "application/sdp"},
                {".sea", "application/octet-stream"},
                {".searchConnector-ms", "application/windows-search-connector+xml"},
                {".setpay", "application/set-payment-initiation"},
                {".setreg", "application/set-registration-initiation"},
                {".settings", "application/xml"},
                {".sgimb", "application/x-sgimb"},
                {".sgml", "text/sgml"},
                {".sh", "application/x-sh"},
                {".shar", "application/x-shar"},
                {".shtml", "text/html"},
                {".sit", "application/x-stuffit"},
                {".sitemap", "application/xml"},
                {".skin", "application/xml"},
                {".sldm", "application/vnd.ms-powerpoint.slide.macroEnabled.12"},
                {".sldx", "application/vnd.openxmlformats-officedocument.presentationml.slide"},
                {".slk", "application/vnd.ms-excel"},
                {".sln", "text/plain"},
                {".slupkg-ms", "application/x-ms-license"},
                {".smd", "audio/x-smd"},
                {".smi", "application/octet-stream"},
                {".smx", "audio/x-smd"},
                {".smz", "audio/x-smd"},
                {".snd", "audio/basic"},
                {".snippet", "application/xml"},
                {".snp", "application/octet-stream"},
                {".sol", "text/plain"},
                {".sor", "text/plain"},
                {".spc", "application/x-pkcs7-certificates"},
                {".spl", "application/futuresplash"},
                {".spx", "audio/ogg"},
                {".src", "application/x-wais-source"},
                {".srf", "text/plain"},
                {".SSISDeploymentManifest", "text/xml"},
                {".ssm", "application/streamingmedia"},
                {".sst", "application/vnd.ms-pki.certstore"},
                {".stl", "application/vnd.ms-pki.stl"},
                {".sv4cpio", "application/x-sv4cpio"},
                {".sv4crc", "application/x-sv4crc"},
                {".svc", "application/xml"},
                {".svg", "image/svg+xml"},
                {".swf", "application/x-shockwave-flash"},
                {".step", "application/step"},
                {".stp", "application/step"},
                {".t", "application/x-troff"},
                {".tar", "application/x-tar"},
                {".tcl", "application/x-tcl"},
                {".testrunconfig", "application/xml"},
                {".testsettings", "application/xml"},
                {".tex", "application/x-tex"},
                {".texi", "application/x-texinfo"},
                {".texinfo", "application/x-texinfo"},
                {".tgz", "application/x-compressed"},
                {".thmx", "application/vnd.ms-officetheme"},
                {".thn", "application/octet-stream"},
                {".tif", "image/tiff"},
                {".tiff", "image/tiff"},
                {".tlh", "text/plain"},
                {".tli", "text/plain"},
                {".toc", "application/octet-stream"},
                {".tr", "application/x-troff"},
                {".trm", "application/x-msterminal"},
                {".trx", "application/xml"},
                {".ts", "video/vnd.dlna.mpeg-tts"},
                {".tsv", "text/tab-separated-values"},
                {".ttf", "application/font-sfnt"},
                {".tts", "video/vnd.dlna.mpeg-tts"},
                {".txt", "text/plain"},
                {".u32", "application/octet-stream"},
                {".uls", "text/iuls"},
                {".user", "text/plain"},
                {".ustar", "application/x-ustar"},
                {".vb", "text/plain"},
                {".vbdproj", "text/plain"},
                {".vbk", "video/mpeg"},
                {".vbproj", "text/plain"},
                {".vbs", "text/vbscript"},
                {".vcf", "text/x-vcard"},
                {".vcproj", "application/xml"},
                {".vcs", "text/plain"},
                {".vcxproj", "application/xml"},
                {".vddproj", "text/plain"},
                {".vdp", "text/plain"},
                {".vdproj", "text/plain"},
                {".vdx", "application/vnd.ms-visio.viewer"},
                {".vml", "text/xml"},
                {".vscontent", "application/xml"},
                {".vsct", "text/xml"},
                {".vsd", "application/vnd.visio"},
                {".vsi", "application/ms-vsi"},
                {".vsix", "application/vsix"},
                {".vsixlangpack", "text/xml"},
                {".vsixmanifest", "text/xml"},
                {".vsmdi", "application/xml"},
                {".vspscc", "text/plain"},
                {".vss", "application/vnd.visio"},
                {".vsscc", "text/plain"},
                {".vssettings", "text/xml"},
                {".vssscc", "text/plain"},
                {".vst", "application/vnd.visio"},
                {".vstemplate", "text/xml"},
                {".vsto", "application/x-ms-vsto"},
                {".vsw", "application/vnd.visio"},
                {".vsx", "application/vnd.visio"},
                {".vtx", "application/vnd.visio"},
                {".wav", "audio/wav"},
                {".wave", "audio/wav"},
                {".wax", "audio/x-ms-wax"},
                {".wbk", "application/msword"},
                {".wbmp", "image/vnd.wap.wbmp"},
                {".wcm", "application/vnd.ms-works"},
                {".wdb", "application/vnd.ms-works"},
                {".wdp", "image/vnd.ms-photo"},
                {".webarchive", "application/x-safari-webarchive"},
                {".webm", "video/webm"},
                {".webp", "image/webp"},
                {".webtest", "application/xml"},
                {".wiq", "application/xml"},
                {".wiz", "application/msword"},
                {".wks", "application/vnd.ms-works"},
                {".WLMP", "application/wlmoviemaker"},
                {".wlpginstall", "application/x-wlpg-detect"},
                {".wlpginstall3", "application/x-wlpg3-detect"},
                {".wm", "video/x-ms-wm"},
                {".wma", "audio/x-ms-wma"},
                {".wmd", "application/x-ms-wmd"},
                {".wmf", "application/x-msmetafile"},
                {".wml", "text/vnd.wap.wml"},
                {".wmlc", "application/vnd.wap.wmlc"},
                {".wmls", "text/vnd.wap.wmlscript"},
                {".wmlsc", "application/vnd.wap.wmlscriptc"},
                {".wmp", "video/x-ms-wmp"},
                {".wmv", "video/x-ms-wmv"},
                {".wmx", "video/x-ms-wmx"},
                {".wmz", "application/x-ms-wmz"},
                {".woff", "application/font-woff"},
                {".wpl", "application/vnd.ms-wpl"},
                {".wps", "application/vnd.ms-works"},
                {".wri", "application/x-mswrite"},
                {".wrl", "x-world/x-vrml"},
                {".wrz", "x-world/x-vrml"},
                {".wsc", "text/scriptlet"},
                {".wsdl", "text/xml"},
                {".wvx", "video/x-ms-wvx"},
                {".x", "application/directx"},
                {".xaf", "x-world/x-vrml"},
                {".xaml", "application/xaml+xml"},
                {".xap", "application/x-silverlight-app"},
                {".xbap", "application/x-ms-xbap"},
                {".xbm", "image/x-xbitmap"},
                {".xdr", "text/plain"},
                {".xht", "application/xhtml+xml"},
                {".xhtml", "application/xhtml+xml"},
                {".xla", "application/vnd.ms-excel"},
                {".xlam", "application/vnd.ms-excel.addin.macroEnabled.12"},
                {".xlc", "application/vnd.ms-excel"},
                {".xld", "application/vnd.ms-excel"},
                {".xlk", "application/vnd.ms-excel"},
                {".xll", "application/vnd.ms-excel"},
                {".xlm", "application/vnd.ms-excel"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsb", "application/vnd.ms-excel.sheet.binary.macroEnabled.12"},
                {".xlsm", "application/vnd.ms-excel.sheet.macroEnabled.12"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".xlt", "application/vnd.ms-excel"},
                {".xltm", "application/vnd.ms-excel.template.macroEnabled.12"},
                {".xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template"},
                {".xlw", "application/vnd.ms-excel"},
                {".xml", "text/xml"},
                {".xmta", "application/xml"},
                {".xof", "x-world/x-vrml"},
                {".XOML", "text/plain"},
                {".xpm", "image/x-xpixmap"},
                {".xps", "application/vnd.ms-xpsdocument"},
                {".xrm-ms", "text/xml"},
                {".xsc", "application/xml"},
                {".xsd", "text/xml"},
                {".xsf", "text/xml"},
                {".xsl", "text/xml"},
                {".xslt", "text/xml"},
                {".xsn", "application/octet-stream"},
                {".xss", "application/xml"},
                {".xspf", "application/xspf+xml"},
                {".xtp", "application/octet-stream"},
                {".xwd", "image/x-xwindowdump"},
                {".z", "application/x-compress"},
                {".zip", "application/zip"},

                {"application/fsharp-script", ".fsx"},
                {"application/msaccess", ".adp"},
                {"application/msword", ".doc"},
                {"application/octet-stream", ".bin"},
                {"application/onenote", ".one"},
                {"application/postscript", ".eps"},
                {"application/step", ".step"},
                {"application/vnd.ms-excel", ".xls"},
                {"application/vnd.ms-powerpoint", ".ppt"},
                {"application/vnd.ms-works", ".wks"},
                {"application/vnd.visio", ".vsd"},
                {"application/x-director", ".dir"},
                {"application/x-shockwave-flash", ".swf"},
                {"application/x-x509-ca-cert", ".cer"},
                {"application/x-zip-compressed", ".zip"},
                {"application/xhtml+xml", ".xhtml"},
                {"application/xml", ".xml"},
                {"audio/aac", ".AAC"},
                {"audio/aiff", ".aiff"},
                {"audio/basic", ".snd"},
                {"audio/mid", ".midi"},
                {"audio/wav", ".wav"},
                {"audio/x-m4a", ".m4a"},
                {"audio/x-mpegurl", ".m3u"},
                {"audio/x-pn-realaudio", ".ra"},
                {"audio/x-smd", ".smd"},
                {"image/bmp", ".bmp"},
                {"image/jpeg", ".jpg"},
                {"image/pict", ".pic"},
                {"image/png", ".png"},
                {"image/tiff", ".tiff"},
                {"image/x-macpaint", ".mac"},
                {"image/x-quicktime", ".qti"},
                {"message/rfc822", ".eml"},
                {"text/html", ".html"},
                {"text/plain", ".txt"},
                {"text/scriptlet", ".wsc"},
                {"text/xml", ".xml"},
                {"video/3gpp", ".3gp"},
                {"video/3gpp2", ".3gp2"},
                {"video/mp4", ".mp4"},
                {"video/mpeg", ".mpg"},
                {"video/quicktime", ".mov"},
                {"video/vnd.dlna.mpeg-tts", ".m2t"},
                {"video/x-dv", ".dv"},
                {"video/x-la-asf", ".lsf"},
                {"video/x-ms-asf", ".asf"},
                {"x-world/x-vrml", ".xof"},
                };

            return mappings;
        }

        public static string GetMimeType(string extension)
        {
            if (extension == null)
            {
                throw new ArgumentNullException("extension");
            }

            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }

            string mime;

            return MimeTypeMappings().TryGetValue(extension, out mime) ? mime : "application/octet-stream";
        }

        public static string GetExtension(string mimeType)
        {
            if (mimeType == null)
            {
                throw new ArgumentNullException("mimeType");
            }

            if (mimeType.StartsWith("."))
            {
                throw new ArgumentException("Requested mime type is not valid: " + mimeType);
            }

            string extension;

            if (MimeTypeMappings().TryGetValue(mimeType, out extension))
            {
                return extension;
            }

            throw new ArgumentException("Requested mime type is not registered: " + mimeType);
        }
        #endregion

#region Get Token
        private const string TokenRestEndpoint = "https://gateway.watsonplatform.net/authorization/api/v1/token";
        private const string TokenStreamEndpoint = "https://stream.watsonplatform.net/authorization/api/v1/token";

        /// <summary>
        /// The OnGetToken callback.
        /// </summary>
        /// <param name="authenticationToken">The authentication token object.</param>
        /// <param name="data">User defined custom data.</param>
        public delegate void OnGetToken(AuthenticationToken authenticationToken, string data);

        /// <summary>
        /// Gets a token to authenticate serivce calls instead of using username and password.
        /// </summary>
        /// <param name="callback">The OnGetToken callback,</param>
        /// <param name="serviceEndpoint">The service endpoint.</param>
        /// <param name="username">The service username.</param>
        /// <param name="password">The service password.</param>
        /// <param name="tokenName">A user defined name for the token.</param>
        /// <returns>True if the call succeeds.</returns>
        public static bool GetToken(OnGetToken callback, string serviceEndpoint, string username, string password, string tokenName = "")
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(serviceEndpoint))
                throw new ArgumentNullException("serviceEndpoint");
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException("username");
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("password");

            string tokenEndpoint;
            if (serviceEndpoint.Contains("stream"))
                tokenEndpoint = TokenStreamEndpoint;
            else
                tokenEndpoint = TokenRestEndpoint;

            Credentials Credentials = new Credentials()
            {
                Username = username,
                Password = password
            };

            RESTConnector connector = RESTConnector.GetConnector(Credentials, tokenEndpoint + "?url=" + serviceEndpoint);
            if (connector == null)
                return false;

            GetTokenReq req = new GetTokenReq();
            req.Callback = callback;
            req.OnResponse = OnGetTokenResp;
            req.TokenName = tokenName;

            return connector.Send(req);
        }

        private class GetTokenReq : RESTConnector.Request
        {
            /// <summary>
            /// Custom data.
            /// </summary>
            public string Data { get; set; }
            /// <summary>
            /// OnGetToken callback delegate
            /// </summary>
            public OnGetToken Callback { get; set; }
            /// <summary>
            /// User defined service name to keep track of authentication tokens.
            /// </summary>
            public string TokenName { get; set; }
        }

        private static void OnGetTokenResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            AuthenticationToken authenticationToken = new AuthenticationToken();
            string token = "";

            if (resp.Success)
            {
                try
                {
                    authenticationToken.Token = token = Encoding.UTF8.GetString(resp.Data);
                    authenticationToken.Created = DateTime.Now;
                    authenticationToken.ServiceName = ((GetTokenReq)req).TokenName;
                }
                catch (Exception e)
                {
                    Log.Error("Utility.OnGetTokenResp()", "Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            string customData = ((GetTokenReq)req).Data;
            if (((GetTokenReq)req).Callback != null)
                ((GetTokenReq)req).Callback(resp.Success ? authenticationToken : null, string.IsNullOrEmpty(customData) ? token : customData);
        }
#endregion
    }

    /// <summary>
    /// The authentication token object.
    /// </summary>
    public class AuthenticationToken
    {
        /// <summary>
        /// The token string for authentication.
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// The DateTime the token was created.
        /// </summary>
        public DateTime Created { get; set; }
        /// <summary>
        /// The user created service name to keep track of authentication tokens.
        /// </summary>
        public string ServiceName { get; set; }
        /// <summary>
        /// Returns the time in minutes until the expiration of the authentication token.
        /// </summary>
        public double TimeUntilExpiration
        {
            get
            {
                return ((Created.AddMinutes(Constants.Token.TOKEN_TIME_TO_LIVE)) - DateTime.Now).TotalMinutes;
            }
        }
        /// <summary>
        /// Is the token expired.
        /// </summary>
        public bool IsTokenExpired
        {
            get
            {
                return TimeUntilExpiration <= 0;
            }
        }
    }
}
