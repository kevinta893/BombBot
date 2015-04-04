using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DefusePanel : MonoBehaviour {


	private static float PANEL_PERCENT_HEIGHT = 0.2f;		//percent height cover from bottom
	public Canvas canvas;
	public ClientNetwork client;

	// Use this for initialization
	void Start () {
		RectTransform rectTransform = this.GetComponent<RectTransform> ();



		rectTransform.offsetMax = new Vector2 (rectTransform.offsetMax.x, -1.0f * Screen.height * canvas.scaleFactor * (1.0f - PANEL_PERCENT_HEIGHT));

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
	
	/*
	*	Attempt this solution to defuse current bomb
	*/
	public void DefuseButton(int solutionAttempt) {
		client.AttemptDefuse(solutionAttempt);
	}
}
