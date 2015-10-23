namespace DentedPixel.LTEditor
{
	#region Using
	using UnityEditor;
	using UnityEngine;
	//using System.Reflection;
	#endregion

	public class LTEditor : object{
		public GUIStyle styleCodeTextArea;

		public GUIStyle styleOneCharButton;
		public GUIStyle styleDeleteButton;
		public GUIStyle styleClearAllButton;
		public GUIStyle styleRefreshButton;
		public GUIStyle styleDeleteGroupButton;
		public GUIStyle styleGroupFoldout;
		public GUIStyle styleGroupButton;
		public GUIStyle styleActionButton;
		public GUIStyle styleAreaFoldout;
		public GUIStyle styleExportedBox;

		public Color colorGroupName = new Color(0f/255f,162f/255f,255f/255f);
		public Color colorDelete = new Color(255f/255f,25f/255f,25f/255f);
		public Color colorDestruct = new Color(255f/255f,70f/255f,70f/255f);
		public Color colorRefresh = new Color(100f/255f,190f/255f,255f/255f);
		public Color colorAdd = new Color(120f/255f,255f/255f,180f/255f);
		public Color colorTweenName = new Color(0f/255f,255f/255f,30f/255f);
		public Color colorAddTween = new Color(0f/255f,209f/255f,25f/255f);
		public Color colorAddGroup = new Color(0f/255f,144f/255f,226f/255f);
		public Color colorEasyPathCreator = new Color(150f/255f,180f/255f,255f/255f);
		public Color colorNodes = new Color(190f/255f,255f/255f,190f/255f);
		public Color colorImportExport = new Color(255f/255f,180f/255f,150f/255f);

		public LTEditor(){
			if(EditorGUIUtility.isProSkin){
				colorGroupName = new Color(0f/255f,162f/255f,255f/255f) * 2f;
				colorDelete = new Color(255f/255f,25f/255f,25f/255f) * 2f;
				colorTweenName = new Color(0f/255f,255f/255f,30f/255f) * 2f;
				colorAddTween = new Color(0f/255f,209f/255f,25f/255f) * 2f;
				colorAddGroup = new Color(0f/255f,144f/255f,226f/255f) * 2f;
			}

			styleCodeTextArea = new GUIStyle(GUI.skin.textArea);
			styleCodeTextArea.richText = true;

			styleDeleteButton = new GUIStyle( GUI.skin.button );
			styleDeleteButton.margin = new RectOffset(styleDeleteButton.margin.left,styleDeleteButton.margin.right,2,0);
			styleDeleteButton.padding = new RectOffset(0,0,0,0);
			styleDeleteButton.fixedHeight = 15;
			styleDeleteButton.fixedWidth = 46;

			styleOneCharButton = new GUIStyle( GUI.skin.button );
			styleOneCharButton.padding = new RectOffset(-2,0,0,0);
			styleOneCharButton.fixedHeight = 15;
			styleOneCharButton.fixedWidth = 20;

			styleClearAllButton = new GUIStyle( styleDeleteButton );
			styleClearAllButton.fixedWidth = 80;

			styleRefreshButton = new GUIStyle( styleDeleteButton );
			styleRefreshButton.fixedWidth = 50;

			styleDeleteGroupButton = new GUIStyle( styleDeleteButton );

			styleActionButton = new GUIStyle( GUI.skin.button );
			styleActionButton.fixedHeight = 25;
			styleActionButton.fixedWidth = 140;

			styleGroupFoldout = new GUIStyle(EditorStyles.foldout);
			styleGroupFoldout.margin = new RectOffset(styleGroupFoldout.margin.left+16,0,styleGroupFoldout.margin.top,0);
			styleGroupFoldout.padding = new RectOffset(0,0,0,0);
			styleGroupFoldout.overflow = new RectOffset(0,0,0,0);
			styleGroupFoldout.fixedWidth = 20;

			styleGroupButton = new GUIStyle(GUI.skin.button);
			styleGroupButton.margin = new RectOffset(0,0,2,0);
			styleGroupButton.padding = new RectOffset(0,0,0,0);
			styleGroupButton.fixedHeight = 15;

			styleAreaFoldout = new GUIStyle(EditorStyles.foldout);
			styleAreaFoldout.margin = new RectOffset(styleAreaFoldout.margin.left+16,0,styleAreaFoldout.margin.top,0);
			styleAreaFoldout.fontStyle = FontStyle.Bold;
		}

		private static LTEditor _sharedInstance;

		public static LTEditor shared{
			get{
				if(_sharedInstance==null){
					// Debug.Log("creating instance once...");
					_sharedInstance = new LTEditor();
				}
				return _sharedInstance;
			}
		}

		// Icon creation script
		const string iconString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAC4ElEQVQ4EUWT3WsUZxTGf+/M7G6yJmsSs9muxlTzUYJGEtGIYiGVNgjeae5KoUKgoPRfaOlF/4oWirYX3kVEpAhVlFZBUaOtiZouSUmMGJGQxKSbmd2Z6fPObirD4R3O53Oec45pKHwbG+OBxJhU8tb+t3T2dQFr11sXg0uMIy2hfow+R2+Y/BEbBdSkZjNEsayRI0lRjdKyumTSoRLE4f/OYegSVDxSaUinavpNP4Ojqs1NPvnWZXYXl+nZvcT+vpe4ToQXK4GtH0YuLbk1Phl+xMKbnTx42i2dx5nR3xkbvU8x/5ZC+wo7WjfJNUEmB5cuH3vfQuC7fDX2K+Njjzn7zXkqmw693Yt8d+4nPmiHd/+SdLb6zmF2oY2yn+PnKydtCxHVMKa9bZnjQ894/CLPw+mduKmAzo4lLl4ZZaq0l+EDJb7+/Cbf/3CaHyfOJIRGcUotiLgwyHB4X4nOgs+Fq/2sr3k0NpW5+6SPW/cHobKNT49OsrJmuH5nGD9waciIQA2lxoFTZeTQFH4F/njUg3ECiBBJAekGyO1Y48jAU/HyIXOLHTSkywkCDQanWjHsKrxlqH+ev+dbBDdPSvAtMjviSmD4qGueYnuZG/cO4osrqIoQa7dTkMORgVnybSETNwV/qRmyMtrlshxr9of2P2OjDHcm9+F5mwqO67tj1EY64NhgiQ2x3FVYZvyL60IgbMbRLsBv94YY7Cvx54sisy+FzvOV2KLQomnhvL49r+ntesP6Bko0w8jhGRuLK59IPDz/p0hP5yt+ufZZAj+brVgAye7oBvBOfTxN23b1KgIdBYYKcuRgq/81U6A5u0HztpC7k/1KauGruK1eF3PixEjclK1K7yiBdtKWT+Ab5l51aGSNInmV2w8PyEcH5dijsgdWew2NX4oR25MkMdj/mgOeTsxxicIMmcZIN1HX14Otv+eJRHuJtTPVeJRo62y3KpGKMLGWNun8PXwF8R9jRAcsz/JsQQAAAABJRU5ErkJggg==";
		static Texture2D icon;

		public static Texture2D editorIcon(){
			if( icon == null ){
				icon = new Texture2D(1,1);
				icon.hideFlags = HideFlags.HideAndDontSave;
				icon.LoadImage( System.Convert.FromBase64String( iconString ) );
			}
			
			return icon;
		}
	}

	[CustomEditor (typeof(LeanTweenVisual))]
	public class LeanTweenVisualEditor : Editor
	{
		public static LeanTweenVisual copyObj = null;

		public void OnEnable(){
			LTVisualShared.updateTweens(target as LeanTweenVisual);
		}

		private Vector2 scrollCodeViewPos;

		public override void OnInspectorGUI()
		{
			GUI.DrawTexture( new Rect(16, EditorGUILayout.GetControlRect().y - 16, 16, 16), LTEditor.editorIcon() );

			LeanTweenVisual tween = target as LeanTweenVisual;

			float overallDelay = 0;
			bool clicked, deleted;
			Vector3 vec;
			clicked = false;

			EditorGUILayout.BeginHorizontal();
			tween.restartOnEnable = EditorGUILayout.Toggle(new GUIContent("Restart on enable","When you enable the gameobject these set of tweens will start again"), tween.restartOnEnable);
			tween.repeat = EditorGUILayout.Toggle(new GUIContent("Repeat All","Repeat the whole set of tween groups once they finish"), tween.repeat);
			EditorGUILayout.EndHorizontal();
			if(tween.repeat){
				tween.repeatDelay = EditorGUILayout.FloatField("All Delay", tween.repeatDelay);
				tween.repeatCount = EditorGUILayout.IntField("All Repeat Count", tween.repeatCount);
			}

			float addedGroupDelay = 0f;
			foreach(LeanTweenGroup group in tween.groupList)
			{
				EditorGUILayout.Space();

				GUI.color = LTEditor.shared.colorGroupName;
				EditorGUILayout.BeginHorizontal();
				
				group.foldout = EditorGUILayout.Foldout(group.foldout,"",LTEditor.shared.styleGroupFoldout);
				clicked = GUILayout.Button("Group: " + group.name + " " + (group.startTime ) + "s - " + (group.endTime ) + "s", LTEditor.shared.styleGroupButton);
				GUI.color = LTEditor.shared.colorDelete;
				deleted = GUILayout.Button("Delete", LTEditor.shared.styleDeleteGroupButton);
				EditorGUILayout.EndHorizontal();
				GUI.color = Color.white;

				if(clicked)
				{
					group.foldout = !group.foldout;
				}
				if (deleted)
				{
					tween.groupList.Remove(group);
					break;
				}

				float addedTweenDelay = 0f;
				if(group.foldout)
				{
					group.name = EditorGUILayout.TextField("Group Name", group.name);
					EditorGUILayout.BeginHorizontal();
					group.repeat = EditorGUILayout.Toggle("Group Repeat", group.repeat);
					group.delay = EditorGUILayout.FloatField("Group Delay", group.delay);

					EditorGUILayout.EndHorizontal();
					if(group.repeat){
						group.repeatCount = EditorGUILayout.IntField("Group Repeat Count", group.repeatCount);
					}

					int i = 0;
					foreach(LeanTweenItem item in group.itemList)
					{
						TweenAction a = (TweenAction)item.action;
						
						EditorGUILayout.BeginHorizontal();
						GUILayout.Space(15);

						item.foldout = EditorGUILayout.Foldout(item.foldout,"Tween ");
						GUI.color = LTEditor.shared.colorTweenName;

						int actionIndex = EditorGUILayout.Popup( LTVisualShared.actionIndex(item) , LTVisualShared.methodLabels );

						LTVisualShared.setActionIndex(item, actionIndex);

						// clicked = GUILayout.Button(""+a + " " + ( group.delay + item.delay) + "s - " + ( group.delay + item.delay + item.duration) + "s");
						GUI.color = LTEditor.shared.colorDelete;
						deleted = GUILayout.Button("Delete", LTEditor.shared.styleDeleteButton);
						EditorGUILayout.EndHorizontal();
						GUI.color = Color.white;

						if(clicked)
						{
							item.foldout = !item.foldout;
						}
						if (deleted)
						{
							group.itemList.Remove(item);
							break;
						}

						if(item.foldout)
						{
							a = item.action;
							bool tweenTypeChanged = (int)item.action != item.actionLast;
							if(tweenTypeChanged && item.actionLast>=0){
								// Setup with the helpful default values
								// Debug.Log("Setting up to default values a:"+a);
								/*item.to = Vector3.zero;
								if((a>=TweenAction.MOVE_X && a<=TweenAction.MOVE_LOCAL_Z) || a==TweenAction.MOVE || a==TweenAction.MOVE_LOCAL)
									item.to = item.from = tween.gameObject.transform.position;
								else if((a>=TweenAction.SCALE_X && a<=TweenAction.SCALE_Z) || a==TweenAction.SCALE)
									item.to = item.from = tween.gameObject.transform.localScale;
								#if !UNITY_4_3 && !UNITY_4_5 
								else if(a==TweenAction.CANVAS_MOVE)
									item.to = item.from = tween.gameObject.GetComponent<RectTransform>().anchoredPosition;
								else if(a==TweenAction.CANVAS_SCALE)
									item.to = item.from = tween.gameObject.GetComponent<RectTransform>().localScale;
								#endif	
								*/
							}
							item.actionLast = (int)item.action;
							// Debug.Log("a:"+a);

							// Path
							bool isCurve = false;
							if(a == TweenAction.MOVE_CURVED || a == TweenAction.MOVE_CURVED_LOCAL)
							{
								item.bezierPath = EditorGUILayout.ObjectField("    LeanTweenPath:",item.bezierPath, typeof(LeanTweenPath), true) as LeanTweenPath;
								
								EditorGUILayout.BeginHorizontal();
								item.orientToPath = EditorGUILayout.Toggle("    Orient to Path", item.orientToPath);
								isCurve = true;

								item.isPath2d = EditorGUILayout.Toggle("    2D Path", item.isPath2d);
								EditorGUILayout.EndHorizontal();
							}else if(a == TweenAction.MOVE_SPLINE || a == TweenAction.MOVE_SPLINE_LOCAL)
							{
								item.splinePath = EditorGUILayout.ObjectField("    LeanTweenPath:",item.splinePath, typeof(LeanTweenPath), true) as LeanTweenPath;
								item.orientToPath = EditorGUILayout.Toggle("    Orient to Path", item.orientToPath);
								isCurve = true;
							}

							if(isCurve==false){
								bool isVector = a == TweenAction.MOVE || a == TweenAction.MOVE_LOCAL || a == TweenAction.CANVAS_MOVE || a == TweenAction.ROTATE || a == TweenAction.ROTATE_LOCAL || a == TweenAction.SCALE || a == TweenAction.CANVAS_SCALE || a == TweenAction.DELAYED_SOUND;
								bool isColor = a >= TweenAction.COLOR && a < TweenAction.CALLBACK;
								bool isPlay = a == TweenAction.CANVAS_PLAYSPRITE;
								bool usesFrom = !isColor && !isPlay;
								
								// From Values
								EditorGUILayout.BeginHorizontal();
								if(usesFrom){ // Not a Color tween
									EditorGUILayout.LabelField(new GUIContent("    From", "Specify where the tween starts from, otherwise it will start from it's current value"), GUILayout.Width(50));
									item.between = EditorGUILayout.Toggle( "", item.between == LeanTweenBetween.FromTo, GUILayout.Width(30)) ? LeanTweenBetween.FromTo : LeanTweenBetween.To;
								}
								if(item.between == LeanTweenBetween.FromTo){
									if(isVector) {
										item.from = EditorGUILayout.Vector3Field("", item.from);
									} else if(isColor) {

									} else {
										vec = Vector3.zero;
										vec.x = EditorGUILayout.FloatField("From", item.from.x);
										item.from = vec;
									}
								}
								EditorGUILayout.EndHorizontal();

								// To Values
								EditorGUILayout.BeginHorizontal();
								if(isVector) {
									EditorGUILayout.LabelField("    To", GUILayout.Width(85));
									item.to = EditorGUILayout.Vector3Field("", item.to);
								} else if(isColor) {
									EditorGUILayout.LabelField("    To", GUILayout.Width(85));
									item.colorTo = EditorGUILayout.ColorField("", item.colorTo);
								} else if(isPlay) {
									GUILayout.Space(24);
									item.spritesMaximized = EditorGUILayout.Foldout(item.spritesMaximized,"Sprites");
									if(item.spritesMaximized){
										EditorGUILayout.LabelField("Add", GUILayout.Width(35));
										UnityEngine.Sprite sprite = EditorGUILayout.ObjectField("",null, typeof(UnityEngine.Sprite), true, GUILayout.Width(150)) as UnityEngine.Sprite;
										if(sprite!=null){
											item.sprites = add(item.sprites, sprite);
										}
										EditorGUILayout.Separator();
										EditorGUILayout.Separator();
										EditorGUILayout.Separator();
									}
								} else {
									vec = Vector3.zero;
									EditorGUILayout.LabelField("    To", GUILayout.Width(85));
									vec.x = EditorGUILayout.FloatField("", item.to.x);
									item.to = vec;
								}
								EditorGUILayout.EndHorizontal();

								// Sprite List
								if(isPlay && item.spritesMaximized) {
									for(int j = 0; j < item.sprites.Length; j++){
										EditorGUILayout.BeginHorizontal();
										EditorGUILayout.LabelField("        sprite"+j, GUILayout.Width(85));
										item.sprites[j] = EditorGUILayout.ObjectField("",item.sprites[j], typeof(UnityEngine.Sprite), true) as UnityEngine.Sprite;
										GUI.color = LTEditor.shared.colorDelete;
										deleted = GUILayout.Button("Delete", LTEditor.shared.styleDeleteButton);
										GUI.color = Color.white;
										EditorGUILayout.EndHorizontal();

										if(deleted){
											item.sprites = remove(item.sprites, j);
											break;
										}
									}
								}
								
								if(a == TweenAction.ROTATE_AROUND || a == TweenAction.ROTATE_AROUND_LOCAL 
								#if !UNITY_4_3 && !UNITY_4_5 
								|| a == TweenAction.CANVAS_ROTATEAROUND || a == TweenAction.CANVAS_ROTATEAROUND_LOCAL 
								#endif 
								)
								{
									item.axis = ShowAxis("    Axis", item.axis);
								}
							}
							EditorGUILayout.Space();

							// Easing
							if(a == TweenAction.DELAYED_SOUND){
								item.audioClip = EditorGUILayout.ObjectField("    AudioClip:",item.audioClip, typeof(AudioClip), true) as AudioClip;
							}else{
								EditorGUILayout.BeginHorizontal();
								EditorGUILayout.LabelField("    Easing", GUILayout.Width(80));

								int easeIndex = LTVisualShared.easeIndex( item );
								easeIndex = EditorGUILayout.Popup("", easeIndex, LTVisualShared.easeStrMapping, GUILayout.Width(128) );
								LTVisualShared.setEaseIndex( item, easeIndex);

								EditorGUILayout.Separator();
								EditorGUILayout.EndHorizontal();

								if(item.ease==LeanTweenType.animationCurve){
									item.animationCurve = EditorGUILayout.CurveField("    Ease Curve", item.animationCurve);
								}
								EditorGUILayout.Space();
							}
							if(item.ease>=LeanTweenType.once && item.ease < LeanTweenType.animationCurve){
								EditorGUILayout.LabelField( new GUIContent("   ERROR: You Specified a non-easing type", "Select a type with the value 'Ease' in front of it (or linear)"), EditorStyles.boldLabel );
							}

							// Timing
							if(i>0)
								item.alignWithPrevious = EditorGUILayout.Toggle(new GUIContent("    Align with Previous","When you change the timing of a previous tween, this tween's timing will be adjusted to follow afterwards."), item.alignWithPrevious);
							EditorGUILayout.BeginHorizontal();
							if(i>0 && item.alignWithPrevious){
								item.delay = addedTweenDelay;
								EditorGUILayout.LabelField("    Delay:   "+item.delay, GUILayout.Width(50));
							}else{
								EditorGUILayout.LabelField("    Delay", GUILayout.Width(50));
								item.delay = EditorGUILayout.FloatField("", item.delay, GUILayout.Width(50));
							}
							if(a == TweenAction.DELAYED_SOUND){
								EditorGUILayout.LabelField("Volume", GUILayout.Width(50));
								item.duration = EditorGUILayout.FloatField("", item.duration);
							}else if(a == TweenAction.CANVAS_PLAYSPRITE){
								EditorGUILayout.LabelField("Frame Rate", GUILayout.Width(85));
								item.frameRate = EditorGUILayout.FloatField("", item.frameRate);
							}else{
								EditorGUILayout.LabelField("Time", GUILayout.Width(50));
								item.duration = EditorGUILayout.FloatField("", item.duration, GUILayout.Width(50));
								if(item.duration<=0.0f)
									item.duration = 0.0001f;
							}
							EditorGUILayout.Separator();
							EditorGUILayout.EndHorizontal();

							// Repeat
							EditorGUILayout.Space();
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField("    Loops", GUILayout.Width(50));
							item.doesLoop = EditorGUILayout.Toggle("",item.doesLoop, GUILayout.Width(50));
							
							if(item.doesLoop){
								EditorGUILayout.LabelField(new GUIContent("Repeat","-1 repeats infinitely"), GUILayout.Width(50));
								item.loopCount = EditorGUILayout.IntField(new GUIContent("",""),item.loopCount, GUILayout.Width(50));
								EditorGUILayout.LabelField(new GUIContent("    Wrap","How the tween repeats\nclamp: restart from beginning\npingpong: goes back and forth"), GUILayout.Width(50));
								int index = item.loopType==LeanTweenType.pingPong ? 1 : 0;
								index = EditorGUILayout.Popup("", index, new string[]{"Clamp","Ping Pong"}, GUILayout.Width(70));
								item.loopType = index==0 ? LeanTweenType.clamp : LeanTweenType.pingPong;
							}
							EditorGUILayout.EndHorizontal();
							
							addedTweenDelay = item.duration + item.delay;
							
							EditorGUILayout.Separator();
							EditorGUILayout.Separator();

							i++;
						}
					}
					if (ShowLeftButton("+ Tween", LTEditor.shared.colorAddTween, 15))
					{
						LeanTweenItem newItem = new LeanTweenItem(addedTweenDelay);
						newItem.alignWithPrevious = true;
						group.itemList.Add(newItem);
					}
					addedGroupDelay += addedTweenDelay;

					EditorGUILayout.Separator();
				}
				overallDelay += group.endTime;
			}

			if (ShowLeftButton("+ Group", LTEditor.shared.colorAddGroup))
			{
				// Debug.Log("adding group with delay:"+addedGroupDelay);
				tween.groupList.Add(new LeanTweenGroup(addedGroupDelay));
			}

			
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			EditorGUILayout.LabelField("Generated C# Code", EditorStyles.boldLabel );
			if(Application.isPlaying==false){
				scrollCodeViewPos = EditorGUILayout.BeginScrollView(scrollCodeViewPos, GUILayout.Height(150) );	
			
				EditorGUILayout.TextArea( tween.buildAllTweens(true), LTEditor.shared.styleCodeTextArea );	
			
				EditorGUILayout.EndScrollView();

			}else{
				EditorGUILayout.LabelField("    Not available during runtime");
			}

		}

		// Sprite Methods
		private UnityEngine.Sprite[] add( UnityEngine.Sprite[] arr, UnityEngine.Sprite sprite ){
			UnityEngine.Sprite[] newArr = new UnityEngine.Sprite[arr.Length+1];
			for(int i = 0; i < arr.Length; i++){
				newArr[i] = arr[i];
			}
			newArr[ newArr.Length - 1 ] = sprite;
			return newArr;
		}

		private UnityEngine.Sprite[] remove( UnityEngine.Sprite[] arr, int removePt ){
			UnityEngine.Sprite[] newArr = new UnityEngine.Sprite[arr.Length-1];
			int k = 0;
			for(int i = 0; i < arr.Length; i++){
				if(i!=removePt){
					newArr[k] = arr[i];
					k++;
				}
			}
			return newArr;
		}

		// Editor Methods
		private bool ShowLeftButton(string label, Color color)
		{
			return ShowLeftButton(label, color, 0);
		}

		private bool ShowLeftButton(string label, Color color, float space)
		{
			bool clicked;
			GUI.color = color;
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(space);
			clicked = GUILayout.Button(label, GUILayout.Width(100));
			EditorGUILayout.EndHorizontal();
			GUI.color = Color.white;
			return clicked;
		}

		private Vector3 ShowAxis(string label, Vector3 value)
		{
			Vector3 axis = EditorGUILayout.Vector3Field("    Axis", value);

			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(12);
			GUILayout.Label("Presets:");
			if(GUILayout.Button("Up"))
				axis = Vector3.up;
			if(GUILayout.Button("Down"))
				axis = Vector3.down;
			if(GUILayout.Button("Left"))
				axis = Vector3.left;
			if(GUILayout.Button("Right"))
				axis = Vector3.right;
			if(GUILayout.Button("Back"))
				axis = Vector3.back;
			if(GUILayout.Button("Forw"))
				axis = Vector3.forward;
			EditorGUILayout.EndHorizontal();
			
			return axis;
		}
	}


	/*
	public class LTVisualEditorTimeline : EditorWindow
	{
	    Vector2 scrollPos;
	    float minVal = -10f;
	    float maxVal = 10f;
	    float minLimit = -20f;
	    float maxLimit = 20f;

	    private GUIStyle styleDeleteButton;

	    
	    // Add menu item named "My Window" to the Window menu
	    // UNCOMMENT TO TURN BACK ON! [MenuItem("Window/LeanTween Timeline Editor")]
	    public static void ShowWindow()
	    {
	        //Show existing window instance. If one doesn't exist, make one.
	        EditorWindow.GetWindow(typeof(LTVisualEditorTimeline));
	    }

	    void OnFocus(){
	        
	    }
	    
	    void OnGUI()
	    {
	        Color guiColorDefault = GUI.color;
	        styleDeleteButton = new GUIStyle( GUI.skin.button );
	            styleDeleteButton.margin = new RectOffset(styleDeleteButton.margin.left,styleDeleteButton.margin.right,2,0);
	            styleDeleteButton.padding = new RectOffset(0,0,0,0);
	            styleDeleteButton.fixedHeight = 15;
	            styleDeleteButton.fixedWidth = 76;

	       scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
	       GUILayout.Label("Animation View - Prototype (not functional)");

	       EditorGUILayout.BeginHorizontal();

	       GUI.color = LTEditor.shared.colorTweenName;
	       GUILayout.Button("moveX", styleDeleteButton);
	       GUI.color = guiColorDefault;
	       EditorGUILayout.MinMaxSlider(ref minVal, ref maxVal, minLimit, maxLimit, GUILayout.Width(300));

	       EditorGUILayout.EndHorizontal();

	       DrawingHelper.DrawLine( new Vector2(10f,34f), new Vector2(10f,50f), Color.red, 2f, true);
	       
	       EditorGUILayout.EndScrollView();
	    }
	}
	*/

	// Line drawing routine originally courtesy of Linusmartensson:
	// http://forum.unity3d.com/threads/71979-Drawing-lines-in-the-editor
	//
	// Rewritten to improve performance by Yossarian King / August 2013.
	 
	public static class DrawingHelper
	{
	    private static Texture2D aaLineTex = null;
	    private static Texture2D lineTex = null;
	    private static Material blitMaterial = null;
	    private static Material blendMaterial = null;
	    private static Rect lineRect = new Rect(0, 0, 1, 1);
	 
	    public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width, bool antiAlias)
	    {
	        // Normally the static initializer does this, but to handle texture reinitialization
	        // after editor play mode stops we need this check in the Editor.
	        #if UNITY_EDITOR
	        if (!lineTex)
	        {
	            Initialize();
	        }
	        #endif
	 
	        // Note that theta = atan2(dy, dx) is the angle we want to rotate by, but instead
	        // of calculating the angle we just use the sine (dy/len) and cosine (dx/len).
	        float dx = pointB.x - pointA.x;
	        float dy = pointB.y - pointA.y;
	        float len = Mathf.Sqrt(dx * dx + dy * dy);
	 
	        // Early out on tiny lines to avoid divide by zero.
	        // Plus what's the point of drawing a line 1/1000th of a pixel long??
	        if (len < 0.001f)
	        {
	            return;
	        }
	 
	        // Pick texture and material (and tweak width) based on anti-alias setting.
	        Texture2D tex;
	        Material mat;
	        if (antiAlias)
	        {
	            // Multiplying by three is fine for anti-aliasing width-1 lines, but make a wide "fringe"
	            // for thicker lines, which may or may not be desirable.
	            width = width * 3.0f;
	            tex = aaLineTex;
	            mat = blendMaterial;
	        }
	        else
	        {
	            tex = lineTex;
	            mat = blitMaterial;
	        }
	 
	        float wdx = width * dy / len;
	        float wdy = width * dx / len;
	 
	        Matrix4x4 matrix = Matrix4x4.identity;
	        matrix.m00 = dx;
	        matrix.m01 = -wdx;
	        matrix.m03 = pointA.x + 0.5f * wdx;
	        matrix.m10 = dy;
	        matrix.m11 = wdy;
	        matrix.m13 = pointA.y - 0.5f * wdy;
	 
	        // Use GL matrix and Graphics.DrawTexture rather than GUI.matrix and GUI.DrawTexture,
	        // for better performance. (Setting GUI.matrix is slow, and GUI.DrawTexture is just a
	        // wrapper on Graphics.DrawTexture.)
	        GL.PushMatrix();
	        GL.MultMatrix(matrix);
	        Graphics.DrawTexture(lineRect, tex, lineRect, 0, 0, 0, 0, color, mat);
	        GL.PopMatrix();
	    }
	 
	    // Other than method name, DrawBezierLine is unchanged from Linusmartensson's original implementation.
	    public static void DrawBezierLine(Vector2 start, Vector2 startTangent, Vector2 end, Vector2 endTangent, Color color, float width, bool antiAlias, int segments)
	    {
	        Vector2 lastV = CubeBezier(start, startTangent, end, endTangent, 0);
	        for (int i = 1; i < segments; ++i)
	        {
	            Vector2 v = CubeBezier(start, startTangent, end, endTangent, i/(float)segments);
	            DrawingHelper.DrawLine(lastV, v, color, width, antiAlias);
	            lastV = v;
	        }
	    }
	 
	    private static Vector2 CubeBezier(Vector2 s, Vector2 st, Vector2 e, Vector2 et, float t)
	    {
	        float rt = 1 - t;
	        return rt * rt * rt * s + 3 * rt * rt * t * st + 3 * rt * t * t * et + t * t * t * e;
	    }
	 
	    // This static initializer works for runtime, but apparently isn't called when
	    // Editor play mode stops, so DrawLine will re-initialize if needed.
	    static DrawingHelper()
	    {
	        Initialize();
	    }
	 
	    private static void Initialize()
	    {
	        if (lineTex == null)
	        {
	            lineTex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
	            lineTex.hideFlags = HideFlags.HideAndDontSave;
	            lineTex.SetPixel(0, 1, Color.white);
	            lineTex.Apply();
	        }
	        if (aaLineTex == null)
	        {
	            // TODO: better anti-aliasing of wide lines with a larger texture? or use Graphics.DrawTexture with border settings
	            aaLineTex = new Texture2D(1, 3, TextureFormat.ARGB32, false);
	            aaLineTex.SetPixel(0, 0, new Color(1, 1, 1, 0));
	            aaLineTex.SetPixel(0, 1, Color.white);
	            aaLineTex.SetPixel(0, 2, new Color(1, 1, 1, 0));
	            aaLineTex.Apply();
	        }
	 
	        // GUI.blitMaterial and GUI.blendMaterial are used internally by GUI.DrawTexture,
	        // depending on the alphaBlend parameter. Use reflection to "borrow" these references.
	        blitMaterial = (Material)typeof(GUI).GetMethod("get_blitMaterial", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).Invoke(null, null);
	        blendMaterial = (Material)typeof(GUI).GetMethod("get_blendMaterial", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).Invoke(null, null);
	    }
	}

}
