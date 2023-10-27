using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.Linq;
using TMPro;
public class GoldGrindersManager : MonoBehaviourPun
{
    [Header("Info")]
    public float timerStartTime;
    private int playerWithMostGold;

    private bool timerActive;
    private float timerCurTime;

    public GameObject WinScreen;
    public TextMeshProUGUI WinText;
    public TextMeshProUGUI timerText;

    
    // instance
    static public GoldGrindersManager instance;

    
    private void Awake()
    {
        instance = this;    
    }

    public void Initialize()
    {
        timerCurTime = timerStartTime;
        timerActive = true;
    }

    private void Update()
    {
        if (timerActive)
            TimerCountDown();
    }

    void TimerCountDown()
    {
        timerCurTime -= Time.deltaTime;
        UpdateTimerUI(timerCurTime);
        if (timerCurTime < 0)
            EndGame();
    }

    void UpdateTimerUI(float timeLeft)
    {
        timerText.text = Math.Round(timeLeft, 2).ToString();
    }
    void EndGame()
    {
        
        Time.timeScale = 0;

        DisplayWinScreen(GetWinner());
    }

    void DisplayWinScreen(int winnerId)
    {
        PlayerController winner = GameManager.instance.GetPlayer(winnerId);
        WinScreen.SetActive(true);
        WinText.text = "The winner is" + winner.photonPlayer.NickName;

    }

    int GetWinner()
    {
        int largestSumOfGold = 0;
        foreach (PlayerController player in GameManager.instance.players)
        {
            if (player.gold > largestSumOfGold)
            {
                playerWithMostGold = player.id;
                largestSumOfGold = player.gold;
            }
        }
        return playerWithMostGold;
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