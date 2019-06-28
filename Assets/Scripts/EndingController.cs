﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingController : MonoBehaviour
{
    [TextArea(1, 10)]
    public string goodEnding, averageEnding, badEnding;

    public static EndingController instance = null;

    private List<Character> arrivedCharacters = new List<Character>();
    private Animator _animator;
    private Typewriter _typewriter;

    void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start ()
    {
        _animator = GetComponent<Animator>();
        _typewriter = GetComponent<Typewriter>();

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

    public void End()
    {
        _animator.SetTrigger("Start Ending");
    }

    public void WriteEnding()
    {
        int numberOfSurvivers = GameController.instance.teamCharacters.Count;
        if (numberOfSurvivers >= 5)
        {
            _typewriter.Write(goodEnding);
        }
        else if (numberOfSurvivers > 1)
        {
            _typewriter.Write(averageEnding);
        }
        else
        {
            _typewriter.Write(badEnding);
        }
        StartCoroutine(WaitUntilCompleted());
    }

    public void GoToMenu()
    {
        GameController.instance.GoToMenu();
    }

    private IEnumerator WaitUntilCompleted()
    {
        while (!_typewriter.Completed())
        {
            yield return null;
        }
        _animator.SetTrigger("Continue Ending");
    }
}
