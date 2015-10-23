namespace DentedPixel.LTEditor{
	#region Using
	using UnityEngine;
	//using System.Reflection;
	#endregion
	public class LTVisualShared : MonoBehaviour {

		public static string[] LT_2_12_MethodNames = new string[]{
			"MOVE_X","MOVE_Y","MOVE_Z","MOVE_LOCAL_X","MOVE_LOCAL_Y","MOVE_LOCAL_Z","MOVE_CURVED","MOVE_CURVED_LOCAL","MOVE_SPLINE","MOVE_SPLINE_LOCAL","SCALE_X","SCALE_Y","SCALE_Z","ROTATE_X","ROTATE_Y","ROTATE_Z","ROTATE_AROUND","ALPHA","ALPHA_VERTEX","CALLBACK","MOVE","MOVE_LOCAL","ROTATE","ROTATE_LOCAL","SCALE","VALUE3","GUI_MOVE","GUI_MOVE_MARGIN","GUI_SCALE","GUI_ALPHA","GUI_ROTATE","DELAYED_SOUND"
		};

		// 0 - Transform 
		// 1 - RectTransform
		public static object[][] methodLabelsGrouping = new object[][]{
			new object[]{"move","0"},
			new object[]{"moveLocal","0"},
			new object[]{"moveX","0"},
			new object[]{"moveY","0"},
			new object[]{"moveZ","0"},
			new object[]{"moveLocalX","0"},
			new object[]{"moveLocalY","0"},
			new object[]{"moveLocalZ","0"},
			new object[]{"move (bezier)","0"},
			new object[]{"moveLocal (bezier)","0"},
			new object[]{"moveSpline","0"},
			new object[]{"moveSplineLocal","0"},
			#if !UNITY_4_3 && !UNITY_4_5
			new object[]{"move (canvas)","1"},
			#endif
			new object[]{"scale","0"},
			new object[]{"scaleX","0"},
			new object[]{"scaleY","0"},
			new object[]{"scaleZ","0"},
			#if !UNITY_4_3 && !UNITY_4_5
			new object[]{"scale (canvas)","1"},
			#endif
			new object[]{"rotateAround","0"},
			new object[]{"rotateAroundLocal","0"},
			#if !UNITY_4_3 && !UNITY_4_5
			new object[]{"play (canvas)","1"},
			#endif
			new object[]{"rotateX","0"},
			new object[]{"rotateY","0"},
			new object[]{"rotateZ","0"},
			new object[]{"alpha","0"},
			#if !UNITY_4_3 && !UNITY_4_5
			new object[]{"alpha (background)","1"},
			new object[]{"alpha (text)","1"},
			#endif
			new object[]{"alphaVertex","0"},
			new object[]{"color","0"},
			#if !UNITY_4_3 && !UNITY_4_5
			new object[]{"color (background)","1"},
			new object[]{"color (text)","1"},
			#endif
			new object[]{"delayedSound","0"},
			new object[]{"rotate (deprecated)","0"},
			new object[]{"rotateLocal (deprecated)","0"},
			#if !UNITY_4_3 && !UNITY_4_5
			new object[]{"rotateAround (canvas)","1"},
			new object[]{"rotateAroundLocal (canvas)","1"},
			#endif
		};

		public static string[] methodLabels;
		public static string[] methodStrMapping;
		public static int[] methodIntMapping;

		public static string[] methodStrMappingGrouping = new string[]{
			"MOVE",
			"MOVE_LOCAL",
			"MOVE_X",
			"MOVE_Y",
			"MOVE_Z",
			"MOVE_LOCAL_X",
			"MOVE_LOCAL_Y",
			"MOVE_LOCAL_Z",
			"MOVE_CURVED",
			"MOVE_CURVED_LOCAL",
			"MOVE_SPLINE",
			"MOVE_SPLINE_LOCAL",
			#if !UNITY_4_3 && !UNITY_4_5
			"CANVAS_MOVE",
			#endif
			"SCALE",
			"SCALE_X",
			"SCALE_Y",
			"SCALE_Z",
			#if !UNITY_4_3 && !UNITY_4_5
			"CANVAS_SCALE",
			#endif
			"ROTATE_AROUND",
			"ROTATE_AROUND_LOCAL",
			#if !UNITY_4_3 && !UNITY_4_5
			"CANVAS_PLAYSPRITE",
			#endif
			"ROTATE_X",
			"ROTATE_Y",
			"ROTATE_Z",
			"ALPHA",
			#if !UNITY_4_3 && !UNITY_4_5
			"CANVAS_ALPHA",
			"TEXT_ALPHA",
			#endif
			"ALPHA_VERTEX",
			"COLOR",
			#if !UNITY_4_3 && !UNITY_4_5
			"CANVAS_COLOR",
			"TEXT_COLOR",
			#endif
			"DELAYED_SOUND",
			"ROTATE",
			"ROTATE_LOCAL",
			#if !UNITY_4_3 && !UNITY_4_5
			"CANVAS_ROTATEAROUND",
			"CANVAS_ROTATEAROUND_LOCAL",
			#endif
			"VALUE3",
			"GUI_MOVE",
			"GUI_MOVE_MARGIN",
			"GUI_SCALE",
			"GUI_ALPHA",
			"GUI_ROTATE",
			"CALLBACK_COLOR",
			"CALLBACK"
		};
		public static int[] methodIntMappingGrouping = new int[]{
			(int)TweenAction.MOVE,
			(int)TweenAction.MOVE_LOCAL,
			(int)TweenAction.MOVE_X,
			(int)TweenAction.MOVE_Y,
			(int)TweenAction.MOVE_Z,
			(int)TweenAction.MOVE_LOCAL_X,
			(int)TweenAction.MOVE_LOCAL_Y,
			(int)TweenAction.MOVE_LOCAL_Z,
			(int)TweenAction.MOVE_CURVED,
			(int)TweenAction.MOVE_CURVED_LOCAL,
			(int)TweenAction.MOVE_SPLINE,
			(int)TweenAction.MOVE_SPLINE_LOCAL,
			#if !UNITY_4_3 && !UNITY_4_5
			(int)TweenAction.CANVAS_MOVE,
			#endif
			(int)TweenAction.SCALE,
			(int)TweenAction.SCALE_X,
			(int)TweenAction.SCALE_Y,
			(int)TweenAction.SCALE_Z,
			#if !UNITY_4_3 && !UNITY_4_5
			(int)TweenAction.CANVAS_SCALE,
			#endif
			(int)TweenAction.ROTATE_AROUND,
			(int)TweenAction.ROTATE_AROUND_LOCAL,
			#if !UNITY_4_3 && !UNITY_4_5
			(int)TweenAction.CANVAS_PLAYSPRITE,
			#endif
			(int)TweenAction.ROTATE_X,
			(int)TweenAction.ROTATE_Y,
			(int)TweenAction.ROTATE_Z,
			(int)TweenAction.ALPHA,
			#if !UNITY_4_3 && !UNITY_4_5
			(int)TweenAction.CANVAS_ALPHA,
			(int)TweenAction.TEXT_ALPHA,
			#endif
			(int)TweenAction.ALPHA_VERTEX,
			(int)TweenAction.COLOR,
			#if !UNITY_4_3 && !UNITY_4_5
			(int)TweenAction.CANVAS_COLOR,
			(int)TweenAction.TEXT_COLOR,
			#endif
			(int)TweenAction.DELAYED_SOUND,
			(int)TweenAction.ROTATE,
			(int)TweenAction.ROTATE_LOCAL,
			#if !UNITY_4_3 && !UNITY_4_5
			(int)TweenAction.CANVAS_ROTATEAROUND,
			(int)TweenAction.CANVAS_ROTATEAROUND_LOCAL,
			#endif
			(int)TweenAction.VALUE3,
			(int)TweenAction.GUI_MOVE,
			(int)TweenAction.GUI_MOVE_MARGIN,
			(int)TweenAction.GUI_SCALE,
			(int)TweenAction.GUI_ALPHA,
			(int)TweenAction.GUI_ROTATE,
			(int)TweenAction.CALLBACK_COLOR,
			(int)TweenAction.CALLBACK,
		};

		public static int[] easeIntMapping = new int[]{
			(int)LeanTweenType.notUsed, (int)LeanTweenType.linear, (int)LeanTweenType.easeOutQuad, (int)LeanTweenType.easeInQuad, (int)LeanTweenType.easeInOutQuad, (int)LeanTweenType.easeInCubic, (int)LeanTweenType.easeOutCubic, (int)LeanTweenType.easeInOutCubic, (int)LeanTweenType.easeInQuart, (int)LeanTweenType.easeOutQuart, (int)LeanTweenType.easeInOutQuart,
			(int)LeanTweenType.easeInQuint, (int)LeanTweenType.easeOutQuint, (int)LeanTweenType.easeInOutQuint, (int)LeanTweenType.easeInSine, (int)LeanTweenType.easeOutSine, (int)LeanTweenType.easeInOutSine, (int)LeanTweenType.easeInExpo, (int)LeanTweenType.easeOutExpo, (int)LeanTweenType.easeInOutExpo, (int)LeanTweenType.easeInCirc, (int)LeanTweenType.easeOutCirc, 
			(int)LeanTweenType.easeInOutCirc, (int)LeanTweenType.easeInBounce, (int)LeanTweenType.easeOutBounce, (int)LeanTweenType.easeInOutBounce, (int)LeanTweenType.easeInBack, (int)LeanTweenType.easeOutBack, (int)LeanTweenType.easeInOutBack, (int)LeanTweenType.easeInElastic, (int)LeanTweenType.easeOutElastic, (int)LeanTweenType.easeInOutElastic, 
			(int)LeanTweenType.easeSpring, (int)LeanTweenType.easeShake, (int)LeanTweenType.punch, (int)LeanTweenType.once, (int)LeanTweenType.clamp, (int)LeanTweenType.pingPong, (int)LeanTweenType.animationCurve
		};

		public static string[] easeStrMapping = new string[]{
			"notUsed", "linear", "easeOutQuad", "easeInQuad", "easeInOutQuad", "easeInCubic", "easeOutCubic", "easeInOutCubic", "easeInQuart", "easeOutQuart", "easeInOutQuart",
			"easeInQuint", "easeOutQuint", "easeInOutQuint", "easeInSine", "easeOutSine", "easeInOutSine", "easeInExpo", "easeOutExpo", "easeInOutExpo", "easeInCirc", "easeOutCirc", 
			"easeInOutCirc", "easeInBounce", "easeOutBounce", "easeInOutBounce", "easeInBack", "easeOutBack", "easeInOutBack", "easeInElastic", "easeOutElastic", "easeInOutElastic", 
			"easeSpring", "easeShake", "punch", "once", "clamp", "pingPong", "animationCurve"
		};

		public static void updateTweens(LeanTweenVisual tween){
			//LeanTweenVisual tween = target as LeanTweenVisual;

			// Debug.Log("tween.versionNum:"+tween.versionNum);
			if(tween.versionNum==0){ // upgrade script from 2.12
				foreach(LeanTweenGroup group in tween.groupList)
				{
					foreach(LeanTweenItem item in group.itemList)
					{
						// search through old lookup table for enums to convert over item.action to new string based saved format
						Debug.Log("to action:"+(int)item.action);
						if((int)item.action<LT_2_12_MethodNames.Length-1){
							item.actionStr = LT_2_12_MethodNames[ (int)item.action ];
						}else{
							item.actionStr = LT_2_12_MethodNames[ LT_2_12_MethodNames.Length-1 ];
						}
						
						Debug.Log("item.action toStr:"+item.actionStr);
					}
				}
				tween.versionNum = 220;
			}else{
				tween.versionNum = 220;
			}
			string editType = "0";
			#if !UNITY_4_3 && !UNITY_4_5
			if(tween.gameObject.GetComponent<RectTransform>()){
				editType = "1";
				// Debug.Log("is RectTransform");
			}
			#endif

			// Debug.Log("OnEnable methodLabels:"+methodLabels);
			if(methodLabels==null){
				int labelsCount = 0;
				for(int i=0;i<methodLabelsGrouping.Length;i++){
					for(int k=0; k<methodLabelsGrouping[i].Length;k++){
						if(methodLabelsGrouping[i][k].Equals(editType))
							labelsCount++;
					}	
				}

				methodLabels = new string[labelsCount];
				methodStrMapping = new string[labelsCount];
				methodIntMapping = new int[labelsCount];
				int inputIter = 0;
				for(int i=0;i<methodLabelsGrouping.Length;i++){
					for(int k=0; k<methodLabelsGrouping[i].Length;k++){
						if(methodLabelsGrouping[i][k].Equals(editType)){
							methodLabels[inputIter] = (string)methodLabelsGrouping[i][0];
							methodStrMapping[inputIter] = methodStrMappingGrouping[i];
							methodIntMapping[inputIter] = methodIntMappingGrouping[i];
							inputIter++;
						}
					}	
				}
			}

			foreach(LeanTweenGroup group in tween.groupList){
				foreach(LeanTweenItem item in group.itemList){
					int actionIndex = LTVisualShared.actionIndex(item);
					item.action = (TweenAction)methodIntMapping[ actionIndex ];
					item.actionStr = methodStrMapping[ actionIndex ];
				}
			}
		}

		public static int actionIndex( LeanTweenItem item ){
			int actionIndex = -1;
			for(int j = 0; j < methodStrMapping.Length; j++){
				if( item.actionStr == methodStrMapping[j] ){
					actionIndex = j;
					// Debug.Log("found match actionIndex:"+actionIndex + " methodStrMapping[actionIndex]:"+methodStrMapping[actionIndex]);
					break;
				}
			}
			if(actionIndex<0) // nothing found set to intial 
				actionIndex = 0;
			return actionIndex;
		}

		public static void setActionIndex( LeanTweenItem item, int newIndex ){
			item.action = (TweenAction)LTVisualShared.methodIntMapping[ newIndex ];
			item.actionStr = LTVisualShared.methodStrMapping[ newIndex ];
		}

		public static int easeIndex( LeanTweenItem item ){
			if(item.easeStr.Length<=0){ // First Time setup
				item.easeStr = easeStrMapping[ (int)item.ease ];
				if(item.ease==LeanTweenType.notUsed)
					item.easeStr = "linear";
			}
			int easeIndex = -1; // easeIndex( item )
			for(int j = 0; j < easeStrMapping.Length; j++){
				if( item.easeStr == easeStrMapping[j] ){
					easeIndex = j;
					// Debug.Log("found match easeIndex:"+easeIndex + " easeStrMapping[easeIndex]:"+easeStrMapping[easeIndex]);
					break;
				}
			}
			if(easeIndex<0) // nothing found set to intial 
				easeIndex = 0;
			return easeIndex;
		}

		public static void setEaseIndex( LeanTweenItem item, int newIndex){
			item.ease = (LeanTweenType)LTVisualShared.easeIntMapping[ newIndex ];
			item.easeStr = LTVisualShared.easeStrMapping[ newIndex ];
		}

	}// end LTVisual Shared

}
