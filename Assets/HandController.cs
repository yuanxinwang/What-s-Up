﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour {

    public Transform degree;
	
	// Update is called once per frame
	void Update () {
        transform.rotation = degree.rotation;
	}
}
