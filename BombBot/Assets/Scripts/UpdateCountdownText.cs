using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UpdateCountdownText : MonoBehaviour {

	public Text countdownText;

	public void UpdateTimer(int timer)
	{
		countdownText.text = timer.ToString();
	}
}
