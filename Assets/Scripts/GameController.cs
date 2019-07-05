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
    void Start()
    {
        if (IntroController.play)
        {
            StartCoroutine(WaitForIntro());
        }
        else if (FadeController.instance.Black())
        {
            StartCoroutine(StartFade());
        }
        else
        {
            selectedCharacter = teamCharacters[selectedIndex];
            selectedCharacter.Selected(true);
            selectedCharacter.MainCharacter(true);
            selectedCharacter.AddedToList(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedCharacter && selectedCharacter.IsOnTheGround())
        {
            if (Input.GetKeyDown(KeyCode.X))
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
            else if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                selectedIndex--;
                if (selectedIndex < 0)
                {
                    selectedIndex = teamCharacters.Count - 1;
                }
                selectedCharacter.Selected(false);
                selectedCharacter = teamCharacters[selectedIndex];
                selectedCharacter.Selected(true);
            }
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

    public void GameOver()
    {
        SoundController.instance.Crossfade(SoundController.instance.game, SoundController.instance.menu);
        selectedCharacter = null;
        SoundController.instance.gameOver.pitch = Random.Range(0.99f, 1.01f);
        SoundController.instance.gameOver.Play();
        FadeController.instance.Black(true);
        StartCoroutine(GameoverFade());
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(0);
    }

    private IEnumerator StartFade()
    {
        EndingController.instance.gameoverFade.color = Color.black;
        for (int i = 5; i > 0; i--)
        {
            yield return null;
        }
        float alpha = 1f;
        while (alpha > 0f)
        {
            alpha -= (Time.deltaTime / EndingController.instance.nonEndingFadeDuration);
            EndingController.instance.gameoverFade.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }
        FadeController.instance.Black(false);
        selectedCharacter = teamCharacters[selectedIndex];
        selectedCharacter.Selected(true);
        selectedCharacter.MainCharacter(true);
        selectedCharacter.AddedToList(true);
    }

    private IEnumerator WaitForIntro()
    {
        while (!IntroController.instance.Completed())
        {
            yield return null;
        }
        selectedCharacter = teamCharacters[selectedIndex];
        selectedCharacter.Selected(true);
        selectedCharacter.MainCharacter(true);
        selectedCharacter.AddedToList(true);
    }

    private IEnumerator GameoverFade()
    {
        float alpha = 0f;
        while (alpha < 1f)
        {
            alpha += (Time.deltaTime / EndingController.instance.nonEndingFadeDuration);
            EndingController.instance.gameoverFade.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }
        Invoke("GoToMenu", 0.2f);
    }
}

[System.Serializable]
public class Puzzle
{
    public Lever lever;
    public Obstacle[] obstacles;
}