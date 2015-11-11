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
 * @author Richard Lyle (rolyle@us.ibm.com)
*/

namespace IBM.Watson.Logging
{
    /// <summary>
    /// This log reactor logs into the UnityEngine.Debug.Log() function.
    /// </summary>
    public class DebugReactor : ILogReactor
    {
        #region Public Properties
        public LogLevel Level { get; set; }
        #endregion

        #region Construction
        /// <summary>
        /// DebugReactor constructor.
        /// </summary>
        /// <param name="level">Minimum level of log messages to log.</param>
        public DebugReactor(LogLevel level = LogLevel.DEBUG)
        {
            Level = level;
        }
        #endregion

        #region ILogReactor Interface
        public void ProcessLog(LogRecord log)
        {
            if (log.m_Level >= Level)
            {
				string logString = string.Format("[{0}][{1}][{2}] {3}",
				                                 log.m_TimeStamp.ToString("MM/dd/yyyy HH:mm:ss"),
				                                 log.m_SubSystem, log.m_Level.ToString(), log.m_Message);

				if(log.m_Level == LogLevel.ERROR || log.m_Level == LogLevel.CRITICAL){
					UnityEngine.Debug.LogError(logString);
				}
				else if(log.m_Level == LogLevel.WARNING){
					UnityEngine.Debug.LogWarning(logString);
				}
				else{
					UnityEngine.Debug.Log(logString);
				}
            }
        }
        #endregion
    }
}
