using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour
{
    public Button creditsButton, startButton, exitButton;
    public GameObject credits;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        //mouse deselects stuff lol
        if (Input.GetMouseButtonDown(0))
        {
            if (credits.activeSelf)
            {
                creditsButton.Select();
            }
            else
            {
                startButton.Select();
            }
        }
    }

    public void CreditsButton()
    {
        SwapNavigation(creditsButton);
        credits.SetActive(!credits.activeSelf);
    }

    public void StartButton()
    {
        SoundController.instance.Crossfade(SoundController.instance.menu, SoundController.instance.game);
        SceneManager.LoadScene(1);
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    private void SwapNavigation(Button button)
    {
        Navigation nav = button.navigation;
        nav.mode = nav.mode == Navigation.Mode.Automatic ? Navigation.Mode.None : Navigation.Mode.Automatic;
        creditsButton.navigation = nav;
    }
}
