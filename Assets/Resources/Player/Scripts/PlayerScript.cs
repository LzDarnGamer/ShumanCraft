
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;
using com.ootii.Cameras;
using com.ootii.Input;
using System.IO;
using System.Collections;
using com.ootii.Utilities.Debug;
using Photon.Realtime;
using TMPro;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

public class PlayerScript : MonoBehaviour {

    private Animator anim;
    [SerializeField] private GameObject canvas;
    [SerializeField] private PhotonView PV;
    [SerializeField] private GameObject cam;
    [SerializeField] private bool isCamFixed = true;
    private float facing;
    [SerializeField] private GameObject inputSource;

    private GameObject instCam, instSource;

    public ConstructionController construction;

    public Interact interact;

    [Header("Save Game")]
    [SerializeField] private ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
    [SerializeField] private bool isLoaded = false;


    [Header("Sound")]
    [SerializeField] private SoundScipt soundScipt;

    [Header("Placeables React")]
    [SerializeField] private float fireplaceDist = 3f;
    [SerializeField] private float pitDist = 3f;

    [Header("Cenas de inventario")]
    [SerializeField] private MatrixInventory inventory;
    [SerializeField] private Hotbar_Manager hotbarManager;
    [SerializeField] private DisplayInventory displayInventory;
    [SerializeField] private int hotbarIndex;
    [SerializeField] private int prevHotbarIndex = -999;
    [SerializeField] private GameObject objectInHand;
    [SerializeField] private GameObject instantiatedObject;
    [SerializeField] private GameObject playerHand;
    [SerializeField] private bool isToolOn;
    [SerializeField] private PlayerAux auxRPC;
    ItemObject aux;
    List<GameObject> objectsAlreadyHere = new List<GameObject>();

    [Header("Movimentos")]
    private float x, y;
    [SerializeField] private bool isWalking = true;
    [SerializeField] private bool isRunning = false;
    [SerializeField] private bool isCrouched = false;
    [SerializeField] private CapsuleCollider playerCollider;
    [SerializeField] private float colliderHeight;
    [SerializeField] private float colliderCenterY;
    private AnimatorStateInfo currentBaseState;

   [Header("Caracteristicas")]
    [SerializeField] private string Playername;
    [SerializeField] [Range(0, 100)] private float health           = 100.0f;
    [SerializeField] [Range(0, 100)] private float stamina          = 100.0f;
    [SerializeField] [Range(0, 100)] private float staminaLoss      = 0.5f;
    [SerializeField] private bool isUsingStamina = false;
    [SerializeField] private bool isOutsideMap = false;
    [SerializeField] private bool firstTime = true;
    [SerializeField] [Range(0, 100)] private float hunger           = 100.0f;
    [SerializeField] [Range(0, 100)] private float hungerLoss       = 0.5f;
    [SerializeField] [Range(0, 100)] private float hungerThreshold  = 10.0f;
    [SerializeField] [Range(0, 100)] private float starvingDecay    = 2.0f;
    [SerializeField] private bool isStarving = false;
    [SerializeField] [Range(0, 100)] private float thirst           = 100.0f;
    [SerializeField] [Range(0, 100)] private float thirstLoss       = 1.5f;
    [SerializeField] [Range(0, 100)] private float thirstThreshold  = 30.0f;
    [SerializeField] [Range(0, 100)] private float thirstDecay      = 0.5f;
    [SerializeField] private bool needsWater                        = false;
    [SerializeField] [Range(0, 100)] private float voidDamage = 10f;

    [Header("Controles")]
    public KeyCode walkRunKey           = KeyCode.LeftShift;
    public KeyCode jumpKey              = KeyCode.Space;
    public KeyCode crouchKey            = KeyCode.LeftControl;
    public KeyCode startKey             = KeyCode.P;
    public KeyCode openChatKey          = KeyCode.T;
    public KeyCode openInventoryKey     = KeyCode.E;

    [Header("UI")]
    [SerializeField] private Image uiHealth;
    [SerializeField] private Image uiStamina;
    [SerializeField] private Image uiHunger;
    [SerializeField] private Image uiThirst;
    [SerializeField] private bool isOnline = true;

    [Header("Chat System")]
    [SerializeField] private GameObject chatPanel;
    [SerializeField] private TMP_InputField chatInput;
    public TMP_Text chatMsgTxt;
    public TMP_Text chatPlayer;
    [SerializeField] private PhotonView chatPublic;
    [SerializeField] private float chatCounter;

    private String instanceDataFile;
    private bool nearboat = false;

    private int PlayerMask = 1 << 8;

    private void Awake() {
        LoadSaveGame();
        instanceDataFile = Application.persistentDataPath + "/InstanceData.json";
    }

    void Start() {

        anim = gameObject.GetComponent<Animator>();
        auxRPC = gameObject.GetComponent<PlayerAux>();
        chatPublic = GameObject.FindGameObjectWithTag("ChatAux").GetComponent<PhotonView>();

        StartCoroutine(staminaCoroutineMinus());
        StartCoroutine(staminaCoroutineAdd());
        StartCoroutine(hungerCoroutine());
        StartCoroutine(thirstCoroutine());
        StartCoroutine(healthCoroutineStarving());
        StartCoroutine(healthCoroutineThirsth());
        StartCoroutine(healthOutsideMap());
        StartCoroutine(UpdateNPCNear());


        colliderHeight = playerCollider.height;
        colliderCenterY = playerCollider.center.y;

        if (isOnline) {
            if (PhotonNetwork.IsMasterClient) {
                if (File.Exists(instanceDataFile)) {
                    AllInstanceData instances = JsonUtility.FromJson<AllInstanceData>(File.ReadAllText(instanceDataFile));

                    if (instances != null) {
                        foreach (InstanceData i in instances.data) {
                            PhotonNetwork.Instantiate(Path.Combine("Scriptable Objects\\Items\\Prefabs\\Placeables", i.name), new Vector3(i.x, i.y, i.z), Quaternion.identity, 0);
                        }
                    }
                } else {
                    File.CreateText(instanceDataFile);
                }

                // Spawn NPCs
                PhotonNetwork.Instantiate(Path.Combine("NPC Spawner", "NPC Handler Isle 1"), new Vector3(87.25554f, 96.97235f, -79.15379f), Quaternion.identity, 0);
                PhotonNetwork.Instantiate(Path.Combine("NPC Spawner", "NPC Handler Isle Ice (1)"), new Vector3(42.8f, 96.97235f, -1068.2f), Quaternion.identity, 0);
                PhotonNetwork.Instantiate(Path.Combine("NPC Spawner", "NPC Handler Isle Ice"), new Vector3(87.25554f, 96.97235f, -79.15379f), Quaternion.identity, 0);

            }

            if (PV.IsMine) {
                StartManager();
            } else {
                DestroyImmediate(gameObject.GetComponent<ConstructionController>(), true);
                DestroyImmediate(canvas, true);
                DestroyImmediate(gameObject.GetComponent<Interact>(), true);
                DestroyImmediate(this, true);
            }
        } else {
            StartManager();
        }
    }

    public PhotonView GetPV() { return PV; }

    private void ChatSystem() {
        if (Input.GetKey(openChatKey)) {
            chatPanel.SetActive(true);
            EventSystem.current.SetSelectedGameObject(chatInput.gameObject);
        } else if (Input.GetKey(KeyCode.Escape)) {
            chatPanel.SetActive(false);
        }

        if (Input.GetKey(KeyCode.Return) && chatInput.isFocused && chatInput.text != "") {
            string aux = chatInput.text + "\n";
            auxRPC.handleChat(aux, chatPublic.ViewID);
            chatInput.text = "";
        }
    }

    private void StartManager() {
        construction = gameObject.GetComponent<ConstructionController>();
        AddCamera();
    }

    void FixedUpdate() {
        if (isOnline) {
            if (PV.IsMine) {
                PlayerManager();
                if (Input.GetKeyUp(KeyCode.H) && nearboat) {
                    // Teleport
                    transform.position = new Vector3(54.47f, 122.73f, -721.6f);
                }
            } else if (!PV.IsMine) {
                DestroyImmediate(instCam, true);
                DestroyImmediate(instSource, true);
            }
        } else {
            PlayerManager();
        }
    }
    bool canspawn = false;
    private void PlayerManager() {
        hotbarIndex = hotbarManager.getIndex();
        
        if (inventory.getHotbar()[hotbarIndex].item != null) {
            objectInHand = inventory.getHotbar()[hotbarIndex].item.inGameObject;
            aux = inventory.getHotbar()[hotbarIndex].item;
        } else {
            canspawn = true;
            objectInHand = null;
            if (instantiatedObject != null) {
                Destroy(instantiatedObject);
            }
        }


        if (objectInHand != null &&
            (prevHotbarIndex == -999 || hotbarIndex != prevHotbarIndex || canspawn)) {

            canspawn = false;
            isToolOn = true;
            if (isOnline) {
                if (instantiatedObject != null) {
                    PhotonNetwork.Destroy(instantiatedObject);
                }

                bool has2 = false;
                
                if (!has2) {
                    if (aux != null && aux.type == ItemType.Weapons) {
                        instantiatedObject = PhotonNetwork.Instantiate(Path.Combine("Scriptable Objects\\Items\\Prefabs\\Weapons", objectInHand.name), playerHand.transform.position, Quaternion.identity, 0);
                    
                    } else if (aux != null && aux.type == ItemType.Foods) {
                        instantiatedObject = PhotonNetwork.Instantiate(Path.Combine("Scriptable Objects\\Items\\Prefabs\\Food", objectInHand.name),
                                                            playerHand.transform.position,
                                                            Quaternion.identity, 0);
                    } else if (aux != null && aux.type == ItemType.Tools) {
                        instantiatedObject = PhotonNetwork.Instantiate(Path.Combine("Scriptable Objects\\Items\\Prefabs\\Tool", objectInHand.name), playerHand.transform.position, Quaternion.identity, 0);

                    } else if (aux != null && aux.type == ItemType.Materials) {
                        instantiatedObject = PhotonNetwork.Instantiate(Path.Combine("Scriptable Objects\\Items\\Prefabs\\Material", objectInHand.name),
                                                            playerHand.transform.position,
                                                            Quaternion.identity, 0);

                    } else if (aux != null && aux.type == ItemType.Placeables) {
                        instantiatedObject = PhotonNetwork.Instantiate(Path.Combine("Scriptable Objects\\Items\\Prefabs\\Placeables", objectInHand.name),
                                                            playerHand.transform.position,
                                                            Quaternion.identity, 0);
                    }
                }                 

                float[] p = {   
                                objectInHand.transform.position.x,
                                objectInHand.transform.position.y,
                                objectInHand.transform.position.z
                            };

                float[] r = { 
                                objectInHand.transform.rotation.x,
                                objectInHand.transform.rotation.y,
                                objectInHand.transform.rotation.z,
                                objectInHand.transform.rotation.w
                            };

                auxRPC.runRPC(objectInHand.name,
                                    playerHand.GetPhotonView().ViewID,
                                    instantiatedObject.GetPhotonView().ViewID,
                                    p,
                                    r);
            } else {
                if (instantiatedObject != null) Destroy(instantiatedObject);
                instantiatedObject = Instantiate(objectInHand, playerHand.transform);
            }
            prevHotbarIndex = hotbarIndex;
        } else if (objectInHand == null) {
            isToolOn = false;
        }

        if (instCam != null && isCamFixed) { 
            facing = instCam.transform.eulerAngles.y;
            transform.eulerAngles = new Vector3(0, facing, 0);
        }
        UIControl();
        Movement();
        Attributes();
        UseHand();
        PickUpItem();
        ChatSystem();
    }

    void HashUpdate() {
        if (isLoaded) {
            if (hash.ContainsKey("Nickname")) { hash["Nickname"] = PhotonNetwork.LocalPlayer.NickName; } else { hash.Add("Nickname", PhotonNetwork.LocalPlayer.NickName); }
            if (hash.ContainsKey("Health")) { hash["Health"] = health; } else { hash.Add("Health", health); }
            if (hash.ContainsKey("Hunger")) { hash["Hunger"] = hunger; } else { hash.Add("Hunger", hunger); }
            if (hash.ContainsKey("posX")) { hash["posX"] = this.transform.position.x; } else { hash.Add("posX", this.transform.position.x); }
            if (hash.ContainsKey("posY")) { hash["posY"] = this.transform.position.y; } else { hash.Add("posY", this.transform.position.y); }
            if (hash.ContainsKey("posZ")) { hash["posZ"] = this.transform.position.z; } else { hash.Add("posZ", this.transform.position.z); }            

            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public void InstantiateBridge() {
        if (GameObject.FindGameObjectWithTag("Bridge") == null) {
            PhotonNetwork.Instantiate(Path.Combine("Scriptable Objects\\Items\\Prefabs\\StoryLine", "bridge"),
                                                            new Vector3(57.6f, 115.77f, -66.4f),
                                                            Quaternion.Euler(-70.11f, 26.494f, 90.00001f), 0);
        }
    }

    public void InstantiateBoat() {
        if (GameObject.FindGameObjectWithTag("Boat") == null) {
            PhotonNetwork.Instantiate(Path.Combine("Scriptable Objects\\Items\\Prefabs\\StoryLine", "boat"),
                                                                new Vector3(47.43024f, 118.0576f, -255.4927f),
                                                                Quaternion.Euler(0.0f, 90.0f, 0.0f), 0);
            PhotonNetwork.Instantiate(Path.Combine("Scriptable Objects\\Items\\Prefabs\\StoryLine", "boatBack"),
                                                                new Vector3(47.43024f, 118.0576f, -714.6508f),
                                                                Quaternion.Euler(0.0f, -90.0f, 0.0f), 0);
        }
    }

    void LoadSaveGame() {
        ExitGames.Client.Photon.Hashtable _hash = PhotonNetwork.LocalPlayer.CustomProperties;
        if (_hash != null) {
            if (_hash["NotFound"] != null) {
                isLoaded = true;
                return;
            }
            if (_hash["Health"] != null) {
                Debug.Log("Load Game");
                Debug.Log("----- Health: " + _hash["Health"].ToString());
                Debug.Log("----- Hunger: " + _hash["Hunger"].ToString());
                Debug.Log("----- Position X" + _hash["PosX"].ToString());
                Debug.Log("----- Potition Y" + _hash["PosY"].ToString());
                Debug.Log("----- Position Z" + _hash["PosZ"].ToString());
                isLoaded = true;
            }

            // Apply load game to this character
            health = (_hash["Health"] != null) ? Int32.Parse(_hash["Health"].ToString()) : 100.0f;
            hunger = (_hash["Hunger"] != null) ? float.Parse(_hash["Hunger"].ToString()) : 100.0f;
            if ((_hash["PosX"] != null) && (_hash["PosY"] != null) && (_hash["PosZ"] != null)) {
                transform.position = new Vector3(   float.Parse(_hash["PosX"].ToString()),
                                                    float.Parse(_hash["PosY"].ToString()),
                                                    float.Parse(_hash["PosZ"].ToString())   );
            }

        }

        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    

    private void UseHand() {
        
        if (Input.GetMouseButtonDown(0)) {
            if (instantiatedObject == null) {
                anim.SetTrigger("Punch");
            } else if (instantiatedObject.GetComponent<Item>().item.type == ItemType.Tools) {
                anim.SetTrigger("Use");
            } else if (instantiatedObject.GetComponent<Item>().item.type == ItemType.Weapons) {
                anim.SetTrigger("Stab");
            }

        }
    }

    private void atackNPC() {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)), out RaycastHit ray, 5f, ~PlayerMask)) {
            if (ray.collider.gameObject.CompareTag("NPC")) {
                int damage = ((WeaponObject)instantiatedObject.GetComponent<Item>().item).damagePoints;
                ray.collider.gameObject.GetComponent<NPC_Animal>().TakeDamage(damage);
            }
        }
    }

    private void mineObject() {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)), out RaycastHit ray, 5f, ~PlayerMask)) {
            switch (ray.collider.gameObject.tag) {
                case "Tree":
                    ray.collider.gameObject.GetComponent<MineTree>().RaycastHit(instantiatedObject.GetComponent<Item>(), ray.point);
                    break;
                case "Ore":
                    ray.collider.gameObject.GetComponent<MineOre>().RaycastHit(instantiatedObject.GetComponent<Item>(), ray.point);
                    break;
            }
        }
    }

    private void AddCamera() {
        instCam = Instantiate(cam, this.gameObject.transform);
        instSource = Instantiate(inputSource, this.gameObject.transform);
        construction.setCamera(instCam.GetComponentInChildren<Camera>());

        if (isCamFixed) gameObject.transform.eulerAngles = new Vector3(0, facing, 0);
        CameraController controller = instCam.GetComponent<CameraController>();

        controller.Anchor = this.gameObject.transform;
        controller.InputSource = instSource.GetComponent<IInputSource>();

        CameraMotor mainMotor = controller.GetMotor("3rd Person Fixed");
        mainMotor.UseRigAnchor = false;
        mainMotor.Anchor = gameObject.transform;
        mainMotor.AnchorOffset = new Vector3(0.0f, 0.9f, 0.0f);

        CameraMotor motor = controller.GetMotor("Targeting");
        motor.UseRigAnchor = false;
        motor.Anchor = gameObject.transform;
        motor.AnchorOffset = new Vector3(0.0f, 1.3f, 0.0f);
    }

    public Camera getCamera() { return cam.GetComponentInChildren<Camera>(); }





    private void Attributes() {
        /* Stamina */
        isRunning = !isWalking;
        if (isRunning && (x != 0 || y != 0)) { isUsingStamina = true; } //stamina -= staminaLoss * Time.deltaTime;
        else if ((!isRunning || (x == 0 || y == 0)) && stamina < 100f) { isUsingStamina = false; } //stamina += (staminaLoss / 2) * Time.deltaTime;
        if(stamina < 21f && stamina > 17f && isUsingStamina) {
            StartCoroutine(soundScipt.PlayTired());
        }
        /* Hunger */
        if (hunger < hungerThreshold) isStarving = true; else isStarving = false;

        // [FALTA] Mostrar Alerta no ecra

        /* Thirst */
        if (thirst < thirstThreshold) needsWater = true; else needsWater = false;

        // [FALTA] Mostrar Alerta no ecra
    }


    private IEnumerator healthOutsideMap() {
        while (true) {
            if (isOutsideMap && health > 0) {
                health -= voidDamage;
                yield return new WaitForSeconds(0.5f);
            } else {
                yield return null;
            }
        }
    }

    private IEnumerator healthCoroutineStarving() {
        while (true) {
            if (isStarving && health > 0) {
                health -= starvingDecay;
                yield return new WaitForSeconds(1);
            } else {
                yield return null;
            }
        }
    }

    private IEnumerator healthCoroutineThirsth() {
        while (true) {
            if (needsWater && health > 0) {
                health -= thirstDecay;
                yield return new WaitForSeconds(1);
            } else {
                yield return null;
            }
        }
    }

    private IEnumerator staminaCoroutineAdd () {
        while (true) {
            if (!isUsingStamina && stamina < 100) {
                stamina += staminaLoss / 2;
                yield return new WaitForSeconds(0.2f);
            } else {
                yield return null;
            }
            
        }
    }

    private IEnumerator staminaCoroutineMinus() {
        while (true) {
            if (isUsingStamina && stamina > 0) {
                stamina -= staminaLoss;
                yield return new WaitForSeconds(0.2f);
            } else {
                yield return null;
            }
            
        }
    }

    private IEnumerator hungerCoroutine() {
        while (true) {
            if (hunger > 0) { 
                hunger -= hungerLoss; 
                yield return new WaitForSeconds(1);
            } else { 
                yield return null;
            }
        }
    }

    private IEnumerator thirstCoroutine() {
        while (true) {
            if (thirst > 0) {
                thirst -= thirstDecay;
                yield return new WaitForSeconds(1);
            } else {
                yield return null;
            }
        }
    }

    private void Movement() {
        int jumpState = Animator.StringToHash("Base Layer.JumpRunning");
        currentBaseState = anim.GetCurrentAnimatorStateInfo(0);
        if (currentBaseState.nameHash == jumpState) {
            float animHeight = anim.GetFloat("ColliderHeight");
            float animOffset = anim.GetFloat("ColliderOffset");

            //playerCollider.height = colliderHeight * animHeight;
            playerCollider.center = new Vector3(playerCollider.center.x,
                                                colliderCenterY * animOffset,
                                                playerCollider.center.z);
        }

        if (Input.GetKey(walkRunKey) && stamina > 0) isWalking = false;
        else isWalking = true;

        if (stamina <= 0) this.isWalking = true;
        if (Input.GetKeyUp(crouchKey)) isCrouched = !isCrouched;
        if (Input.GetKeyUp(jumpKey) && isRunning && (x != 0 || y != 0)) anim.SetTrigger("RunningJump");
        else if (Input.GetKeyUp(jumpKey) && (x == 0 || y == 0)) anim.SetTrigger("JumpSteady");

        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");

        if (instantiatedObject != null) {
            if (instantiatedObject.GetComponent<Item>().item.type == ItemType.Weapons) {
                anim.SetBool("IsHoldingSpear", true);
            } else {
                anim.SetBool("IsHoldingSpear", false);
            }
        } 
        if (isWalking) {
            float newX = (float)(this.x * .5);
            float newY = (float)(this.y * .5);
            x = newX;
            y = newY;
        }

        // Set animator variables
        anim.SetFloat("VelX", x, 1f, Time.deltaTime * 10f);
        anim.SetFloat("VelY", y, 1f, Time.deltaTime * 10f);
        anim.SetBool("isCrouched", isCrouched);
    }

    private void UIControl() {
        //Debug.Log("UI CONTROL");
        uiHealth.fillAmount = health / 100.0f;
        uiStamina.fillAmount = stamina / 100.0f;
        uiHunger.fillAmount = hunger / 100.0f;
        uiThirst.fillAmount = thirst / 100.0f;
    }

    private void PickUpItem() {
        if (Input.anyKeyDown) {

            if (Input.GetKeyDown(KeyCode.Mouse1)) {
                if (Eat()) {
                    displayInventory.UpdateDisplay();
                } else if (Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)), out RaycastHit ray
                     , 6f, ~PlayerMask)) {
                    GameObject hitObj = ray.transform.gameObject;
                    Debug.Log(hitObj.name);
                    if (hitObj.CompareTag("Pickupable")) {
                        ItemObject it = ray.transform.GetComponent<Item>().item;
                        if (displayInventory.PickupItem(it, 1)) {
                            Destroy(hitObj);
                        }
                    }

                }
            }

        }
    }
    public void activateCollider() {
        if (instantiatedObject != null) {
            instantiatedObject.GetComponent<BoxCollider>().enabled = !instantiatedObject.GetComponent<BoxCollider>().enabled;
        }
    }


    public bool Eat() {
        if (inventory.getHotbar()[hotbarManager.scrollPosition].item != null && typeof(FoodObject).IsInstanceOfType
              (inventory.getHotbar()[hotbarManager.scrollPosition].item)) {
            FoodObject Foodobj = (FoodObject)inventory.getHotbar()[hotbarManager.scrollPosition].item;
            addHealth(Foodobj.healthRestore);
            addHunger(Foodobj.hungerRestore);
            addThirst(Foodobj.thirstRestore);
            inventory.getHotbar()[hotbarManager.scrollPosition].RemoveAmount(1);
            //Debug.Log("HERE");
            return true;
        }
        return false;
    }

    public void GotBitten(float amount) { health -= amount; }
    public void addHealth(float amount) { if (health < 100.0f) health += amount; else if (health > 100.0f) health = 100.0f; }
    public void addThirst(float amount) { if (thirst < 100.0f) thirst += amount; else if (thirst > 100.0f) thirst = 100.0f; }
    public void addHunger(float amount) { hunger += amount; }

    private GameObject GetClosestNPC() {
        float closestNPCdistance = float.MaxValue;
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");
        GameObject closestNPC = null;
        for (int i = 0; i < npcs.Length; ++i) {
            Vector3 ajuda = transform.position - npcs[i].transform.position;
            //distToPlayers.Add(ajuda);
            if (ajuda.magnitude < closestNPCdistance) {
                closestNPCdistance = ajuda.magnitude;
                closestNPC = npcs[i];
            }
        }
        return closestNPC;
    }

    private void FireplaceHandler() {
        GameObject[] fireplaces = GameObject.FindGameObjectsWithTag("Fireplace");
        for (int i = 0; i < fireplaces.Length; ++i) {
            Vector3 ajuda = transform.position - fireplaces[i].transform.position;
            if (ajuda.magnitude < fireplaceDist) {
                addHealth(1.5f);
            }
        }
    }

    private void PitHandler() {
        GameObject[] pits = GameObject.FindGameObjectsWithTag("Pit");
        for (int i = 0; i < pits.Length; ++i) {
            Vector3 ajuda = transform.position - pits[i].transform.position;
            if (ajuda.magnitude < pitDist) {
                addThirst(1.5f);
            }
        }
    }
    IEnumerator UpdateNPCNear() {
        while (true) {
            FireplaceHandler();
            PitHandler();
            GameObject n = GetClosestNPC();
            if (n != null && Vector3.Distance(transform.position, n.transform.position) < 3f && n.GetComponent<NPC_Animal>().IsChaser()) GotBitten(4f);
            if (chatPublic != null && chatPublic.GetComponent<TMP_Text>()!= null) chatMsgTxt.text = chatPublic.GetComponent<TMP_Text>().text;
            if (!isLoaded) LoadSaveGame();

            HashUpdate();
            yield return new WaitForSeconds(1);
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.gameObject.tag.Equals("Boat")) {
            Debug.Log("Player - Near boat");
            nearboat = true;

        }
    }
}
