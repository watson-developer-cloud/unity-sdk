using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class WSTextToSpeech : WatsonService {

	#region Service URL , UserName, Password 

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

	//Accept Parameters
	public enum AcceptedAudioFormat{
		oggWithCodecOpus = 0,
		wav,
		flac,
		NaN
	};

	/// <summary>
	/// The selected audio format. The default one is WAV
	/// </summary>
	public AcceptedAudioFormat selectedAudioFormat = AcceptedAudioFormat.wav;

	public string selectedAudioFormatString{
		get{
			string audioFormatSelected = "";
			switch (selectedAudioFormat) {
			case AcceptedAudioFormat.oggWithCodecOpus:		audioFormatSelected = "audio/ogg;codecs=opus";			break;
			case AcceptedAudioFormat.wav:					audioFormatSelected = "audio/wav";						break;
			case AcceptedAudioFormat.flac:					audioFormatSelected = "audio/flac";						break;
			default:
				audioFormatSelected = "audio/wav";
				Debug.LogError("Unknown AudioFormat appeared as "+selectedAudioFormat+" - Audio Format needs to be defined!");
				break;
			}
			return audioFormatSelected;
		}
	}

	public string selectedAudioFormatExtension{
		get{
			string audioFormatSelectedExtension = "";
			switch (selectedAudioFormat) {
			case AcceptedAudioFormat.oggWithCodecOpus:		audioFormatSelectedExtension = "ogg";			break;
			case AcceptedAudioFormat.wav:					audioFormatSelectedExtension = "wav";			break;
			case AcceptedAudioFormat.flac:					audioFormatSelectedExtension = "flac";			break;
			default:
				audioFormatSelectedExtension = "wav";
				Debug.LogError("Unknown AudioFormat appeared as "+selectedAudioFormat+" - Audio Format needs to be defined!");
				break;
			}
			return audioFormatSelectedExtension;
		}
	}

	//Voice Parameters
	public enum Voice{
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

	public Voice selectedVoice = Voice.en_US_Michael;

	public string selectedVoiceString{
		get{
			string voiceSelected = "";
			switch (selectedVoice) {
			case Voice.en_US_Michael: 		voiceSelected = "en-US_MichaelVoice"; 		break;
			case Voice.en_US_Lisa: 			voiceSelected = "en-US_LisaVoice"; 			break;
			case Voice.en_US_Allison:		voiceSelected = "en-US_AllisonVoice";		break;
			case Voice.en_GB_Kate:			voiceSelected = "en-GB_KateVoice";			break;
			case Voice.es_ES_Enrique:		voiceSelected = "es-ES_EnriqueVoice";		break;
			case Voice.es_ES_Laura:			voiceSelected = "es-ES_LauraVoice";			break;
			case Voice.es_US_Sofia:			voiceSelected = "es-US_SofiaVoice";			break;
			case Voice.de_DE_Dieter:		voiceSelected = "de-DE_DieterVoice";		break;
			case Voice.de_DE_Birgit:		voiceSelected = "de-DE_BirgitVoice";		break;
			case Voice.fr_FR_Renee:			voiceSelected = "fr-FR_ReneeVoice";			break;
			case Voice.it_IT_Francesca:		voiceSelected = "it-IT_FrancescaVoice";		break;
			
			default:
				voiceSelected = "en-US_MichaelVoice";
				Debug.LogError("Unknown Voice appeared as "+selectedVoice+" - Voice needs to be defined!");
				break;
			}
			return voiceSelected;
		}
	}

	public string selectedStringToSpeech = "";
	public string selectedStringToSpeechForGETParameter{
		get{
			return selectedStringToSpeech.Replace(" ","%20");
		}
	}

	public string urlSynthesize{
		get{
			return getServiceFunctionURL("synthesize");
		}
	}

	public string parameterAcceptedAudio{
		get{
			return string.Concat("accept", "=", selectedAudioFormatString);
		}
	}

	public string parameterVoice{
		get{
			return string.Concat("voice", "=", selectedVoiceString);
		}
	}

	public string parameterText{
		get{
			return string.Concat("text", "=", selectedStringToSpeechForGETParameter);
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

	#region Test Code - To be Deleted
	
	public bool isTestingTextToSpeech = false;
	public string testString = "Hello my people";
	private int sampleRate = WATSONSDK_TTS_AUDIO_CODEC_TYPE_WAV_SAMPLE_RATE;

	// Use this for initialization
	void Start () {
		//WServer.Login("a5aa6c60-c917-4519-abf8-7256169536f2", "YVeOHnAMm48F");
	}
	
	// Update is called once per frame
	void Update () {
		if (isTestingTextToSpeech) {
			isTestingTextToSpeech = false;
			testTextToSpeechAllOptions();
		}
	}

	private void testTextToSpeechAllOptions(){
//		for (int i = 0; i < (int)Voice.NaN; i++) {
//			for (int j = 0; j < (int)AcceptedAudioFormat.NaN; j++) {
//				selectedVoice = (Voice) i;
//				selectedAudioFormat = (AcceptedAudioFormat) j;
//				StartSpeeking (testString, null);
//			}
//		}

//		selectedVoice = Voice.en_US_Michael;
//		selectedAudioFormat = AcceptedAudioFormat.oggWithCodecOpus;
//		StartSpeeking (testString, null);

		StartSpeeking (testString, null);
	}

	#endregion

	#region Public Static Functions to be used by other classes

	public static void StartSpeaking(string textToSpeech){
		Instance.StartSpeeking(textToSpeech, null);
	}

	#endregion

	private void StartSpeeking(string voiceString, Action <byte[]>  callback)
	{
		Debug.Log("StartSpeeking for: " + selectedVoiceString);
		selectedStringToSpeech = voiceString;
		string ttsURL = getGETRequestUrl(urlSynthesize, parameterAcceptedAudio, parameterVoice, parameterText);

		WWWForm form = new WWWForm();
		Dictionary<string, string> headers = form.headers.AddAuthorizationHeader(serviceCredentialUserName, serviceCredentialPassword);
		WWW uri = new WWW (ttsURL, null, headers );

		//Debug.Log (uri.url);
		StartCoroutine(SendVoiceRequest(uri, callback));
	}

	private IEnumerator SendVoiceRequest(WWW www, Action <byte[]> callback)
	{
		string currentVoice = selectedVoiceString;
		string currentFormat = selectedAudioFormatExtension;
		yield return www;

		if (www.error == null && www.isDone) 
		{
			byte[] dataNewHeader = getAudioByteArrayAfterStrippingAndAddingNewWAVHeader(www.bytes);

			WAudioWAV wav = new WAudioWAV(dataNewHeader);
			AudioClip audioClip = AudioClip.Create("WatsonTextToSpeech_AudioClip", wav.SampleCount, 1,wav.Frequency, false);
			audioClip.SetData(wav.LeftChannel, 0);	//We are using left channel

			AudioSource attachedAudioSource = this.transform.GetComponent<AudioSource>();
			if(attachedAudioSource == null){
				attachedAudioSource = this.gameObject.AddComponent<AudioSource>();
			}

			attachedAudioSource.spatialBlend = 0.0f; //2D Sound
			attachedAudioSource.PlayOneShot(audioClip);
			attachedAudioSource.Play();
		} 
		else 
		{
			Debug.Log ("Error: " + www.error + " - url:" + www.url);
		}

		www.Dispose();
	}

	private byte[] getAudioByteArrayAfterStrippingAndAddingNewWAVHeader(byte[] wav) {
		
		int headerSize = 44;
		int metadataSize = 48;

		if(sampleRate == 0 && wav.Length > 28){
			//Get WAV Sample rate from 24
			sampleRate = BitConverter.ToInt32(wav, 24);
		}

		//Creating a new WAV without header information
		byte[] wavNoheader = new byte[wav.Length - (headerSize+metadataSize)];
		System.Buffer.BlockCopy(wav,headerSize+metadataSize, wavNoheader, 0, wav.Length - (headerSize+metadataSize));

		//Adding WAV header information
		byte[] newWavData = getAudioByteArrayWithWAVHeader(wavNoheader);

		return newWavData;
	}

	private byte[]	getAudioByteArrayWithWAVHeader(byte[] wavNoheader) {

		int headerSize = 44;
		long totalAudioLen = wavNoheader.Length;
		long totalDataLen = wavNoheader.Length + headerSize-8;
		long longSampleRate = (sampleRate == 0 ? 48000 : sampleRate);
		int channels = 1;
		long byteRate = 16 * 11025 * channels/8;

		//Byte *header = (Byte*)malloc(44);
		byte[] header = new byte[44];
		header[0] = (byte)'R';  // RIFF/WAVE header
		header[1] = (byte)'I';
		header[2] = (byte)'F';
		header[3] = (byte)'F';
		header[4] = (Byte) (totalDataLen & 0xff);
		header[5] = (Byte) ((totalDataLen >> 8) & 0xff);
		header[6] = (Byte) ((totalDataLen >> 16) & 0xff);
		header[7] = (Byte) ((totalDataLen >> 24) & 0xff);
		header[8] = (byte)'W';
		header[9] = (byte)'A';
		header[10] = (byte)'V';
		header[11] = (byte)'E';
		header[12] = (byte)'f';  // 'fmt ' chunk
		header[13] = (byte)'m';
		header[14] = (byte)'t';
		header[15] = (byte)' ';
		header[16] = 16;  // 4 bytes: size of 'fmt ' chunk
		header[17] = 0;
		header[18] = 0;
		header[19] = 0;
		header[20] = 1;  // format = 1
		header[21] = 0;
		header[22] = (Byte) channels;
		header[23] = 0;
		header[24] = (Byte) (longSampleRate & 0xff);
		header[25] = (Byte) ((longSampleRate >> 8) & 0xff);
		header[26] = (Byte) ((longSampleRate >> 16) & 0xff);
		header[27] = (Byte) ((longSampleRate >> 24) & 0xff);
		header[28] = (Byte) (byteRate & 0xff);
		header[29] = (Byte) ((byteRate >> 8) & 0xff);
		header[30] = (Byte) ((byteRate >> 16) & 0xff);
		header[31] = (Byte) ((byteRate >> 24) & 0xff);
		header[32] = (Byte) (2 * 8 / 8);  // block align
		header[33] = 0;
		header[34] = 16;  // bits per sample
		header[35] = 0;
		header[36] = (byte)'d';
		header[37] = (byte)'a';
		header[38] = (byte)'t';
		header[39] = (byte)'a';
		header[40] = (Byte) (totalAudioLen & 0xff);
		header[41] = (Byte) ((totalAudioLen >> 8) & 0xff);
		header[42] = (Byte) ((totalAudioLen >> 16) & 0xff);
		header[43] = (Byte) ((totalAudioLen >> 24) & 0xff);

		byte[] newWavData = new byte[wavNoheader.Length + 44];

		System.Buffer.BlockCopy(header,0, newWavData, 0, 44);
		System.Buffer.BlockCopy(wavNoheader,0, newWavData, 44, wavNoheader.Length);

		return newWavData;
	}

}
