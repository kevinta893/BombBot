using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DefusePanel : MonoBehaviour {


	private static float PANEL_PERCENT_HEIGHT = 0.2f;		//percent height cover from bottom
	public Canvas canvas;
	public ClientNetwork client;
	public Button[] defuseButtons;
	// Use this for initialization
	void Start () {

		ResizeUI ();

		//this.transform.position.Set (this.transform.position.x, START_Y - 1000, this.transform.position.z);
		this.gameObject.SetActive (false);
	}


	// Update is called once per frame
	void Update () {
		
	}

	private const float LEFT_PADDING = 5.0f;
	private const float RIGHT_PADDING = 5.0f;
	private const float BETWEEN_PADDING = 5.0f;

	private const float BUTTON_HEIGHT_FILL_PERCENT = 0.78f;

	private void ResizeUI(){
		RectTransform panelTransform = this.GetComponent<RectTransform> ();
		panelTransform.offsetMax = new Vector2 (panelTransform.offsetMax.x, -1.0f * Screen.height * canvas.scaleFactor * (1.0f - PANEL_PERCENT_HEIGHT));

		float buttonWidth = ((panelTransform.rect.width - LEFT_PADDING - RIGHT_PADDING) - (BETWEEN_PADDING * defuseButtons.Length)) / defuseButtons.Length;
		buttonWidth *= canvas.scaleFactor;


		for (int i  =0; i < defuseButtons.Length; i++) {
			RectTransform buttonTrans = defuseButtons[i].transform.GetComponent<RectTransform>();
			buttonTrans.sizeDelta = new Vector2 (buttonWidth, panelTransform.rect.height * canvas.scaleFactor * BUTTON_HEIGHT_FILL_PERCENT);

			Vector2 buttonPos = new Vector2 (LEFT_PADDING + (i * buttonWidth) + (i * BETWEEN_PADDING) + (buttonTrans.sizeDelta.x/2.0f), panelTransform.rect.height/2.0f);
			buttonPos *= canvas.scaleFactor;
			buttonTrans.position = buttonPos;
		}

	}




	public void HideDefusePanel(){
		//Debug.Log ("HIDING");
		//this.transform.position.Set (this.transform.position.x, START_Y - 1000, this.transform.position.z);
		this.gameObject.SetActive (false);
	}

	public void ShowDefusePanel(){
		//Debug.Log ("SHowing");
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
