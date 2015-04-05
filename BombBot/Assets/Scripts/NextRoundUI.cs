using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NextRoundUI : MonoBehaviour {

	public GameObject nextRoundPanel;
	public Text completedRoundText;

	public Button nextRoundButton;
	public Text nextRoundButtonText;
	
	public bool useButton;
	public bool startShown;				//shows the next round as "start" on startup

	private bool show = false;

	private float SHOW_ALPHA_DELTA = 0.02f;
	private float HIDE_ALPHA_DELTA= 0.05f;

	private float MAX_ALPHA_PANEL = 0.8f;
	private float currentAlpha = 0.0f;


	// Use this for initialization
	void Start () {


		if (startShown == true) {
			ShowPanel (0);
			SetAlpha (1.0f);
		} else {
			SetAlpha (0.0f);
			HidePanel ();
		}


	}
	
	// Update is called once per frame
	void Update () {
	

		
		currentAlpha = currentAlpha + (show == true ? SHOW_ALPHA_DELTA : -1 * HIDE_ALPHA_DELTA);

		currentAlpha = (currentAlpha <= 0.0f) ? 0.0f : currentAlpha;
		currentAlpha = (currentAlpha >= 1.0f) ? 1.0f : currentAlpha;

		SetAlpha (currentAlpha);

		SetButtonActive(show);
	}

	private void SetAlpha(float alpha){

		Color current = nextRoundPanel.GetComponent<Image> ().color;
		nextRoundPanel.GetComponent<Image> ().color = new Color (current.r, current.g, current.b, Mathf.Min (alpha, MAX_ALPHA_PANEL));

		Color currentText = completedRoundText.GetComponent<Text> ().color;
		completedRoundText.GetComponent<Text> ().color = new Color (currentText.r, currentText.g, currentText.b, alpha);

		Color currentButton = nextRoundButton.GetComponent<Image> ().color;
		nextRoundButton.GetComponent<Image> ().color = new Color (currentButton.r, currentButton.g, currentButton.b, alpha);
	}





	/*
	 * Animate shows the panel
	 */
	public void ShowPanel(int levelComplete)
	{
		nextRoundPanel.SetActive (true);

		if (levelComplete == 0) {
			completedRoundText.text = "Ready to play?";
		} else {
			completedRoundText.text = "Round " + levelComplete + " Complete!";
		}

		nextRoundButtonText.text = "Start Round " + (levelComplete + 1);
		SetButtonActive(true);
		show = true;
	}




	/*
	 * Animate hides the panel
	 */
	public void HidePanel()
	{
		SetButtonActive(false);
		show = false;
	}



	private void SetButtonActive(bool val)
	{
		nextRoundButton.gameObject.SetActive (val && useButton);
	}
}
