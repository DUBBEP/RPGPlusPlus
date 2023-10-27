using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.Linq;

public class GoldGrindersManager : MonoBehaviourPun
{
    [Header("Info")]
    public float timerStartTime;
    private int playerWithMostGold;
    private int largestSumOfGold;
    private bool timerActive;
    private bool gameEnded;
    private float timerCurTime;

    public GameObject WinScreen;
    public GameObject Timer;
    public TextMeshProUGUI WinText;
    public TextMeshProUGUI timerText;
    public GameObject[] EnemySpawners;
    
    // instance
    static public GoldGrindersManager instance;

    
    private void Awake()
    {
        instance = this;    
    }

    public void Initialize()
    {
        timerCurTime = timerStartTime;
        Timer.SetActive(true);
        timerActive = true;
        largestSumOfGold = 0;
        playerWithMostGold = 0;
        ActivateSpawners();
    }

    private void Update()
    {
        if (timerActive)
            TimerCountDown();
    }


    void ActivateSpawners()
    {
        foreach (GameObject spawner in EnemySpawners)
        {
            spawner.SetActive(true);
        }
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
        DisplayWinScreen(playerWithMostGold);
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
            if (player.gold > largestSumOfGold)
            {
                largestSumOfGold = player.gold;
                playerWithMostGold = player.id;
            }
        }
    }
}

/*
 * 
 * Gold grinders is essentially a item collection competition.
 * Whichever player collects the most gold before the timer wins.
 * 
 * Functionality that needs to be included in this:
 * 1. players can collect gold and their total gold count is tracked.
 * 2. a timer is set and once it hits zero the game ends.
 * 3. player with the most ammount of gold will be listed as the winner.
 * 4. after the game ends the players will be sent back to the game screen.
 * 
 * things we need to make this happen.
 * 1. timer that counts down and is visible on screen.
 * 2. function to find player with the most gold.
 * 3. 
 */