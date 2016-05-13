using UnityEngine;
using IBM.Watson.DeveloperCloud.Services.Dialog.v1;

public class ExampleDIalog : MonoBehaviour
{
	private Dialog m_Dialog = new Dialog();
	private string m_DialogID = "a4015960-39c2-4d6b-9571-38c74aecfffd";

	void Start ()
	{
		Debug.Log("User: 'Hello'");
		m_Dialog.Converse(m_DialogID, "Hello", OnConverse);
	}

	private void OnConverse(ConverseResponse resp)
	{
		foreach (string r in resp.response)
			Debug.Log("Watson: " + r);
	}
}
