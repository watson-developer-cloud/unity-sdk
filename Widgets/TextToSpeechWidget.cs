using UnityEngine;
using IBM.Watson.Services.v1;
using IBM.Watson.Logging;

[RequireComponent(typeof(AudioSource))]
public class TextToSpeechWidget : MonoBehaviour
{
    #region Private Data
    TextToSpeech m_TTS = new TextToSpeech();

    [SerializeField]
    private string m_Text = "Hello world, my name is Watson.";
    [SerializeField]
    private TextToSpeech.VoiceType m_Voice = TextToSpeech.VoiceType.en_US_Michael;
    #endregion

    private void OnEnable()
    {
        Logger.InstallDefaultReactors();
    }

    private void OnGUI()
    {
        m_Text = GUILayout.TextField( m_Text );

        if ( GUILayout.Button( "Play" ) )
        {
            if ( m_TTS.Voice != m_Voice )
                m_TTS.Voice = m_Voice;

            m_TTS.ToSpeech( m_Text, OnSpeech );
        }
    }

    private void OnSpeech( AudioClip clip )
    {
 		AudioSource source = GetComponent<AudioSource>();
        if ( source != null )
        {
            source.spatialBlend = 0.0f;     // 2D sound
            source.loop = false;            // do not loop
            source.clip = clip;             // clip
            source.Play();
        }
    }
}
