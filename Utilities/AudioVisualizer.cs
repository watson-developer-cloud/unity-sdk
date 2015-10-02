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
using System.Collections.Generic;
using UnityEngine.UI;

using IBM.Watson.Utilities;
using IBM.Watson.Logging;

namespace IBM.Watson.Audio{

	public enum VisualizerType 
	{
		LineGraph,
		Bar,
		NaN
	}
	//[ExecuteInEditMode]
	[RequireComponent (typeof (RectTransform))]
	[RequireComponent (typeof (CanvasRenderer))]
	[RequireComponent (typeof (Image))]
	[RequireComponent (typeof (LineRenderer))]
	public class AudioVisualizer : MonoBehaviour {

		#region Public Properties
		public static AudioVisualizer Instance { get { return Singleton<AudioVisualizer>.Instance; } }

		private RectTransform m_RectTransform;
		public RectTransform RectTransform{
			get{
				if(m_RectTransform == null){
					m_RectTransform = this.GetComponent<RectTransform>();
				}
				return m_RectTransform;
			}
		}

		private Image m_Image;
		public Image Image{
			get{
				if(m_Image == null){
					m_Image = this.GetComponent<Image>();
				}
				return m_Image;
			}
		}

		private int m_NumberOfPoint = 1024; //This represents the smoothness of wave shape
		public int NumberOfPoint{
			get{
				return m_NumberOfPoint;
			}
			set{
				if(value > 0){
					m_NumberOfPoint = value;
					LineRenderer.SetVertexCount(value);
				}
				else{
					Log.Error("AudioVisualizer", "Number of points should be greater than zero");
				}
						
			}
		}

		private LineRenderer m_LineRenderer = null;
		public LineRenderer LineRenderer{
			get{
				if(m_LineRenderer == null){
					m_LineRenderer = this.GetComponent<LineRenderer>();
					m_LineRenderer.SetVertexCount(NumberOfPoint);
					m_LineRenderer.SetWidth(0.01f, 0.01f);
				}
				return m_LineRenderer;
			}
		}

		public bool isActive = false;
		public bool isDirty = false;	//Change values, 


		#endregion

	void Start(){
			this.gameObject.name = "AudioVisualizer";	//Changing the name of the object attached!

			Canvas[] canvasListOnScene = GameObject.FindObjectsOfType<Canvas> ();
			GameObject canvasObject = null;
			if (canvasListOnScene != null) {
				for (int i = 0; i < canvasListOnScene.Length; i++) {
					if(canvasListOnScene[i].isRootCanvas && canvasListOnScene[i].renderMode == RenderMode.WorldSpace){
						if(canvasObject != null){
							Log.Debug("AudioVisualizer", "There are more than one Canvas in Root & ScreenSpaceOverlay");
						}
						canvasObject = canvasListOnScene[i].gameObject;
					}
				}
			} 

			if(canvasObject == null){
				//Create Canvas as screen overlay
				canvasObject = new GameObject ();
				canvasObject.name = "Canvas";
				canvasObject.layer = LayerMask.NameToLayer("UI");
				RectTransform rectTransform = canvasObject.AddComponent<RectTransform>();
				Canvas newCanvas = canvasObject.AddComponent<Canvas>();
				newCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
				CanvasScaler newCanvasScalar = canvasObject.AddComponent<CanvasScaler>();
				GraphicRaycaster graphicRaycaster = canvasObject.AddComponent<GraphicRaycaster>();

//				RectTransform.anchorMin = new Vector2 (0.5f, 0.5f);
//				RectTransform.anchorMax = new Vector2 (0.5f, 0.5f);
//				RectTransform.pivot = new Vector2 (0.5f, 0.5f);
//				
//				RectTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, Screen.width * 0.25f);
//				RectTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, Screen.height * 0.125f);
//				RectTransform.anchoredPosition3D = Vector3.zero;
			}

			if(this.transform.parent != canvasObject.transform)
				this.transform.parent = canvasObject.transform;



			Image.color = new Color (1.0f, 1.0f, 1.0f, 0.3f);

			//Audio Source from microphone in 1 sec
			if(audioSource == null){
				audioSource = GetComponent<AudioSource>();
				if(audioSource == null){
					audioSource = this.gameObject.AddComponent<AudioSource>();
				}
			}
			
			audioSource.clip = Microphone.Start("Built-in Microphone", true, 1, 44100);
			audioSource.Play();
			audioSource.loop = true;

			isActive = true;

			startingPointIndex = NumberOfPoint / 2;
			sampleData = new float[audioSource.clip.samples * audioSource.clip.channels];
			audioWaveData = new float[ (int) ( sampleData.Length / (float)NumberOfPoint ) ];
			linePoints = new Vector3[ NumberOfPoint ];

			Vector3[] fourCornerPoints = new Vector3[4];
			//RectTransform.
			RectTransform.GetWorldCorners (fourCornerPoints);
			graphicsBottomLeftOnScene = fourCornerPoints [0];
			graphicsTopRightOnScene = fourCornerPoints [2];
			graphicsCenterHeight = (graphicsTopRightOnScene.y - graphicsBottomLeftOnScene.y) / 2.0f;
			distanceBetweenPoint = (graphicsTopRightOnScene.x - graphicsBottomLeftOnScene.x) / graphicResolution;

			angleWaving = new float[NumberOfPoint];
			for (int i = 0; i < NumberOfPoint; i++) {
				angleWaving[i] = float.MinValue;
			}
			//linePointsInTime = new Vector3[totalNumberOfFrameForAnimation, graphicResolution];
	}


//#else
		// Update is called once per frame
		AudioSource audioSource = null; 
		float[] sampleData;
		float[] audioWaveData;
		Vector3[] linePoints;
		private float modifier = 100.0f;
		private int startingPointIndex = 0;


		private int audioPlayedDataIndex = -1;
		private void UpdateAudioData(){
			int current = (int)(audioSource.timeSamples / (float)NumberOfPoint);

			if (audioPlayedDataIndex < current) {
				//Refresh the Audio Clip Data
				audioSource.clip.GetData (sampleData, 0);
				for (int i = 0; i < audioWaveData.Length; i++) {
					audioWaveData[i] = 0;
					for (int j = 0; j < NumberOfPoint; j++) {
						audioWaveData[i] += Mathf.Abs(sampleData[(i * NumberOfPoint) + j]);
					}
					audioWaveData[i] = audioWaveData[i] / NumberOfPoint;
				}
			} else {
				//It is playing the current audioWaveData - no need to refresh, changing pointer location to mark
				audioPlayedDataIndex = current;
			}


		}

//		private Vector3 EaseOutQuad (float currentTime, Vector3 startValue, float changeInValue, float duration) {
//			currentTime /= duration;
//			return -changeInValue * currentTime*(currentTime-2) + startValue;
//		}

		public float changeInValue = 0.1f;
		public float duration = 1.0f;


		//All new Values
		//Graphic is laying in Range [0.0, 1.0] as normalized width depends on panel size.
		private float graphicStartPoint = 0.0f;	//between [0.0, graphicEndPoint)
		private float graphicEndPoint = 1.0f;	//between (graphicStartPoint, 1.0]
		private float waveCrestPoint =  0.5f;	//between [graphicStartPoint, graphicEndPoint] , the peak point of the wave

		private int graphicResolution = 1024;

		Vector3[,] linePointsInTime;

		private float soundWaveValue = 0.0f;
		private float soundWaveRawValue = 0.0f;
		private float frequency = 5.0f; 				//How many wave crest will be depends on it. If it is one, one big wave crest greater than zero and two small values lower than 0 will be visible. 
		private float waveRawModifier = 10.0f;		
		private float waveCrestModifier = 2.0f;		
		private float nextWaveCrestModifier = 0.5f;	// between [0.0f, 1.0f]
		private float distanceBetweenPoint = 0.01f; //It is defined by panel width on actual screen space.
		private float graphicsCenterHeight = 0.0f;
		private Vector3 graphicsBottomLeftOnScene;
		private Vector3 graphicsTopRightOnScene;

		public float timeForWavingMin	= 2.0f;	// time for waving at min data (data will be come as 0.0 in this case as minimum)
		public float timeForWavingMax = 0.2f; //For each timeForWaving, the graph will be osilated. 
		private float timeForWaving = 0.0f;
		private float[] angleWaving = null;


		public bool isDebugging = false;
		public int frameDebug = 200;
		private float timeForOnePeriod = 0.1f;

		float phase = 0.0f;

		void Update () {

			//int currentFrame = (Time.frameCount % totalNumberOfFrameForAnimation);
			//int previousFrame = ((currentFrame - 1) < 0) ? (totalNumberOfFrameForAnimation - 1) : (currentFrame - 1);
			UpdateAudioData ();

			phase += 0.5f;

			if (isActive) {

				int current = (int)(audioSource.timeSamples / (float)NumberOfPoint);

				if(current < audioWaveData.Length){
					soundWaveRawValue = Mathf.Clamp(0.1f, 1.0f, audioWaveData[current]);
					//soundWaveValue = 1.0f; //Mathf.PingPong(Time.time, 1.0f);


					Debug.Log("soundWaveValue: " + soundWaveValue + " : timeForWaving: " + (timeForWaving * Mathf.Rad2Deg));
				}
				else{
					current = audioWaveData.Length - 1;
					//Log.Error("AudioVisualizer", " Current index value: " + current + " is greater than sound wave length, which is " + wavData.Length);
				}

				//Calculating the time needed for waving
				float currentTimeNeedForWave = Mathf.Lerp(timeForWavingMin, timeForWavingMax, soundWaveValue);
				//timeForWaving = Mathf.Repeat(Time.time, currentTimeNeedForWave) * (1.0f / currentTimeNeedForWave) * (2.0f * Mathf.PI);
				

				soundWaveValue = Mathf.Lerp(soundWaveValue, soundWaveRawValue, 0.1f);


				//float timeAddition = (Time.time * timeForOnePeriod)



				int graphicStartPointIndex = (int) Mathf.Lerp(0.0f, (float)graphicResolution, graphicStartPoint);
				int graphicEndPointIndex = (int) Mathf.Lerp(0.0f, (float)graphicResolution, graphicEndPoint);
				int waveCrestPointIndex = (int) Mathf.Lerp(0.0f, (float)graphicResolution, waveCrestPoint);
				float normalizedDistanceForEachHalfPeriod = (graphicEndPoint - graphicStartPoint) / frequency;
				//int waveCrestPointIndex = Mathf.Lerp(0.0f, (float)graphicResolution, (graphicEndPoint - graphicStartPoint)

				float incrementForPoint = audioWaveData.Length / (float)(graphicEndPointIndex - graphicStartPointIndex);

				//Creating Actual Graph
				for(int i = graphicStartPointIndex; i < graphicEndPointIndex; i++) { 

					if(frameDebug == i && isDebugging)
						Debug.Log("Testing");
					float normalizedDistanceFromWaveCrest = Mathf.Abs(i - waveCrestPointIndex) / (float)(graphicEndPointIndex - graphicStartPointIndex);

					float waveIndexFloat = (int)Mathf.Lerp(current, current - audioWaveData.Length - 0.01f, normalizedDistanceFromWaveCrest);
					if(waveIndexFloat < 0)
						waveIndexFloat += audioWaveData.Length;

					int waveIndexBase = (int) waveIndexFloat;
					int waveIndexNext = ((waveIndexBase + 1) >= audioWaveData.Length)?0:(waveIndexBase + 1);
					float waveDataValueAtPoint = Mathf.Lerp(audioWaveData[waveIndexBase], audioWaveData[waveIndexNext], waveIndexFloat - waveIndexBase);

					float normalizedDistanceFromWaveCrestRaw = (normalizedDistanceFromWaveCrest > 0.0f)? (1.0f / normalizedDistanceFromWaveCrest) : (0.0f);

					int numberOfWaveFromWaveCrest = (int)(frequency * normalizedDistanceFromWaveCrest);

					float nextWaveCrestModifierStartValue = Mathf.Pow(nextWaveCrestModifier, numberOfWaveFromWaveCrest);
					float nextWaveCrestModifierEndValue = Mathf.Pow(nextWaveCrestModifier, numberOfWaveFromWaveCrest + 1);

					while(normalizedDistanceFromWaveCrest > normalizedDistanceForEachHalfPeriod && (normalizedDistanceFromWaveCrest - normalizedDistanceForEachHalfPeriod) > 0.0f ){
						normalizedDistanceFromWaveCrest -= normalizedDistanceForEachHalfPeriod;
					}
					normalizedDistanceFromWaveCrest = normalizedDistanceFromWaveCrest / normalizedDistanceForEachHalfPeriod;



					float nextWaveCrestModifierValue = Mathf.Lerp(nextWaveCrestModifierStartValue, nextWaveCrestModifierEndValue, normalizedDistanceFromWaveCrest);

					if(angleWaving[i] == float.MinValue){	//First value 
						angleWaving[i] = Mathf.Lerp( 0.0f, 2.0f * Mathf.PI, normalizedDistanceFromWaveCrest); //initial angle for all wave points
					}
					else{
						angleWaving[i] -= (1.0f/currentTimeNeedForWave) * Time.deltaTime;
//						if(angleWaving[i] * Mathf.Rad2Deg < -360){
//							angleWaving[i] += (2.0f * Mathf.PI);
//						}
					}

					//APPLE SIRI CODE 
					float waveValueInGraph = Mathf.Cos(angleWaving[i]) * soundWaveValue * waveCrestModifier * nextWaveCrestModifierValue ; //

					float mid = (graphicEndPointIndex - graphicStartPointIndex) / 2.0f;
					float scaling = - Mathf.Pow(1.0f / mid * (i - mid), 2) + 1;
					float maxAmplitude = 1.0f;

					float y = scaling * maxAmplitude * waveValueInGraph * Mathf.Sin(2 * Mathf.PI *(i) * frequency + phase);

					//waveValueInGraph = Mathf.Clamp(waveValueInGraph, -1.0f, 1.0f);

					//linePoints[i] = Vector3.Lerp(linePoints[i],  graphicsBottomLeftOnScene + (waveValueInGraph * Vector3.up) + (graphicsCenterHeight * Vector3.up) + (Vector3.right * (distanceBetweenPoint * i)), 0.1f);

					//APPLE SIRI :) 
					linePoints[i] = graphicsBottomLeftOnScene + (/*waveValueInGraph*/ y * Vector3.up) + (graphicsCenterHeight * Vector3.up) + (Vector3.right * (distanceBetweenPoint * i));

					//Dogukan Visualization
					//linePoints[i] = graphicsBottomLeftOnScene + (waveValueInGraph * Vector3.up) + (graphicsCenterHeight * Vector3.up) + (Vector3.right * (distanceBetweenPoint * i));


					//if(waveIndex < wavData.Length)
					//linePoints[i] = graphicsBottomLeftOnScene + (waveDataValueAtPoint * Vector3.up) + (graphicsCenterHeight * Vector3.up) + (Vector3.right * (distanceBetweenPoint * i)); //Vector3.Lerp(linePoints[i],  graphicsBottomLeftOnScene + (wavData[waveIndex] * Vector3.up) + (graphicsCenterHeight * Vector3.up) + (Vector3.right * (distanceBetweenPoint * i)), 0.3f);
					//else
					//	Log.Error("AudioVisualizer", " waveIndex index value: " + waveIndex + " is greater than sound wave length, which is " + wavData.Length);
					//linePoints[i] = Vector3.Lerp(linePoints[i],  graphicsBottomLeftOnScene + (wavData[(int)(i * incrementForPoint)] * Vector3.up) + (graphicsCenterHeight * Vector3.up) + (Vector3.right * (distanceBetweenPoint * i)), 0.3f);


					if(i < waveCrestPointIndex){

					}
					else if(i > waveCrestPointIndex){

					}
					else{ //The Wave Crest Point Value

					}
					//Mathf.Cos( Mathf.PingPong( i / 
				}

				for (int indexOfPoint = 0; indexOfPoint < NumberOfPoint; indexOfPoint++) {
					LineRenderer.SetPosition(indexOfPoint, linePoints[indexOfPoint] );
				}

				//Filling out the Remaning Parts from the Grapth

				//Filling the start part
				for(int i = graphicStartPointIndex - 1; i >= 0; i--) { 

				}

				//Filling the end part
				for(int i = graphicEndPointIndex; i < graphicResolution; i++) { 

				}



			} else {

			}


		}


	}

}

