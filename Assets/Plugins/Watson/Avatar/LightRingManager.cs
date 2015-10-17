using UnityEngine;
using System.Collections;

public class LightRingManager : MonoBehaviour {

	public LeanTweenType lightFlareEase;
	public float lightFlareAnimationTime = 1.0f;
	public GameObject[] lightFlareList;
	public Vector3[][] listFlarePathList;

	public Color colorTest;
	
	// Use this for initialization
	void Start () {
		listFlarePathList = new Vector3[3][];

		listFlarePathList [0] = new Vector3[] {
			new Vector3 (28.365f, -6.06f, -16.385f),
			new Vector3 (22.58f, -6.06f, 32.78f),
			new Vector3 (41.7f, -6.06f, 8.4f),
			new Vector3 (0.062f, -6.06f, 32.764f)
		};

		listFlarePathList [1] = new Vector3[] {
			new Vector3 (-28.37233f, -6.06f, -16.3723f),
			new Vector3 (17.0983f, -6.06f, -35.94485f),
			new Vector3 (-13.57539f, -6.06f, -40.31325f),
			new Vector3 (28.34345f, -6.06f, -16.4357f)
		};

		listFlarePathList [2] = new Vector3[] {
			new Vector3 (0.007372856f, -6.060182f, 32.75727f),
			new Vector3 (-39.67829f, -6.060145f, 3.164852f),
			new Vector3 (-28.12456f, -6.060268f, 31.91325f),
			new Vector3 (-28.40545f, -6.06f, -16.32832f)
		};

		ChangeToColor (colorTest);
		AnimateLightFlare ();
	}

	public void ChangeToColor(Color color){
		LeanTween.color (gameObject, color, 1.0f).setLoopPingPong();
	}

	public void AnimateLightFlare(){
		if (lightFlareList.Length == listFlarePathList.Length) {
			for (int i = 0; i < lightFlareList.Length; i++) {
				LeanTween.cancel(lightFlareList[i]);	
				LeanTween.moveLocal (lightFlareList[i], listFlarePathList [i], lightFlareAnimationTime).setOrientToPath(true).setAxis(Vector3.forward).setEase(lightFlareEase).setLoopPingPong ();
			}
		}
	}

	public void StopAnimationLightFlare(){
		for (int i = 0; i < lightFlareList.Length; i++) {
			LeanTween.cancel(lightFlareList[i]);
		}
	}

#if UNITY_EDITOR
	private LeanTweenType m_lightFlareEase;
	private float m_lightFlareAnimationTime = 1.0f;
	private Color m_colorTest;
	// Update is called once per frame
	void Update () {
		if (isThereAnyChange) {
			MakeCurrentStateDefault();
			ChangeToColor(colorTest);
			AnimateLightFlare();
		}
		
	}

	bool isThereAnyChange{
		get{
			return (m_lightFlareEase != lightFlareEase) || (m_lightFlareAnimationTime != lightFlareAnimationTime) || (m_colorTest != colorTest);
		}
	}

	void MakeCurrentStateDefault(){
		m_lightFlareEase = lightFlareEase;
		m_lightFlareAnimationTime = lightFlareAnimationTime;
		m_colorTest = colorTest;
	}

#endif
}
