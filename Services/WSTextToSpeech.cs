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

		Debug.Log (uri.url);
		StartCoroutine(SendVoiceRequest(uri, callback));
	}

	private IEnumerator SendVoiceRequest(WWW www, Action <byte[]> callback)
	{
		string currentVoice = selectedVoiceString;
		string currentFormat = selectedAudioFormatExtension;
		yield return www;

		if (www.error == null && www.isDone) 
		{
			byte[] data=www.bytes;

			byte[] dataNewHeader = stripAndAddWavHeader(data);

			WWUtils.Audio.WAV wav = new WWUtils.Audio.WAV(dataNewHeader);
			AudioClip audioClip = AudioClip.Create("testSound", wav.SampleCount, 1,wav.Frequency, false, false);
			audioClip.SetData(wav.LeftChannel, 0);
			//audio.clip = audioClip;
			//audio.Play();


			Debug.Log("Reading Wav now: " + wav.ToString());
			System.IO.File.WriteAllBytes("tempWavTesting_"+currentVoice+"."+currentFormat,data);

//
			//byte[] dataNewHeader = stripAndAddWavHeader(data);
			System.IO.File.WriteAllBytes("tempWavTesting_CHANGED_"+currentVoice+"."+currentFormat,dataNewHeader);

			//convertAudio(data);
			//www.bytes
			AudioSource attachedAudioSource = this.transform.GetComponent<AudioSource>();
			//attachedAudioSource.clip = audioClip;
			attachedAudioSource.PlayOneShot(audioClip);
			attachedAudioSource.Play();
			//callback(data);
			//Debug.Log ("FINISHED: " + www.url);
		} 
		else 
		{
			Debug.Log ("Error: " + www.error + " - url:" + www.url);
		}
	}

	private byte[] stripAndAddWavHeader(byte[] wav) {
		
		int headerSize = 44;
		int metadataSize = 48;

		if(sampleRate == 0 && wav.Length > 28){
			//Get WAV Sample rate from 24
			sampleRate = WWUtils.Audio.WAV.bytesToInt(wav,24);
		}
			
		//byte[]	wavNoheader;
		//NSData *wavNoheader= [NSMutableData dataWithData:[wav subdataWithRange:NSMakeRange(headerSize+metadataSize, [wav length])]];

		byte[] wavNoheader = new byte[wav.Length - (headerSize+metadataSize)];
		System.Buffer.BlockCopy(wav,headerSize+metadataSize, wavNoheader, 0, wav.Length - (headerSize+metadataSize));

		//NSMutableData *newWavData;
		//newWavData = [self addWavHeader:wavNoheader];
		byte[] newWavData = addWavHeader(wavNoheader);

		return newWavData;
	}

	byte[]	addWavHeader(byte[] wavNoheader) {

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

	private void convertAudio(byte[] data)
	{
		//First convert byte[] to float []
		int rescaleFactor = 32767;
		int dataLength = data.Length - 44;
		float[] floatData = new float[dataLength / 2];
		float[] newFloatData = new float[(floatData.Length / 3)];
		int a = 0;
		for(int i = 0; i<dataLength;i+=2)
		{
			byte[] aShort=new byte[]{data[i+44],data[i+45]};
			floatData[a]=(float)BitConverter.ToInt16(data,i);
			a++;
		}
		//Decimation from 48kHz to 16kHz decimation factor of 3 order of 1
		
		float b0 = 0.0584f;
		float b1 = 0.8831f;
		float b2 = 0.0584f;
		//do float[0] seperatly
		floatData [0] = b0 * data [44];
		floatData [1] = b0 * data [45] + b1 * data[44]; 
		for (int i = 2; i<floatData.Length; i++) 
		{
			//floatData[i]= b0*data[i+44] + b1*data[i+43]; first order
			//y[n]=b0x[n]+b1x[n-1] for the float[]
			//floatData[i] = b0 * data [i+44] + b1 * data [i+43] + b2 * data [i+42];
		}
		//Keep a number then remove 2
		a = 0;
		for (int i=0; i<newFloatData.Length; i+=3) 
		{
			newFloatData[a]=floatData[i];
			a++;
		}
		
		Int16[] intData = new Int16[newFloatData.Length];
		Byte[] bytesData = new byte[(newFloatData.Length * 2)+44];
		
		string hexHeader = "5249464674ee000057415645666d74201000000001000100803e0000007d00000200100064617461e6ed0000";
		
		//create header
		for (int i = 0; i<44; i++) 
		{
			bytesData[i]=byte.Parse(hexHeader.Substring(i*2,2),System.Globalization.NumberStyles.HexNumber);
		}
		
		//reconvert to a byte[]
		for (int i = 0; i < newFloatData.Length; i++) 
		{
			intData[i] = (short)(newFloatData[i]*rescaleFactor);
			Byte[] byteArr = new byte[2];
			byteArr = BitConverter.GetBytes(intData[i]);
			byteArr.CopyTo(bytesData,i*2+44);
		}
		Debug.Log ("finished");
		//save as a new file
		System.IO.File.WriteAllBytes ("testingTTS.wav", bytesData);
	}
}

/*

- (NSMutableData *)addWavHeader:(NSData *)wavNoheader {
    
    int headerSize = 44;
    long totalAudioLen = [wavNoheader length];
    long totalDataLen = [wavNoheader length] + headerSize-8;
    long longSampleRate = (self.sampleRate == 0 ? 48000 : self.sampleRate);
    int channels = 1;
    long byteRate = 16 * 11025 * channels/8;
    
    
    
    Byte *header = (Byte*)malloc(44);
    header[0] = 'R';  // RIFF/WAVE header
    header[1] = 'I';
    header[2] = 'F';
    header[3] = 'F';
    header[4] = (Byte) (totalDataLen & 0xff);
    header[5] = (Byte) ((totalDataLen >> 8) & 0xff);
    header[6] = (Byte) ((totalDataLen >> 16) & 0xff);
    header[7] = (Byte) ((totalDataLen >> 24) & 0xff);
    header[8] = 'W';
    header[9] = 'A';
    header[10] = 'V';
    header[11] = 'E';
    header[12] = 'f';  // 'fmt ' chunk
    header[13] = 'm';
    header[14] = 't';
    header[15] = ' ';
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
    header[36] = 'd';
    header[37] = 'a';
    header[38] = 't';
    header[39] = 'a';
    header[40] = (Byte) (totalAudioLen & 0xff);
    header[41] = (Byte) ((totalAudioLen >> 8) & 0xff);
    header[42] = (Byte) ((totalAudioLen >> 16) & 0xff);
    header[43] = (Byte) ((totalAudioLen >> 24) & 0xff);
    
    NSMutableData *newWavData = [NSMutableData dataWithBytes:header length:44];
    [newWavData appendBytes:[wavNoheader bytes] length:[wavNoheader length]];
    return newWavData;
}


-(NSData*) stripAndAddWavHeader:(NSData*) wav {
	
	int headerSize = 44;
	int metadataSize = 48;
	
	if(sampleRate == 0 && [wav length] > 28)
		[wav getBytes:&sampleRate range: NSMakeRange(24, 4)]; // Read wav sample rate from 24
	
	NSData *wavNoheader= [NSMutableData dataWithData:[wav subdataWithRange:NSMakeRange(headerSize+metadataSize, [wav length])]];
	
	NSMutableData *newWavData;
	newWavData = [self addWavHeader:wavNoheader];
	
	return newWavData;
	
	
	
}
*/
