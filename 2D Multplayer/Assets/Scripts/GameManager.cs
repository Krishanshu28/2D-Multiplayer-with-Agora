using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine.Networking;
using Photon.Pun.Demo.SlotRacer;

public class GameManager : MonoBehaviourPunCallbacks
{
    
    [Header("Status")]
    public bool gameEnded = false;

    [Header("Players")]
    public string playerPrefabLocation;
    public Transform[] spawnPoints;
    public PlayerController[] players;
    private int playersInGame;
    private List<int> pickedSpawnIndex;

    [Header("Reference")]
    public GameObject joinBtn;
    public GameObject Enable;

    //instance
    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        pickedSpawnIndex = new List<int>();
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
        //DefaultTrackableEventHandler.isTracking = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        AgoraUnityVideo.instance.CheckPermissions();
    }

    [PunRPC]
    void ImInGame()
    {
        playersInGame++;
        if(playersInGame == PhotonNetwork.PlayerList.Length)
        {
            SpawnPlayer();
        }
    }

    void SpawnPlayer()
    {
        Debug.Log("Spawn........");
        int rand = Random.Range(0,spawnPoints.Length);
        while (pickedSpawnIndex.Contains(rand))
        {
            rand = Random.Range(0,spawnPoints.Length);  
        }
        pickedSpawnIndex.Add(rand);

        GameObject playerObject = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[rand].position, Quaternion.identity);

        //intialize the player

        PlayerController playerScript = playerObject.GetComponent<PlayerController>();
        playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    public PlayerController GetPlayer(int playerID)
    {
        return players.First(x  => x.id == playerID);
    }
    public PlayerController GetPlayer(GameObject playerObj)
    {
        return players.First(x => x.gameObject == playerObj);
    }

    public void OnVisible()
    {
        Enable.SetActive(true);
        AgoraUnityVideo.instance.SetupUI();
        AgoraUnityVideo.instance.SetupVideoSDKEngine();
        AgoraUnityVideo.instance.InitEventHandler();
    }
    
}
