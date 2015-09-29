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

using System;
using System.IO;

namespace IBM.Watson.Logging
{
    public class FileReactor : ILogReactor
    {
        #region Public Properties
        public string LogFile { get; set; }
        public LogLevel Level { get; set; }
        #endregion

        #region Construction
        public FileReactor(string a_LogFile, LogLevel a_Level = LogLevel.DEBUG, int a_LogHistory = 2 )
        {
            LogFile = a_LogFile;
            Level = a_Level;

            // rotate existing log files..
            for(int i=a_LogHistory;i>=0;--i)
            {
                string src = i > 0 ? LogFile + "." + i.ToString() : LogFile;
                if ( File.Exists( src ) )
                {
                    string dst = LogFile + "." + (i + 1).ToString();
                    File.Copy( src, dst, true );
                }
            }

            File.WriteAllText(LogFile, string.Format("Log File Started {0}...\n",
                DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss")) );
        }
        #endregion

        #region ILogReactor interface
        public void ProcessLog(LogRecord a_Log)
        {
            if (a_Log.m_Level >= Level)
            {
                File.AppendAllText(LogFile, string.Format("[{0}][{1}][{2}] {3}\n",
                    a_Log.m_TimeStamp.ToString("MM/dd/yyyy HH:mm:ss"),
                    a_Log.m_SubSystem, a_Log.m_Level.ToString(), a_Log.m_Message));
            }
        }
        #endregion
    }
}
