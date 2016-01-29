using UnityEngine;

public class WorldCam : MonoBehaviour {

	[SerializeField] private Vector3 point = Vector3.zero;
	[SerializeField] private Vector3 axis = Vector3.forward;
	[SerializeField] private float angle = 0.2f;
    private float distance;

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

	void Update () {
        if (cam.enabled == true)
        {
            transform.RotateAround(point, axis, angle);
        }
	}

    public void SetDistance(float distance)
    {

    }
}
