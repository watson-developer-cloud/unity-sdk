/**
* Copyright 2019 IBM Corp. All Rights Reserved.
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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Text.RegularExpressions;

namespace IBM.Cloud.SDK.Utilities
{
    public static class SystemInformation
    {
        private static string osInfo = null;
        public static string OsInfo
        {
            get
            {
                if (string.IsNullOrEmpty(osInfo))
                {
                    osInfo = SystemInfo.operatingSystem;
                }

                return osInfo;
            }
        }

        private static string os = null;
        public static string Os
        {
            get
            {
                if (string.IsNullOrEmpty(os))
                {
                    string tempos = OsInfo.Replace(OsVersion, "").Replace(" ", "");
                    if (tempos.Contains("()"))
                    {
                        os = tempos.Replace("()", "-");
                    }
                }

                return os;
            }
        }

        private static string osVersion = null;
        public static string OsVersion
        {
            get
            {
                if (string.IsNullOrEmpty(osVersion))
                {
                    Regex pattern = new Regex("\\d+(\\.\\d+)+");
                    Match m = pattern.Match(OsInfo);
                    osVersion = m.Value;
                }

                return osVersion;
            }
        }
    }
}
