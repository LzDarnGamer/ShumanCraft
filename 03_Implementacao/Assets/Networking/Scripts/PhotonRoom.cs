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
    private string instanceDataFile;

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
        instanceDataFile = Application.persistentDataPath + "/InstanceData.json";

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

    public override void OnLeftRoom() {
        base.OnLeftRoom();
        if (PhotonNetwork.IsMasterClient) SavePlayerInfo(PhotonNetwork.LocalPlayer);
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

        if (PhotonNetwork.IsMasterClient) {
            // Check JSON file
            if (File.Exists(playerDataFile)) {
                AllPlayerData everything = JsonUtility.FromJson<AllPlayerData>(File.ReadAllText(playerDataFile));
                bool a1 = false;
                if (everything != null) {
                    foreach (PlayerData p in everything.data) {
                        // Check if player has data stored
                        if (p.nickname.Equals(newPlayer.NickName)) {
                            // Load JSON to Player CustomProperties
                            LoadPlayerInfo(p, newPlayer);
                            a1 = true;
                        }
                    }
                }
                if (!a1) {
                    ExitGames.Client.Photon.Hashtable hash1 = new ExitGames.Client.Photon.Hashtable();
                    hash1["NotFound"] = 1;
                    newPlayer.SetCustomProperties(hash1);
                }
            } else {
                File.CreateText(playerDataFile);
                ExitGames.Client.Photon.Hashtable hash1 = new ExitGames.Client.Photon.Hashtable();
                hash1["NotFound"] = 1;
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash1);
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
        hash["Hunger"] = player.hunger;
        hash["PosX"] = player.x;
        hash["PosY"] = player.y;
        hash["PosZ"] = player.z;


        newPlayer.SetCustomProperties(hash);

        Debug.Log("Loaded new player: " + player.nickname);
        Debug.Log("----- Health: " + hash["Health"].ToString());
        Debug.Log("----- Hunger: " + hash["Hunger"].ToString());
        Debug.Log("----- Position X: " + hash["PosX"].ToString());
        Debug.Log("----- Position Y: " + hash["PosY"].ToString());
        Debug.Log("----- Position Z: " + hash["PosZ"].ToString());
        Debug.Log("--------------------------");
    }

    void SavePlayerInfo(Player player) {
        ExitGames.Client.Photon.Hashtable _info = player.CustomProperties;

        Debug.Log(_info["Nickname"].ToString());

        // Positions
        float x = float.Parse(_info["posX"].ToString());
        float y = float.Parse(_info["posY"].ToString());
        float z = float.Parse(_info["posZ"].ToString());

        // Player characteristics
        int health = Int32.Parse(_info["Health"].ToString());
        float hunger = float.Parse(_info["Hunger"].ToString());

        SaveIntoJson(_info["Nickname"].ToString(), health, x, y, z, hunger);

        if (_info["iName"] != null) {
            String n = _info["iName"].ToString();

            float ix = float.Parse(_info["iposX"].ToString());
            float iy = float.Parse(_info["iposY"].ToString());
            float iz = float.Parse(_info["iposZ"].ToString());

            SaveInstancesToJSON(n, ix, iy, iz);
        }
    }

    private void SaveInstancesToJSON(string n, float ix, float iy, float iz) {
        InstanceData id = new InstanceData(n, ix, iy, iz);


        if (File.Exists(instanceDataFile)) {
            AllInstanceData ev = JsonUtility.FromJson<AllInstanceData>(File.ReadAllText(instanceDataFile));

            int counter = 1;

            InstanceData[] ajuda;

            if (ev != null) {
                counter += ev.data.Length;
                bool hasAlready = false;

                // Search for duplicates
                foreach (InstanceData p in ev.data) {
                    if (p.x == id.x && p.y == id.y && p.y == id.y) {
                        // Atualizar valores da instancia
                        p.x = id.x;
                        p.y = id.y;
                        p.z = id.z;

                        hasAlready = true;
                    }
                }

                // Fill the array
                if (hasAlready) { counter--; ajuda = new InstanceData[counter]; } else { ajuda = new InstanceData[counter]; ajuda[counter - 1] = id; }

                for (int i = 0; i < ev.data.Length; ++i) { ajuda[i] = ev.data[i]; }
            } else { ajuda = new InstanceData[counter]; ajuda[0] = id; }
            AllInstanceData elFinal = new AllInstanceData(ajuda);
            string instance = JsonUtility.ToJson(elFinal);

            File.WriteAllText(instanceDataFile, instance);
        } else {
            File.CreateText(instanceDataFile);
            InstanceData[] ajuda = new InstanceData[1];
            ajuda[0] = id;

            AllInstanceData elFinal = new AllInstanceData(ajuda);
            string instance = JsonUtility.ToJson(elFinal);

            File.WriteAllText(instanceDataFile, instance);
        }
        
    }

    private void SaveIntoJson(string nickname, int health, float x, float y, float z, float hunger) {
        PlayerData pd = new PlayerData(nickname, health, x, y, z, hunger);

        if (File.Exists(playerDataFile)) {
            AllPlayerData everything = JsonUtility.FromJson<AllPlayerData>(File.ReadAllText(playerDataFile));
            int counter = 1;

            PlayerData[] ajuda;

            if (everything != null) {
                counter += everything.data.Length;
                bool hasAlready = false;

                // Search for duplicates
                foreach (PlayerData p in everything.data) {
                    if (p.nickname.Equals(pd.nickname)) {
                        // Atualizar valores do player
                        p.health = pd.health;
                        p.hunger = pd.hunger;

                        p.x = pd.x;
                        p.y = pd.y;
                        p.z = pd.z;

                        hasAlready = true;
                    }
                }

                // Fill the array
                if (hasAlready) { counter--; ajuda = new PlayerData[counter]; } else { ajuda = new PlayerData[counter]; ajuda[counter - 1] = pd; }

                for (int i = 0; i < everything.data.Length; ++i) { ajuda[i] = everything.data[i]; }

            } else { ajuda = new PlayerData[counter]; ajuda[0] = pd; }
            AllPlayerData elFinal = new AllPlayerData(ajuda);
            string player = JsonUtility.ToJson(elFinal);

            File.WriteAllText(playerDataFile, player);
        } else {
            File.CreateText(playerDataFile);
            PlayerData[] ajuda = new PlayerData[1];
            ajuda[0] = pd;

            AllPlayerData elFinal = new AllPlayerData(ajuda);
            string player = JsonUtility.ToJson(elFinal);

            File.WriteAllText(playerDataFile, player);
        }
        
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

        // Check JSON file
        string read = "";
        if (File.Exists(playerDataFile)) {
            read = File.ReadAllText(playerDataFile);
            AllPlayerData everything = JsonUtility.FromJson<AllPlayerData>(read);

            bool a2 = false;
            if (everything != null) {
                foreach (PlayerData p in everything.data) {
                    // Check if player has data stored
                    if (p.nickname.Equals(PhotonNetwork.LocalPlayer.NickName)) {
                        // Load JSON to Player CustomProperties
                        LoadPlayerInfo(p, PhotonNetwork.LocalPlayer);
                        a2 = true;
                    }
                }
            }

            if (!a2) {
                ExitGames.Client.Photon.Hashtable hash1 = new ExitGames.Client.Photon.Hashtable();
                hash1["NotFound"] = 1;
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash1);
            }
        } else {
            File.CreateText(playerDataFile);
            ExitGames.Client.Photon.Hashtable hash1 = new ExitGames.Client.Photon.Hashtable();
            hash1["NotFound"] = 1;
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash1);
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
    public float x, y, z;
    public float hunger;

    public PlayerData (string nickname, int health, float x, float y, float z, float hunger) {
        this.nickname = nickname;
        
        this.health = health;
        this.hunger = hunger;

        this.x = x;
        this.y = y;
        this.z = z;
    }
}

[Serializable]
public class AllPlayerData {
    public PlayerData[] data;

    public AllPlayerData(PlayerData[] data) {
        this.data = data;
    }
}

[Serializable]
public class InstanceData {
    public string name;
    public float x, y, z;

    public InstanceData (string name, float x, float y, float z) {
        this.name = name;

        this.x = x;
        this.y = y;
        this.z = z;
    }
}

[Serializable]
public class AllInstanceData {
    public InstanceData[] data;

    public AllInstanceData(InstanceData[] data) {
        this.data = data;
    }
}



