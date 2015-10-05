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

using UnityEngine;
using System.Collections;
using IBM.Watson.Services.v1;
using IBM.Watson.Logging;

namespace IBM.Watson.UnitTests
{
    public class TestTextToSpeech : UnitTest
    {
        private TextToSpeech m_TTS = new TextToSpeech();
        private int m_CallbackCount = 0;
                
        public override IEnumerator RunTest()
        {
            m_TTS.ToSpeech( "Hello World using GET", OnSpeechGET );                  // Test GET

            // wait for both callbacks
            while (m_CallbackCount < 2 )
                yield return null;

            yield break;
        }

        private void OnSpeechGET( AudioClip clip )
        {
            Log.Debug( "TestTestToSpeech", "OnSpeechGET invoked." );

            Test( clip != null );
            m_CallbackCount += 1;

            PlayClip( clip );

            Log.Debug( "TestTextToSpeech", "ToSpeech POST." );
            m_TTS.ToSpeech( "Hello World using POST", OnSpeechPOST, true );            // Test POST
        }

        private void OnSpeechPOST( AudioClip clip )
        {
            Log.Debug( "TestTestToSpeech", "OnSpechPOST invoked." );

            Test( clip != null );
            m_CallbackCount += 1;

            PlayClip( clip );
        }

        private void PlayClip( AudioClip clip )
        {
            if ( Application.isPlaying && clip != null )
            {
                GameObject audioObject = new GameObject( "AudioObject" );
                AudioSource source = audioObject.AddComponent<AudioSource>();
                source.spatialBlend = 0.0f;     // 2D sound
                source.loop = false;            // do not loop
                source.clip = clip;             // clip
                source.Play();

                // automatically destroy the object after the sound has played..
                GameObject.Destroy( audioObject, clip.length );
            }
        }

    }
}
