using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;


public class GameUI : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    [Header("Screens")]
    public GameObject PauseMenuScreen;
    public GameObject UnpausedUI;
    public GameObject PauseMainScreen;
    public GameObject GamesScreen;

    [Header("PauseMenuScreen")]
    public Button LeaveMenuButton;
    public bool pauseMenuIsOpen;

    [Header("PauseMainScreen")]
    public Button MiniGamesButton;
    public Button LeaveButton;

    [Header("GamesScreen")]
    // add Game Buttons

    [Header("UnpausedUI")]
    public TextMeshProUGUI goldText;
    public Button OpenPauseMenuButton;

    // instance
    public static GameUI instance;

    void Awake()
    {
        instance = this;
    }

    void start()
    {
        pauseMenuIsOpen = false;
    }

    void SetMenuScreen(GameObject screen)
    {
        PauseMainScreen.SetActive(false);
        GamesScreen.SetActive(false);

        screen.SetActive(true);
    }


    // UNPAUSED UI
    public void UpdateGoldText (int gold)
    {
        goldText.text = "<b>Gold:</b> " + gold;
    }

    public void ToggleMenuButton()
    {
        if (pauseMenuIsOpen)
        {
            PauseMenuScreen.SetActive(false);
            UnpausedUI.SetActive(true);
            PlayerController.me.pauseMenuOpen = false;
            pauseMenuIsOpen = false;
        }
        else
        {
            PauseMenuScreen.SetActive(true);
            SetMenuScreen(PauseMainScreen);
            UnpausedUI.SetActive(false);
            PlayerController.me.pauseMenuOpen = true;
            pauseMenuIsOpen = true;
        }
    }

    // PAUSE MAIN SCREEN

    public void OnLeaveGameButton()
    {
        NetworkManager.instance.ChangeScene("Menu");
    }
    
    public void OnMiniGamesButton()
    {
        SetMenuScreen(GamesScreen);
    }
    
    // GAMES SCREEN

    public void OnBackButton()
    {

    }

    public void OnStartButton()
    {
        //impliment later
    }

    // impliment select game button
}
