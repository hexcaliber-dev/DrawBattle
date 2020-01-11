﻿using UnityEngine;

// Adopted from https://github.com/Brackeys/Smooth-Camera-Follow/blob/master/Smooth%20Camera/Assets/CameraFollow.cs
public class CameraFollow : MonoBehaviour {

    public Transform player;

    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    void FixedUpdate() {
        Vector3 desiredPosition = player.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, -10f);
    }

}