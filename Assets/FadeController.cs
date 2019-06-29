using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeController : MonoBehaviour 
{
    public static FadeController instance = null;

    private bool white = false, black = false;

    public bool White()
    {
        return white;
    }

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

    public void White(bool value)
    {
        white = value;
    }

    public bool Black()
    {
        return black;
    }

    public void Black(bool value)
    {
        black = value;
    }
}