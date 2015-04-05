using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NextRoundPanel : MonoBehaviour {


	public Text completedRoundText;
	public Button nextRoundButton;
	public Text nextRoundButtonText;
	

	private bool show = false;

	private float SHOW_ALPHA_DELTA = 0.02f;
	private float HIDE_ALPHA_DELTA= 0.05f;

	private float MAX_ALPHA_PANEL = 0.8f;

	// Use this for initialization
	void Start () {
		SetAlpha (0.0f);
		HidePanel ();
	}
	
	// Update is called once per frame
	void Update () {
	


		Color current = this.GetComponent<Image> ().color;
		float alpha = current.a + (show == true ? SHOW_ALPHA_DELTA : -1 * HIDE_ALPHA_DELTA);

		alpha = (alpha <= 0.0f) ? 0.0f : alpha;
		alpha = (alpha >= 1.0f) ? 1.0f : alpha;

		SetAlpha (alpha);
		

		nextRoundButton.gameObject.SetActive (show);
	}

	private void SetAlpha(float alpha){

		Color current = this.GetComponent<Image> ().color;
		this.GetComponent<Image> ().color = new Color (current.r, current.g, current.b, Mathf.Min (alpha, MAX_ALPHA_PANEL));

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
		if (levelComplete == 0) {
			completedRoundText.text = "Ready to play?";
		} else {
			completedRoundText.text = "Round " + levelComplete + " Complete!";
		}

		nextRoundButtonText.text = "Start Round " + (levelComplete + 1);
		nextRoundButton.gameObject.SetActive (true);
		show = true;
	}


	/*
	 * Animate hides the panel
	 */
	public void HidePanel()
	{
		show = false;
		nextRoundButton.gameObject.SetActive (false);
	}


}
