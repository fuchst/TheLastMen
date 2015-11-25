using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {
    	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Weapon fired");

            RaycastHit hit;

            if (Physics.Raycast(new Ray(transform.position, transform.forward), out hit))
            {
                print("There is something in front of the object!");
                if(hit.transform.gameObject.tag.Equals("Enemy"))
                {
                    hit.transform.SendMessage("OnHit", 20);
                }
            }
        }  
    }
}
