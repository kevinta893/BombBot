using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WaitingPlayerText : MonoBehaviour {


	private const float UPDATE_INTERVAL = 1.0f;
	private float updateTimer;
	private bool running = false;


	private const string WAITING_FOR_PLAYER_TEXT = "Waiting for player";
	private string[] animatedText = {"   ", ".  ", ".. ", "..."};
	private int animateCursor = 0;


	public Text waitingText;
	public Text ipSubText;


	// Use this for initialization
	void Start () {
		StartText ();

	}
	
	// Update is called once per frame
	void Update () {


		if (running == false) 
		{
			return;
		}
		
		updateTimer -= Time.deltaTime;
		
		if (updateTimer <= 0.0f) 
		{
			waitingText.text = WAITING_FOR_PLAYER_TEXT + animatedText[animateCursor];
			animateCursor = (animateCursor + 1) % animatedText.Length;

			updateTimer = UPDATE_INTERVAL;
		}
	}

	public void StartText()
	{
		animateCursor = 0;
		ipSubText.text = "Server IP: " + Network.player.ipAddress;
		this.gameObject.SetActive (true);
		running = true;
	}

	public void StopText()
	{
		this.gameObject.SetActive (false);
	}
}
