using UnityEngine;
using System.Collections;

[AddComponentMenu("LeanTween/LeanTweenPath")]

public class LeanTweenPath:MonoBehaviour {
	public int count;
	
	public Transform[] pts;
	public Vector3[] path;
	public enum LeanTweenPathType{
		bezier,
		spline
	}
	public LeanTweenPathType pathType;
	public float controlSize = 1f;
	public bool showArrows = true;

	public bool nodesMaximized = true;
	public bool creatorMaximized = false;
	public bool importMaximized = false;

	private int i;
	private int k;
	public int lastCount = 1;

	public static Color curveColor;
	public static Color lineColor;

	private void init(){
		
		if(path!=null && path.Length!=0 && (pts == null || pts.Length == 0)){ // transfer over paths made with the legacy path variable
			for (i=transform.childCount-1; i>=0; i--) {
				DestroyImmediate( transform.GetChild(i).gameObject );
			}
			pts = new Transform[ path.Length ];
			for(i=0;i<path.Length;i++){
				if(pathType==LeanTweenPathType.bezier){
					if(i>3 && i%4==0){
						
					}else{
						pts[i] = createChild(i, path[i]);
						// Debug.Log("creating i:"+i+" at:"+path[i]);
					}
				}else{
					pts[i] = createChild(i, path[i]);
				}
			}
			reset();
			path = new Vector3[0];
			lastCount = count = path.Length;
		}

		if(pts == null || pts.Length == 0){ // initial creation
			if(pathType==LeanTweenPathType.bezier){
				pts = new Transform[]{createChild(0, new Vector3(0f,0f,0f)), createChild(1, new Vector3(5f,0f,0f)), createChild(2, new Vector3(4f,0f,0f)), createChild(3, new Vector3(5f,5f,0f))};
			}else{
				pts = new Transform[]{createChild(0, new Vector3(0f,0f,0f)), createChild(1, new Vector3(2f,0f,0f)), createChild(2, new Vector3(3f,2f,0f))};
			}
			reset();
			//lastCount = count = 1;
		}

		// Debug.Log("count:"+count+" lastCount:"+lastCount);
		if(lastCount!=count){ // A curve must have been added or subtracted
			
			if(pathType==LeanTweenPathType.bezier){ // BEZIER
				Vector3 lastPos = Vector3.zero;
				Transform[] newPts = new Transform[ count ];

				if(lastCount>count){ // remove unused points
					//Debug.Log("removing stuff count:"+count);
					k = 0;
					for(i=0;i<pts.Length;i++){ // loop through to see if it was any of the in-between nodes deleted (otherwise it will just delete it from the end)
						if(i%4!=0 && pts[i]==null){
							// Debug.Log("deleting i:"+i);
							DestroyImmediate( pts[i-1].gameObject );
							DestroyImmediate( pts[i-2].gameObject );
							i += 1;
							k += -2;
						}else if(k < newPts.Length){
							// Debug.Log("k:"+k+" i:"+i);
							newPts[k] = pts[i];
							//Debug.Log("transfer over:"+k);
							initNode(newPts[k],k);
							k++;
							if(pts[i])
								lastPos = pts[i].localPosition;
						}
					}
				}else{
					int lastI = 0;
					for(i=0;i<newPts.Length;i++){
						if(i<pts.Length){ // transfer old points
							newPts[i] = pts[i];
							if(pts[i])
								lastPos = pts[i].position;
							lastI = i;
						}else{ // add in a new point
							k = i;
							// Debug.Log("adding new "+k);

							Vector3 addPos = new Vector3(5f,5f,0f);
							if(i>=2){
								Vector3 one = newPts[lastI].position;
								Vector3 two = newPts[lastI-2].position;
								
								addPos = one - two;
								if(addPos.magnitude<controlSize*2f){
									addPos = addPos.normalized*controlSize*2f;
								}
							}

							if(i%4==1){
								newPts[i] = createChild(k, lastPos+addPos*0.6f);
							}else if(i%4==2){
								newPts[i] = createChild(k, lastPos+addPos*0.3f);
							}else if(i%4==3){
								newPts[i] = createChild(k, lastPos+addPos);
							}
						}
					}
				}

				pts = newPts;
			}else{ // SPLINE
				Transform[] newPts = new Transform[ count ];
				k = 0;
				for(i=0; i<pts.Length; i++){ // Loop over points to find Transforms that have been deleted
					if(pts[i]!=null){
						if(k<newPts.Length){
							newPts[k] = pts[i];
							initNode(newPts[k], k);
							k++;
						}
					}
				}
				
				k = 0;
				if(count>lastCount){ // Add in new points
					int diff =  count - lastCount;
					// Debug.Log("adding in point diff:"+diff);
					for (i=0; i<diff; i++) {
						k = pts.Length + i;
						// Debug.Log("new k:"+k+" newPts.Length-1:"+(newPts.Length-1));
						if(k <= newPts.Length-1){
							Vector3 addPos = new Vector3(1f,1f,0f);
							if(k>=2){
								Vector3 diff3 = newPts[k-1].localPosition - newPts[k-2].localPosition;
								addPos = newPts[k-1].localPosition + diff3;
							}
							newPts[k] = createChild(k,addPos);
						}
					}
				}
				pts = newPts;
			}
			lastCount = count;
		}

		reset();
	}

	private void reset(){
		if(pathType==LeanTweenPathType.bezier){
			for(i=0;i<pts.Length;i++){
				LeanTweenPathControl[] ct = new LeanTweenPathControl[2];
				if(i%4==0){
					if( i+2 < pts.Length && pts[i+2] )
						ct[0] = pts[i+2].gameObject.GetComponent<LeanTweenPathControl>();
				}else if(i%4==3){
					ct[0] = gameObject.GetComponent<LeanTweenPathControl>();
					if(i+3<pts.Length && pts[i+3])
						ct[1] = pts[i+3].gameObject.GetComponent<LeanTweenPathControl>();
				}

				if(pts[i]){
					LeanTweenPathControl pathControl = pts[i].gameObject.GetComponent<LeanTweenPathControl>();
					pathControl.init( ct, i, i%4==0||i%4==3);
				}
			}
		}else{
			for(i=0;i<pts.Length;i++){
				// LeanTweenPathControl[] ct = new LeanTweenPathControl[2];
				if(pts[i]){
					LeanTweenPathControl pathControl = pts[i].gameObject.GetComponent<LeanTweenPathControl>();
					pathControl.init( i );
				}
			}
		}
		this.count = this.lastCount = pts.Length;
	}

	public Transform createChild(int i, Vector3 pos ){
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
		go.AddComponent<LeanTweenPathControl>();
		Transform trans = go.transform;
		DestroyImmediate( go.GetComponent<BoxCollider>() );
		trans.parent = transform;
		initNode(trans, i);
		// trans.name = "pt"+i;
		trans.position = pos;
	
		return trans;
	}

	private void initNode( Transform trans, int i ){
		if(trans!=null){
			if(pathType==LeanTweenPathType.bezier){
				int iMod = i%4;
				bool isPoint = iMod==0||iMod==3;
				string type = isPoint ? "node" : "control";
				
				int ptArea = (i+2)/4+1;
				//if(i==3)
				//	ptArea = 2;
				string cntrlNum = "";

				//Debug.Log("setting i:"+i+" ptArea:"+ptArea);
				if(isPoint==false){
					//cntrlNum = iMod==2 ? "-0" : "-1";
					trans.localScale = new Vector3(0.5f,0.5f,0.25f);
				}else{
					trans.localScale = Vector3.one * 0.5f;
				}
				trans.name = /*"path"+Mathf.Floor(i/4)+"-"+t*/type+ptArea+cntrlNum;
			}else{
				trans.localScale = Vector3.one * 0.25f;
				trans.name = "node"+i;
			}
		}
	}

	void Start () {
		init();
		for(i=0; i < pts.Length; i++){
			if(pts[i])
				pts[i].gameObject.SetActive(false);
		}
	}

	public void OnDrawGizmos(){
		init();
		
		if(pathType==LeanTweenPathType.bezier){
			for(int i = 0; i < pts.Length-3; i += 4){
				if(pts[i+1] && pts[i+2] && pts[i+3]){
					Vector3 first = Vector3.zero;

					if(i>3 && pts[i-1]){
						first = pts[i-1].position;
					}else if(pts[i]){
						first = pts[i].position;
					}
					
					Gizmos.color = Color.magenta;
					LeanTween.drawBezierPath(first, pts[i+2].position, pts[i+1].position, pts[i+3].position, showArrows ? controlSize * 0.25f : 0.0f, transform);
					
					Gizmos.color = Color.white;
					Gizmos.DrawLine(first,pts[i+2].position);
					Gizmos.DrawLine(pts[i+1].position,pts[i+3].position);
				}
			}
			for(int i = 0; i < pts.Length; i++){
				int iMod = i%4;
				bool isPoint = iMod==0||iMod==3;
				if(pts[i])
					pts[i].localScale = isPoint ? Vector3.one * controlSize * 0.5f : new Vector3(1f,1f,0.5f) * controlSize * 0.5f;
			}
		}else{
			for(i=0;i<pts.Length;i++){
				if(pts[i]){
					pts[i].localScale = Vector3.one * controlSize * 0.25f;
				}
			}
			LTSpline s = new LTSpline( splineVector() );
			Gizmos.color = Color.magenta;
			s.gizmoDraw();
		}
	}

	public Vector3[] splineVector(){
		Vector3[] p = new Vector3[ pts.Length + 2 ];
		int k = 0;
		for(int i = 0; i < p.Length; i++){
			if(pts[k]){
				p[i] = pts[k].position;
				// Debug.Log("k:"+k+" i:"+i);
				if(i>=1 && i < p.Length-2){
					k++;
				}
			}
		}
		return p;
	}

	public Vector3[] splineVectorNoEndCaps(){
		Vector3[] p = new Vector3[ pts.Length ];
		for(int i = 0; i < p.Length; i++){
			p[i] = pts[i].position;
		}
		return p;
	}

	public Vector3[] vec3{
		get{
			if(pathType==LeanTweenPathType.bezier){
				Vector3[] p = new Vector3[ pts.Length ];
				// Debug.Log("p.Length:"+p.Length+" pts.Length:"+pts.Length);
				for(i=0; i < p.Length; i++){
					p[i] = i>3 && i%4==0 ? pts[i-1].position : pts[i].position;
				}
				return p;
			}else{
				return splineVector();
			}
		}
		set{

		}
	}

	void resetPath(){
		this.pts = null; // set to zero to reset
		for (int i=gameObject.transform.childCount-1; i>=0; i--) { // Destroy anything currently a child
			DestroyImmediate( gameObject.transform.GetChild(i).gameObject );
		}
	}

	public void addNode(){
		this.count += pathType==LeanTweenPathType.bezier ? 4 : 1;
	}

	public void addNodeAfter( int after ){
		if(after>=this.pts.Length-1){
			addNode();
		}else{
			bool isBezier = pathType==LeanTweenPathType.bezier;

			Vector3[] from = isBezier ? this.vec3 : splineVectorNoEndCaps();
			// Debug.Log("from:"+from);
			this.pts = null;
			// resetPath();

			int addAmt = pathType==LeanTweenPathType.bezier ? 4 : 1;
			this.path = new Vector3[ from.Length + addAmt ];
			Vector3 a = Vector3.zero;
			Vector3 b = Vector3.zero;
			Vector3 diff = Vector3.zero;

			//Debug.Log("addNodeAfter:"+after+" pathLength:"+from.Length);

			if(isBezier){
				for(int i = 0; i<this.path.Length; i++){
					if(i==after+1){
						a = from[i];
						if(from.Length>4){
							b = from[i+3];
						}else{
							b = from[3];
						}
						
						diff = b - a;
						//Debug.Log("from a:"+a+" to b:"+b+" diff:"+diff);
					}
						
					if(i>after && i<after+4){
						if(i%4==0){
							path[i] = a + diff * 0.0f;
						}else if(i%4==1){
							path[i] = a + diff * 0.33f;
						}else if(i%4==2){
							path[i] = a + diff * 0.17f;
						}else{
							//path[i] = a + diff * 0.3f; // Not used I don't think
						}
						// Debug.Log("new item:"+path[i]+" i:"+i+" k:"+k);
					}else{
						// Debug.Log("i:"+i+" k:"+k+" after:"+after+" directly after:"+(i<after+8));
						if(i<=after){
							path[i] = from[i];
						}else if(i<after+8){
							if(i%4==0){
								//path[i] = a + diff * 0.3f; // Not used I don't think
							}else if(i%4==1){
								path[i] = a + diff * 0.83f;
							}else if(i%4==2){
								path[i] = a + diff * 0.67f;
							}else{
								path[i] = a + diff * 0.5f;
							}
						}else{ // much after
							path[i] = from[i-4];
						}
					}	
				}

				init();
			}else{
				a = from[after];
				b = from[after+1];
				diff = b - a;

				for(int i = 0; i<this.path.Length; i++){
					if(i>after && i<after+1){
						path[i] = a + diff * 0.33f;
					}else{
						if(i<=after){
							path[i] = from[i];
						}else if(i<after+2){
							path[i] = a + diff * 0.66f;
						}else{ // much after
							path[i] = from[i-1];
						}
					}
				}

				init();
			}
		}
	}


	public void deleteNode( int i ){
		Transform t = this.pts[i];
		GameObject.DestroyImmediate(t.gameObject);
		// Debug.Log("before delete:"+path.count);
		this.count += pathType==LeanTweenPathType.bezier ? -4 : -1;
	}
}