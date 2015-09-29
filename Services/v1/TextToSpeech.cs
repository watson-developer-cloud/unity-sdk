using UnityEngine;
using System.Collections;
using IBM.Watson.Utilities;

namespace IBM.Watson.Services.v1
{
    public class TextToSpeech
    {

        public delegate void SpeechCallback(AudioClip a_Audio);

        public void ToSpeech(string a_Text, SpeechCallback a_Callback)
        {
            Runnable.Run(ToSpeechCR(a_Text, a_Callback));
        }

        private IEnumerator ToSpeechCR(string a_Text, SpeechCallback a_Callback)
        {
            // TODO: We probably want to communicate with the back-end with something like this..

            //var request = new Connection.Request();
            //request["text"] = a_Text;
            //request["voiceType"] = m_Voice;
            //request.Function = "SpeechToText";
            //request.Type = POST;
            //yield return request.Submit();

            //var response = request.Response();
            

            yield break;
        }
    }

}
