using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhotonLobby : MonoBehaviourPunCallbacks {

    public static PhotonLobby lobby;

    [SerializeField] private List<string> roomNames;

    [Header("Login System")]
    public GameObject loginSystem;
    public TMP_InputField loginField;

    [Header("Buttons")]
    public GameObject battleButton;
    public GameObject cancelButton;

    [Header("Join Lobby Window")]
    public GameObject joinUI;
    public GameObject content;
    public GameObject roomObj;

    [Header("Connecting Text")]
    public Text connectText;
    public GameObject joinButton;

    [Header("Create Lobby Window")]
    public GameObject createUI;
    public GameObject createUIPanel;
    public GameObject createButton;
    public TMP_InputField createField;
    public GameObject createOKButton;

    public UnityEngine.Audio.AudioMixer mixer;


    private void Awake() {
        lobby = this;
        roomNames = new List<string>();

        float savedVol = PlayerPrefs.GetFloat("volume", 1);
        SetVolume(savedVol);
        int value = PlayerPrefs.GetInt("quality", 3);
        setQuality(value);
    }

    void Start() {
        PhotonNetwork.ConnectUsingSettings();
        createUIPanel.transform.localScale = new Vector3(0, 0, 0);
        joinUI.transform.localScale = new Vector3(0, 0, 0);
    }

    public override void OnConnectedToMaster() {

        Debug.Log("We are now connected to the " + PhotonNetwork.CloudRegion + " Server");
        PhotonNetwork.AutomaticallySyncScene = true;
        //setOnlineUI();
        connectText.text = "Connected";
        loginSystem.SetActive(true);
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public void loginBtnClicked() {
        PhotonNetwork.NickName = loginField.text;
        setOnlineUI();
    }

    private void setOnlineUI() {
        battleButton.GetComponent<Button>().interactable = true;
        createButton.GetComponent<Button>().interactable = true;
        joinButton.GetComponent<Button>().interactable = true;
        //connectText.text = "Connected";
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList) {
        Debug.Log("Room List Updated: " + roomList.ToArray().Length);

        foreach (Transform child in content.transform) Destroy(child.gameObject);

        foreach (RoomInfo game in roomList) {
            Debug.Log("Got in FOR");
            if (!roomNames.Contains(game.Name)) {
                Debug.Log("Got in contains");
                GameObject sala = Instantiate(roomObj) as GameObject;
                sala.transform.parent = content.transform;
                sala.transform.localScale = new Vector3(1, 1, 1);
                TMP_Text[] atributos = sala.GetComponentsInChildren<TMP_Text>();
                atributos[0].text = game.Name;
                atributos[1].text = game.PlayerCount.ToString();
                roomNames.Add(game.Name);

                Button btn = sala.GetComponent<Button>();

                if (game.PlayerCount != game.MaxPlayers)
                    btn.onClick.AddListener(delegate { OnRoomButtonClicked(game.Name); });
            } else {

            }
        }
    }

    public void OnCreateButtonOKClicked() {
        string _roomName = createField.text;
        CreateRoom(_roomName);
    }

    public void OnRoomButtonClicked(string name) {
        PhotonNetwork.JoinRoom(name);
        Debug.Log("Room " + name + " clicked!");
    }

    public void OnCreateLobbyButtonClicked() {
        Debug.Log("Create Button was clicked");
        createUI.SetActive(true);
        if (joinUI.activeInHierarchy) {
            StartCoroutine(delete(joinUI));
            LeanTween.scale(joinUI, new Vector3(0, 0, 0), 0.8f);
        }
        LeanTween.scale(createUIPanel, new Vector3(1, 1, 1), 0.8f);
    }

    public void OnJoinLobbyButtonClicked() {
        Debug.Log("Create Button was clicked");
        joinUI.SetActive(true);
        if (createUI.activeInHierarchy) {
            StartCoroutine(delete(createUI));
            LeanTween.scale(createUI, new Vector3(0, 0, 0), 0.8f);
        }
        LeanTween.scale(joinUI, new Vector3(1, 1, 1), 0.8f);
    }

    IEnumerator delete(GameObject a) {
        yield return new WaitForSeconds(2);
        a.SetActive(false);
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

    private void CreateRoom(string roomName = null) {
        Debug.Log("Trying to create room...");
        int randomRoomName = Random.Range(0, 10000);
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, MaxPlayers = (byte)MultiplayerSettings.multiplayerSettings.maxPlayers };
        if (roomName == null) PhotonNetwork.CreateRoom("Room" + randomRoomName, roomOps);
        else PhotonNetwork.CreateRoom(roomName, roomOps);
    }

    public override void OnCreateRoomFailed(short returnCode, string message) {
        Debug.Log("Error creating a room");
        CreateRoom();
    }

    public void OnMouseHover(GameObject btn) {
        LeanTween.scale(btn, new Vector3(1.2f, 1.2f, 1.2f), 0.2f);
    }
    public void OnMouseExit(GameObject btn) {
        LeanTween.scale(btn, new Vector3(1, 1, 1), 0.2f);
    }

    public void UpdateQuality(Dropdown dropdown) {
        setQuality(dropdown.value);
    }

    private void setQuality(int value) {
        switch (value) {
            case 0:
            QualitySettings.SetQualityLevel(0, true);
            break;
            case 1:
            QualitySettings.SetQualityLevel(1, true);
            break;
            case 2:
            QualitySettings.SetQualityLevel(2, true);
            break;
            case 3:
            QualitySettings.SetQualityLevel(3, true);
            break;
            case 4:
            QualitySettings.SetQualityLevel(4, true);
            break;
            case 5:
            QualitySettings.SetQualityLevel(5, true);
            break;
            default:
            Debug.Log("Button does not change the quality settings!");
            break;
        }
        PlayerPrefs.SetInt("quality", value);
        PlayerPrefs.Save();
    }

    public void OnSliderChanged(Slider s) {
        SetVolume(s.value);
    }

    public float ConvertToDecibel(float value) {
        return Mathf.Log10(value) * 20f;
    }
    
    private void SetVolume(float s) {
        mixer.SetFloat("Volume", ConvertToDecibel(s)); 
        PlayerPrefs.SetFloat("volume", s);
        PlayerPrefs.Save();
    }

    public void QuitGame() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}




