﻿using UnityEngine;
using System.Collections;

public class DummyEnemyAnimator : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Animation>().wrapMode = WrapMode.Loop;

    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
