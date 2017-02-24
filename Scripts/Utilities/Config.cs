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

using System.Collections;
using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.Logging;
using UnityEngine;
using FullSerializer;
using System.IO;
using MiniJSON;
using System;

namespace IBM.Watson.DeveloperCloud.Utilities
{
  /// <summary>
  /// This class is used to hold configuration data for SDK.
  /// </summary>
  public class Config
  {
    /// <summary>
    /// Serialized class for holding generic key/value pairs.
    /// </summary>
    [fsObject]
    public class Variable
    {
      /// <summary>
      /// The key name.
      /// </summary>
      public string Key { get; set; }
      /// <summary>
      /// The value referenced by the key.
      /// </summary>
      public string Value { get; set; }
    };

    /// <summary>
    /// Serialized class for holding the user credentials for a service.
    /// </summary>
    [fsObject]
    public class CredentialInfo
    {
      /// <summary>
      /// The ID of the service this is the credentials.
      /// </summary>
      public string m_ServiceID;
      /// <summary>
      /// The URL for these credentials.
      /// </summary>
      public string m_URL;
      /// <summary>
      /// The user name for these credentials.
      /// </summary>
      public string m_User;
      /// <summary>
      /// The password for these credentials.
      /// </summary>
      public string m_Password;
      /// <summary>
      /// The API key for this service.
      /// </summary>
      public string m_Apikey;
      /// <summary>
      /// A note for the applied service.
      /// </summary>
      public string m_Note;

      /// <summary>
      /// Generate JSON credentials.
      /// </summary>
      /// <returns>Returns a string of the JSON.</returns>
      public string MakeJSON()
      {
        return "{\n\t\"credentials\": {\n\t\t\"url\": \"" + m_URL + "\",\n\t\t\"username\": \"" + m_User + "\",\n\t\t\"password\": \"" + m_Password + "\",\n\t\t\"apikey\": \"" + m_Apikey + "\",\n\t\t\"note\": \"" + m_Note + "\"\n\t}\n}";
      }

      /// <summary>
      /// Parses a BlueMix json credentials into this object.
      /// </summary>
      /// <param name="json">The JSON data to parse.</param>
      /// <returns>Returns true on success.</returns>
      public bool ParseJSON(string json)
      {
        try
        {
          IDictionary iParse = Json.Deserialize(json) as IDictionary;
          IDictionary iCredentials = iParse["credentials"] as IDictionary;
          m_URL = (string)iCredentials["url"];
          m_User = (string)iCredentials["username"];
          m_Password = (string)iCredentials["password"];
          m_Note = (string)iCredentials["note"];
          if (!string.IsNullOrEmpty((string)iCredentials["apikey"]))
            m_Apikey = (string)iCredentials["apikey"];
          if (!string.IsNullOrEmpty((string)iCredentials["api_key"]))
            m_Apikey = (string)iCredentials["api_key"];

          return true;
        }
        catch (Exception e)
        {
          try
          {
            IDictionary iParse = Json.Deserialize(json) as IDictionary;
            m_URL = (string)iParse["url"];
            m_User = (string)iParse["username"];
            m_Password = (string)iParse["password"];
            m_Note = (string)iParse["note"];
            if (!string.IsNullOrEmpty((string)iParse["apikey"]))
              m_Apikey = (string)iParse["apikey"];
            if (!string.IsNullOrEmpty((string)iParse["api_key"]))
              m_Apikey = (string)iParse["api_key"];

            return true;
          }
          catch
          {
            Log.Error("Config", "Caught Exception: {0}", e.ToString());
          }
          Log.Error("Config", "Caught Exception: {0}", e.ToString());
        }

        return false;
      }

      /// <summary>
      /// Determines whether this instance has credentials.
      /// </summary>
      /// <returns><c>true</c> if this instance has credentials; otherwise, <c>false</c>.</returns>
      public bool HasCredentials()
      {
        return (!string.IsNullOrEmpty(m_User) && !string.IsNullOrEmpty(m_Password));
      }

      /// <summary>
      /// Determines whether this instance has API key.
      /// </summary>
      /// <returns><c>true</c> if this instance has API key; otherwise, <c>false</c>.</returns>
      public bool HasAPIKey()
      {
        return !string.IsNullOrEmpty(m_Apikey);
      }
    }

    #region Private Data
    [fsProperty]
    private string m_ClassifierDirectory = "Watson/Scripts/Editor/Classifiers/";
    [fsProperty]
    private float m_TimeOut = 30.0f;
    [fsProperty]
    private int m_MaxRestConnections = 5;
    [fsProperty]
    private List<CredentialInfo> m_Credentials = new List<CredentialInfo>();
    [fsProperty]
    private List<Variable> m_Variables = new List<Variable>();

    private static fsSerializer sm_Serializer = new fsSerializer();
    #endregion

    #region Public Properties
    /// <summary>
    /// Returns true if the configuration is loaded or not.
    /// </summary>
    [fsIgnore]
    public bool ConfigLoaded { get; set; }
    /// <summary>
    /// Returns the singleton instance.
    /// </summary>
    public static Config Instance { get { return Singleton<Config>.Instance; } }
    /// <summary>
    /// Returns the location of the classifiers
    /// </summary>
    public string ClassifierDirectory { get { return m_ClassifierDirectory; } set { m_ClassifierDirectory = value; } }
    /// <summary>
    /// Returns the Timeout for requests made to the server.
    /// </summary>
    public float TimeOut { get { return m_TimeOut; } set { m_TimeOut = value; } }
    /// <summary>
    /// Maximum number of connections Watson will make to the server back-end at any one time.
    /// </summary>
    public int MaxRestConnections { get { return m_MaxRestConnections; } set { m_MaxRestConnections = value; } }
    /// <summary>
    /// Returns the list of credentials used to login to the various services.
    /// </summary>
    public List<CredentialInfo> Credentials { get { return m_Credentials; } set { m_Credentials = value; } }
    /// <summary>
    /// Returns a list of variables which can hold key/value data.
    /// </summary>
    public List<Variable> Variables { get { return m_Variables; } set { m_Variables = value; } }
    #endregion

    /// <summary>
    /// Default constructor will call LoadConfig() automatically.
    /// </summary>
    public Config()
    {
      LoadConfig();
    }

    /// <summary>
    /// Find BlueMix credentials by the service ID.
    /// </summary>
    /// <param name="serviceID">The ID of the service to find.</param>
    /// <returns>Returns null if the credentials cannot be found.</returns>
    public CredentialInfo FindCredentials(string serviceID)
    {
      foreach (var info in m_Credentials)
        if (info.m_ServiceID == serviceID)
          return info;
      return null;
    }

    /// <summary>
    /// Invoking this function will start the co-routine to load the configuration. The user should check the 
    /// ConfigLoaded property to check when the configuration is actually loaded.
    /// </summary>
    public void LoadConfig()
    {
#if !UNITY_ANDROID || UNITY_EDITOR
      try
      {
        if (!Directory.Exists(Application.streamingAssetsPath))
          Directory.CreateDirectory(Application.streamingAssetsPath);
        LoadConfig(System.IO.File.ReadAllText(Application.streamingAssetsPath + Constants.Path.CONFIG_FILE));
      }
      catch (System.IO.FileNotFoundException)
      {
        // mark as loaded anyway, so we don't keep retrying..
        Log.Error("Config", "Failed to load config file.");
        ConfigLoaded = true;
      }
#else
            Runnable.Run(LoadConfigCR());
#endif
    }

    /// <summary>
    /// Load the configuration from the given JSON data.
    /// </summary>
    /// <param name="json">The string containing the configuration JSON data.</param>
    /// <returns></returns>
    public bool LoadConfig(string json)
    {
      try
      {
        fsData data = null;
        fsResult r = fsJsonParser.Parse(json, out data);
        if (!r.Succeeded)
        {
          Log.Error("Config", "Failed to parse Config.json: {0}", r.ToString());
          return false;
        }

        object obj = this;
        r = sm_Serializer.TryDeserialize(data, GetType(), ref obj);
        if (!r.Succeeded)
        {
          Log.Error("Config", "Failed to parse Config.json: {0}", r.ToString());
          return false;
        }

        ConfigLoaded = true;
        return true;
      }
      catch (Exception e)
      {
        Log.Error("Config", "Failed to load config: {0}", e.ToString());
      }

      ConfigLoaded = true;
      return false;
    }

    /// <summary>
    /// Save this COnfig into JSON.
    /// </summary>
    /// <param name="pretty">If true, then the json data will be formatted for readability.</param>
    /// <returns></returns>
    public string SaveConfig(bool pretty = true)
    {
      fsData data = null;
      sm_Serializer.TrySerialize(GetType(), this, out data);

      if (!System.IO.Directory.Exists(Application.streamingAssetsPath))
        System.IO.Directory.CreateDirectory(Application.streamingAssetsPath);

      if (pretty)
        return fsJsonPrinter.PrettyJson(data);

      return fsJsonPrinter.CompressedJson(data);
    }

    /// <summary>
    /// Save this config to fileSystem
    /// </summary>
    public bool SaveConfigToFileSystem()
    {
      bool success = true;
      try
      {
        File.WriteAllText(Application.streamingAssetsPath + Constants.Path.CONFIG_FILE, SaveConfig(true));
      }
      catch (Exception ex)
      {
        success = false;
        Log.Error("Config", "Exception on SaveConfigToFileSystem. {0}", ex.Message);
      }

      return success;
    }

    /// <summary>
    /// Finds a variable name and returns the Variable object
    /// </summary>
    /// <param name="key">The name of the variable to find.</param>
    /// <returns>Returns the Variable object or null if not found.</returns>
    public Variable GetVariable(string key)
    {
      foreach (var var in m_Variables)
        if (var.Key == key)
          return var;

      return null;
    }

    /// <summary>
    /// Gets an API key by service ID if it exists.
    /// </summary>
    /// <param name="serviceID"></param>
    /// <returns></returns>
    public string GetAPIKey(string serviceID)
    {
      foreach (var info in m_Credentials)
        if (info.m_ServiceID == serviceID)
          return info.m_Apikey;
      return null;
    }

    /// <summary>
    /// Gets the variable value.
    /// </summary>
    /// <returns>The variable value.</returns>
    /// <param name="key">Key.</param>
    public string GetVariableValue(string key)
    {
      Variable v = GetVariable(key);
      if (v != null)
        return v.Value;

      return null;
    }

    /// <summary>
    /// Sets the variable value.
    /// </summary>
    /// <returns><c>true</c>, if variable value was set, <c>false</c> otherwise.</returns>
    /// <param name="key">Key.</param>
    /// <param name="value">Value.</param>
    public bool SetVariableValue(string key, string value, bool bAdd = false)
    {
      Variable v = GetVariable(key);
      if (v == null)
      {
        if (!bAdd)
          return false;

        v = new Variable();
        v.Key = key;
        m_Variables.Add(v);
      }

      v.Value = value;
      return true;
    }

    /// <summary>
    /// Resolves any variables found in the input string and returns the variable values in the returned string.
    /// </summary>
    /// <param name="input">A string containing variables.</param>
    /// <returns>Returns the string with all variables resolved to their actual values. Any missing variables are removed from the string.</returns>
    public string ResolveVariables(string input, bool recursive = true)
    {
      string output = input;
      foreach (var var in m_Variables)
      {
        string value = var.Value;
        if (recursive && value.Contains("${"))
          value = ResolveVariables(value, false);

        output = output.Replace("${" + var.Key + "}", value);
      }

      // remove any variables still in the string..
      int variableIndex = output.IndexOf("${");
      while (variableIndex >= 0)
      {
        int endVariable = output.IndexOf("}", variableIndex);
        if (endVariable < 0)
          break;      // end not found..

        output = output.Remove(variableIndex, (endVariable - variableIndex) + 1);

        // next..
        variableIndex = output.IndexOf("${");
      }

      return output;
    }

    private IEnumerator LoadConfigCR()
    {
      // load the config using WWW, since this works on all platforms..
      WWW request = new WWW(Application.streamingAssetsPath + Constants.Path.CONFIG_FILE);
      while (!request.isDone)
        yield return null;

      LoadConfig(request.text);
      yield break;
    }
  }
}
