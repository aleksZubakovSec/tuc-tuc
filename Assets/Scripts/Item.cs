using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Item : MonoBehaviour
{
    public float range = 3f;
    public float speed = 3f;
    public float offset = 1f;


    private float topY;
    private float bottomY;
    [SerializeField] private bool _movingUp = true;

    public UnityEvent ev;

    void Start()
    {
        var startY = transform.position.y;
        topY = startY + range;
        bottomY = startY - range;
    }

    void Update()
    {
        var currentPosition = transform.position;
        if (_movingUp && Mathf.Abs(currentPosition.y - topY) < offset ||
            !_movingUp & Mathf.Abs(currentPosition.y - bottomY) < offset)
        {
            Flip();
        }

        transform.position = new Vector3(currentPosition.x,
            Mathf.Lerp(currentPosition.y, _movingUp ? topY : bottomY, speed * Time.deltaTime), currentPosition.z);
    }

    void Flip()
    {
        _movingUp = !_movingUp;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ev.Invoke();
        }
        Destroy(gameObject);
    }
}