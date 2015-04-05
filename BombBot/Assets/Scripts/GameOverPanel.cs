using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameOverPanel : MonoBehaviour {

	public Text gameOverText;
	public Text finalScoreText;

	private bool showing = false;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	/*
	 * Shows gameover without final score
	 */
	public void ShowPanel(){
		if (showing == true) {
			//already shown, do not do anything
			return;
		}



		this.gameObject.SetActive (true);

		finalScoreText.gameObject.SetActive (false);

		showing = true;
	}


	/*
	 * Shows gameover with final score
	 */
	public void ShowPanel(int finalScore){
		if (showing == true) {
			//already shown, do not do anything
			return;
		}
		
		
		this.gameObject.SetActive (true);

		if (finalScore < 0) {
			finalScoreText.gameObject.SetActive (false);
		} else {
			finalScoreText.text = "Final Score: " + finalScore;
		}

		showing = true;
	}


	/*
	 * Shows connection lost without final score
	 */
	public void ShowPanelConnectionLost(){
		if (showing == true) {
			//already shown, do not do anything
			return;
		}
		this.gameObject.SetActive (true);
		
		gameOverText.text = "Connection lost!";
		finalScoreText.gameObject.SetActive (false);

		showing = true;
	}


	/*
	 * Shows connection lost with final score
	 */
	public void ShowPanelConnectionLost(int finalScore){
		if (showing == true) {
			//already shown, do not do anything
			return;
		}

		this.gameObject.SetActive (true);

		gameOverText.text = "Connection lost!";

		if (finalScore < 0) {
			finalScoreText.gameObject.SetActive (false);
		} else {
			finalScoreText.text = "Final Score: " + finalScore;
		}

		showing = true;
	}


	public void QuitToMainMenu(){
		Application.LoadLevel ("MainMenu");
	}

}
