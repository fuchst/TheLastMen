using UnityEngine;
using System.Collections;

public struct Vector2i
{
    public int x;
    public int y;

    public Vector2i(int _x, int _y)
    {
        x = _x;
        y = _y;
    }

    public static Vector2i operator+ (Vector2i lhs, Vector2i rhs)
    {
        return new Vector2i(lhs.x + rhs.x, lhs.y + rhs.y);
    }
};
