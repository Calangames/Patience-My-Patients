using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{
    public bool up = true;

    private bool isButton;
    private SpriteRenderer _spriteRenderer;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        isButton = CompareTag("FloorButton");
    }

    public void SwapState()
    {
        up = !up;
        _spriteRenderer.sprite = up ? GameController.instance.handLeverUp : GameController.instance.handLeverDown;
        SoundController.instance.interact.pitch = Random.Range(0.95f, 1.05f);
        SoundController.instance.interact.Play();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (isButton && up)
        {
            if (other.CompareTag("Player"))
            {
                up = false;
                _spriteRenderer.sprite = GameController.instance.floorButtonDown;
                GameController.instance.ActivateButton(GetInstanceID());
                SoundController.instance.interact.pitch = Random.Range(0.95f, 1.05f);
                SoundController.instance.interact.Play();
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (isButton && other.CompareTag("Player"))
        {
            up = true;
            _spriteRenderer.sprite = GameController.instance.floorButtonUp;
            GameController.instance.ActivateButton(GetInstanceID());
        }
    }
}
