using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private Animator _animator;
    private bool open = false;

    // Use this for initialization
    void Start ()
    {
        _animator = GetComponent<Animator>();
    }

    public void SwapState()
    {
        if (_animator)
        {
            open = !open;
            _animator.SetBool("Open", open);
        }
        else
        {
            foreach (SpriteRenderer childSprite in GetComponentsInChildren<SpriteRenderer>())
            {
                childSprite.enabled = !childSprite.enabled;
            }
            foreach(BoxCollider2D childCollider in GetComponentsInChildren<BoxCollider2D>())
            {
                childCollider.enabled = !childCollider.enabled;
            }
        }
    }
}
