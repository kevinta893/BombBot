using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameOverPanel : MonoBehaviour {

	public Text gameOverText;
	public Text finalScoreText;
	public Text subText;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ShowPanel(int finalScore){
		this.gameObject.SetActive (true);

		finalScoreText.text = "Final Score: " + finalScore;

	}

	public void ShowPanelConnectionLost(int finalScore){
		this.gameObject.SetActive (true);

		gameOverText.text = "Connection lost!";
		finalScoreText.text = "Final Score: " + finalScore;
	}
}
