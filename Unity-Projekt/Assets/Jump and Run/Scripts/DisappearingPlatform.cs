﻿using UnityEngine;
using System.Collections;

public class DisappearingPlatform : MonoBehaviour {

    private float zTime;
    public float waitTime;
    private bool startTimer;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (startTimer)
        {
            zTime += Time.deltaTime;

        }

        if (zTime >= waitTime)
        {
            startTimer = false;
            Vector3 neu=transform.position;
            neu.y -= 0.05f;
            transform.position = neu;
        }

        if (transform.position.y < -10)
        {
            Destroy(this);
        }	
	}

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.tag == "player"&&other.transform.position.y>this.transform.position.y+(GetComponent<Collider2D>().bounds.size.y/2))
        {
            startTimer = true;
        }
    }
}
