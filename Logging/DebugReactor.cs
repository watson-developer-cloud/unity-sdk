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

namespace IBM.Watson.Logging
{
    //! Unity debug log reactor
    public class DebugReactor : ILogReactor
    {
        #region Public Properties
        public LogLevel Level { get; set; }
        #endregion

        #region Construction
        public DebugReactor( LogLevel a_Level = LogLevel.DEBUG )
        {
            Level = a_Level;
        }
        #endregion

        #region ILogReactor Interface
        public void ProcessLog( LogRecord a_Log )
        {
            if ( a_Log.m_Level >= Level )
            {
                UnityEngine.Debug.Log( string.Format( "[{0}][{1}][{2}] {3}", 
                    a_Log.m_TimeStamp.ToString("MM/dd/yyyy HH:mm:ss"),
                    a_Log.m_SubSystem, a_Log.m_Level.ToString(), a_Log.m_Message ) );
            }
        }
        #endregion
    }
}
