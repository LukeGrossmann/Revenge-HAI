using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowmeCheat : MonoBehaviour
{
    public Image image;
    public KeyCode keytopress;
    void Update()
    {
        if (Input.GetKey(keytopress))
        {
            image.enabled = true;
        }
        else image.enabled = false;
    }
}
