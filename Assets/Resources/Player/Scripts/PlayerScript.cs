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

    [Header("Sound")]
    [SerializeField] private SoundScipt soundScipt;

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


    void Start() {
        anim = gameObject.GetComponent<Animator>();
        auxRPC = gameObject.GetComponent<PlayerAux>();
        StartCoroutine(staminaCoroutineMinus());
        StartCoroutine(staminaCoroutineAdd());
        StartCoroutine(hungerCoroutine());
        StartCoroutine(thirstCoroutine());
        StartCoroutine(healthCoroutineStarving());
        StartCoroutine(healthCoroutineThirsth());
        StartCoroutine(healthOutsideMap());
        colliderHeight = playerCollider.height;
        colliderCenterY = playerCollider.center.y;

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

        if (instCam != null && isCamFixed) { 
            facing = instCam.transform.eulerAngles.y;
            Vector3 look = new Vector3(0, facing, 0);
            transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, look, 0.04f);
        }

        UIControl();
        Movement();
        Attributes();
        UseHand();
        PickUpItem();
        TakeDamage();
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

    public Camera getCamera() { return cam.GetComponentInChildren<Camera>(); }

    private void TakeDamage() {
        if(transform.position.y < 1) {
            isOutsideMap = true;
            if(health <= 0 && firstTime) {
                firstTime = false;
                StartCoroutine(achivementLog.deadAchivement());
            }
        }
    }


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

        if (Input.GetKeyUp(walkRunKey) && stamina > 0) isWalking = !isWalking;
        if (stamina <= 0) this.isWalking = true;
        if (Input.GetKeyUp(crouchKey)) isCrouched = !isCrouched;
        if (Input.GetKeyUp(jumpKey) && isRunning && (x != 0 || y != 0)) anim.SetTrigger("RunningJump");
        else if (Input.GetKeyUp(jumpKey) && (x == 0 || y == 0)) anim.SetTrigger("JumpSteady");

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

    public void addHealth(float amount) {health += amount;}
    public void addThirst(float amount) {thirst += amount;}
    public void addHunger(float amount) {hunger += amount;}

}
