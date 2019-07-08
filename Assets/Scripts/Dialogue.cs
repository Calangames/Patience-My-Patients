using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour 
{
    public SpriteRenderer adultSpriteRenderer;
    public Sentence[] sentences = new Sentence[2];

    public static Dialogue instance = null;
    public static bool started = false;

    void Awake()
    {
        instance = this;
    }

    public void Begin()
    {
        StartCoroutine(DialogueCoroutine());
    }

    private IEnumerator DialogueCoroutine()
    {
        started = true;
        adultSpriteRenderer.flipX = true;
        GameController.instance.SelectedCharacter().Transitioning(true);
        foreach (Sentence s in sentences)
        {
            Image image = s.typewriter.GetComponentInChildren<Image>();
            RectTransform rectTransform = s.typewriter.GetComponent<RectTransform>();
            Vector3 newPosition = new Vector3(s.character.position.x, s.character.position.y, rectTransform.position.z) + (Vector3.up * s.yOffset);
            rectTransform.position = newPosition;
            image.enabled = true;

            s.typewriter.Write(s.sentence, true);

            while (!s.typewriter.Completed())
            {
                yield return null;
            }
            while (!Input.anyKeyDown)
            {
                yield return null;
            }
            s.typewriter.text.text = "";
            image.enabled = false;
            yield return null;
        }
        GameController.instance.SelectedCharacter().Transitioning(false);
    }

    [System.Serializable]
    public class Sentence
    {
        public Typewriter typewriter;
        public Transform character;
        public float yOffset = 2f;
        [TextArea(1, 5)]
        public string sentence;
    }
}