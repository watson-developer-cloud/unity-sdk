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
        /// <summary>
        /// 
        /// </summary>
        public PebbleRow [] PebbleRowList { get { return m_PebbleRowList; } set { m_PebbleRowList = value; } }

        [SerializeField,FormerlySerializedAs("pebbleRowList")]
        private PebbleRow[] m_PebbleRowList = null;

        // Use this for initialization
        private void Start()
        {
            if (m_PebbleRowList == null)
            {
                Log.Error("PebbleManager", "pebbleRowList is null! This shouldn't be!");
                return;
            }
        }

        [SerializeField,FormerlySerializedAs("smoothnessLimitBetweenRows")]
        private Vector2 m_SmoothnessLimitBetweenRows; //Smothness MIN, MAX

        private bool m_SetDataOnFrame = false;
		private bool m_IsWatsonIsTalking = false;
        private float m_LatestValueReceived = 0.0f;

		public float m_SpeedFadingOut = 5.0f;
        // Update is called once per frame
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

		public static float Hermite(float start, float end, float value)
		{
			return Mathf.Lerp(start, end, value * value * (3.0f - 2.0f * value));
		}

		public bool isTestingClamp = false;
		public float smoothness = 1.0f;
		public float minValue = 0.9f;
		public float midValue = 0.5f;
		public float maxValue = 1.0f;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="centerHitNormalized"></param>
        /// <param name="setDataOnFrame"></param>
		public void SetAudioData(float centerHitNormalized, bool isWatsonTalking = false, bool setDataOnFrame = true)
        {
            this.m_SetDataOnFrame = setDataOnFrame;
			this.m_IsWatsonIsTalking = isWatsonTalking;

            if (centerHitNormalized == float.NaN)
            {
                Log.Error("PebbleManager", "Value for SetAudioData is NAN");
                centerHitNormalized = 0.0f;
            }
            m_LatestValueReceived = centerHitNormalized;

			int numberOfWaves = 3;
			int numberOfPebbleInOneWave = m_PebbleRowList[0].pebbleList.Length / numberOfWaves; //120
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
								if(isWatsonTalking){

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

									float valueToSet = centerHitNormalized;


									//float speedByPebbleLocation = minValue + (maxValue - minValue) * distanceFromCenterNormalized;
									float speedByPebbleLocation = 0.0f;
									if(distanceFromBoundariesNormalized < 0.5f){
										speedByPebbleLocation = LeanTween.easeInOutSine( Mathf.Max(minValue, midValue) , Mathf.Min(minValue, midValue), distanceFromBoundariesNormalized);
									}
									else{
										speedByPebbleLocation = LeanTween.easeInOutSine(midValue, maxValue, distanceFromBoundariesNormalized);
									}


									if(pebbleIndexInWave < numberOfPebbleInHalfWave){ //lower end part
										valueToSet = Mathf.Lerp(m_PebbleRowList[i].pebbleList[pebbleIndex].transform.localPosition.y, m_PebbleRowList[i].pebbleList[pebbleIndex + 1].transform.localPosition.y, speedByPebbleLocation);
									}
									else if(pebbleIndexInWave > numberOfPebbleInHalfWave){ //higher end after center point!
										valueToSet = Mathf.Lerp(m_PebbleRowList[i].pebbleList[pebbleIndex].transform.localPosition.y, m_PebbleRowList[i].pebbleList[pebbleIndex - 1].transform.localPosition.y, speedByPebbleLocation);
									}
									else{
										valueToSet = centerHitNormalized;
									}

	                                //Debug.Log("centerHitNormalized: " + centerHitNormalized);
									m_PebbleRowList[i].pebbleList[pebbleIndex].transform.localPosition = Vector3.Lerp(
										m_PebbleRowList[i].pebbleList[pebbleIndex].transform.localPosition,
	                                    new Vector3(
										m_PebbleRowList[i].pebbleList[pebbleIndex].transform.localPosition.x, 
										valueToSet, 
										m_PebbleRowList[i].pebbleList[pebbleIndex].transform.localPosition.z),
										smoothness);

								}
								else{
									m_PebbleRowList[i].pebbleList[j].transform.localPosition = Vector3.Lerp(
										m_PebbleRowList[i].pebbleList[j].transform.localPosition,
										new Vector3(
										m_PebbleRowList[i].pebbleList[j].transform.localPosition.x, 
										centerHitNormalized, 
										m_PebbleRowList[i].pebbleList[j].transform.localPosition.z),
										smoothness);

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
