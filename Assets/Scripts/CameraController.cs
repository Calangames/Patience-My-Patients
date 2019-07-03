using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    //[Range(0f, 20f)]
    //public float cameraSpeed = 3f;
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
    private Vector3 newPos;
    private GameObject player;

    private const float PixelSize = 1f / 32f; //0.03125f

    // Use this for initialization
    void Start()
    {
        mainCamera = GetComponent<Camera>();
        perfectPixelCamera = GetComponent<GGEZ.PerfectPixelCamera>();
#if UNITY_WEBGL
        fullScreen = Screen.fullScreen;
#endif
        startingOrthographicSize = mainCamera.orthographicSize;
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
        float newX = Mathf.Round((player.transform.position.x + cameraOffset.x) / PixelSize) * PixelSize;
        float newY = Mathf.Round((player.transform.position.y + cameraOffset.y) / PixelSize) * PixelSize;
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
            currentWidth = (float)mainCamera.pixelWidth;
            currentHeight = (float)mainCamera.pixelHeight;
        }
#else
        currentWidth = (float)mainCamera.pixelWidth;
        currentHeight = (float)mainCamera.pixelHeight;
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
        mainCamera.orthographicSize = startingOrthographicSize * targetAspect / currentAspect;
        if (secondaryCamera)
        {
            secondaryCamera.orthographicSize = mainCamera.orthographicSize;
        }
    }
}