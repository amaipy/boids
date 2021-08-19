using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoidMath 
{
    const int numDirections = 300;
    public static readonly Vector3[] directions;

    static BoidMath() 
    {
        directions = new Vector3[BoidMath.numDirections];
        float goldenRatio = (1 + Mathf.Sqrt (5)) / 2;
        float angleInc = Mathf.PI * 2 * goldenRatio;

        for (int i = 0; i < numDirections; i++) 
        {
            float t = (float) i / numDirections;
            float incl = Mathf.Acos(1 - 2 * t);
            float arc = angleInc * i;

            float x = Mathf.Sin(incl) * Mathf.Cos(arc);
            float y = Mathf.Sin(incl) * Mathf.Sin(arc);
            float z = Mathf.Cos(incl);
            directions[i] = new Vector3 (x, y, z);
        }
    }

}