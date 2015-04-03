﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class Countdown : MonoBehaviour {

	public Text countdownText;
	private float countdownTimer;

	private int count = 3;

	private bool done = true;

	private const int START_COUNT_NUM = 3;
	private const float COUNTDOWN_INTERVAL = 1.0f;


	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
	
		if (done == true) 
		{
			return;
		}

		countdownTimer -= Time.deltaTime;

		if (countdownTimer <= 0.0f) 
		    {
			//time's up.

			count--;

			networkView.RPC("SyncTimer", RPCMode.All, count);
		

			countdownTimer = COUNTDOWN_INTERVAL;
		}
	}


	public void PlayTimer ()
	{

		done = false;

		SetupTimer ();
	}

	//sets up timer
	private void SetupTimer()
	{
		count = START_COUNT_NUM;
		countdownText.gameObject.SetActive(true);
		
		countdownTimer = COUNTDOWN_INTERVAL;
		countdownText.text = count.ToString ();
	}



	/*
	 * Returns true if the countdown is completed 
	 * false otherwise
	 */
	public bool DoneCount(){ return done; }




	//=============================================================
	//RPC

	[RPC]
	private void SyncTimer(int time)
	{

		//do setup one time if time at max
		if (time == START_COUNT_NUM) 
		{
			SetupTimer ();
		}

		//show text always on higher numbers
		if (time >= 0) 
		{
			countdownText.gameObject.SetActive(true);
		}



		//update text
		if (time > 0)
		{
			countdownText.text = time.ToString();
		}
		else if (time == 0)
		{
			//start text
			countdownText.text = "START!";
		}
		else
		{
			//negative, hide and finish
			countdownText.gameObject.SetActive(false);
			done = true;
		}
	}


}