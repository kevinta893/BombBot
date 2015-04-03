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


	// Use this for initialization
	void Start () 
	{
	
	}
	
	/*
	 * 	Check bomb manager for current bomb
	 */
	void Update () 
	{
		if (bm.currentBomb == -1)
			activated = false;
		else
			activated = true;
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
		
		/*switch (colour)
		{
		case 0:	// red
			//red.colors.pressedColor = new Color(255/255, 0/255, 0/255);
			//blue.colors.normalColor = new Color(152/255, 152/255, 255/255);
			//green.colors.normalColor = new Color(152/255, 255/255, 152/255);
			break;
		case 1:	// blue
			red.GetComponent<Image>().color = new Color(255/255, 152/255, 152/255);
			blue.GetComponent<Image>().color = new Color(0/255, 0/255, 255/255);
			green.GetComponent<Image>().color = new Color(152/255, 255/255, 152/255);
			break;
		case 2: // green
			red.GetComponent<Image>().color = new Color(255/255, 152/255, 152/255);
			blue.GetComponent<Image>().color = new Color(152/255, 152/255, 255/255);
			green.GetComponent<Image>().color = new Color(0/255, 255/255, 0/255);
			break;
		}*/

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
