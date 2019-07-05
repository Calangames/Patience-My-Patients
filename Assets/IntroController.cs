using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroController : MonoBehaviour 
{
    [TextArea(1, 10)]
    public string monologue;

    public static IntroController instance = null;
    public static bool play = true;

    private Animator _animator;
    private Typewriter _typewriter;
    private bool completed = false;

    void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start()
    {
        _animator = GetComponent<Animator>();
        _typewriter = GetComponent<Typewriter>();
        _animator.SetBool("Play", play);
    }

    public bool Completed()
    {
        return completed;
    }

    public void End()
    {
        completed = true;
    }

    public void Completed(bool b)
    {
        completed = b;
    }

    public void WriteIntro()
    {
        play = false;
        completed = false;
        _typewriter.Write(monologue);
        StartCoroutine(WaitUntilCompleted());
    }

    private IEnumerator WaitUntilCompleted()
    {
        while (!_typewriter.Completed())
        {
            yield return null;
        }
        _animator.SetTrigger("Continue Intro");
    }
}
