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

using System;
using System.IO;

namespace IBM.Watson.DeveloperCloud.Logging
{
#if !UNITY_WEBPLAYER

    /// <summary>
    /// FileReactor log reactor class.
    /// </summary>
    public class FileReactor : ILogReactor
    {
        #region Public Properties
        /// <summary>
        /// The filename of the log file.
        /// </summary>
        public string LogFile { get; set; }
        /// <summary>
        /// Minimum level of messages to save into the log file.
        /// </summary>
        public LogLevel Level { get; set; }

        /// <summary>
        /// How many log files to maintain.
        /// </summary>
        public int LogHistory { get; set; }

        /// <summary>
        ///  Maximum size of a log file before we rotate.
        /// </summary>
        public int MaxLogSize { get; set; }

        /// <summary>
        /// Gets the log text written to the file system
        /// </summary>
        /// <value>The log text written.</value>
        public string LogTextWritten
        {
            get
            {
                return File.ReadAllText(LogFile);
            }
        }
        #endregion

        #region Construction
        /// <summary>
        /// FileReactor constructor.
        /// </summary>
        /// <param name="logFile">The FileName of the log file.</param>
        /// <param name="level">The minimum level of log messages to be logged into the file.</param>
        /// <param name="logHistory">How many log files to keep as they are rotated each time this reactor is constructed.</param>
        public FileReactor(string logFile, LogLevel level = LogLevel.DEBUG, int logHistory = 2, int maxLogSize = 1024 * 1024)
        {
            LogFile = logFile;
            Level = level;
            LogHistory = logHistory;
            MaxLogSize = maxLogSize;
        }

        public void RotateLogs()
        {
            // rotate existing log files..
            for (int i = LogHistory; i >= 0; --i)
            {
                string src = i > 0 ? LogFile + "." + i.ToString() : LogFile;
                if (File.Exists(src))
                {
                    string dst = LogFile + "." + (i + 1).ToString();
                    File.Copy(src, dst, true);
                }
            }
            File.WriteAllText(LogFile, string.Format("Log File Started {0}...\n", DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss")));
        }

        #endregion

        #region ILogReactor interface
        /// <summary>
        /// Process a LogRecord object.
        /// </summary>
        /// <param name="log">The log record.</param>
        public void ProcessLog(LogRecord log)
        {
            if (log._level >= Level)
            {
                File.AppendAllText(LogFile, string.Format("[{0}][{1}][{2}] {3}\n",
                    log._timeStamp.ToString("MM/dd/yyyy HH:mm:ss"),
                    log._subSystem, log._level.ToString(), log._message));

                // automatically rotate logs once our size is large enough..
                if (new FileInfo(LogFile).Length > MaxLogSize)
                    RotateLogs();
            }
        }
        #endregion
    }
#endif

}
