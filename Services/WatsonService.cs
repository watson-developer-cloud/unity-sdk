using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

interface IWatsonService
{
	string serviceURL {get; set;}
	string serviceCredentialUserName {get; set;}
	string serviceCredentialPassword {get; set;}
	string serviceVersion {get; set;}

}

public class WatsonService : MonoBehaviour, IWatsonService{

	// codecs
	public const int WATSONSDK_TTS_AUDIO_CODEC_TYPE_OPUS_SAMPLE_RATE = 48000;
	public const int WATSONSDK_TTS_AUDIO_CODEC_TYPE_WAV_SAMPLE_RATE = 0;   	// zero means decoder detected sample rate

	protected string _serviceURL;
	protected string _serviceCredentialUserName;
	protected string _serviceCredentialPassword;
	protected string _serviceVersion;

	#region Service URL , UserName, Password 
	
	public virtual string serviceURL{
		get{
			if(String.IsNullOrEmpty(_serviceURL)){
				Debug.LogError("Error - serviceURL is null");
			}
			return _serviceURL;
		}
		set{
			_serviceURL = value;
		}
	}
	
	public virtual string serviceCredentialUserName{
		get{
			if(String.IsNullOrEmpty(_serviceCredentialUserName)){
				Debug.LogError("Error - serviceCredentialUserName is null");
			}
			return _serviceCredentialUserName;
		}
		set{
			_serviceCredentialUserName = value;
		}
	}
	
	public virtual string serviceCredentialPassword{
		get{
			if(String.IsNullOrEmpty(_serviceCredentialPassword)){
				Debug.LogError("Error - serviceCredentialPassword is null");
			}
			return _serviceCredentialPassword;
		}
		set{
			_serviceCredentialPassword = value;
		}
	}
	
	public virtual string serviceVersion{
		get{
			if(String.IsNullOrEmpty(_serviceVersion)){
				Debug.LogError("Error - serviceVersion is null");
			}
			return _serviceVersion;
		}
		set{
			_serviceVersion = value;
		}
	}
	
	#endregion

	public string getURLCombined(params string[] urlParts){
		return string.Join("/", urlParts);
	}

	public string getGETRequestUrl(string baseUrl, params string[] parameters){
		return string.Concat(baseUrl, "?", string.Join("&", parameters));
	}

	public string getServiceFunctionURL(string functionName){
		return getURLCombined(serviceURL, serviceVersion, functionName);
	}
}