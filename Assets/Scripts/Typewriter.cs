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
	private bool completedTyping = true, skiped = false;

	// Use this for initialization
	void Start () 
	{
		delay = new WaitForSeconds ( typingSpeed == 0f ? 0f: 1f/typingSpeed);
	}

    void Update()
    {
        if (!completedTyping && !skiped)
        {
            skiped = Input.anyKeyDown;
        }
    }

    public bool Completed()
	{
		return completedTyping;
	}

	public void Write(string fullText, bool canSkip = false)
	{
		completedTyping = false;
		coroutine = TypeText (fullText, canSkip);
		StartCoroutine (coroutine);
	}

	private IEnumerator TypeText(string fullText, bool canSkip)
	{
		charArray = fullText.ToCharArray ();
		text.text = "";

		foreach (char letter in charArray)
        {
            if (canSkip && skiped)
            {
                text.text = fullText;
                completedTyping = true;
                skiped = false;
                StopCoroutine(coroutine);
            }
            else
            {
                yield return delay;
                text.text += letter;
            }
        }
        completedTyping = true;
    }
}