using UnityEngine;
using System.Collections;

public class s_ObjectRotation : MonoBehaviour {

    public Transform[] affectedObjects = new Transform[0];
    public float rotationSpeed;
    public Vector3 rotationAxis;
    
    void Update () {
        foreach (Transform @o in affectedObjects) {
            @o.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
        }
    }
}
