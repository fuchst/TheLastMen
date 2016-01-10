using UnityEngine;

public class HideCursor : MonoBehaviour {

	void Awake()
    {
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            Cursor.visible = !Cursor.visible;
        }
    }


    void OnDestroy()
    {
        Cursor.visible = true;
    }
}
