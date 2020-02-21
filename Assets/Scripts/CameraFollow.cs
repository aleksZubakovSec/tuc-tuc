using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Required")] [SerializeField] private GameObject player;

    [Header("Parameters")] [SerializeField]
    private float speed = 5f;

    [SerializeField] private float smoothTime = 1f;
    [SerializeField] private Vector3 offset = Vector3.zero;

    private Camera mainCamera;
    private Rigidbody2D playerRb;
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        mainCamera = Camera.main;
        playerRb = player.GetComponent<Rigidbody2D>();
    }

    void LateUpdate()
    {
        Vector3 oldPosition = transform.position;
        Vector3 playerPosition = player.transform.position;

        var xDistance = Vector2.Distance(Vector2.right * oldPosition.x, Vector2.right * playerPosition.x);
        var yDistance = Vector2.Distance(Vector2.up * oldPosition.y, Vector2.up * playerPosition.y);

        Vector3 threshold = GetThreshold();
        Vector3 newPosition = oldPosition;
        if (Mathf.Abs(xDistance) >= threshold.x)
        {
            newPosition.x = playerPosition.x;
        }

        if (Mathf.Abs(yDistance) >= threshold.y)
        {
            newPosition.y = playerPosition.y;
        }

        var playerVelocityMagnitude = playerRb.velocity.magnitude;
        var currentSpeed = playerVelocityMagnitude > speed ? playerVelocityMagnitude : speed;
        // TODO: mb it's better to change to MoveTowards
        transform.position = Vector3.SmoothDamp(oldPosition, newPosition, ref velocity, smoothTime, currentSpeed);
        // transform.position = Vector3.MoveTowards(oldPosition, newPosition, currentSpeed * Time.deltaTime);
    }


    private Vector3 GetThreshold()
    {
        Rect cameraPixelRect = mainCamera.pixelRect;
        float orthographicSize = mainCamera.orthographicSize;
        var threshold = new Vector3(orthographicSize * cameraPixelRect.width / cameraPixelRect.height,
            orthographicSize);

        return threshold - offset;
    }

    private void OnDrawGizmos()
    {
        if (mainCamera != null)
        {
            Vector3 border = GetThreshold();
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, new Vector3(border.x * 2, border.y * 2, 1));
        }
    }
}