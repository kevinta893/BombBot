using UnityEngine;
using UnityEngine.UI;
using System.Collections;



/*
 * Note this class must be enabled on both ends because the networkview
 * needs to send calls to both objects with the same network view ID
 * Otherwise the RPC calls from other network views will be caught here.
 * 
 * Ensure Network IDs are the same in the components
 */
public class CountdownUI : MonoBehaviour {

	public Text countdownText;
	private float countdownTimer;

	private int count = 3;

	private bool done = true;

	private const int START_COUNT_NUM = 3;
	private const float COUNTDOWN_INTERVAL = 1.0f;

	private CountDownFinishCallback callbackFinish;


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

			GetComponent<NetworkView>().RPC("SyncTimer", RPCMode.All, count);
		

			countdownTimer = COUNTDOWN_INTERVAL;
		}
	}


	public void PlayTimer ()
	{
		callbackFinish = null;
		SetupTimer ();
	}


	/*
	 * Plays the timer and calls back on the method
	 * Does not play callback on clients. Only the caller that
	 * plays the countdown will have its callback played.
	 */
	public void PlayTimer(CountDownFinishCallback callback)		//only for the caller, do not apply to clients
	{
		callbackFinish = callback;
		SetupTimer ();
	}


	private void SetupTimer()
	{
		done = false;
		
		count = START_COUNT_NUM + 1;			//1 second delay before starting
		
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



		//show text always on higher numbers
		if ((time >= 0) && (time <= START_COUNT_NUM)) {
			countdownText.gameObject.SetActive (true);
		} else {
			countdownText.gameObject.SetActive (false);
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

			if (callbackFinish != null)
			{
				callbackFinish.CountDownFinishedCallback();
			}
		}
	}

	public interface CountDownFinishCallback
	{
		void CountDownFinishedCallback();
	}
}


