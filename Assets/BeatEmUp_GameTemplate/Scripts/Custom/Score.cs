using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore;
using TMPro;

public class Score : MonoBehaviour
{
    public GameObject textmeshpro;
    static private TextMeshProUGUI TextMeshProUGUI;
    static public int score;
    private void Start()
    {
        TextMeshProUGUI = textmeshpro.GetComponent<TextMeshProUGUI>();
    }
    public void addscore(int amount)
    {
        score += amount;
        TextMeshProUGUI.SetText(score.ToString());
    }
    public static void addscoreStatic(int amount)
    {
        score += amount;
        TextMeshProUGUI.SetText(score.ToString());
    }
}
