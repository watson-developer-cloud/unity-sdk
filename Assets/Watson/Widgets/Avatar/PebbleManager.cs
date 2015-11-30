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
* @author Dogukan Erenel (derenel@us.ibm.com)
*/

using UnityEngine;
using UnityEngine.Serialization;
using IBM.Watson.Logging;
using IBM.Watson.Utilities;
using System;
 
namespace IBM.Watson.Widgets.Avatar
{
    /// <exclude />
    [Serializable]
    public class PebbleRow
    {
        public GameObject[] pebbleList;
    }

    /// <summary>
    /// The class manages the pebble audio visualization on the avatar.
    /// </summary>
    public class PebbleManager : MonoBehaviour
    {

		#region Private Members

		[SerializeField,FormerlySerializedAs("pebbleRowList")]
		private PebbleRow[] m_PebbleRowList = null;

		private bool m_SetDataOnFrame = false;
		private bool m_IsWatsonIsTalking = false;
		private float m_LatestValueReceived = 0.0f;

		[SerializeField]
		private float m_SpeedFadingOut = 4.0f;
        [SerializeField]
        private float m_SpeedAudioLevel = 50.0f;
        [SerializeField]
		private float m_SmoothnessPebbleMovementInTheFirstRow = 1.0f;
		[SerializeField]
		private float m_SmoothnessForBottom = 0.95f;
		[SerializeField]
		private float m_SmoothnessForMid = 0.2f;
		[SerializeField]
		private float m_SmoothnessForPeak = 0.99f;
		[SerializeField]
		private int m_NumberOfWaves = 3;
       

        [SerializeField,FormerlySerializedAs("smoothnessLimitBetweenRows")]
		private Vector2 m_SmoothnessLimitBetweenRows; //Smothness MIN, MAX
	
		#endregion

		#region Public Members
        public PebbleRow [] PebbleRowList { get { return m_PebbleRowList; } set { m_PebbleRowList = value; } }
		#endregion
       
		#region OnEnable / OnDisable For Event Registration
		
		void OnEnable(){
			EventManager.Instance.RegisterEventReceiver(Constants.Event.ON_AVATAR_SPEAKING, AvatarSpeaking);
			EventManager.Instance.RegisterEventReceiver(Constants.Event.ON_USER_SPEAKING, UserSpeaking);
		}
		
		void OnDisable(){
			EventManager.Instance.UnregisterEventReceiver(Constants.Event.ON_AVATAR_SPEAKING, AvatarSpeaking);
			EventManager.Instance.UnregisterEventReceiver(Constants.Event.ON_USER_SPEAKING, UserSpeaking);
		}
		
		#endregion
       
		#region Awake / Start / Update
		void Awake(){
			if (m_PebbleRowList == null) {
				//TODO: setup all pebbles!
			}
		}

		private void Start()
		{
			if (m_PebbleRowList == null)
			{
				Log.Error("PebbleManager", "pebbleRowList is null! This shouldn't be!");
				return;
			}
		}

        void Update()
        {
            if (m_SetDataOnFrame)
            {   //skip value lowering
                m_SetDataOnFrame = false;
            }
            else
            {
				m_LatestValueReceived = Mathf.Lerp(m_LatestValueReceived, 0.0f, Time.deltaTime * m_SpeedFadingOut);
				SetAudioData(m_LatestValueReceived, isWatsonTalking: m_IsWatsonIsTalking, setDataOnFrame: false);
            }
        }
        #endregion

        private AvatarWidget m_AvatarWidgetAttached = null;
        public AvatarWidget AvatarAttached
        {
            get
            {
                if(m_AvatarWidgetAttached == null)
                    m_AvatarWidgetAttached = this.transform.GetComponentInParent<AvatarWidget>();
                return m_AvatarWidgetAttached;
            }
        }

        public void AvatarSpeaking(System.Object[] args)
        {
            if (AvatarAttached != null && AvatarAttached.State == AvatarWidget.AvatarState.ANSWERING)
            {
                if (args != null && args.Length == 1 && args[0] is float)
                {
                    float audioLevelOutput = (float)args[0];
                    SetAudioData(audioLevelOutput, isWatsonTalking: true);
                }
            }
        }

        public void UserSpeaking(System.Object[] args)
        {
            if (AvatarAttached != null && AvatarAttached.State != AvatarWidget.AvatarState.ANSWERING)
            {
                if (args != null && args.Length == 1 && args[0] is float)
                {
                    float audioLevelOutput = (float)args[0];
                    SetAudioData(audioLevelOutput, isWatsonTalking: false);
                }
            }
        }

        /// <summary>
        /// Sets the audio data as Audio Data in delta time
        /// </summary>
        /// <param name="audioLevelData">Audio Level value.</param>
        /// <param name="isWatsonTalking">If set to <c>true</c> is watson talking.</param>
        /// <param name="setDataOnFrame">If set to <c>true</c> set data on frame.</param>
        public void SetAudioData(float audioLevelData, bool isWatsonTalking = false, bool setDataOnFrame = true)
        {
            this.m_SetDataOnFrame = setDataOnFrame;
			this.m_IsWatsonIsTalking = isWatsonTalking;

            if (audioLevelData == float.NaN)
            {
                Log.Error("PebbleManager", "Value for SetAudioData is NAN");
                audioLevelData = 0.0f;
            }
            m_LatestValueReceived = audioLevelData;


			int numberOfPebbleInOneWave = m_PebbleRowList[0].pebbleList.Length / m_NumberOfWaves; //120
			int numberOfPebbleInHalfWave = (int)(numberOfPebbleInOneWave / 2.0f);   //60


            for (int i = m_PebbleRowList.Length - 1; i >= 0; i--)
            {
                float smoothnessBetweenRows = Mathf.Lerp(m_SmoothnessLimitBetweenRows.y, m_SmoothnessLimitBetweenRows.x, (float)i / m_PebbleRowList.Length);

                if (m_PebbleRowList[i].pebbleList != null)
                {
                    for (int j = 0; j < m_PebbleRowList[i].pebbleList.Length; j++)
                    {

                        if (m_PebbleRowList[i].pebbleList[j] != null)
                        {
                            if (i > 0)
                            {
                                m_PebbleRowList[i].pebbleList[j].transform.localPosition = Vector3.Lerp(
                                    m_PebbleRowList[i].pebbleList[j].transform.localPosition,
                                    new Vector3(
                                        m_PebbleRowList[i].pebbleList[j].transform.localPosition.x, 
                                        m_PebbleRowList[i - 1].pebbleList[j].transform.localPosition.y, 
                                        m_PebbleRowList[i].pebbleList[j].transform.localPosition.z),
                                    smoothnessBetweenRows);

								if(isWatsonTalking){
									m_PebbleRowList[i].pebbleList[j].transform.localPosition = new Vector3(
									m_PebbleRowList[i].pebbleList[j].transform.localPosition.x,
									Mathf.Clamp(m_PebbleRowList[i].pebbleList[j].transform.localPosition.y, 0.0f, m_PebbleRowList[i - 1].pebbleList[j].transform.localPosition.y) ,
									m_PebbleRowList[i].pebbleList[j].transform.localPosition.z);

								}
                            }
                            else
                            {
								if(isWatsonTalking && m_NumberOfWaves > 0){

									int waveIndex = (j / numberOfPebbleInOneWave);
									int pebbleIndexInWaveForWaveAnimation = ( (j + numberOfPebbleInHalfWave)  % numberOfPebbleInOneWave);
									
									// it works like 60,61,62, ....., 119,59,58,.....0,180,181,...,239,179,178,....120,300,301,....359,299,298,....240
									if(pebbleIndexInWaveForWaveAnimation < numberOfPebbleInHalfWave){ //lower end part
										pebbleIndexInWaveForWaveAnimation = (numberOfPebbleInHalfWave - pebbleIndexInWaveForWaveAnimation) - 1;
									}
									
									int pebbleIndex = pebbleIndexInWaveForWaveAnimation + (waveIndex * numberOfPebbleInOneWave);
									int pebbleIndexInWave = (pebbleIndex  % numberOfPebbleInOneWave);

									float distanceFromCenterNormalized = Mathf.Abs( ((pebbleIndexInWave == numberOfPebbleInOneWave -1)?pebbleIndexInWave-1:pebbleIndexInWave) + 1 - numberOfPebbleInHalfWave) / (float)numberOfPebbleInHalfWave; //center is 0, boundaries are 1
									float distanceFromBoundariesNormalized = (1.0f - distanceFromCenterNormalized); //center is 1, boundaries are 0

									float valueToSet = audioLevelData;

									//float speedByPebbleLocation = minValue + (maxValue - minValue) * distanceFromCenterNormalized;
									float speedByPebbleLocation = 0.0f;
									if(distanceFromBoundariesNormalized < 0.5f){
										speedByPebbleLocation = LeanTween.easeInOutSine( Mathf.Max(m_SmoothnessForBottom, m_SmoothnessForMid) , Mathf.Min(m_SmoothnessForBottom, m_SmoothnessForMid), distanceFromBoundariesNormalized);
									}
									else{
										speedByPebbleLocation = LeanTween.easeInOutSine(m_SmoothnessForMid, m_SmoothnessForPeak, distanceFromBoundariesNormalized);
									}

									if(pebbleIndexInWave < numberOfPebbleInHalfWave){ //lower end part
										valueToSet = Mathf.Lerp(m_PebbleRowList[i].pebbleList[pebbleIndex].transform.localPosition.y, m_PebbleRowList[i].pebbleList[pebbleIndex + 1].transform.localPosition.y, speedByPebbleLocation);
									}
									else if(pebbleIndexInWave > numberOfPebbleInHalfWave){ //higher end after center point!
										valueToSet = Mathf.Lerp(m_PebbleRowList[i].pebbleList[pebbleIndex].transform.localPosition.y, m_PebbleRowList[i].pebbleList[pebbleIndex - 1].transform.localPosition.y, speedByPebbleLocation);
									}
									else{   //Our center main data
										valueToSet = Mathf.Lerp(m_PebbleRowList[i].pebbleList[pebbleIndex].transform.localPosition.y, audioLevelData, Time.deltaTime * m_SpeedAudioLevel);
                                    }

	                                //Debug.Log("centerHitNormalized: " + centerHitNormalized);
									m_PebbleRowList[i].pebbleList[pebbleIndex].transform.localPosition = Vector3.Lerp(
										m_PebbleRowList[i].pebbleList[pebbleIndex].transform.localPosition,
	                                    new Vector3(
										m_PebbleRowList[i].pebbleList[pebbleIndex].transform.localPosition.x, 
										valueToSet, 
										m_PebbleRowList[i].pebbleList[pebbleIndex].transform.localPosition.z),
										m_SmoothnessPebbleMovementInTheFirstRow);

								}
								else{
									m_PebbleRowList[i].pebbleList[j].transform.localPosition = Vector3.Lerp(
										m_PebbleRowList[i].pebbleList[j].transform.localPosition,
										new Vector3(
										m_PebbleRowList[i].pebbleList[j].transform.localPosition.x, 
										audioLevelData, 
										m_PebbleRowList[i].pebbleList[j].transform.localPosition.z),
										m_SmoothnessPebbleMovementInTheFirstRow);

								}
							}
						}
						else
						{
							Log.Error("PebbleManager", "pebbleRowList[{0}].pebbleList[{1}].pebbleList is null", i, j);
						}
					}
				}
				else
                {
                    Log.Error("PebbleManager", "pebbleRowList[{0}].pebbleList is null", i);
                }
            }
        }
    }

}
