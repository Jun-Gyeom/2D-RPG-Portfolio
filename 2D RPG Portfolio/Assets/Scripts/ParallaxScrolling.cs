using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxScrolling : MonoBehaviour
{
    public Vector2 parallaxEffect;

    private Transform camTransform;
    private Vector3 lastCamPos;

    private void Start()
    {
        camTransform = Camera.main.transform;
        lastCamPos = camTransform.position;
    }

    private void FixedUpdate()
    {
        Vector3 deltaMovement = camTransform.position - lastCamPos;
        transform.position += new Vector3(deltaMovement.x * parallaxEffect.x, deltaMovement.y * parallaxEffect.y);
        lastCamPos = camTransform.position;
    }
}
