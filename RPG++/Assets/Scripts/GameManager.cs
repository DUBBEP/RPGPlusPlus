using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class GameManager : MonoBehaviourPun
{
    [Header("Players")]
    public string playerPrefabPath;
    public PlayerController[] players;
    public bool[] usedSpawnPoints;
    public Transform[] spawnPoints;
    public float respawnTime;
    public int spawnPointIndex;

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
        usedSpawnPoints = new bool[spawnPoints.Length];
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
            case "AxeThrowers":
                if (PhotonNetwork.IsMasterClient)
                    photonView.RPC("InitializeAxeThrowers", RpcTarget.All);
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
    void InitializeAxeThrowers()
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
            AxeThrowersManager.instance.Initialize();
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
        photonView.RPC("GetSpawnPoint", RpcTarget.All);
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabPath, spawnPoints[spawnPointIndex].position, Quaternion.identity);

        // initialize Player
        playerObj.GetComponent<PhotonView>().RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    [PunRPC]
    void GetSpawnPoint()
    {
        bool rolledValidSpawn;
        do
        {
            spawnPointIndex = Random.Range(0, spawnPoints.Length);
            rolledValidSpawn = true;
            if (usedSpawnPoints[spawnPointIndex])
                rolledValidSpawn = false;
        }
        while (!rolledValidSpawn);

        usedSpawnPoints[spawnPointIndex] = true;

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
