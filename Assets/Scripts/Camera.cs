using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    [Header("Required")]
    [SerializeField] private GameObject player;

    [Header("Parameters")]
    [SerializeField] private float speed;


    // Update is called once per frame
    void Update()
    {
        var interpolationSpeed = speed * Time.deltaTime;
        
        var playerPosition = player.transform.position;
        var oldCamerPosition = transform.position;

        var x = Mathf.Lerp(oldCamerPosition.x, playerPosition.x, interpolationSpeed);
        var y = Mathf.Lerp(oldCamerPosition.y, playerPosition.y, interpolationSpeed); 
        
        transform.position = new Vector3(x, y, oldCamerPosition.z);
    }
}
