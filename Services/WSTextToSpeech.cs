using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class WatsonTextToSpeech{

	#region Enumarations Watson Text to Speech

	public enum AudioFormatType{
		oggWithCodecOpus = 0,	
		wav,					//Currently used
		flac,					
		NaN
	};

	public enum VoiceType{
		en_US_Michael = 0,
		en_US_Lisa,
		en_US_Allison,
		en_GB_Kate,
		es_ES_Enrique,
		es_ES_Laura,
		es_US_Sofia,
		de_DE_Dieter,
		de_DE_Birgit,
		fr_FR_Renee,
		it_IT_Francesca,
		NaN
	};

	#endregion

	#region Contructor - Watson Text To Speech and Override ToString

	/// <summary>
	/// Initializes a new instance of the <see cref="WatsonTextToSpeech"/> class.
	/// </summary>
	/// <param name="textToSpeech">Text to speech. String paremeter</param>
	/// <param name="voice">Voice.</param>
	/// <param name="audioFormat">Audio format.</param>
	public WatsonTextToSpeech(string textToSpeech, WatsonTextToSpeech.VoiceType voice = WatsonTextToSpeech.VoiceType.en_US_Michael, WatsonTextToSpeech.AudioFormatType audioFormat = WatsonTextToSpeech.AudioFormatType.wav){
		this.textToSpeech = textToSpeech;
		if(audioFormat != AudioFormatType.wav){
			Debug.LogWarning("Currently, WAV format is the only format supported by Unity; so using WAV format now.");
		}
		this.audioFormat = AudioFormatType.wav;
		this.voice = voice;
	}
	
	public override string ToString (){
		return string.Format ("Watson TextToSpeech: Text={0}, Voice={1}, Format={2}", textToSpeech, voiceString, audioFormatString);
	}

	#endregion

	#region Watson Text To Speech Variables

	/// <summary>
	/// The selected audio format. The default one is WAV
	/// </summary>
	public AudioFormatType audioFormat {get;internal set;}
	/// <summary>
	/// The voice. The default one is English US Michael
	/// </summary>
	public VoiceType voice {get;internal set;}
	/// <summary>
	/// The text to speech. String parameter
	/// </summary>
	public string textToSpeech {get;internal set;}

	/// <summary>
	/// Gets the audio format string.
	/// </summary>
	/// <value>The audio format string.</value>
	public string audioFormatString{
		get{
			string audioFormatSelected = "";
			switch (audioFormat) {
			case AudioFormatType.oggWithCodecOpus:		audioFormatSelected = "audio/ogg;codecs=opus";			break;
			case AudioFormatType.wav:					audioFormatSelected = "audio/wav";						break;
			case AudioFormatType.flac:					audioFormatSelected = "audio/flac";						break;
			default:
				audioFormatSelected = "audio/wav";
				Debug.LogError("Unknown AudioFormat appeared as "+audioFormat+" - Audio Format needs to be defined!");
				break;
			}
			return audioFormatSelected;
		}
	}

	/// <summary>
	/// Gets the audio format extension.
	/// Default is: wav
	/// </summary>
	/// <value>The audio format extension.</value>
	public string audioFormatExtension{
		get{
			string audioFormatSelectedExtension = "";
			switch (audioFormat) {
			case AudioFormatType.oggWithCodecOpus:		audioFormatSelectedExtension = "ogg";			break;
			case AudioFormatType.wav:					audioFormatSelectedExtension = "wav";			break;
			case AudioFormatType.flac:					audioFormatSelectedExtension = "flac";			break;
			default:
				audioFormatSelectedExtension = "wav";
				Debug.LogError("Unknown AudioFormat appeared as "+audioFormat+" - Audio Format needs to be defined!");
				break;
			}
			return audioFormatSelectedExtension;
		}
	}

	/// <summary>
	/// Gets the voice string. 
	/// Default is US English Michael Voice. 
	/// </summary>
	/// <value>The voice string.</value>
	public string voiceString{
		get{
			string voiceSelected = "";
			switch (voice) {
			case VoiceType.en_US_Michael: 		voiceSelected = "en-US_MichaelVoice"; 		break;
			case VoiceType.en_US_Lisa: 			voiceSelected = "en-US_LisaVoice"; 			break;
			case VoiceType.en_US_Allison:		voiceSelected = "en-US_AllisonVoice";		break;
			case VoiceType.en_GB_Kate:			voiceSelected = "en-GB_KateVoice";			break;
			case VoiceType.es_ES_Enrique:		voiceSelected = "es-ES_EnriqueVoice";		break;
			case VoiceType.es_ES_Laura:			voiceSelected = "es-ES_LauraVoice";			break;
			case VoiceType.es_US_Sofia:			voiceSelected = "es-US_SofiaVoice";			break;
			case VoiceType.de_DE_Dieter:		voiceSelected = "de-DE_DieterVoice";		break;
			case VoiceType.de_DE_Birgit:		voiceSelected = "de-DE_BirgitVoice";		break;
			case VoiceType.fr_FR_Renee:			voiceSelected = "fr-FR_ReneeVoice";			break;
			case VoiceType.it_IT_Francesca:		voiceSelected = "it-IT_FrancescaVoice";		break;
				
			default:
				voiceSelected = "en-US_MichaelVoice";
				Debug.LogError("Unknown Voice appeared as "+voice+" - Voice needs to be defined!");
				break;
			}
			return voiceSelected;
		}
	}
	
	#endregion

	#region Parameters from Bluemix

	public string textToSpeechForGETParameter{
		get{
			return textToSpeech.Replace(" ","%20");
		}
	}

	public string parameterAcceptedAudio{
		get{
			return string.Concat("accept", "=", audioFormatString);
		}
	}
	
	public string parameterVoice{
		get{
			return string.Concat("voice", "=", voiceString);
		}
	}
	
	public string parameterText{
		get{
			return string.Concat("text", "=", textToSpeechForGETParameter);
		}
	}
	
	#endregion


}

[RequireComponent(typeof(AudioSource))]
public class WSTextToSpeech : WatsonService {

	#region Service URL , UserName, Password  - Each Service should be implement these variables!
	
	public override string serviceURL{
		get{
			if(String.IsNullOrEmpty(_serviceURL)){
				_serviceURL = "https://stream.watsonplatform.net/text-to-speech/api";
			}
			return _serviceURL;
		}
		set{
			_serviceURL = value;
		}
	}

	public override string serviceCredentialUserName{
		get{
			if(String.IsNullOrEmpty(_serviceCredentialUserName)){
				_serviceCredentialUserName = "a5aa6c60-c917-4519-abf8-7256169536f2";
			}
			return _serviceCredentialUserName;
		}
		set{
			_serviceCredentialUserName = value;
		}
	}

	public override string serviceCredentialPassword{
		get{
			if(String.IsNullOrEmpty(_serviceCredentialPassword)){
				_serviceCredentialPassword = "YVeOHnAMm48F";
			}
			return _serviceCredentialPassword;
		}
		set{
			_serviceCredentialPassword = value;
		}
	}

	public override string serviceVersion{
		get{
			if(String.IsNullOrEmpty(_serviceVersion)){
				_serviceVersion = "v1";
			}
			return _serviceVersion;
		}
		set{
			_serviceVersion = value;
		}
	}

	#endregion

	#region Service Related URL and Parameters

	public string urlSynthesize{
		get{
			return getServiceFunctionURL("synthesize");
		}
	}

	public WatsonTextToSpeech textToSpeech{
		get{
			return null;
		}
	}

	#endregion

	#region Watson Service Text to Speech Singleton object
	private static WSTextToSpeech _instance;
	public static WSTextToSpeech Instance{
		get{
			if(_instance == null){
				_instance = WConfiguration.gameObjectWatson.AddComponent<WSTextToSpeech>();
			}
			return _instance;
		}
	}
	#endregion

	#region Text To Speech Functionality - StartSpeaking , ServiceRequest to Bluemix

	public static void StartSpeaking(string textToSpeech, WatsonTextToSpeech.VoiceType voice = WatsonTextToSpeech.VoiceType.en_US_Michael, WatsonTextToSpeech.AudioFormatType audioFormat = WatsonTextToSpeech.AudioFormatType.wav){
		WatsonTextToSpeech watsonTextToSpeech = new WatsonTextToSpeech(textToSpeech, voice, audioFormat);
		Instance.StartSpeaking(watsonTextToSpeech, Instance.AsyncTextToSpeechRequestResult);
	}

	private void StartSpeaking(WatsonTextToSpeech watsonTextToSpeech, Action < WatsonTextToSpeech, TimeSpan> callback)
	{
		string urlTextToSpeechServiceGETRequest = getGETRequestUrl(urlSynthesize, watsonTextToSpeech.parameterAcceptedAudio, watsonTextToSpeech.parameterVoice, watsonTextToSpeech.parameterText);
		
		WWWForm form = new WWWForm();
		Dictionary<string, string> headers = form.headers.AddAuthorizationHeader(serviceCredentialUserName, serviceCredentialPassword);
		WWW uri = new WWW (urlTextToSpeechServiceGETRequest, null, headers );

		Debug.Log("Bluemix Request: " + watsonTextToSpeech.ToString());
		StartCoroutine(AsyncTextToSpeechRequestToBluemix(uri, watsonTextToSpeech, callback));
	}

	private IEnumerator AsyncTextToSpeechRequestToBluemix(WWW wwwRequestToBluemixForTextToSpeech, WatsonTextToSpeech watsonTextToSpeech,  Action < WatsonTextToSpeech, TimeSpan> callback)
	{
		DateTime timeRequestStart = System.DateTime.Now;
		yield return wwwRequestToBluemixForTextToSpeech;
		TimeSpan timeRequestElapsed = System.DateTime.Now.Subtract(timeRequestStart);

		if (wwwRequestToBluemixForTextToSpeech.error == null && wwwRequestToBluemixForTextToSpeech.isDone) 
		{
			if(watsonTextToSpeech.audioFormat == WatsonTextToSpeech.AudioFormatType.wav){
				PlayAudioWAV(wwwRequestToBluemixForTextToSpeech.bytes);
				yield return new WaitForEndOfFrame();
				WaveFormTest waveFormTest = gameObject.GetComponent<WaveFormTest>();
				if(waveFormTest == null){
					this.gameObject.AddComponent<WaveFormTest> ();
				}
			}
			else{
				//TODO: add all supported file playing audio files!
				Debug.LogError("File type : " + watsonTextToSpeech.audioFormatString + " is not supported!" );
			}

			if(callback != null){
				callback.Invoke(watsonTextToSpeech, timeRequestElapsed);
			}

		} 
		else 
		{
			Debug.Log ("Error: " + wwwRequestToBluemixForTextToSpeech.error + " - url: " + wwwRequestToBluemixForTextToSpeech.url);
		}
		
		wwwRequestToBluemixForTextToSpeech.Dispose();
	}

	private void PlayAudioWAV(byte[] audio){
		WAudioWAV wav = new WAudioWAV(audio, true);
		AudioClip audioClip = AudioClip.Create("WatsonTextToSpeech_AudioClip", wav.sampleCount, 1,wav.frequency, false);
		audioClip.SetData(wav.leftChannel, 0);	//We are using left channel - as 2D sound
		
		AudioSource attachedAudioSource = this.transform.GetComponent<AudioSource>();
		if(attachedAudioSource == null){
			attachedAudioSource = this.gameObject.AddComponent<AudioSource>();
		}


		attachedAudioSource.spatialBlend = 0.0f; //2D Sound
		attachedAudioSource.loop = false;
		//attachedAudioSource.PlayOneShot(audioClip);
		attachedAudioSource.clip = audioClip;
		attachedAudioSource.Play();
	}

	private void AsyncTextToSpeechRequestResult(WatsonTextToSpeech watsonTextToSpeech, TimeSpan timeRequestElapsed){
		Debug.Log(string.Format( "Bluemix Response: {0} - Request time: {1}:{2} ",  watsonTextToSpeech.ToString(), timeRequestElapsed.Seconds, timeRequestElapsed.Milliseconds));
	}

	#endregion

	
}



public class WaveFormTest : MonoBehaviour {

	private float modifierSpectrum = 5.0f;
	int resolution = 60;
	
	float[] waveForm;
	Transform[] waveFormCubes;

	float[] samples;
	AudioSource audioSource;

	float[] spectrumData;

	// Use this for initialization
	void Start () {
		spectrumData = new float[1024];
		resolution = (int)(1 / Time.fixedDeltaTime);
		audioSource = this.transform.GetComponent<AudioSource>();
		resolution = audioSource.clip.frequency / resolution;
		
		samples = new float[audioSource.clip.samples*audioSource.clip.channels];
		audioSource.clip.GetData(samples,0);
		
		waveForm = new float[(samples.Length/resolution)];
		
		for (int i = 0; i < waveForm.Length; i++)
		{
			waveForm[i] = 0;
			
			for(int ii = 0; ii<resolution; ii++)
			{
				waveForm[i] += Mathf.Abs(samples[(i * resolution) + ii]);
			}          
			
			waveForm[i] /= resolution;
		}

		int numberOfCubes = 50;
		float displacementX = 0.5f;
		Vector3 size = new Vector3 (0.1f, 1.0f, 1.0f);
		waveFormCubes = new Transform[numberOfCubes];
		for (int i = 0; i < numberOfCubes; i++) {
			GameObject cubeTemp = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cubeTemp.transform.position = new Vector3(i * displacementX - (numberOfCubes/2)* displacementX, 1.0f, 1.0f);
			cubeTemp.transform.localScale = size;
			waveFormCubes[i] = cubeTemp.transform;
		}


	}
	
	// Update is called once per frame
	void FixedUpdate () {
		for (int i = 0; i < waveForm.Length - 1; i++)
		{
			Vector3 sv = new Vector3(i * 0.1f, waveForm[i]*10 , 0);
			Vector3 ev = new Vector3(i * 0.1f, -waveForm[i] * 10, 0);
			
			Debug.DrawLine(sv, ev, Color.yellow);
			//Debug.Log ("waveForm[" + i + "] = " + waveForm[i]);
		}
		
		int current = audioSource.timeSamples / resolution;

		//Debug.Log ("audio.timeSamples: " + audioSource.timeSamples + " - resolution: " + resolution + " - current: " + current + " - waveForm.Length: " + waveForm.Length);

		//Debug.Break ();
		//current *= 2;


		Vector3 c = new Vector3(current* 0.1f,0,0);
		
		Debug.DrawLine(c, c + Vector3.up * 10, Color.white);
	}

	void Update () { 
		AudioListener.GetOutputData (spectrumData, 0);
		//float[] spectrum  = AudioListener.GetSpectrumData(1024, 0, FFTWindow.Hamming); 
		/* c1 = 64hz c3 = 256hz c4 = 512hz c5 = 1024 */ 
		float c1 = spectrumData[3] + spectrumData[2] + spectrumData[4]; 
		float c3 = spectrumData[11] + spectrumData[12] + spectrumData[13]; 
		float c4 = spectrumData[22] + spectrumData[23] + spectrumData[24]; 
		float c5 = spectrumData[44] + spectrumData[45] + spectrumData[46] + spectrumData[47] + spectrumData[48] + spectrumData[49]; 

		GameObject[] cubes = GameObject.FindGameObjectsWithTag("Player"); 

		for(var i = 0; i < cubes.Length; i++) { 
			switch (cubes[i].name) { 
			case "c1": cubes[i].transform.localScale = new Vector3(cubes[i].transform.localScale.x, c1 * modifierSpectrum, cubes[i].transform.localScale.z); break; 
			case "c3": cubes[i].transform.localScale = new Vector3(cubes[i].transform.localScale.x, c3 * modifierSpectrum, cubes[i].transform.localScale.z); ; break; 
			case "c4": cubes[i].transform.localScale = new Vector3(cubes[i].transform.localScale.x, c4 * modifierSpectrum, cubes[i].transform.localScale.z); ; break; 
			case "c5": cubes[i].transform.localScale = new Vector3(cubes[i].transform.localScale.x, c5 * modifierSpectrum, cubes[i].transform.localScale.z); ; break; 
			} 
		}

		int current = audioSource.timeSamples / resolution;

		//From Right to Left
//		for(var i = 0; i < waveFormCubes.Length; i++) { 
//			if(i < waveFormCubes.Length - 1){
//				waveFormCubes[i].localScale = waveFormCubes[i+1].localScale;
//			}
//			else{
//				if(current < waveForm.Length){
//					Debug.Log ("waveForm[" + current + "] = " + waveForm[current]);
//					waveFormCubes[i].localScale = new Vector3(waveFormCubes[i].localScale.x, waveForm[current] * 10, waveFormCubes[i].localScale.z);
//				}
//				else
//					Debug.LogError("Current: " + current + " - waveForm.Length: " + waveForm.Length);
//			}
//		}

		float modifierWaveform = 20.0f;
		//From Center to Corners
		for (var i = waveFormCubes.Length - 1; i > (waveFormCubes.Length/2); i--) { 
			//Right part from the center
			waveFormCubes[i].localScale = waveFormCubes[i-1].localScale;
		}

		for(var i = 0; i < (waveFormCubes.Length/2); i++) { 
			//Left part from the center
			waveFormCubes[i].localScale = waveFormCubes[i+1].localScale;
		}

		//center
		if(current < waveForm.Length){
			//Debug.Log ("waveForm[" + current + "] = " + waveForm[current]);
			waveFormCubes[(waveFormCubes.Length/2)].localScale = new Vector3(waveFormCubes[(waveFormCubes.Length/2)].localScale.x, waveForm[current] * modifierWaveform, waveFormCubes[(waveFormCubes.Length/2)].localScale.z);
		}
		else
			Debug.LogError("Current: " + current + " - waveForm.Length: " + waveForm.Length);
	}
}
