using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tools
{
    public static float ListMin(List<float> list)
    {
        float min = list[0];

        foreach (float f in list)
        {
            if (f < min)
                min = f;
        }

        return min;
    }

    public static float ListMax(List<float> list)
    {
        float max = list[0];

        foreach (float f in list)
        {
            if (f > max)
                max = f;
        }

        return max;
    }


}
