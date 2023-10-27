using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private void Update()
    {
        // does the player exist?
        if (PlayerController.me != null && !PlayerController.me.dead)
        {
            Vector3 targetPos = PlayerController.me.transform.position;
            targetPos.z = -6;

            transform.position = targetPos;
        }
    }

}
