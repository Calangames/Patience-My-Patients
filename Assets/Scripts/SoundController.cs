using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundController : MonoBehaviour
{
    [Range(0.1f, 5f)]
    public float crossfadeDuration = 1f;
    public AudioSource menu, game, ending, steps, jump, interact, death, gameOver;

    public static SoundController instance = null;

    private bool running = false;
    private IEnumerator coroutine;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (sceneIndex == 0)
        {
            menu.volume = 1f;
            menu.Play();
        }
        else if (sceneIndex == 1)
        {
            game.volume = 1f;
            game.Play();
        }
    }

    public void Crossfade(AudioSource current, AudioSource next)
    {
        if (running)
        {
            StopCoroutine(coroutine);
        }
        coroutine = CrossfadeCoroutine(current, next);
        StartCoroutine(coroutine);
    }

    private IEnumerator CrossfadeCoroutine(AudioSource current, AudioSource next)
    {
        running = true;
        next.Play();
        float time = 0f;
        while (time < crossfadeDuration)
        {
            time += Time.deltaTime;
            current.volume -= (Time.deltaTime / crossfadeDuration);
            next.volume += (Time.deltaTime / crossfadeDuration);
            yield return null;
        }
        current.Stop();
        running = false;
    }
}