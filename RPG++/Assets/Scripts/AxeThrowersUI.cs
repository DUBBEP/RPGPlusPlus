using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AxeThrowersUI : MonoBehaviour
{
    public TextMeshProUGUI KillCount;

    // instance
    public static AxeThrowersUI instance;

    void Awake()
    {
        instance = this;
    }

    public void UpdateKillText(int kills)
    {
        KillCount.text = "<b>Kills:</b> " + kills;
    }
}