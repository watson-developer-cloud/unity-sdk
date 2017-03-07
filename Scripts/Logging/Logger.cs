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

using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Debug;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IBM.Watson.DeveloperCloud.Logging
{
  /// <summary>
  /// All log messages are assigned to a log level, typically reactors have filters and will filter log
  /// messages that have lower levels. NONE is considered the lowest level, ALL is considered the highest level.
  /// </summary>
  public enum LogLevel
  {
    /// <summary>
    /// Not used.
    /// </summary>
    NONE,
    /// <summary>
    /// Debug level log message, this should be used for messages for the developer.
    /// </summary>
    DEBUG,
    /// <summary>
    /// Status level log message, this should inform the user what is happening in the application.
    /// </summary>
    STATUS,
    /// <summary>
    /// Warning log level should be used for messages when something may be wrong.
    /// </summary>
    WARNING,
    /// <summary>
    /// Error log level should be used for messages when something has going wrong.
    /// </summary>
    ERROR,
    /// <summary>
    /// Critical level log messages should be used for catastrophic failures.
    /// </summary>
    CRITICAL,
  };

  /// <summary>
  /// This data class is passed to all Reactors when a log message is passed into the Logger singleton.
  /// </summary>
  public class LogRecord
  {
    /// <summary>
    /// The time stamp this log message in UTC time.
    /// </summary>
    public DateTime m_TimeStamp = DateTime.UtcNow;
    /// <summary>
    /// The level of this log message.
    /// </summary>
    public LogLevel m_Level = LogLevel.STATUS;
    /// <summary>
    /// What sub-system sent this message.
    /// </summary>
    public string m_SubSystem;
    /// <summary>
    /// The log message.
    /// </summary>
    public string m_Message;

    /// <summary>
    /// The default constructor for a LogRecord.
    /// </summary>
    /// <param name="level">The log level.</param>
    /// <param name="subSystem">The subsystem that orignates this log messages.</param>
    /// <param name="messageFmt">The message string with format parameters.</param>
    /// <param name="args">The format parameters.</param>
    public LogRecord(LogLevel level, string subSystem, string messageFmt, params object[] args)
    {
      m_Level = level;
      m_SubSystem = subSystem;
      m_Message = string.Format(messageFmt, args);
    }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents the current <see cref="IBM.Watson.DeveloperCloud.Logging.LogRecord"/>.
    /// </summary>
    /// <returns>A <see cref="System.String"/> that represents the current <see cref="IBM.Watson.DeveloperCloud.Logging.LogRecord"/>.</returns>
    public override string ToString()
    {
      return string.Format("[{0:MM/dd/yyyy HH:mm:ss}][{1}][{2}] {3}", m_TimeStamp, m_SubSystem, m_Level.ToString(), m_Message);
    }
  };

  /// <summary>
  /// This singleton class maintains the of list of installed reactors and handles all LogRecord 
  /// objects. See the static class Log for functions the end user of this system should actually
  /// be calling. This class is thread safe.
  /// </summary>
  public class LogSystem
  {
    #region Public Properties
    /// <summary>
    /// Returns the singleton instance of the Logger object.
    /// </summary>
    public static LogSystem Instance { get { return Singleton<LogSystem>.Instance; } }
    #endregion

    #region Private Data
    private static bool sm_bInstalledDefaultReactors = false;
    List<ILogReactor> m_Reactors = new List<ILogReactor>();
    #endregion

    #region Public Functions
    public static List<ILogReactor> ReactorsInstalled
    {
      get
      {
        return LogSystem.Instance.m_Reactors;
      }
    }

    /// <summary>
    /// Install a default debug and file reactor.
    /// </summary>
    public static void InstallDefaultReactors(int logHistory = 2, LogLevel logLevelFileReactor = LogLevel.STATUS)
    {
      if (!sm_bInstalledDefaultReactors)
      {
        // install the default reactors...
        sm_bInstalledDefaultReactors = true;
#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID
        LogSystem.Instance.InstallReactor(new DebugReactor());
#endif

        if (!string.IsNullOrEmpty(Constants.Path.LOG_FOLDER) && !System.IO.Directory.Exists(Application.persistentDataPath + Constants.Path.LOG_FOLDER))
          System.IO.Directory.CreateDirectory(Application.persistentDataPath + Constants.Path.LOG_FOLDER);

        LogSystem.Instance.InstallReactor(new FileReactor(Application.persistentDataPath + Constants.Path.LOG_FOLDER + "/" + Application.productName + ".log", logLevelFileReactor, logHistory));

        Application.logMessageReceived += UnityLogCallback;
      }
    }

    static void UnityLogCallback(string condition, string stacktrace, LogType type)
    {
      if (type == LogType.Exception)
        Log.Critical("Unity", "Unity Exception {0} : {1}", condition, stacktrace);
    }

    /// <summary>
    /// Installs a reactor into this Logger.
    /// </summary>
    /// <param name="reactor">The reactor object.</param>
    public void InstallReactor(ILogReactor reactor)
    {
      lock (m_Reactors)
      {
        m_Reactors.Add(reactor);
      }
      // set our default reactor flag to true if the user installs their own reactors.
      sm_bInstalledDefaultReactors = true;
    }

    /// <summary>
    /// Removes a reactor from this Logger.
    /// </summary>
    /// <param name="reactor">The reactor to remove.</param>
    /// <returns>Returns true on success.</returns>
    public bool RemoveReactor(ILogReactor reactor)
    {
      lock (m_Reactors)
      {
        return m_Reactors.Remove(reactor);
      }
    }

    /// <summary>
    /// Send the given LogRecord to all installed reactors.
    /// </summary>
    /// <param name="log">The LogRecord to pass to all reactors.</param>
    public void ProcessLog(LogRecord log)
    {
      lock (m_Reactors)
      {
        foreach (var reactor in m_Reactors)
          reactor.ProcessLog(log);
      }
    }
    #endregion
  }

  /// <summary>
  /// Helper static class for logging into the Logger.
  /// </summary>
  public static class Log
  {
    /// <summary>
    /// Log a DEBUG level message.
    /// </summary>
    /// <param name="subSystem">Name of the subsystem.</param>
    /// <param name="messageFmt">Message with formatting.</param>
    /// <param name="args">Formatting arguments.</param>
#if UNITY_EDITOR
    public static void Debug(string subSystem, string messageFmt, params object[] args)
    {
      LogSystem.Instance.ProcessLog(new LogRecord(LogLevel.DEBUG, subSystem, messageFmt, args));
      System.Console.WriteLine("[{0}][{1}]: {2}", LogLevel.DEBUG, subSystem, string.Format(messageFmt, args));
    }
#else
        // We compile out Log.Debug() functions in release builds.
        public static void Debug(string subSystem, string messageFmt, params object[] args )
        {}
#endif

    /// <summary>
    /// Log a STATUS level message.
    /// </summary>
    /// <param name="subSystem">Name of the subsystem.</param>
    /// <param name="messageFmt">Message with formatting.</param>
    /// <param name="args">Formatting arguments.</param>
    public static void Status(string subSystem, string messageFmt, params object[] args)
    {
      LogSystem.Instance.ProcessLog(new LogRecord(LogLevel.STATUS, subSystem, messageFmt, args));
      System.Console.WriteLine("[{0}][{1}]: {2}", LogLevel.STATUS, subSystem, string.Format(messageFmt, args));
    }
    /// <summary>
    /// Log a WARNING level message.
    /// </summary>
    /// <param name="subSystem">Name of the subsystem.</param>
    /// <param name="messageFmt">Message with formatting.</param>
    /// <param name="args">Formatting arguments.</param>
    public static void Warning(string subSystem, string messageFmt, params object[] args)
    {
      LogSystem.Instance.ProcessLog(new LogRecord(LogLevel.WARNING, subSystem, messageFmt, args));
      System.Console.WriteLine("[{0}][{1}]: {2}", LogLevel.WARNING, subSystem, string.Format(messageFmt, args));
    }
    /// <summary>
    /// Log a ERROR level message.
    /// </summary>
    /// <param name="subSystem">Name of the subsystem.</param>
    /// <param name="messageFmt">Message with formatting.</param>
    /// <param name="args">Formatting arguments.</param>
    public static void Error(string subSystem, string messageFmt, params object[] args)
    {
      LogSystem.Instance.ProcessLog(new LogRecord(LogLevel.ERROR, subSystem, messageFmt, args));
      System.Console.WriteLine("[{0}][{1}]: {2}", LogLevel.ERROR, subSystem, string.Format(messageFmt, args));
    }
    /// <summary>
    /// Log a CRITICAL level message.
    /// </summary>
    /// <param name="subSystem">Name of the subsystem.</param>
    /// <param name="messageFmt">Message with formatting.</param>
    /// <param name="args">Formatting arguments.</param>
    public static void Critical(string subSystem, string messageFmt, params object[] args)
    {
      LogSystem.Instance.ProcessLog(new LogRecord(LogLevel.CRITICAL, subSystem, messageFmt, args));
      System.Console.WriteLine("[{0}][{1}]: {2}", LogLevel.CRITICAL, subSystem, string.Format(messageFmt, args));
    }
  }
}
