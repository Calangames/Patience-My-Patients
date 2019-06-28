using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public Sprite handLeverUp, handLeverDown, floorButtonUp, floorButtonDown;
    public List<Puzzle> puzzles = new List<Puzzle>();
    public List<Character> teamCharacters = new List<Character>(new Character[1]);

    public static GameController instance = null;

    private Character selectedCharacter = null;
    private int selectedIndex = 0;

    public Character SelectedCharacter()
    {
        return selectedCharacter;
    }

    void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start ()
    {
        selectedCharacter = teamCharacters[selectedIndex];
        selectedCharacter.Selected(true);
        selectedCharacter.AddedToList(true);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && selectedCharacter.IsOnTheGround())
        {
            selectedIndex++;
            if (selectedIndex >= teamCharacters.Count)
            {
                selectedIndex = 0;
            }
            selectedCharacter.Selected(false);
            selectedCharacter = teamCharacters[selectedIndex];
            selectedCharacter.Selected(true);
        }
	}

    public void AddCharacterToList(Character newCharacter)
    {
        teamCharacters.Add(newCharacter);
    }

    public void RemoveCharacterFromList()
    {
        teamCharacters.RemoveAt(selectedIndex);
        if (selectedIndex >= teamCharacters.Count)
        {
            selectedIndex = 0;
        }
        if (teamCharacters.Count <= 0)
        {
            selectedCharacter = null;
            Invoke("GameOver", 0.2f);
        }
        else
        {
            selectedCharacter = teamCharacters[selectedIndex];
            selectedCharacter.Selected(true);
        }
    }

    public void RemoveCharacterFromList(int index)
    {
        teamCharacters.RemoveAt(index);
        if (teamCharacters.Count <= 0)
        {
            selectedCharacter = null;
            SoundController.instance.gameOver.pitch = Random.Range(0.99f, 1.01f);
            SoundController.instance.gameOver.Play();
            Invoke("GameOver", 0.2f);
            return;
        }
        else
        {
            SoundController.instance.death.pitch = Random.Range(0.95f, 1.05f);
            SoundController.instance.death.Play();
        }
        if (index == selectedIndex)
        {
            if (selectedIndex >= teamCharacters.Count)
            {
                selectedIndex = 0;
            }
            selectedCharacter = teamCharacters[selectedIndex];
            selectedCharacter.Selected(true);
        }
        else if (selectedIndex > index)
        {
            selectedIndex--;
        }
        for (int i = 0; i < teamCharacters.Count; i++)
        {
            teamCharacters[i].Index(i);
        }
    }

    public void ActivateLever(int leverId)
    {
        foreach (Puzzle puzzle in puzzles)
        {
            if (puzzle.lever.GetInstanceID() == leverId)
            {
                puzzle.lever.SwapState();
                foreach (Obstacle obstacle in puzzle.obstacles)
                {
                    obstacle.SwapState();
                }
                return;
            }
        }
    }

    public void ActivateButton(int buttonId)
    {
        foreach (Puzzle puzzle in puzzles)
        {
            if (puzzle.lever.GetInstanceID() == buttonId)
            {
                foreach (Obstacle obstacle in puzzle.obstacles)
                {
                    obstacle.SwapState();
                }
                return;
            }
        }
    }

    private void GameOver()
    {
        SceneManager.LoadScene(0);
    }
}

[System.Serializable]
public class Puzzle
{
    public Lever lever;
    public Obstacle[] obstacles;
}