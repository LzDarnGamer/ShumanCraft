using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotonLobby : MonoBehaviourPunCallbacks {

    public static PhotonLobby lobby;

    [SerializeField] private List<string> roomNames;
    
    [Header ("Buttons")]
    public GameObject battleButton, cancelButton, createButton, joinButton;

    [Header ("Lobby window")]
    public GameObject joinUI, content, roomObj;

    [Header ("Connecting Text")]
    public Text connectText;

    private void Awake() {
        lobby = this;
        roomNames = new List<string>();
    }

    void Start() {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() {
        Debug.Log("We are now connected to the " + PhotonNetwork.CloudRegion + " Server");
        PhotonNetwork.AutomaticallySyncScene = true;
        battleButton.GetComponent<Button>().interactable = true;
        connectText.text = "Connected";
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList) {
        Debug.Log("Room List Updated: " + roomList.ToArray().Length);

        foreach (RoomInfo game in roomList) {
            if (!roomNames.Contains(game.Name)) {
                GameObject sala = Instantiate(roomObj) as GameObject;
                sala.transform.parent = content.transform;
                Text[] atributos = sala.GetComponentsInChildren<Text>();
                atributos[0].text = game.Name;
                atributos[1].text = game.PlayerCount.ToString();
                atributos[2].text = game.MaxPlayers.ToString();
                roomNames.Add(game.Name);
            } else {
                
            }
        }
    }

    public void OnCreateLobbyButtonClicked () {
        
    }

    public void OnJoinLobbyButtonClicked () {
        joinUI.SetActive(true);
    }

    public void OnJoinButtonClicked() {
        Debug.Log("Battle Button was clicked");
        battleButton.GetComponent<Button>().interactable = true;
        cancelButton.SetActive(true);
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnCancelButtonClicked() {
        Debug.Log("Cancel Button was clicked");
        cancelButton.SetActive(false);
        battleButton.GetComponent<Button>().interactable = true;
        PhotonNetwork.LeaveRoom();
    }

    public void OnCreateButtonClicked() {
        Debug.Log("Create Button was clicked");
        cancelButton.SetActive(true);
        battleButton.GetComponent<Button>().interactable = false;
        CreateRoom();
    }

    public override void OnJoinedRoom() {
        Debug.Log("We are now in a room " + PhotonNetwork.InLobby);
    }

    public override void OnJoinedLobby() {
        Debug.Log("We are now in a lobby");
    }

    public override void OnJoinRandomFailed(short returnCode, string message) {
        Debug.Log("Tried to join a random game but failed. There must be no open games available");
        CreateRoom();
    }

    private void CreateRoom (string roomName = null) { 
        Debug.Log("Trying to create room...");
        int randomRoomName = Random.Range(0, 10000);
        RoomOptions roomOps = new RoomOptions() {IsVisible = true, MaxPlayers = (byte)MultiplayerSettings.multiplayerSettings.maxPlayers};
        PhotonNetwork.CreateRoom("Room" + randomRoomName, roomOps);
    }

    public override void OnCreateRoomFailed(short returnCode, string message) {
        Debug.Log("Error creating a room");
        CreateRoom();
    }
}
