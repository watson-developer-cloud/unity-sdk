using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LeanAudioGenerated : MonoBehaviour {

	public bool disableEditor = false;

	public float clipLength = 1.0f;

	public string frequencyType = "Freeform";
	public Vector2 freqFrom = new Vector2(0f,0.003f);
	public Vector2 freqTo = new Vector2(1f,0.003f);
	public string volumeType = "Freeform";
	public Vector2 volFrom = new Vector2(0f,1f);
	public Vector2 volTo = new Vector2(1f,0f);


	public AnimationCurve volumeCurve = AnimationCurve.Linear(0f,1f,1f,0f);
	public AnimationCurve frequencyCurve = AnimationCurve.Linear(0f,0.001f,1f,0.001f);

	public bool useVibrato = true;
	public Vector3[] vibrato = new Vector3[]{ new Vector3(0.33f, 0f, 0f) };

	public string[] vibratorXStr = new string[]{"0.1","0.1","0.1","0.1","0.1","0.1"};
	public string[] vibratorYStr = new string[]{"0","0","0","0","0","0"};
	public string selectedTime = "";
	public string selectedValue = "";
	public string selectedInTangent = "";
	public string selectedOutTangent = "";
	public string frequencyRateStr = "11025";
	public string clipLengthStr = "1";

	public int frequencyRate = 11025;

	public bool compareToSourceAudio = false;

	public AudioClip compareAudio;
	public AnimationCurve compareAudioCurve;

	public float graphZoom = 1.1f;
	public bool zoomHalfCurve = true;

	public int fileSectionIndex = -1;
	public int presetIndex = 0;
	public string presetName = "";

	public float playDt = 0f;
	public int playOnCount = -1;

	// Use this for initialization
	void Start () {
	
	}

	public void setLinearVectors(AnimationCurve curve, ref Vector2 from, ref Vector2 to){
		if(curve.length>=2){
			from = new Vector2(curve[0].time,curve[0].value);
			to = new Vector2(curve[1].time,curve[1].value);
		}
	}

	public void deserialize( string str ){
		string[] split = str.Split(":"[0]);
		
		if(split[1]=="fvb"){
			split = split[2].Split("~"[0]);
			// Debug.Log("frequency curve str:"+split[0]);
			frequencyCurve = curveFromString( split[0] );
			setLinearVectors(frequencyCurve, ref freqFrom, ref freqTo);
			volumeCurve = curveFromString( split[1] );
			setLinearVectors(volumeCurve, ref volFrom, ref volTo);

			vibrato = vec3ArrayFromString( split[2] );
			for(int i = 0; i < vibrato.Length; i++){
				vibratorXStr[i] = ""+vibrato[i].x;
				vibratorYStr[i] = ""+vibrato[i].y;
			}
			useVibrato = vibrato.Length>=0;

			frequencyRateStr = split[3];
			frequencyRate = (int)float.Parse(frequencyRateStr);

			clipLength = volumeCurve[ volumeCurve.length - 1].time;
			clipLengthStr = ""+clipLength;
		}
	}

	public string serialize(){
		System.Text.StringBuilder strBuild = new System.Text.StringBuilder(512);
		strBuild.Append("a:fvb:"); // Audio: frequency, volume, vibrato, rate
		curve(strBuild, frequencyCurve);
		strBuild.Append("~");
		curve(strBuild, volumeCurve);
		strBuild.Append("~");
		vec3Array(strBuild, vibrato);
		strBuild.Append("~"+frequencyRate);

		return strBuild.ToString();
	}

	public AnimationCurve curveFromString( string str ){
		string[] split = str.Split(","[0]);

		int lengthMinusEnds = split.Length - 2;
		Keyframe[] frames = new Keyframe[lengthMinusEnds/4];
		int k = 0;
		for(int i = 1; i<split.Length-4; i+=4){
			frames[k] = new Keyframe( float.Parse(split[i]), float.Parse(split[i+1]), float.Parse(split[i+2]), float.Parse(split[i+3]) );
			// Debug.Log("i:"+i+" new keyframe:"+frames[k]);
			k++;
		}

		AnimationCurve curve =  new AnimationCurve( frames );
		curve.preWrapMode = (UnityEngine.WrapMode)(int)float.Parse(split[0]);
		curve.postWrapMode = (UnityEngine.WrapMode)(int)float.Parse(split[split.Length-1]);
		return curve;
	}

	public void curve( System.Text.StringBuilder strBuild, AnimationCurve curve ){
		strBuild.Append(""+(int)curve.preWrapMode+",");
		for(int i = 0; i < curve.length; i++){
			Keyframe key = curve[i];
			strBuild.Append(""+key.time+","+key.value+","+key.inTangent+","+key.outTangent+",");
		}
		strBuild.Append(""+(int)curve.postWrapMode);
	}

	public void vec3Array( System.Text.StringBuilder strBuild, Vector3[] arr ){
		for(int i = 0; i < arr.Length; i++){
			Vector3 vec3 = arr[i];
			strBuild.Append(""+vec3.x+","+vec3.y+","+vec3.z+",");
		}
	}

	public Vector3[] vec3ArrayFromString( string str ){
		string[] split = str.Split(","[0]);
		Vector3[] vec3s = new Vector3[split.Length/3];
		int k = 0;
		for(int i = 0; i<split.Length-2; i+=3){
			vec3s[k] = new Vector3( float.Parse(split[i]), float.Parse(split[i+1]), float.Parse(split[i+2]) );
			k++;
		}

		return vec3s;
	}
	
}
