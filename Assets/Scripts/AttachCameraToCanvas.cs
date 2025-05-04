using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachCameraToCanvas : MonoBehaviour
{
    private Canvas canvas;

    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponent<Canvas>();

        canvas.worldCamera = Camera.main;
    }
}
