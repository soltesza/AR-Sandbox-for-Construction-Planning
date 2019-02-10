using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SumoGlobals : MonoBehaviour
{
    float SCALE = 100.0f;

    public float GetScale()
    {
        return SCALE;
    }

    public void SetScale(float sx)
    {
        SCALE = sx;
    }
}
