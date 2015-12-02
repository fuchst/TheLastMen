﻿using UnityEngine;

public class ArtifactIsland : MonoBehaviour {

    private bool collected = false;
    private string baseMaterial = "SimpleMats/UnassignedIsland";

	void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player" && collected == false)
        {
            collected = true;
            collision.gameObject.SendMessage("AddArtifact");
            gameObject.GetComponent<MeshRenderer>().material = Resources.Load(baseMaterial, typeof(Material)) as Material;
        }
    }
}