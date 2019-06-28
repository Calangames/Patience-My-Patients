using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    [Range(1f, 10f)]
    public float horizontalSpeed = 5f;
    [Range(1f, 20f)]
    public float jumpSpeed = 5f;
    [Range(0f, 1f)]
    public float horizontalDecelerationRate = 0.5f;
    [Range(0f, 2f)]
    public float stepsPitch = 0.5f;
    [Range(0f, 2f)]
    public float jumpPitch = 1f;
    public Image endingImage;
    public SpriteRenderer blindnessSprite;

    private Rigidbody2D _rigidbody2d;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private float horizontalForce = 0f, horizontalAcceleration = 0f;
    private bool mainCharacter = false, selected = false, addedToList = false, dead = false, canJump = true, ridingElevator = false;
    private int index = 0;
    private WaitForSeconds timer = new WaitForSeconds(0.3f);

    public void AddedToList(bool value)
    {
        addedToList = value;
    }

    public void MainCharacter(bool value)
    {
        mainCharacter = value;
    }

    public void Selected(bool value)
    {
        selected = value;
        TriggerSelection();
    }

    public void Index(int updatedIndex)
    {
        index = updatedIndex;
    }

    public void RidingElevator(bool b)
    {
        ridingElevator = b;
    }

    private void TriggerSelection()
    {
        if (selected)
        {
            _spriteRenderer.sortingOrder = 1;
        }
        else
        {
            _spriteRenderer.sortingOrder = 0;
            _animator.SetBool("WalkingOrJumping", false);
        }
        //_boxCollider2D.isTrigger = dead;
        if (dead)
        {
            gameObject.layer = 10;
        }
        else
        {
            gameObject.layer = 2;
        }

        if (blindnessSprite)
        {
            blindnessSprite.enabled = selected;
        }
    }

    void Awake()
    {
        _rigidbody2d = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        TriggerSelection();
    }

    void Update()
    {
        _animator.SetBool("Dead", dead);
        if (selected && !dead)
        {
            if (!ridingElevator)
            {
                float hInputRaw = Input.GetAxisRaw("Horizontal");
                if (Input.GetAxisRaw("Jump") > 0f || hInputRaw != 0f)
                {
                    if (hInputRaw != 0f)
                    {
                        _spriteRenderer.flipX = hInputRaw < 0f;
                    }
                    _animator.SetBool("WalkingOrJumping", true);
                }
                else
                {
                    _animator.SetBool("WalkingOrJumping", false);
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (selected)
        {
            if (!dead && !ridingElevator)
            {
                if (Input.GetAxisRaw("Horizontal") != 0f)
                {
                    horizontalAcceleration = _rigidbody2d.mass * horizontalSpeed / Time.fixedDeltaTime;
                    horizontalForce = horizontalAcceleration * Input.GetAxisRaw("Horizontal") * (1f - Mathf.Min(Mathf.Abs(_rigidbody2d.velocity.x), horizontalSpeed) / horizontalSpeed);
                    _rigidbody2d.AddForce(Vector2.right * horizontalForce, ForceMode2D.Force);
                    if (IsOnTheGround() && !SoundController.instance.steps.isPlaying)
                    {
                        SoundController.instance.steps.pitch = Random.Range(stepsPitch - 0.02f, stepsPitch + 0.02f);
                        SoundController.instance.steps.Play();
                    }
                }
                else
                {
                    horizontalAcceleration = _rigidbody2d.mass * horizontalSpeed / Time.fixedDeltaTime;
                    horizontalForce = horizontalAcceleration * horizontalDecelerationRate * -Mathf.Sign(_rigidbody2d.velocity.x) * (Mathf.Min(Mathf.Abs(_rigidbody2d.velocity.x), horizontalSpeed) / horizontalSpeed);
                    _rigidbody2d.AddForce(Vector2.right * horizontalForce, ForceMode2D.Force);
                }
                if (IsOnTheGround() && canJump)
                {
                    if (Input.GetAxisRaw("Jump") > 0f)
                    {
                        StartCoroutine(JumpDelayToAvoidDoubleJump());
                        _rigidbody2d.AddForce(Vector2.up * jumpSpeed * _rigidbody2d.mass, ForceMode2D.Impulse);
                        if (!SoundController.instance.jump.isPlaying)
                        {
                            SoundController.instance.jump.pitch = Random.Range(jumpPitch - 0.02f, jumpPitch + 0.02f);
                            SoundController.instance.jump.Play();
                        }
                    }
                }
            }
        }
        else
        {
            horizontalAcceleration = _rigidbody2d.mass * horizontalSpeed / Time.fixedDeltaTime;
            horizontalForce = horizontalAcceleration * horizontalDecelerationRate * -Mathf.Sign(_rigidbody2d.velocity.x) * (Mathf.Min(Mathf.Abs(_rigidbody2d.velocity.x), horizontalSpeed) / horizontalSpeed);
            _rigidbody2d.AddForce(Vector2.right * horizontalForce, ForceMode2D.Force);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !selected)
        {
            if (!addedToList && !dead)
            {
                GameController.instance.AddCharacterToList(this);
                index = GameController.instance.teamCharacters.Count - 1;
                addedToList = true;
            }
        }
        else if (other.CompareTag("Water"))
        {
            StartCoroutine(Die());
        }
        else if (other.CompareTag("ExitArea"))
        {
            EndingController.instance.AddCharacterToEnding(this);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (selected)
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Z))
            {
                if (other.CompareTag("Lever"))
                {
                    GameController.instance.ActivateLever(other.GetComponent<Lever>().GetInstanceID());
                }
                else if (other.CompareTag("Elevator"))
                {
                    if (!ridingElevator)
                    {
                        ridingElevator = true;
                        other.GetComponent<Elevator>().Fade();
                    }
                }
                else if (other.CompareTag("Exit"))
                {
                    if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Z))
                    {
                        if (!ridingElevator)
                        {
                            ridingElevator = true;
                            EndingController.instance.End();
                        }
                    }
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("ExitArea"))
        {
            EndingController.instance.RemoveCharacterFromEnding(GetInstanceID());
        }
    }

    public bool IsOnTheGround()
    {
        ContactPoint2D[] contacts = new ContactPoint2D[5];
        int contactCount = _rigidbody2d.GetContacts(contacts);
        int whyTheHellINeedThisCount = contactCount;
        foreach (ContactPoint2D contact in contacts)
        {
            if (contact.normal.y >= 0.9f)
            {
                contactCount = whyTheHellINeedThisCount;
                return true;
            }
        }
        contactCount = whyTheHellINeedThisCount;
        return false;
    }

    private IEnumerator JumpDelayToAvoidDoubleJump() //its 4am lol
    {
        canJump = false;
        yield return timer;
        canJump = true;
    }

    private IEnumerator Die()
    {
        dead = true;
        Selected(false);
        if (mainCharacter)
        {
            GameController.instance.GameOver();
            yield break;
        }
        else
        {
            GameController.instance.RemoveCharacterFromList(index);
        }
        index = -1;
        addedToList = false;
        while (!IsOnTheGround())
        {
            yield return null;
        }
        _rigidbody2d.bodyType = RigidbodyType2D.Static;
    }
}