﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    //[Range(0f, 20f)]
    //public float cameraSpeed = 3f;
    [Range(0f, 1f)]
    public float cameraShakeDuration = 0.5f;
    [Range(0f, 1f)]
    [Tooltip("In World Units")]
    public float cameraShakeMagnitude = 0.2f;
    public Vector2 targetResolution;
    public float heightInWorldUnits = 9f, widthInWorldUnits = 16f;
    public Vector3 cameraOffset;
    public Camera secondaryCamera;

#if UNITY_WEBGL
    private bool fullScreen = false;
#endif
    private Camera mainCamera;
    private GGEZ.PerfectPixelCamera perfectPixelCamera;
    private float xCameraExtent, yCameraExtent, aspectRatio, startingOrthographicSize, currentWidth, currentHeight, currentAspect, targetAspect;
    private Vector2 screenShake = Vector2.zero;
    private Vector3 newPos;
    private GameObject player;
    private Coroutine shaking;

    private const float PixelSize = 1f / 32f; //0.03125f

    // Use this for initialization
    void Start()
    {
        mainCamera = GetComponent<Camera>();
        perfectPixelCamera = GetComponent<GGEZ.PerfectPixelCamera>();
#if UNITY_WEBGL
        fullScreen = Screen.fullScreen;
#endif
        if (secondaryCamera)
        {
            startingOrthographicSize = secondaryCamera.orthographicSize;
        }
        else
        {
            startingOrthographicSize = mainCamera.orthographicSize;
        }
        targetAspect = targetResolution.x / targetResolution.y;
        ResizeCamera();
    }

    // Update is called once per frame
    void LateUpdate()
    {
#if UNITY_WEBGL
        if (fullScreen != Screen.fullScreen)
        {
            fullScreen = Screen.fullScreen;
            ResizeCamera();
        }
#endif
        if (GameController.instance && GameController.instance.SelectedCharacter())
        {
            player = GameController.instance.SelectedCharacter().gameObject;
        }
        else
        {
            player = null;
        }
        if (player == null || Time.timeScale == 0f)
        {
            return;
        }
        float newX = Mathf.Round((player.transform.position.x + cameraOffset.x + screenShake.x) / PixelSize) * PixelSize;
        float newY = Mathf.Round((player.transform.position.y + cameraOffset.y + screenShake.y) / PixelSize) * PixelSize;
        newPos = new Vector3(newX, newY, cameraOffset.z);
        transform.position = Vector3.MoveTowards(transform.position, newPos, 100f);
    }

    public void ResizeCamera()
    {
#if UNITY_WEBGL
        if (fullScreen)
        {
            currentWidth = (float)Screen.currentResolution.width;
            currentHeight = (float)Screen.currentResolution.height;
        }else
        {
            if (secondaryCamera)
            {
                currentWidth = (float)secondaryCamera.pixelWidth;
                currentHeight = (float)secondaryCamera.pixelHeight;
            }
            else
            {
                currentWidth = (float)mainCamera.pixelWidth;
                currentHeight = (float)mainCamera.pixelHeight;
            }
        }
#else
        if (secondaryCamera)
        {
            currentWidth = (float)secondaryCamera.pixelWidth;
            currentHeight = (float)secondaryCamera.pixelHeight;
        }
        else
        {
            currentWidth = (float)mainCamera.pixelWidth;
            currentHeight = (float)mainCamera.pixelHeight;
        }
#endif
        currentAspect = currentWidth / currentHeight;
        if (perfectPixelCamera)
        {
            if (mainCamera.aspect >= targetAspect)
            {
                perfectPixelCamera.TexturePixelsPerWorldUnit = Mathf.CeilToInt(currentHeight / heightInWorldUnits);
            }
            else
            {
                perfectPixelCamera.TexturePixelsPerWorldUnit = Mathf.CeilToInt(currentWidth / widthInWorldUnits);
            }
        }
        if (secondaryCamera)
        {
            secondaryCamera.orthographicSize = startingOrthographicSize * targetAspect / currentAspect;
        }
        else
        {
            mainCamera.orthographicSize = startingOrthographicSize * targetAspect / currentAspect;
        }
    }

    public void Shake()
    {
        if (shaking != null)
        {
            StopCoroutine(shaking);
        }
        shaking = StartCoroutine(ShakeCoroutine(cameraShakeDuration, cameraShakeMagnitude));
    }

    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        float decayRate = magnitude / duration;
        while (duration > 0)
        {
            duration -= Time.deltaTime;
            screenShake = Random.insideUnitCircle * magnitude;
            magnitude -= (Time.deltaTime * decayRate);
            yield return null;
        }
        screenShake = Vector2.zero;
    }
}