using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonRoom : MonoBehaviourPunCallbacks, IInRoomCallbacks {

    public static PhotonRoom room;
    private PhotonView PV;

    [SerializeField] string gamerTag;

    private string playerDataFile;

    public bool isGameLoaded;
    public int currentScene;

    private Player[] photonPlayers;
    public int playersInRoom;
    public int myNumberInRoom;

    public int playerInGame;

    [SerializeField] private bool readyToCount;
    [SerializeField] private bool readyToStart;
    public float startingTime;
    private float lessThanMaxPlayers;
    private float atMaxPlayer;
    [SerializeField] private float timeToStart;

    private void Awake() {
        if (PhotonRoom.room == null)
            PhotonRoom.room = this;
        else { 
            if (PhotonRoom.room != this) {
                Destroy(PhotonRoom.room.gameObject);
                PhotonRoom.room = this;
            }
        }
        playerDataFile = Application.persistentDataPath + "/PlayersData.json";
        DontDestroyOnLoad(this.gameObject);
    }

    public override void OnEnable() {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    public override void OnDisable() {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }

    private void Start() {
        PV = GetComponent<PhotonView>();
        readyToCount = false;
        readyToStart = false;
        lessThanMaxPlayers = startingTime;
        atMaxPlayer = 6;
        timeToStart = startingTime;
    }

    private void Update() {
        gamerTag = PhotonNetwork.NickName;
        if (MultiplayerSettings.multiplayerSettings.delayStart) {
            if (playersInRoom == 1) {
                RestartTimer();
            }
            if (!isGameLoaded) {
                if (readyToStart) {
                    atMaxPlayer -= Time.deltaTime;
                    lessThanMaxPlayers = atMaxPlayer;
                    timeToStart = atMaxPlayer;
                } else if (readyToCount) {
                    lessThanMaxPlayers -= Time.deltaTime;
                    timeToStart = lessThanMaxPlayers;
                }
                //Debug.Log("Display time to start to players " + timeToStart);
                if (timeToStart <= 0)
                    StartGame();
            }
        }
    }

    public override void OnJoinedRoom() {
        base.OnJoinedRoom();
        Debug.Log("We are now in a room");
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom = photonPlayers.Length;
        myNumberInRoom = playersInRoom;
        //PhotonNetwork.NickName = myNumberInRoom.ToString();
        if (MultiplayerSettings.multiplayerSettings.delayStart) {
            Debug.Log("Displayer players in room out of max players possible (" + playersInRoom + ":" + MultiplayerSettings.multiplayerSettings.maxPlayers + ")");    
            if (playersInRoom > 1) {
                readyToCount = true;
            }
            if (playersInRoom == MultiplayerSettings.multiplayerSettings.maxPlayers) { 
                readyToStart = true;
                if (!PhotonNetwork.IsMasterClient)
                    return;
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
        } else {
            StartGame();
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        base.OnPlayerLeftRoom(otherPlayer);
        
        Debug.Log("Player \"" + otherPlayer.NickName + "\" just left the game");

        if (otherPlayer.IsMasterClient) {
            DisconnectGame();
        } else {
            if (PhotonNetwork.IsMasterClient) {
                SavePlayerInfo(otherPlayer);
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        base.OnPlayerEnteredRoom(newPlayer);

        Debug.Log("Player \"" + newPlayer.NickName + "\" just joined the game");

        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom++;

        if (MultiplayerSettings.multiplayerSettings.delayStart) {
            Debug.Log("Displayer players in room out of max players possible (" + playersInRoom + ":" + MultiplayerSettings.multiplayerSettings.maxPlayers + ")");
            if (playersInRoom > 1) {
                readyToCount = true;
            }
            if (playersInRoom == MultiplayerSettings.multiplayerSettings.maxPlayers) {
                readyToStart = true;
                if (!PhotonNetwork.IsMasterClient)
                    return;
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
        }

        // Check JSON file
        AllPlayerData everything = JsonUtility.FromJson<AllPlayerData>(File.ReadAllText(playerDataFile));
        if (everything != null) {
            foreach (PlayerData p in everything.data) {
                // Check if player has data stored
                if (p.nickname.Equals(newPlayer.NickName)) {
                    // Load JSON to Player CustomProperties
                    LoadPlayerInfo(p, newPlayer);
                }
            }
        }
    }

    void DisconnectGame() {
        SceneManager.LoadScene(0);
    }

    void LoadPlayerInfo(PlayerData player, Player newPlayer) {
        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();

        hash["Nickname"] = player.nickname;
        hash["Health"] = player.health;

        newPlayer.SetCustomProperties(hash);

        Debug.Log("Loaded new player...");
        Debug.Log(hash["Health"].ToString());
        Debug.Log(hash["Nickname"].ToString());
    }

    void SavePlayerInfo(Player player) {
        ExitGames.Client.Photon.Hashtable _info = player.CustomProperties;

        Debug.Log(_info["Health"].ToString());
        Debug.Log(_info["Nickname"].ToString());

        SaveIntoJson(_info["Nickname"].ToString(), Int32.Parse(_info["Health"].ToString()));
    }
    private void SaveIntoJson(string nickname, int health) {
        PlayerData pd = new PlayerData(nickname, health);

        AllPlayerData everything = JsonUtility.FromJson<AllPlayerData>(File.ReadAllText(playerDataFile));
        int counter = 1;

        PlayerData[] ajuda;

        if (everything != null) {
            counter += everything.data.Length;
            bool hasAlready = false;

            // Search for duplicates
            foreach (PlayerData p in everything.data) { if (p.nickname.Equals(pd.nickname)) hasAlready = true; }

            // Fill the array
            if (hasAlready) { counter--; ajuda = new PlayerData[counter]; } else { ajuda = new PlayerData[counter]; ajuda[counter - 1] = pd; }

            for (int i = 0; i < everything.data.Length; ++i) { ajuda[i] = everything.data[i]; }
        } else { ajuda = new PlayerData[counter]; ajuda[0] = pd; }
        AllPlayerData elFinal = new AllPlayerData(ajuda);
        string player = JsonUtility.ToJson(elFinal);

        File.WriteAllText(playerDataFile, player);
    }

    private void RestartTimer() {
        lessThanMaxPlayers = startingTime;
        timeToStart = startingTime;
        atMaxPlayer = 6;
        readyToCount = false;
        readyToStart = false;
    }

    // Comecar o jogo
    private void StartGame() {
        isGameLoaded = true;
        if (!PhotonNetwork.IsMasterClient)
            return;
        if (MultiplayerSettings.multiplayerSettings.delayStart) {
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
        PhotonNetwork.LoadLevel(MultiplayerSettings.multiplayerSettings.multiplayerScene); 
    }

    private void OnSceneFinishedLoading (Scene scene, LoadSceneMode mode) {
        currentScene = scene.buildIndex;
        if (currentScene == MultiplayerSettings.multiplayerSettings.multiplayerScene) {
            isGameLoaded = true;
            if (MultiplayerSettings.multiplayerSettings.delayStart) {
                PV.RPC("RPC_LoadedGameScene", RpcTarget.MasterClient);
            } else {
                RPC_CreatePlayer();
            }
        }
    }

    [PunRPC]
    private void RPC_LoadedGameScene() {
        playerInGame++;
        if (playerInGame == PhotonNetwork.PlayerList.Length) {
            PV.RPC("RPC_CreatePlayer", RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPC_CreatePlayer() {
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonNetworkPlayer"), transform.position, Quaternion.identity, 0);
    }


}

[Serializable]
public class PlayerData {
    public string nickname;
    public int health;

    public PlayerData (string nickname, int health) {
        this.nickname = nickname;
        this.health = health;
    }
}

[Serializable]
public class AllPlayerData {
    public PlayerData[] data;

    public AllPlayerData(PlayerData[] data) {
        this.data = data;
    }
}
