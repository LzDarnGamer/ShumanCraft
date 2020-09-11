using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
public class QuickStartLobbyController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject quickstatButton;

    [SerializeField]
    private GameObject quickendButton;

    [SerializeField]
    private int RoomSize;

    [SerializeField]
    private string RoomName;

    [SerializeField]
    private int RoomNumber;


    public override void OnConnectedToMaster() {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message) {
        Debug.Log("Failed to join a room");
        CreateRoom(RoomSize, RoomName, RoomNumber);
    }

    private void CreateRoom(int maxplayers, string roomName, int RoomNumber) {
        Debug.Log("Creating a room");
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)maxplayers };
        PhotonNetwork.CreateRoom(roomName + " #" + RoomNumber, roomOps);
        
    }


    public override void OnCreateRoomFailed(short returnCode, string message) {
        Debug.Log("Failed to create a room... trying again");
        CreateRoom(RoomNumber, RoomName, RoomNumber);
    }


}
