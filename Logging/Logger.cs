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
*/

using IBM.Watson.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IBM.Watson.Logging
{
    public enum LogLevel
    {
        NONE,
        DEBUG,
        STATUS,
        WARNING,
        ERROR,
        CRITICAL,
        ALL,
    };

    public class LogRecord
    {
        public DateTime m_TimeStamp = DateTime.UtcNow;
        public LogLevel m_Level = LogLevel.STATUS;
        public string m_SubSystem;
        public string m_Message;

        public LogRecord(LogLevel a_Level, string a_SubSystem, string a_MessageFmt, params object[] a_Args)
        {
            m_Level = a_Level;
            m_SubSystem = a_SubSystem;
            m_Message = string.Format(a_MessageFmt, a_Args);
        }
    };

    public class Logger
    {
        #region Public Properties
        public static Logger Instance { get { return Singleton<Logger>.Instance; } }
        #endregion

        #region Private Data
        private static bool sm_bInstalledDefaultReactors = false;
        List<ILogReactor> m_Reactors = new List<ILogReactor>();
        #endregion

        #region Public Functions
        public static void InstallDefaultReactors()
        {
            if (! sm_bInstalledDefaultReactors )
            {
                // install the default reactors...
                sm_bInstalledDefaultReactors = true;
                Logger.Instance.InstallReactor( new DebugReactor() );
                Logger.Instance.InstallReactor( new FileReactor( Application.persistentDataPath + "/Watson.log" ) );
            }
        }
        public void InstallReactor(ILogReactor a_Reactor)
        {
            lock (m_Reactors)
            {
                m_Reactors.Add(a_Reactor);
            }
        }
        public bool RemoveReactor(ILogReactor a_Reactor)
        {
            lock (m_Reactors)
            {
                return m_Reactors.Remove(a_Reactor);
            }
        }
        public void ProcessLog(LogRecord a_Log)
        {
            lock (m_Reactors)
            {
                foreach (var reactor in m_Reactors)
                    reactor.ProcessLog(a_Log);
            }
        }
        #endregion
    }

    public static class Log
    {
        public static void Debug(string a_SubSystem, string a_MessageFmt, params object[] a_Args)
        {
            Logger.Instance.ProcessLog(new LogRecord(LogLevel.DEBUG, a_SubSystem, a_MessageFmt, a_Args));
        }
        public static void Status(string a_SubSystem, string a_MessageFmt, params object[] a_Args)
        {
            Logger.Instance.ProcessLog(new LogRecord(LogLevel.STATUS, a_SubSystem, a_MessageFmt, a_Args));
        }
        public static void Warning(string a_SubSystem, string a_MessageFmt, params object[] a_Args)
        {
            Logger.Instance.ProcessLog(new LogRecord(LogLevel.WARNING, a_SubSystem, a_MessageFmt, a_Args));
        }
        public static void Error(string a_SubSystem, string a_MessageFmt, params object[] a_Args)
        {
            Logger.Instance.ProcessLog(new LogRecord(LogLevel.ERROR, a_SubSystem, a_MessageFmt, a_Args));
        }
        public static void Critical(string a_SubSystem, string a_MessageFmt, params object[] a_Args)
        {
            Logger.Instance.ProcessLog(new LogRecord(LogLevel.CRITICAL, a_SubSystem, a_MessageFmt, a_Args));
        }
    }
}
