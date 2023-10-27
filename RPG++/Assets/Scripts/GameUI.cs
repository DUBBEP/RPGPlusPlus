using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class GameUI : MonoBehaviour
{
    [Header("Screens")]
    public GameObject PauseMenuScreen;
    public GameObject UnpausedUI;
    public GameObject PauseMainScreen;
    public GameObject GamesScreen;

    [Header("PauseMenuScreen")]
    public bool pauseMenuIsOpen;

    [Header("GamesScreen")]
    public string selectedGame;
    public Button PlayGameButton;

    [Header("UnpausedUI")]
    public TextMeshProUGUI goldText;

    // instance
    public static GameUI instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
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
            PlayerController.me.DisableControls = false;
            pauseMenuIsOpen = false;
        }
        else
        {
            PauseMenuScreen.SetActive(true);
            SetMenuScreen(PauseMainScreen);
            UnpausedUI.SetActive(false);
            PlayerController.me.DisableControls = true;
            pauseMenuIsOpen = true;
        }
    }

    // PAUSE MAIN SCREEN

    public void OnLeaveGameButton()
    {
        NetworkManager.instance.ChangeScene("Menu");
        Destroy(NetworkManager.instance.gameObject);
    }
    
    public void OnMiniGamesButton()
    {
        SetMenuScreen(GamesScreen);

    }
    
    // GAMES SCREEN

    public void OnBackButton()
    {
        SetMenuScreen(PauseMainScreen);
        if (!PhotonNetwork.IsMasterClient)
            PlayGameButton.interactable = false;
    }

    public void OnStartButton()
    {
        NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, selectedGame);
    }

    public void OnSelectGameButton(Button buttonClicked)
    {
        selectedGame = buttonClicked.GetComponentInChildren<TMP_Text>().text;
    }
}
