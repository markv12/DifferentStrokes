using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobUpAndDown : MonoBehaviour
{

    // User Inputs
    public float amplitude = 0.1f;
    public float frequency = 0.3f;
    private float randomOffset = 0;

    // Position Storage Variables
    Vector3 posOffset = new Vector3();
    Vector3 tempPos = new Vector3();

    // Use this for initialization
    void Start() {
        // Store the starting position & rotation of the object
        posOffset = transform.position;
        randomOffset = Random.value;
    }

    // Update is called once per frame
    void Update() {
        // Float up/down with a Sin()
        tempPos = posOffset;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency + randomOffset) * amplitude;

        transform.position = tempPos;
    }
}
