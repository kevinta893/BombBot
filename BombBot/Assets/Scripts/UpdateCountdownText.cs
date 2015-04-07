using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UpdateCountdownText : MonoBehaviour {

	public Text countdownText;
	public Sprite explosion;		// a minimum-wage-working-man's explosion
	
	private Image img;

	public void UpdateTimer(int timer)
	{
		countdownText.text = timer.ToString();
		img = gameObject.GetComponentInChildren<Image>();
	}
	
	// change sprite to explosion
	public void Explode ()
	{
		img.sprite = explosion;
	}
}
