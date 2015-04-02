using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Countdown : MonoBehaviour {

	public Text countdownText;
	private float countdownTimer;

	private int count = 3;

	private bool done = false;

	private const int START_COUNT_NUM = 3;
	private const float COUNTDOWN_INTERVAL = 1.0f;


	// Use this for initialization
	void Start () {
		playTimer ();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
		if (done == true) {
			return;
		}

		countdownTimer -= Time.deltaTime;

		if (countdownTimer <= 0.0f) 
		    {
			//time's up.

			count--;

			if (count == 0)
			{
				//start text
				countdownText.text = "START!";
			}
			else if (count > 0)
			{
				countdownText.text = count.ToString();
			}
			else
			{
				//negative, hide and finish
				countdownText.gameObject.SetActive(false);
				done = true;
			}

			countdownTimer = COUNTDOWN_INTERVAL;
		}
	}


	public void playTimer (){

		done = false;

		count = START_COUNT_NUM;
		countdownText.gameObject.SetActive(true);
		
		countdownTimer = COUNTDOWN_INTERVAL;
		countdownText.text = count.ToString ();
	}

	public bool doneCount(){
		return done;
	}

}
