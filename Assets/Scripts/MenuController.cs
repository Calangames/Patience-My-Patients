using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour
{
    [Range(0.1f, 1f)]
    public float fadeDuration = 0.5f;
    [Range(0.1f, 4f)]
    public float whiteFadeDuration = 2.5f;
    public Button creditsButton, startButton, exitButton;
    public GameObject credits;
    public Image fade;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (FadeController.instance.Black())
        {
            StartCoroutine(FadeFromBlack());
        }
        else if (FadeController.instance.White())
        {
            StartCoroutine(FadeFromWhite());
        }
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
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            exitButton.Select();
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
        StartCoroutine(FadeToBlackAndStart());
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    private void SwapNavigation(Button button)
    {
        Navigation nav = button.navigation;
        nav.mode = nav.mode == Navigation.Mode.Automatic ? Navigation.Mode.None : Navigation.Mode.Automatic;
        button.navigation = nav;
    }

    private IEnumerator FadeFromBlack()
    {
        EventSystem.current.sendNavigationEvents = false;
        SwapNavigation(startButton);
        SwapNavigation(exitButton);
        SwapNavigation(creditsButton);
        fade.color = Color.black;
        yield return null;
        float alpha = 1f;
        while (alpha > 0f)
        {
            alpha -= (Time.deltaTime / fadeDuration);
            fade.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }
        FadeController.instance.Black(false);
        SwapNavigation(startButton);
        SwapNavigation(exitButton);
        SwapNavigation(creditsButton);
        EventSystem.current.sendNavigationEvents = true;
    }

    private IEnumerator FadeFromWhite()
    {
        EventSystem.current.sendNavigationEvents = false;
        SwapNavigation(startButton);
        SwapNavigation(exitButton);
        SwapNavigation(creditsButton);
        fade.color = Color.white;
        yield return null;
        float alpha = 1f;
        while (alpha > 0f)
        {
            alpha -= (Time.deltaTime / whiteFadeDuration);
            fade.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }
        FadeController.instance.White(false);
        SwapNavigation(startButton);
        SwapNavigation(exitButton);
        SwapNavigation(creditsButton);
        EventSystem.current.sendNavigationEvents = true;
    }

    private IEnumerator FadeToBlackAndStart()
    {
        EventSystem.current.sendNavigationEvents = false;
        SwapNavigation(startButton);
        SwapNavigation(exitButton);
        SwapNavigation(creditsButton);
        fade.color = new Color(0f, 0f, 0f, 0f);
        yield return null;
        float alpha = 0f;
        while (alpha < 1f)
        {
            alpha += (Time.deltaTime / fadeDuration);
            fade.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }
        FadeController.instance.Black(true);
        SceneManager.LoadScene(1);
    }
}