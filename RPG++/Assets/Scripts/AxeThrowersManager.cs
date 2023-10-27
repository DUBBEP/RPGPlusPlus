using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.Linq;

public class AxeThrowersManager : MonoBehaviourPun
{
    [Header("Info")]
    public float timerStartTime;
    private int playerWithHighestKills;
    private int highestKillCount;
    private bool timerActive;
    private bool gameEnded;
    private float timerCurTime;

    public GameObject WinScreen;
    public GameObject Timer;
    public TextMeshProUGUI WinText;
    public TextMeshProUGUI timerText;
    
    // instance
    static public AxeThrowersManager instance;

    
    private void Awake()
    {
        instance = this;    
    }

    public void Initialize()
    {
        timerCurTime = timerStartTime;
        Timer.SetActive(true);
        timerActive = true;
        highestKillCount = 0;
        playerWithHighestKills = 0;
        PlayerController.me.photonView.RPC("ChangeWeapons", RpcTarget.All, "Axe", PlayerController.me.id);
    }

    private void Update()
    {
        if (timerActive)
            TimerCountDown();
    }

    void TimerCountDown()
    {
        timerCurTime -= Time.deltaTime;
        UpdateTimerUI((int)timerCurTime);
        if (timerCurTime < 0 && !gameEnded)
        {
            EndGame();
            timerActive = false;
        }
    }

    void UpdateTimerUI(int timeLeft)
    {
        timerText.text = string.Format("<b>{0}</b>", timeLeft);
    }
    void EndGame()
    {
        gameEnded = true;
        GetWinner();
        DisplayWinScreen(playerWithHighestKills);
        StartCoroutine(ReturnToGameScene());

        IEnumerator ReturnToGameScene()
        {
            yield return new WaitForSeconds(5);
            if(PhotonNetwork.IsMasterClient)
                NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "Game");
        }

    }

    void DisplayWinScreen(int winnerId)
    {
        PlayerController winner = GameManager.instance.GetPlayer(winnerId);
        WinScreen.SetActive(true);
        WinText.text = "The winner is " + winner.photonPlayer.NickName;

    }

    [PunRPC]
    void GetWinner()
    {
        foreach (PlayerController player in GameManager.instance.players)
        {
            if (player.kills > highestKillCount)
            {
                highestKillCount = player.kills;
                playerWithHighestKills = player.id;
            }
        }
    }
}

/*
 * 
 * Axe throwers wll be a death match. whoever gets the most kills before time runs out wins
 * 
 */