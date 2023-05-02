using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore;
using TMPro;

public class Score : MonoBehaviour
{
    public GameObject textmeshpro;
    private TextMeshProUGUI TextMeshProUGUI;
    public int score;
    private void Start()
    {
        TextMeshProUGUI = textmeshpro.GetComponent<TextMeshProUGUI>();
    }
    public void addscore(int amount)
    {
        score += amount;
        TextMeshProUGUI.SetText(score.ToString());
    }
}
