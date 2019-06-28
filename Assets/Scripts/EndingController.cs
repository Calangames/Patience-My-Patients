using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingController : MonoBehaviour
{
    public static EndingController instance = null;

    private List<Character> arrivedCharacters = new List<Character>();

    void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void ActivateCharactersEnding()
    {
        foreach (Character character in arrivedCharacters)
        {
            character.endingImage.enabled = true;
        }
    }

    public void AddCharacterToEnding(Character character)
    {
        arrivedCharacters.Add(character);
    }

    public void RemoveCharacterFromEnding(int characterId)
    {
        foreach (Character character in arrivedCharacters)
        {
            if (character.GetInstanceID() == characterId)
            {
                arrivedCharacters.Remove(character);
                return;
            }
        }
    }
}
