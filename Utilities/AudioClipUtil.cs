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

using IBM.Watson.Logging;
using UnityEngine;

namespace IBM.Watson.Utilities
{
    public static class AudioClipUtil
    {
        /// <summary>
        /// This function will combine any number of AudioClips into a single AudioClip. The clips must be the same number of channels
        /// and frequency.
        /// </summary>
        /// <param name="clips">Variable number of AudioClip objects may be provided.</param>
        /// <returns></returns>
        public static AudioClip Combine(params AudioClip[] clips)
        {
            if (clips == null || clips.Length == 0)
                return null;

            AudioClip firstClip = null;

            int length = 0;
            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i] == null)
                    continue;

                if ( firstClip != null )
                {
                    if ( firstClip.channels != clips[i].channels 
                        || firstClip.frequency != clips[i].frequency )
                    {
                        Log.Error( "AudioClipUtil", "Combine() requires clips to have the sample number of channels and same frequency." );
                        return null;
                    }
                }
                else
                    firstClip = clips[i];

                length += clips[i].samples * clips[i].channels;
            }

            float[] data = new float[length];
            length = 0;
            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i] == null)
                    continue;

                float[] buffer = new float[clips[i].samples * clips[i].channels];
                clips[i].GetData(buffer, 0);
                buffer.CopyTo(data, length);
                length += buffer.Length;
            }

            if (length == 0)
                return null;

            AudioClip result = AudioClip.Create( firstClip.name, length / firstClip.channels, firstClip.channels, firstClip.frequency, false );
            result.SetData(data, 0);

            return result;
        }
    }
}
