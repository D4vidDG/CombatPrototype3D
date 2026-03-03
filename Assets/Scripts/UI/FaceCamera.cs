using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FaceCamera : MonoBehaviour
{
    void Update()
    {
        Face();
    }

    private void Face()
    {
        transform.forward = Camera.main.transform.forward;
    }
}
