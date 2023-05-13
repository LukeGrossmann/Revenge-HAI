using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowmeCheat : MonoBehaviour
{
    public Image image;
    void Update()
    {
        if (Input.GetKey(KeyCode.I))
        {
            image.enabled = true;
        }
        else image.enabled = false;
    }
}
