using UnityEngine;
using System.Collections;

public class ServerMenu : MonoBehaviour {

	public string IP = "192.168.1.67";
	public int Port = 59981;

	public GameObject camera;

	private GUIStyle myStyle;

	void Start(){

	}

	void OnGUI()
	{
		myStyle = new GUIStyle (GUI.skin.textArea);
		myStyle.fontSize = 40;

		if(Network.peerType == NetworkPeerType.Disconnected)
		{

			if (IP.Equals("") == true){
				IP = GUI.TextArea(new Rect(1000,75,300,100), "192.168.1.67", myStyle);
			}
			else{
				IP = GUI.TextArea(new Rect(1000,75,300,100), IP, myStyle);
			}

			if(GUI.Button(new Rect(1000, 200,300,100),"Connect to Expert", myStyle))
			{
				Network.Connect(IP, Port);

			}

		}
		else {
			if(Network.peerType == NetworkPeerType.Client)
			{
				if(GUI.Button(new Rect(1000, 200,300,100),"Logout"))
				{
					Network.Disconnect(250);	
				}
			}
		}
	}

	public void SendCameraData (Quaternion rotation) {
		networkView.RPC ("UpdateCamera", RPCMode.Server, rotation);
	}

	[RPC]
	void UpdateCamera(Quaternion rotation)
	{
		camera.transform.rotation = rotation;
	}


}
