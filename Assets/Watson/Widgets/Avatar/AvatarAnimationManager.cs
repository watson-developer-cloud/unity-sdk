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
using System.Collections;
using IBM.Watson.Utilities;

namespace IBM.Watson.Widgets.Avatar
{
/// <summary>
/// Avatar animation manager.
/// All animations related with Avatar is listed here. 
/// </summary>
public class AvatarAnimationManager : WatsonBaseAnimationManager {

	[SerializeField,FormerlySerializedAs("rotationVector")]
	private Vector3 m_RotationVector = new Vector3( 0.0f, 1.0f, 0.0f );
	[SerializeField,FormerlySerializedAs("rotationSpeed")]
	private float m_RotationSpeed = 5.0f;

	[SerializeField]
	private Vector3 m_AvatarMoveDown;
	[SerializeField]
	private LeanTweenType m_EaseOnAvatarMovement = LeanTweenType.easeInOutCubic;
	
	#region Private Members
	private float m_AnimationSpeedModifier = 1.0f;

	[SerializeField]
	private float m_AnimationTime = 1.0f;
	private float m_AnimationInitialTime;

	private LTDescr m_AnimationRotation = null;
	private LTDescr m_AnimationMoveDefault = null;
	private LTDescr m_AnimationMoveDown = null;
	#endregion
	

	void Awake()
	{
		m_AnimationInitialTime = m_AnimationTime;
	}
	// Use this for initialization
	void Start () {
		//transform.Rotate(m_RotationVector * Time.deltaTime * m_Speed);
		
		AnimateRotation ();
		//AnimateMoveDown ();
	}
	

	#region Overriden function on WatsonAnimationManager
	
	protected override void OnAnimationStop ()
	{
		StopAllAnimations ();
	}
	
	protected override void OnAnimationPause ()
	{
		if (m_AnimationRotation != null) 
		{
			m_AnimationRotation.pause();
		}
		if (m_AnimationMoveDefault != null) 
		{
			m_AnimationMoveDefault.pause();
		}
		if (m_AnimationMoveDown != null) 
		{
			m_AnimationMoveDown.pause();
		}
	}

	protected override void OnAnimationResume ()
	{
		if (m_AnimationRotation != null) 
		{
			m_AnimationRotation.resume();
		}
		if (m_AnimationMoveDefault != null) 
		{
			m_AnimationMoveDefault.resume();
		}
		if (m_AnimationMoveDown != null) 
		{
			m_AnimationMoveDown.resume();
		}
	}

	protected override void OnAnimationSpeedChange (float speedModifier)
	{
		m_AnimationSpeedModifier = speedModifier;
		m_AnimationTime = m_AnimationInitialTime * (1.0f / speedModifier);
		
		if (m_AnimationMoveDefault != null) 
		{
			m_AnimationMoveDefault.setTime(m_AnimationTime);
		}

		if (m_AnimationMoveDown != null) 
		{
			m_AnimationMoveDown.setTime(m_AnimationTime);
		}
	}

	#endregion

	#region All Animation - Stop / Speed Up - Down
	
	void StopAllAnimations(){
		StopAnimateRotation ();
		StopAnimateMoveDefault ();
		StopAnimateMoveDown ();
	}

	void SetupAnimationSpeedModifier(float speedMofier){
		m_AnimationSpeedModifier = speedMofier;
	}

	#endregion
	
	#region Avatar Rotation Animation
	
	void AnimateRotation(){
		if (m_AnimationRotation == null) 
		{
			m_AnimationRotation = LeanTween.value (gameObject, 0.0f, 10.0f, 10.0f).setLoopType (LeanTweenType.linear).setOnUpdate ((float f) => {
				transform.Rotate(m_RotationVector * Time.deltaTime * m_RotationSpeed * m_AnimationSpeedModifier);
			});
		} 
		else 
		{
			//do nothing		
		}
	}
	
	void StopAnimateRotation(){
		if (m_AnimationRotation != null) 
		{
			LeanTween.cancel(m_AnimationRotation.uniqueId);
		}
	}

	
	#endregion

	#region Avatar Movements Up / Down / Left / Right - Event

	public void AnimateMoveDefault(System.Object[] args = null){
		if (m_AnimationMoveDefault != null) 
		{
			LeanTween.cancel(m_AnimationMoveDefault.uniqueId);
		}
		
		float animationTime = m_AnimationTime;
		if (args != null && args.Length == 1 && float.TryParse(args[0].ToString(), out animationTime)) {
			//animation time come as argument
			
		}
		
		m_AnimationMoveDefault = LeanTween.moveLocalY (gameObject, 0.0f, animationTime).setEase(m_EaseOnAvatarMovement);
		
	}
	
	public void StopAnimateMoveDefault(System.Object[] args = null){
		if (m_AnimationMoveDefault != null) 
		{
			LeanTween.cancel(m_AnimationMoveDefault.uniqueId);
		}
	}

	public void AnimateMoveDown(System.Object[] args = null){
		if (m_AnimationMoveDown != null) 
		{
			LeanTween.cancel(m_AnimationMoveDown.uniqueId);
		}
		
		float animationTime = m_AnimationTime;
			if (args != null && args.Length == 1 && float.TryParse(args[0].ToString(), out animationTime)) {
			//animation time come as argument
			
		}

		m_AnimationMoveDown =	LeanTween.moveLocalY (gameObject, m_AvatarMoveDown.y, animationTime).setEase(m_EaseOnAvatarMovement);
	}
	
	public void StopAnimateMoveDown(System.Object[] args = null){
		if (m_AnimationMoveDown != null) 
		{
			LeanTween.cancel(m_AnimationMoveDown.uniqueId);
		}
	}

	#endregion
}

}
