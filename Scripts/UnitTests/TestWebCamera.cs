using UnityEngine;
using System.Collections;
using IBM.Watson.DeveloperCloud.Logging;
using UnityEngine.UI;

public class TestWebCamera : MonoBehaviour
{
    private WebCamDevice[] m_WebCams;
    private int WebCamIndex = 0;

    [SerializeField]
    private GameObject m_Screen;
    private WebCamTexture m_WebCamTexture;
    public RawImage rawimage;
    public Material material3D;
    public Button SwitchCameraButton;
    public Button DetectButton;

    void Awake()
    {
        LogSystem.InstallDefaultReactors();
    }

    void Start()
    {
        Application.RequestUserAuthorization(UserAuthorization.WebCam);

        m_WebCamTexture = new WebCamTexture(800, 600, 60);
        m_WebCams = WebCamTexture.devices;

        foreach (WebCamDevice cam in m_WebCams)
            Log.Debug("TestWebCamera", "cam: {0}", cam);


        if (rawimage != null)
        {
			rawimage.texture = m_WebCamTexture;
			rawimage.material.mainTexture = m_WebCamTexture;
		}

		if (material3D != null)
            material3D.mainTexture = m_WebCamTexture;

        m_WebCamTexture.Play();
    }

    public void OnDetectButtonClicked()
    {

    }

    public void OnSwitchCameraButtonClicked()
    {
        if (m_WebCams.Length > 1)
        {
            m_WebCamTexture.Stop();
            m_WebCamTexture.deviceName = (m_WebCamTexture.deviceName == m_WebCams[0].name) ? m_WebCams[1].name : m_WebCams[0].name;
            
            m_WebCamTexture.Play();
        }
        else
        {
            Log.Warning("TestWebCamera", "There is only one camera!");
        }
    }
}
