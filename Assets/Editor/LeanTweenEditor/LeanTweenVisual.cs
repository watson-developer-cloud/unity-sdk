namespace DentedPixel.LTEditor{
	#region Using
	using UnityEngine;
	using System.Collections.Generic;
	using System;
	#endregion

	public class LeanTweenVisual : MonoBehaviour
	{
		#region Fields
		#region Public
		// Structure containing the tween groups and items.
		// Holds information to run sequential and concurrent tweens.
		public List<LeanTweenGroup> groupList = new List<LeanTweenGroup>();

		// Indicates whether the entire tween should continue repeating.
		public bool repeat;

		// Delay between repeats.
		public float repeatDelay;

		// How many times the group of tweens repeat
		public int repeatCount;

		public int repeatIter;

		// If it is repeating, time in game time to call next repeat.
		public float nextCall;

		// Indicates if the whole tween should restart when object is
		// enabled.
		public bool restartOnEnable;

		public int versionNum;

		#endregion

		#endregion

		#region MonoBehaviour Methods

		private void Awake(){
			LTVisualShared.updateTweens(this);
		}

		// Initialize variables.  This is put in start
		// to be compatible with object recycler.
		private void Start()
		{
			buildAllTweens(false);
		}

		// Starts tween or calls again if repeat is turned on.
		private void Update()
		{
			/*if(!_calledOnce)
			{
				buildAllTweens(0, false);
				_calledOnce = true;
			}
			else if(repeat && nextCall < Time.time)
			{
				buildAllTweens(repeatDelay, false);
			}*/
		}


		// Called on enable and if you want the tween
		// to restart on enable / on active.
		private void OnEnable()
		{
			if(restartOnEnable)
			{
				buildAllTweens(false);
			}
		}

		// Remove unnecessary tweens from LeanTween.
		private void OnDisable()
		{
			if(restartOnEnable)
			{
				LeanTween.cancel(gameObject);
			}
		}

		// If object is destroyed, get rid of tweens.
		private void OnDestroy()
		{
			LeanTween.cancel(gameObject);
		}
		#endregion

		#region Public Methods
		public void CopyTo(LeanTweenVisual tween)
		{
			tween.nextCall = nextCall;
			tween.repeat = repeat;
			tween.repeatDelay = repeatDelay;
			tween.restartOnEnable = restartOnEnable;

			tween.groupList = new List<LeanTweenGroup>();
			foreach(LeanTweenGroup group in groupList)
			{
				tween.groupList.Add(new LeanTweenGroup(group));
			}
		}
		#endregion

		#region Private Methods

		private System.Text.StringBuilder codeBuild;
		private string tabs;
		private LTDescr tween;
		private float allTweensDelaySaved;

		public void buildAllTweensAgain(){
			LeanTween.delayedCall(gameObject, repeatDelay, buildAllTweensAgainNow);
		}

		public void buildAllTweensAgainNow(){
			buildAllTweens(false);
		}

		public LTDescr append( string method, float to, float duration ){
			codeBuild.Append(tabs+"LeanTween."+method+"(gameObject, "+to+"f, "+duration+"f)");
			return null;
		}

		public LTDescr appendRect( string method, float to, float duration ){
			codeBuild.Append(tabs+"LeanTween."+method+"(rectTransform, "+to+"f, "+duration+"f)");
			return null;
		}

		public LTDescr append( string method, Vector3 to, float duration ){
			codeBuild.Append(tabs+"LeanTween."+method+"(gameObject, "+vecToStr(to)+", "+duration+"f)");
			return null;
		}

		public LTDescr appendRect( string method, Vector3 to, float duration ){
			codeBuild.Append(tabs+"LeanTween."+method+"(rectTransform, "+vecToStr(to)+", "+duration+"f)");
			return null;
		}

		public LTDescr appendRect( string method, Color color, float duration ){
			codeBuild.Append(tabs+"LeanTween."+method+"(rectTransform, "+colorToStr(color)+", "+duration+"f)");
			return null;
		}

		public LTDescr append( string method, Color color, float duration ){
			codeBuild.Append(tabs+"LeanTween."+method+"(gameObject, "+colorToStr(color)+", "+duration+"f)");
			return null;
		}

		public LTDescr append( string method, Vector3[] to, float duration ){
			codeBuild.Append(tabs+"LeanTween."+method+"(gameObject, new vector3[]{");

			if(to==null){
				codeBuild.Append("null");
			}else{
				for(int i = 0; i < to.Length; i++){
					codeBuild.Append(vecToStr(to[i]));
					if(i<to.Length-1)
						codeBuild.Append(", ");
				}
			}
			codeBuild.Append("} , "+duration+"f)");
			return null;
		}

		public void append( AnimationCurve curve ){
			codeBuild.Append("new AnimationCurve(");
			for(int i = 0; i < curve.length; i++){
				codeBuild.Append("new Keyframe("+curve[i].time+"f,"+curve[i].value+"f)");
				if(i<curve.length-1)
					codeBuild.Append(", ");
			}
			codeBuild.Append(")");
		}

		private string vecToStr( Vector3 vec3 ){
			return "new Vector3("+vec3.x+"f,"+vec3.y+"f,"+vec3.z+"f)";
		}

		private string colorToStr( Color color ){
			return "new Color("+color.r+"f,"+color.g+"f,"+color.b+"f,"+color.a+"f)";
		}

		private void buildTween(LeanTweenItem item, float delayAdd, bool generateCode){
			float delay = item.delay + delayAdd;
			bool code = generateCode;
			float d = item.duration;
			// Debug.Log("item:"+item.action);
			if(item.action == TweenAction.ALPHA)
			{
				tween = code ? append("alpha", item.to.x, d) : LeanTween.alpha(gameObject, item.to.x, d);
			}
			else if(item.action == TweenAction.ALPHA_VERTEX)
			{
				tween = code ? append("alphaVertex", item.to.x, d) : LeanTween.alphaVertex(gameObject, item.to.x, d);
			}
			else if(item.action == TweenAction.MOVE)
			{
				tween = code ? append("move", item.to, d) : LeanTween.move(gameObject, item.to, d);
			}
			else if(item.action == TweenAction.MOVE_LOCAL)
			{
				tween = code ? append("moveLocal", item.to, d) : LeanTween.moveLocal(gameObject, item.to, d);
			}
			else if(item.action == TweenAction.MOVE_LOCAL_X)
			{
				tween = code ? append("moveLocalX", item.to.x, d) : LeanTween.moveLocalX(gameObject, item.to.x, d);
			}
			else if(item.action == TweenAction.MOVE_LOCAL_Y)
			{
				tween = code ? append("moveLocalY", item.to.x, d) : LeanTween.moveLocalY(gameObject, item.to.x, d);
			}
			else if(item.action == TweenAction.MOVE_LOCAL_Z)
			{
				tween = code ? append("moveLocalZ", item.to.x, d) : LeanTween.moveLocalZ(gameObject, item.to.x, d);
			}
			else if(item.action == TweenAction.MOVE_X)
			{
				tween = code ? append("moveX", item.to.x, d) : LeanTween.moveX(gameObject, item.to.x, d);
			}
			else if(item.action == TweenAction.MOVE_Y)
			{
				tween = code ? append("moveY", item.to.x, d) : LeanTween.moveY(gameObject, item.to.x, d);
			}
			else if(item.action == TweenAction.MOVE_Z)
			{
				tween = code ? append("moveZ", item.to.x, d) : LeanTween.moveZ(gameObject, item.to.x, d);
			}
			else if(item.action == TweenAction.MOVE_CURVED)
			{
				tween = code ? append("move", item.bezierPath ? item.bezierPath.vec3 : null, d) : LeanTween.move(gameObject, item.bezierPath.vec3, d);
				if(item.orientToPath){
					if(code){
						codeBuild.Append(".setOrientToPath("+item.orientToPath+")");
					}else{
						tween.setOrientToPath(item.orientToPath);
					}
				}
				if(item.isPath2d){
					if(code){
						codeBuild.Append(".setOrientToPath2d(true)");
					}else{
						tween.setOrientToPath2d(item.isPath2d);
					}
				}
			}
			else if(item.action == TweenAction.MOVE_CURVED_LOCAL)
			{
				tween = code ? append("moveLocal", item.bezierPath ? item.bezierPath.vec3 : null, d) : LeanTween.moveLocal(gameObject, item.bezierPath.vec3, d);
				if(item.orientToPath){
					if(code){
						codeBuild.Append(".setOrientToPath("+item.orientToPath+")");
					}else{
						tween.setOrientToPath(item.orientToPath);
					}
				}
				if(item.isPath2d){
					if(code){
						codeBuild.Append(".setOrientToPath2d(true)");
					}else{
						tween.setOrientToPath2d(item.isPath2d);
					}
				}
			}
			else if(item.action == TweenAction.MOVE_SPLINE)
			{
				tween = code ? append("moveSpline", item.splinePath ? item.splinePath.splineVector() : null, d) : LeanTween.moveSpline(gameObject, item.splinePath.splineVector(), d);
				if(item.orientToPath){
					if(code){
						codeBuild.Append(".setOrientToPath("+item.orientToPath+")");
					}else{
						tween.setOrientToPath(item.orientToPath);
					}
				}
				if(item.isPath2d){
					if(code){
						codeBuild.Append(".setOrientToPath2d(true)");
					}else{
						tween.setOrientToPath2d(item.isPath2d);
					}
				}
			}
			else if(item.action == TweenAction.ROTATE)
			{
				tween = code ? append("rotate", item.to, d) : LeanTween.rotate(gameObject, item.to, d);
			}
			else if(item.action == TweenAction.ROTATE_AROUND)
			{
				if(generateCode){
					codeBuild.Append(tabs+"LeanTween.rotateAround(gameObject, "+vecToStr(item.axis)+", "+item.to.x+"f , "+d+"f)");
				}else{
					tween = LeanTween.rotateAround(gameObject, item.axis, item.to.x, d);
				}
			}
			else if(item.action == TweenAction.ROTATE_AROUND_LOCAL)
			{
				if(generateCode){
					codeBuild.Append(tabs+"LeanTween.rotateAroundLocal(gameObject, "+vecToStr(item.axis)+", "+item.to.x+"f , "+d+"f)");
				}else{
					tween = LeanTween.rotateAroundLocal(gameObject, item.axis, item.to.x, d);
				}
			}
			else if(item.action == TweenAction.ROTATE_LOCAL)
			{
				tween = code ? append("rotateLocal", item.to, d) : LeanTween.rotateLocal(gameObject, item.to, d);
			}
			else if(item.action == TweenAction.ROTATE_X)
			{
				tween = code ? append("rotateX", item.to.x, d) : LeanTween.rotateX(gameObject, item.to.x, d);
			}
			else if(item.action == TweenAction.ROTATE_Y)
			{
				tween = code ? append("rotateY", item.to.x, d) : LeanTween.rotateY(gameObject, item.to.x, d);
			}
			else if(item.action == TweenAction.ROTATE_Z)
			{
				tween = code ? append("rotateZ", item.to.x, d) : LeanTween.rotateZ(gameObject, item.to.x, d);
			}
			else if(item.action == TweenAction.SCALE)
			{
				tween = code ? append("scale", item.to, d) : LeanTween.scale(gameObject, item.to, d);
			}
			else if(item.action == TweenAction.SCALE_X)
			{
				tween = code ? append("scaleX", item.to.x, d) : LeanTween.scaleX(gameObject, item.to.x, d);
			}
			else if(item.action == TweenAction.SCALE_Y)
			{
				tween = code ? append("scaleY", item.to.x, d) : LeanTween.scaleY(gameObject, item.to.x, d);
			}
			else if(item.action == TweenAction.SCALE_Z)
			{
				tween = code ? append("scaleZ", item.to.x, d) : LeanTween.scaleZ(gameObject, item.to.x, d);
			}
			#if !UNITY_4_3 && !UNITY_4_5
			else if(item.action == TweenAction.CANVAS_MOVE)
			{
				tween = code ? appendRect("move", item.to, d) : LeanTween.move(GetComponent<RectTransform>(), item.to, d);
			}
			else if(item.action == TweenAction.CANVAS_SCALE)
			{
				tween = code ? appendRect("scale", item.to, d) : LeanTween.scale(GetComponent<RectTransform>(), item.to, d);
			}
			else if(item.action == TweenAction.CANVAS_ROTATEAROUND)
			{
				if(generateCode){
					codeBuild.Append(tabs+"LeanTween.rotateAround(rectTransform, "+vecToStr(item.axis)+", "+item.to.x+"f , "+d+"f)");
				}else{
					tween = LeanTween.rotateAround(GetComponent<RectTransform>(), item.axis, item.to.x, d);
				}
			}
			else if(item.action == TweenAction.CANVAS_ROTATEAROUND_LOCAL)
			{
				if(generateCode){
					codeBuild.Append(tabs+"LeanTween.rotateAroundLocal(rectTransform, "+vecToStr(item.axis)+", "+item.to.x+"f , "+d+"f)");
				}else{
					tween = LeanTween.rotateAroundLocal(GetComponent<RectTransform>(), item.axis, item.to.x, d);
				}
			}
			else if(item.action == TweenAction.CANVAS_ALPHA)
			{
				tween = code ? appendRect("alpha", item.to.x, d) : LeanTween.alpha(GetComponent<RectTransform>(), item.to.x, d);
			}
			else if(item.action == TweenAction.CANVAS_COLOR)
			{
				tween = code ? appendRect("color", item.colorTo, d) : LeanTween.color(GetComponent<RectTransform>(), item.colorTo, d);
			}
			else if(item.action == TweenAction.TEXT_ALPHA)
			{
				tween = code ? appendRect("textAlpha", item.to.x, d) : LeanTween.textAlpha(GetComponent<RectTransform>(), item.to.x, d);
			}
			else if(item.action == TweenAction.TEXT_COLOR)
			{
				tween = code ? appendRect("textColor", item.colorTo, d) : LeanTween.textColor(GetComponent<RectTransform>(), item.colorTo, d);
			}else if(item.action == TweenAction.CANVAS_PLAYSPRITE){
				if(generateCode){
					codeBuild.Append(tabs+"LeanTween.play(rectTransform, sprites).setFrameRate("+item.frameRate+"f)");
				}else{
					tween = LeanTween.play(GetComponent<RectTransform>(), item.sprites).setFrameRate(item.frameRate);
				}
			}
			#endif
			else if(item.action == TweenAction.COLOR)
			{
				tween = code ? append("color", item.colorTo, d) : LeanTween.color(gameObject, item.colorTo, d);
			}
			else if(item.action == TweenAction.DELAYED_SOUND)
			{
				if(generateCode){
					codeBuild.Append(tabs+"LeanTween.delayedSound(gameObject, passAudioClipHere, "+vecToStr(item.from)+", "+d+"f)");
				}else{
					tween = LeanTween.delayedSound(gameObject, item.audioClip, item.from, item.duration);
				}
			}
			else
			{
				tween = null;
				Debug.Log("The tween '" + item.action.ToString() + "' has not been implemented. info item:"+item);
				return;
			}


			// Append Extras
			if(generateCode){
				if(delay>0f)
					codeBuild.Append(".setDelay("+delay+"f)");
			}else{
				tween = tween.setDelay(delay);
			}
			if(item.ease == LeanTweenType.animationCurve){
				if(generateCode){
					codeBuild.Append(".setEase("); 
					append( item.animationCurve );
					codeBuild.Append(")");
				}else{
					tween.setEase(item.animationCurve);
				}
			}else{
				if(generateCode){
					if(item.ease!=LeanTweenType.linear)
						codeBuild.Append(".setEase(LeanTweenType."+item.ease+")");
				}else{
					tween.setEase(item.ease);
				}
			}
			// Debug.Log("curve:"+item.animationCurve+" item.ease:"+item.ease);
			if(item.between == LeanTweenBetween.FromTo)
			{
				if(generateCode)
					codeBuild.Append(".setFrom("+item.from+")");
				else
					tween.setFrom(item.from);
			}
			if(item.doesLoop){
				if(generateCode)
					codeBuild.Append(".setRepeat("+item.loopCount+")");
				else
					tween.setRepeat(item.loopCount);

				if(item.loopType==LeanTweenType.pingPong){
					if(generateCode)
						codeBuild.Append(".setLoopPingPong()");
					else
						tween.setLoopPingPong();
				}
			}
			if(generateCode) codeBuild.Append(";\n");
		}

		public void buildGroup( object g ){
			LeanTweenGroup group = (LeanTweenGroup)g;
			// Debug.Log("buildGroup group:"+group+" group.delay:"+group.delay+" len:"+group.itemList.Count+" code:"+group.generateCode+" time:"+Time.time);
			foreach(LeanTweenItem item in group.itemList)
			{
				buildTween(item, group.delay, group.generateCode);
			}

			// Debug.Log("group.iter:"+group.repeatIter+" count:"+group.repeatCount);
			if(!group.generateCode && group.repeat && (group.repeatCount<0 || group.repeatIter < group.repeatCount-1)){
				LeanTween.delayedCall(gameObject, group.endTime, buildGroup).setOnCompleteParam( group ).setDelay(group.delay);
				group.repeatIter++;
			}
		}

		// Builds the tween structure with all the appropriate values.
		// Cancels all previous tweens to keep a clean tween list.
		// The overallDelay variable is used to set a delay
		// to the entire group.
		// <param name="overallDelay">Overall delay.</param>
		public string buildAllTweens(bool generateCode)
		{
			if(generateCode){
				codeBuild = new System.Text.StringBuilder(1024);
				tabs = "";
				if(repeat)
					tabs += "\t";
			}else{
				LeanTween.cancel(gameObject);
			}
			
			string preTabs = tabs;
			float lastTweenTime = 0.0f;
			int i = 0;
			foreach(LeanTweenGroup group in groupList)
			{
				bool wrapCode = (group.repeat && group.itemList.Count>0) || group.delay>0f;
				if(generateCode){
					if(i!=0)
						codeBuild.Append("\n");
					codeBuild.Append(tabs+"// "+group.name+" Group\n");
				}
				if(generateCode && wrapCode){
					codeBuild.Append(tabs+"LeanTween.delayedCall(gameObject, "+group.endTime+"f, ()=>{\n");
					tabs += "\t";
				}

				if(generateCode)
					group.setGenerateCode();
				group.repeatIter = 0;
				buildGroup(group);

				tabs = preTabs;
				if(generateCode && wrapCode){
					if(group.delay>0f){
						codeBuild.Append(".setDelay("+group.delay+"f)");
					}
					codeBuild.Append(tabs+"}).setOnCompleteOnStart(true)");
					if(group.repeat)	
						codeBuild.Append(".setRepeat("+(group.repeatCount < 0 ? -1 : group.repeatCount-1)+")");
					codeBuild.Append(";\n");
				}

				if(group.endTime>lastTweenTime)
					lastTweenTime = group.endTime;
				i++;
			}

			if(repeat){
				if(generateCode){
					codeBuild.Insert(0, "LeanTween.delayedCall(gameObject, "+lastTweenTime+"f, ()=>{\n");
					codeBuild.Append("}).setRepeat("+repeatIter+").setOnCompleteOnStart(true);\n");
				}else{
					repeatIter = 0;
					LeanTween.delayedCall(gameObject, lastTweenTime, buildAllTweensAgain);
				}
			}
			if(generateCode){
				return codeBuild.ToString();
			}else{
				return null;
			}
		}
		
		#endregion
	}

	// Special lean tween actions used just for the mono GUI.
	// Adds variables like From and FromTo and also puts them
	// in a nice legible format for displaying in gui.
	public enum LeanTweenBetween{FromTo, To};

	// Contains a single lean tween item.
	[System.Serializable]
	public class LeanTweenItem
	{
		public string name = "Tween";

		// Holds the action type performed by the tween
		public TweenAction action = TweenAction.MOVE;
		public int actionLast = -1; // For keeping track of when the action has changed.
	
		// Holds the action type performed by the tween in string format
		public string actionStr;

		// Indicates if the action if from a certain value to a certain value or
		// from its current value to a value.
		public LeanTweenBetween between = LeanTweenBetween.To;

		// The easing to use.
		public LeanTweenType ease = LeanTweenType.linear;
		public string easeStr = "";

		public AnimationCurve animationCurve = AnimationCurve.Linear(0,0,1,1);

		// The start value if provided.
		// If it is propagated using a float value, it is just stored in the x value.
		public Vector3 from;

		// The end vector value.
		// If it is propagated using a float value, it is just stored in the x value.
		public Vector3 to;

		// The end color value.
		public Color colorTo = Color.red;

		// Axis to rotate around, useful only for rotate around tween.
		public Vector3 axis = Vector3.forward;

		// The duration of the tween.
		public float duration = 1f;

		// The delay of this tween based on the begining of the group.
		public float delay;

		// If set this will adjust the delay time of this item to line up with the previous tween
		public bool alignWithPrevious = false;

		// Foldout used for GUI display.
		public bool foldout = true;

		// Bezier Path used if the tween follows along one
		public LeanTweenPath bezierPath;

		// Spline Path used if the tween follows along one
		public LeanTweenPath splinePath;

		// AudioClip used in delayedSound
		
		public AudioClip audioClip;

		// For use on path tweens
		public bool orientToPath = true;

		// Set the path to behave in a 2d way
		public bool isPath2d = false;

		public bool doesLoop = false;
		public int loopCount = -1;
		public LeanTweenType loopType;

		public UnityEngine.Sprite[] sprites = new UnityEngine.Sprite[]{};
		public bool spritesMaximized = true;
		public float frameRate = 6f;

		// Instantiates LeanTweenGroup.
		public LeanTweenItem()
		{
		}

		public LeanTweenItem( float delay )
		{
			this.delay = delay;
		}
		
		// Instantiates LeanTweenGroup by making a copy of group.
		// <param name="group">Group.</param>
		public LeanTweenItem(LeanTweenItem item)
		{
			name = item.name;
			action = item.action;
			between = item.between;
			ease = item.ease;
			from = item.from;
			to = item.to;
			axis = item.axis;
			duration = item.duration;
			delay = item.delay;
			foldout = item.foldout;
		}
	}

	// A single lean tween group that can have concurrently
	// running lean tween items.
	[System.Serializable]
	public class LeanTweenGroup
	{
		// Name of this group.
		public string name = "Tweens";

		// Whether the group repeats
		public bool repeat = false;

		// The delay before tweens in this group start.
		public float delay = 0;

		// How many times the repeat is made (-1 means infinite)
		public int repeatCount = -1;

		public int repeatIter = 0;

		// Foldout used for GUI display.
		public bool foldout = true;

		// List of concurrent tweens.
		public List<LeanTweenItem> itemList = new List<LeanTweenItem>();

		private bool _generateCode = false;

		// Instantiates LeanTweenGroup.
		public LeanTweenGroup()
		{
		}
		public LeanTweenGroup(float delay)
		{
			this.delay = delay;
		}

		// Instantiates LeanTweenGroup by making a copy of group.
		// <param name="group">Group.</param>
		public LeanTweenGroup(LeanTweenGroup group)
		{
			name = group.name;
			delay = group.delay;
			foldout = group.foldout;
			itemList.Clear();
			foreach(LeanTweenItem item in group.itemList)
			{
				itemList.Add(new LeanTweenItem(item));
			}
		}

		// Gets the time in which the first tween will start
		// including all delays.
		// <value>The start time.</value>
		public float startTime
		{
			get
			{
				float min = float.MaxValue;
				foreach(LeanTweenItem item in itemList)
				{
					min = Mathf.Min(item.delay + delay, min);
				}
				if(itemList.Count == 0)
				{
					return 0;
				}
				else
				{
					return min;
				}
			}
		}

		// Gets the time in which the last tween will finish
		// including all delays and durations.
		// <value>The end time.</value>
		public float endTime
		{
			get
			{
				float max = float.MinValue;
				foreach(LeanTweenItem item in itemList)
				{
					max = Mathf.Max(item.delay + item.duration + delay, max);
				}
				if(itemList.Count == 0)
				{
					return 0;
				}
				else
				{
					return max;
				}
			}
		}

		public void setGenerateCode(){
			_generateCode = true;
		}

		public bool generateCode{
			get
			{
				return _generateCode;
			}
		}
	}
}