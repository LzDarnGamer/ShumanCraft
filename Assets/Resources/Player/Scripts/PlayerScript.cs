using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;
using com.ootii.Cameras;
using com.ootii.Input;
using UnityEngine.TextCore;
using System;

public class PlayerScript : MonoBehaviour
{
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
    [SerializeField]
    private GameObject inputSource;

    private GameObject instCam, instSource;

    public ConstructionController construction;

    public Interact interact;

    [Header("Movimentos")]
    private float x, y;
    [SerializeField]
    private bool isWalking = true;
    [SerializeField]
    private bool isRunning = false;
    [SerializeField]
    private bool isCrouched = false;

    [Header("Caracteristicas")]
    [SerializeField]
    private string name;
    [SerializeField]
    [Range(0, 100)]
    private float health = 100.0f;
    [SerializeField]
    [Range(0, 100)]
    private float stamina = 100.0f;
    [SerializeField]
    [Range(0, 100)]
    private float staminaLoss = 10.0f;
    [SerializeField]
    [Range(0, 100)]
    private float hunger = 100.0f;
    [SerializeField]
    [Range(0, 100)]
    private float hungerLoss = 0.5f;
    [SerializeField]
    [Range(0, 100)]
    private float hungerThreshold = 10.0f;
    [SerializeField]
    [Range(0, 100)]
    private float starvingDecay = 2.0f;
    [SerializeField]
    private bool isStarving = false;
    [SerializeField]
    [Range(0, 100)]
    private float thirst = 100.0f;
    [SerializeField]
    [Range(0, 100)]
    private float thirstLoss = 1.5f;
    [SerializeField]
    [Range(0, 100)]
    private float thirstThreshold = 30.0f;
    [SerializeField]
    [Range(0, 100)]
    private float thirstDecay = 5.0f;
    [SerializeField]
    private bool needsWater = false;


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
    [SerializeField]
    private Image uiHealth;
    [SerializeField]
    private Image uiStamina;
    [SerializeField]
    private Image uiHunger;
    [SerializeField]
    private Image uiThirst;
    [SerializeField]
    private bool isOnline = true;
    void Start() {
        anim = gameObject.GetComponent<Animator>();
        if (isOnline) {
            if (PV.IsMine) {
                construction = gameObject.GetComponent<ConstructionController>();
                construction.SetHotKey(openConstructionkey);
                AddCamera();
            } else {
                Destroy(gameObject.GetComponent<ConstructionController>());
                Destroy(canvas);
                Destroy(gameObject.GetComponent<Interact>());
                Destroy(this);
            }
        } else {
            construction = gameObject.GetComponent<ConstructionController>();
            construction.SetHotKey(openConstructionkey);
            AddCamera();
        }
    }

    void FixedUpdate() {
        if (isOnline) {
            if (PV.IsMine) {
                if (instCam != null && isCamFixed) {
                    facing = instCam.transform.eulerAngles.y;
                    Debug.Log("ROTATING PLAYER: " + facing);
                    transform.eulerAngles = new Vector3(0, facing, 0);
                }
                if (Input.GetKeyUp(changeConstructionKey)) construction.increaseIndex();
                UIControl();
                Movement();
                Attributes();
                
            } else if (!PV.IsMine) {
                Destroy(instCam);
                Destroy(instSource);
            }
        } else {
            facing = cam.transform.eulerAngles.y;
            if (isCamFixed) { transform.eulerAngles = new Vector3(0, facing, 0); };
            if (Input.GetKeyUp(changeConstructionKey)) construction.increaseIndex();
            UIControl();
            Movement();
            Attributes();
        }
    }

    private void AddCamera() {
        instCam = Instantiate(cam, this.gameObject.transform);
        instSource = Instantiate(inputSource, this.gameObject.transform);

        if (isCamFixed) gameObject.transform.eulerAngles = new Vector3( 0,facing, 0);
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
        Debug.Log("Running Attributes");
        /* Stamina */
        isRunning = !isWalking;
        if (isRunning && (x != 0 || y != 0)) stamina -= staminaLoss * Time.deltaTime;
        else if ((!isRunning || (x == 0 || y == 0)) && stamina < 100f) stamina += (staminaLoss/2) * Time.deltaTime;

        /* Hunger */
        if (hunger < hungerThreshold) isStarving = true; else isStarving = false;
        if (isStarving) healthDecay(starvingDecay);
        hunger -= hungerLoss * Time.deltaTime;
        // [FALTA] Mostrar Alerta no ecra

        /* Thirst */
        if (thirst < thirstThreshold) needsWater = true; else needsWater = false;
        if (needsWater) healthDecay(thirstDecay);
        thirst -= thirstLoss * Time.deltaTime;
        // [FALTA] Mostrar Alerta no ecra
    }

    // Tirar vida ao longo do tempo
    private void healthDecay(float i) {
        this.health -= i * Time.deltaTime;
    }

    private void Movement() {
        if (Input.GetKeyUp(walkRunKey) && stamina > 0) this.isWalking = !isWalking;
        if (stamina <= 0) this.isWalking = true;
        if (Input.GetKeyUp(crouchKey)) this.isCrouched = !isCrouched;

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
        Debug.Log("UI CONTROL");
        uiHealth.fillAmount = health / 100.0f;
        uiStamina.fillAmount = stamina / 100.0f;
        uiHunger.fillAmount = hunger / 100.0f;
        uiThirst.fillAmount = thirst / 100.0f;
    }

    public void addHealth(float amount) {health += amount;}
    public void addThirst(float amount) {thirst += amount;}
    public void addHunger(float amount) {hunger += amount;}

}
