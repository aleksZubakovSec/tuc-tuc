using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    [Header("Required")]
    [SerializeField] private GameObject player;

    [Header("Parameters")]
    [SerializeField] private float speed = 0.125f;

    private Vector3 offset;

    private void Start()
    {
        offset = new Vector3(0f, 0f, transform.position.z);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        var interpolationSpeed = speed * Time.deltaTime;
        transform.position = Vector3.Lerp(transform.position, player.transform.position + offset, interpolationSpeed);
    }
}
