using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenChanger : MonoBehaviour
{
    public List<Canvas> ListofCanvas;
    private int currentScreen;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (currentScreen < ListofCanvas.Count - 1)
            {
                ListofCanvas[currentScreen].enabled = false;
                currentScreen++;
                ListofCanvas[currentScreen].enabled = true;
            }
            else SceneManager.LoadScene("Game");
        }
    }
}
