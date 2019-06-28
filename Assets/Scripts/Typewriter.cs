using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class Typewriter : MonoBehaviour 
{
	public float typingSpeed = 40f;

    public Text text;

	private char[] charArray;
	private WaitForSeconds delay;
	private IEnumerator coroutine;
	private bool completedTyping = false;

	// Use this for initialization
	void Start () 
	{
		delay = new WaitForSeconds ( typingSpeed == 0f ? 0f: 1f/typingSpeed);
	}

	public bool Completed()
	{
		return completedTyping;
	}

	public void Write(string ending)
	{
		completedTyping = false;
		coroutine = TypeText (ending);
		StartCoroutine (coroutine);
	}

	private IEnumerator TypeText(string ending)
	{
		charArray = ending.ToCharArray ();
		text.text = "";

		foreach (char letter in charArray)
        {
			yield return delay;
            text.text += letter;
        }
		completedTyping = true;
	}
}