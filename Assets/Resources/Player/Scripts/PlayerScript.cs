﻿
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;
using com.ootii.Cameras;
using com.ootii.Input;
using System.IO;
using System.Collections;
using com.ootii.Utilities.Debug;

public class PlayerScript : MonoBehaviour {

    private Animator anim;
    [SerializeField]
    private GameObject canvas;
    [SerializeField]
    private PhotonView PV;
    [SerializeField]
    private GameObject cam;
    [SerializeField]
    private bool isCamFixed = true;
    private float facing;
    [SerializeField] private GameObject inputSource;

    private GameObject instCam, instSource;

    public ConstructionController construction;

    public Interact interact;

    [Header("Achivement")]
    [SerializeField] private AchivementLog achivementLog;

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

    [Header("Movimentos")]
    private float x, y;
    [SerializeField] private bool isWalking = true;
    [SerializeField] private bool isRunning = false;
    [SerializeField] private bool isCrouched = false;
    [SerializeField] private Collider playerCollider;

    [Header("Caracteristicas")]
    [SerializeField] private string name;
    [SerializeField] [Range(0, 100)] private float health           = 100.0f;
    [SerializeField] [Range(0, 100)] private float stamina          = 100.0f;
    [SerializeField] [Range(0, 100)] private float staminaLoss      = 10.0f;
    [SerializeField] [Range(0, 100)] private float hunger           = 100.0f;
    [SerializeField] [Range(0, 100)] private float hungerLoss       = 0.5f;
    [SerializeField] [Range(0, 100)] private float hungerThreshold  = 10.0f;
    [SerializeField] [Range(0, 100)] private float starvingDecay    = 2.0f;
    [SerializeField] private bool isStarving = false;
    [SerializeField] [Range(0, 100)] private float thirst           = 100.0f;
    [SerializeField] [Range(0, 100)] private float thirstLoss       = 1.5f;
    [SerializeField] [Range(0, 100)] private float thirstThreshold  = 30.0f;
    [SerializeField] [Range(0, 100)] private float thirstDecay      = 5.0f;
    [SerializeField] private bool needsWater                        = false;

    [Header("Controles")]
    public KeyCode walkRunKey = KeyCode.LeftShift;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode startKey = KeyCode.P;
    public KeyCode openChatKey = KeyCode.T;
    public KeyCode openInventoryKey = KeyCode.E;
    public KeyCode openConstructionkey = KeyCode.R;
    public KeyCode changeConstructionKey = KeyCode.T;

    [Header("UI")]
    [SerializeField] private Image uiHealth;
    [SerializeField] private Image uiStamina;
    [SerializeField] private Image uiHunger;
    [SerializeField] private Image uiThirst;
    [SerializeField] private bool isOnline = false;

    private IEnumerator staminaMinus, staminaPlus, hungerC, thirstC, healthCS, healthCT;

    void Start() {
        anim = gameObject.GetComponent<Animator>();
        auxRPC = gameObject.GetComponent<PlayerAux>();
        staminaMinus = staminaCoroutineMinus();
        staminaPlus = staminaCoroutineAdd();
        hungerC = hungerCoroutine();
        thirstC = thirstCoroutine();
        healthCS = healthCoroutineStarving();
        healthCT = healthCoroutineThirsth();

        if (isOnline) {
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

    private void StartManager() {
        construction = gameObject.GetComponent<ConstructionController>();
        AddCamera();
    }

    void FixedUpdate() {
        if (isOnline) {
            if (PV.IsMine) {
                PlayerManager();
            } else if (!PV.IsMine) {
                DestroyImmediate(instCam, true);
                DestroyImmediate(instSource, true);
                
            }
        } else {
            PlayerManager();
        }
    }

    private void PlayerManager() {
        hotbarIndex = hotbarManager.getIndex();

        if (inventory.getHotbar()[hotbarIndex].item != null)
            objectInHand = inventory.getHotbar()[hotbarIndex].item.inGameObject;
        else {
            objectInHand = null;
            if (instantiatedObject != null) {
                Debug.Log("@@@@@1");
                PhotonNetwork.Destroy(instantiatedObject);
            }
        }

        if (objectInHand != null &&
            (prevHotbarIndex == -999 || hotbarIndex != prevHotbarIndex)) {

            isToolOn = true;
            if (isOnline) {
                if (instantiatedObject != null) {
                    Debug.Log("@@@@@2");
                    PhotonNetwork.Destroy(instantiatedObject);
                }

                instantiatedObject = PhotonNetwork.Instantiate(Path.Combine("Scriptable Objects\\Items\\Prefabs\\Tool", objectInHand.name),
                                                                        playerHand.transform.position,
                                                                        Quaternion.identity, 0);
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
            //if (instantiatedObject != null) DestroyImmediate(instantiatedObject);
        }

        if (instCam != null && isCamFixed) { facing = instCam.transform.eulerAngles.y; transform.eulerAngles = new Vector3(0, facing, 0); }

        UIControl();
        Movement();
        Attributes();
        UseTool();
        PickUpItem();
    }

    private void UseTool() {
        if (Input.GetMouseButtonDown(0)) {
            anim.SetTrigger("Use");
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

        CameraMotor motor = controller.GetMotor("Targeting");
        motor.UseRigAnchor = false;
        motor.Anchor = gameObject.transform;
        motor.AnchorOffset = new Vector3(0.0f, 1.3f, 0.0f);
    }

    public Camera getCamera() { return this.cam.GetComponentInChildren<Camera>(); }

    private void Attributes() {
        //Debug.Log("Running Attributes");
        /* Stamina */
        isRunning = !isWalking;
        if (isRunning && (x != 0 || y != 0)) { StartCoroutine(staminaMinus); StopCoroutine(staminaPlus); } //stamina -= staminaLoss * Time.deltaTime;
        else if ((!isRunning || (x == 0 || y == 0)) && stamina < 100f) { StartCoroutine(staminaPlus); StopCoroutine(staminaMinus); } //stamina += (staminaLoss / 2) * Time.deltaTime;

        /* Hunger */
        if (hunger < hungerThreshold) isStarving = true; else isStarving = false;
        if (isStarving) StartCoroutine(healthCS);
        StartCoroutine (hungerC); //hunger -= hungerLoss * Time.deltaTime;
        // [FALTA] Mostrar Alerta no ecra

        /* Thirst */
        if (thirst < thirstThreshold) needsWater = true; else needsWater = false;
        if (needsWater) StartCoroutine(healthCT);
        StartCoroutine (thirstC); //thirst -= thirstLoss * Time.deltaTime;
        // [FALTA] Mostrar Alerta no ecra
    }

    private IEnumerator healthCoroutineStarving() {
        while (true) {
            yield return new WaitForSeconds(1);
            health -= starvingDecay;
        }
    }

    private IEnumerator healthCoroutineThirsth() {
        while (true) {
            yield return new WaitForSeconds(1);
            health -= thirstDecay;
        }
    }

    private IEnumerator staminaCoroutineAdd () {
        while (true) {
            yield return new WaitForSeconds(1);
            stamina += staminaLoss / 2;
        }
    }

    private IEnumerator staminaCoroutineMinus() {
        while (true) {
            yield return new WaitForSeconds(1);
            stamina -= staminaLoss;
        }
    }

    private IEnumerator hungerCoroutine() {
        while (true) {
            yield return new WaitForSeconds(1);
            hunger -= hungerLoss;
        }
    }

    private IEnumerator thirstCoroutine() {
        while (true) {
            yield return new WaitForSeconds(1);
            thirst -= thirstLoss;
        }
    }

    private void Movement() {
        int jumpState = Animator.StringToHash("Base Layer.JumpRunning");
        AnimatorStateInfo currentBaseState = anim.GetCurrentAnimatorStateInfo(0);
        if (currentBaseState.nameHash == jumpState) {
            CapsuleCollider a;
            //a.center.y = 
            //playerCollider.he = anim.GetFloat("ColliderHeight");
        }

        if (Input.GetKeyUp(walkRunKey) && stamina > 0) this.isWalking = !isWalking;
        if (stamina <= 0) this.isWalking = true;
        if (Input.GetKeyUp(crouchKey)) this.isCrouched = !isCrouched;
        if (Input.GetKeyUp(jumpKey) && isRunning && (x != 0 || y != 0)) anim.SetTrigger("RunningJump");
        else if (Input.GetKeyUp(jumpKey) && (x == 0 || y == 0)) { }

        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");


        if (isWalking) {
            float newX = (float)(this.x * .5);
            float newY = (float)(this.y * .5);
            x = newX;
            y = newY;
        }

        // Set animator variables
        anim.SetFloat("VelX", x);
        anim.SetFloat("VelY", y);
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
                     , 5.5f)) {
                    GameObject hitObj = ray.transform.gameObject;
                    if (hitObj.CompareTag("Pickupable")) {
                        ItemObject it = ray.transform.GetComponent<Item>().item;
                        if (displayInventory.PickupItem(it, 1)) {
                            achivementLog.advanceAchivement(it);
                            Destroy(hitObj);
                        }
                    }

                }
            }

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

    public void addHealth(float amount) {health += amount;}
    public void addThirst(float amount) {thirst += amount;}
    public void addHunger(float amount) {hunger += amount;}

}
