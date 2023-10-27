using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPun
{
    [Header("Players")]
    public string playerPrefabPath;
    public PlayerController[] players;
    public Transform[] spawnPoints;
    public float respawnTime;

    private int playersInGame;

    [Header("GoldGrinders")]
    public TextMeshProUGUI countdown;


    // instance

    public static GameManager instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);

        
    }

    void SceneInitialize(string sceneName)
    {
        switch(sceneName)
        {
            case "GoldGrinders":
                if (PhotonNetwork.IsMasterClient)
                {
                    photonView.RPC("InitializeGoldGrinders", RpcTarget.All);
                }
                break;
        }
    }

    [PunRPC]
    void InitializeGoldGrinders()
    {

        StartCoroutine(CountDown());
        
        IEnumerator CountDown()
        {
            PlayerController.me.DisableControls = true;
            int count = 5;
            countdown.text = string.Format("<b>{0}</b>", count);
            while (count > 0)
            {
                yield return new WaitForSeconds(1f);
                count--;
                countdown.text = string.Format("<b>{0}</b>", count);

            }
            countdown.gameObject.SetActive(false);
            PlayerController.me.DisableControls = false;
            GoldGrindersManager.instance.Initialize();
        }
    }

    [PunRPC]
    void ImInGame()
    {
        playersInGame++;

        if (playersInGame == PhotonNetwork.PlayerList.Length)
            SpawnPlayer();
            SceneInitialize(SceneManager.GetActiveScene().name);
    }

    void SpawnPlayer()
    {
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabPath, spawnPoints[(Random.Range(0, spawnPoints.Length))].position, Quaternion.identity);

        // initialize Player
        playerObj.GetComponent<PhotonView>().RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    public PlayerController GetPlayer(int id)
    {
        foreach (PlayerController player in players)
        {
            if (player != null && player.id == id)
                return player;
        }
        return null;
    }

    public PlayerController GetPlayer(GameObject playerObject)
    {
        foreach (PlayerController player in players)
        {
            if (player != null && player.gameObject == playerObject)
                return player;
        }
        return null;
    }
}
