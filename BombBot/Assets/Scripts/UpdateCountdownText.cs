using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UpdateCountdownText : MonoBehaviour {

	public Text countdownText;
	public Sprite explosion;		// a minimum-wage-working-man's explosion
	public Sprite correct;
	private Image img;

	public void UpdateTimer(int timer)
	{
		countdownText.text = timer.ToString();
		img = gameObject.GetComponentInChildren<Image>();
	}
	
	// change sprite to explosion
	public void Explode ()
	{
		countdownText.text = "";
		img.sprite = explosion;
	}

	public void Correct()
	{
		countdownText.text = "";
		img.sprite = correct;
	}
}
