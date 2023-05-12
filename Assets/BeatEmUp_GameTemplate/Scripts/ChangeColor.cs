using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    public Color newColor;
    private SpriteRenderer rend;
    void Start()
    {
        //newColor = new Color();
        //newColor.r = 164;
        //newColor.g = 16;
        //newColor.b = 221;
        //newColor.a = 255;
        rend = GetComponent<SpriteRenderer>();
        rend.color = newColor;
    }
}
