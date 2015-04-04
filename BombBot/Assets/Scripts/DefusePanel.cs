using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DefusePanel : MonoBehaviour {


	private static int PANEL_HEIGHT = 50;
	public Canvas canvas;
	// Use this for initialization
	void Start () {
		RectTransform rectTransform = this.GetComponent<RectTransform> ();



		rectTransform.offsetMax = new Vector2 (rectTransform.offsetMax.x, -1*(Screen.height - PANEL_HEIGHT) * canvas.scaleFactor);

		//this.transform.position.Set (this.transform.position.x, START_Y - 1000, this.transform.position.z);
		this.gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void HideDefusePanel(){
		Debug.Log ("HIDING");
		//this.transform.position.Set (this.transform.position.x, START_Y - 1000, this.transform.position.z);
		this.gameObject.SetActive (false);
	}

	public void ShowDefusePanel(){
		Debug.Log ("SHowing");
		//this.transform.position.Set (this.transform.position.x, START_Y + 1000, this.transform.position.z);
		this.gameObject.SetActive (true);
	}
}
