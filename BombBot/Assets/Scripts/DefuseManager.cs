using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DefuseManager : MonoBehaviour {

	public Button red;
	public Button blue;
	public Button green;
	public Button square;
	public Button circle;
	public Button triangle;
	public Text solution;
	public BombManager bm;

	private int colourAttempt;
	private int shapeAttempt;

	private bool activated;


	void Start () 
	{
	
	}
	
	/*
	 * 	Check bomb manager for current bomb
	 */
	void Update () 
	{
		if (bm.currentBomb == -1) {
			ToggleDefuseBox(false);
			colourAttempt = -1;
			shapeAttempt = -1;
		}
		else
			ToggleDefuseBox(true);
	}


	private void ToggleDefuseBox (bool active)
	{
		if (active) 
		{
			red.interactable = true;
			blue.interactable = true;
			green.interactable = true;
			square.interactable = true;
			circle.interactable = true;
			triangle.interactable = true;
		}
		else
		{
			red.interactable = false;
			blue.interactable = false;
			green.interactable = false;
			square.interactable = false;
			circle.interactable = false;
			triangle.interactable = false;
		}
	}


	//**************************************
	//**********  Button events ************
	//**************************************

	/*
	 * 	Button click event for colour
	 *  0 = red, 1 = blue, 2 = green
	 */
	public void ColourClicked (int colour) 
	{
		colourAttempt = colour;
	}

	/*
	 * 	Button click event for shape
	 *  0 = square, 1 = circle, 2 = tri
	 */
	public void ShapeClicked (int shape) 
	{
		shapeAttempt = shape;
	}
}
