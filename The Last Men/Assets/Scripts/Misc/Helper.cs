using UnityEngine;
using System.Collections.Generic;

public static class Helper
{
    public static GameObject[] FindChildrenWithTag(this Transform trans, string tag)
    {
        List<GameObject> result = new List<GameObject>();

        foreach (Transform tr in trans)
        {
            if (tr.tag == tag)
            {
                result.Add(tr.gameObject);
            }
            else
            {
                //Debug.Log(tr.ToString());
                result.AddRange(FindChildrenWithTag(tr, tag));
            }
        }

        return result.ToArray();
    }
}
