using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiseWaterLevel : MonoBehaviour {

    public float RaiseSpeed = 0.01f;
    public float RaiseMultiplayer = 0.01f;
	// Use this for initialization
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + (RaiseSpeed * RaiseMultiplayer), transform.position.z);		
	}

    public void IncreaseWaterMultiplyer()
    {
        RaiseMultiplayer += 0.001f;
    }

    internal void IncreaseWaterSpeed()
    {
        RaiseMultiplayer += 0.01f;
    }
}
