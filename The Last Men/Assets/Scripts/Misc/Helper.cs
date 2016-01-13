using UnityEngine;
using System.Collections.Generic;

public static class Helper
{
    public static GameObject[] FindChildrenWithTag(this Transform parent, string tag)
    {
        List<GameObject> result = new List<GameObject>();

        Transform t = parent.transform;
        foreach (Transform tr in t)
        {
            if (tr.tag == tag)
            {
                result.Add(tr.gameObject);
            }
            else
            {
                result.AddRange(FindChildrenWithTag(tr, tag));
            }
        }

        return result.ToArray();
    }
}
