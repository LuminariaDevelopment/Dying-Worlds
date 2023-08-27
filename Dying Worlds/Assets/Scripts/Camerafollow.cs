using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camerafollow : MonoBehaviour
{
    [Header("Reference")]
    public Transform cameraPos;

    void Update()
    {
        transform.position = cameraPos.position;
        transform.rotation = cameraPos.rotation;
    }

}
