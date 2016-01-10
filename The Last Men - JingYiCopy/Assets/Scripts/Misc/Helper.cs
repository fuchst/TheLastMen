using UnityEngine;
using System.Collections.Generic;

public static class Helper
{
    public static GameObject[] FindChildrenWithTag(this GameObject parent, string tag)
    {
        List<GameObject> result = new List<GameObject>();

        Transform t = parent.transform;
        foreach (Transform tr in t)
        {
            if (tr.tag == tag)
            {
                result.Add(tr.gameObject);
            }
        }

        return result.ToArray();
    }
}
