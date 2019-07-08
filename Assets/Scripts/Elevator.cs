using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public Transform connectedElevator;
    public SpriteRenderer fade;

    private WaitForSeconds timer = new WaitForSeconds(0.2f);

    public void Fade()
    {
        StartCoroutine(FadeCoroutine());
    }

    private IEnumerator FadeCoroutine()
    {
        float alpha = 0f;
        while (alpha < 1f)
        {
            alpha += (Time.deltaTime * 4f);
            fade.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }
        Vector3 newPosition = new Vector3(connectedElevator.position.x, connectedElevator.position.y - 0.4f, connectedElevator.position.z);
        GameController.instance.SelectedCharacter().transform.position = newPosition;
        yield return timer;
        while (alpha > 0f)
        {
            alpha -= (Time.deltaTime * 4f);
            fade.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }
        GameController.instance.SelectedCharacter().Transitioning(false);
    }
}
