using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NPC_Animal : MonoBehaviour {

    public Transform[] waypoints;
    public NavMeshAgent navMeshAgent;

    public int currentTarget;
    public float maxDistancetoCheck = 30.0f;

    [SerializeField] private float health = 100.0f;
    public bool dead = false;
    [SerializeField] private bool deadH = false;

    public Animator anim;
    Vector3 disttoPlayer;
    public GameObject player, cameraPlayer;
    RaycastHit hit;
    Vector3 worldDeltaPosition;

    [SerializeField] private bool isEating;
    [SerializeField] private bool eatingEnabled = false;
    [SerializeField] private bool[] eatingWaypoints;

    public GameObject me;
    public PhotonView PV;

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
        me = this.gameObject;
        navMeshAgent = GetComponent<NavMeshAgent>();
        PV = GetComponent<PhotonView>();
        anim = GetComponent<Animator>();
        navMeshAgent.updatePosition = true;
        navMeshAgent.updateRotation = true;
    }

    void Update() {
        if (dead) {
            navMeshAgent.isStopped = true;
        }
        if (health > 0.0f) {
            // Idea: apresentar UI de vida do npc quando o player esta perto
            disttoPlayer = (transform.position - player.transform.position);

            anim.SetFloat("DistToPlayer", disttoPlayer.magnitude);

            worldDeltaPosition = navMeshAgent.nextPosition - transform.position;

            if (worldDeltaPosition.magnitude > navMeshAgent.radius)
                navMeshAgent.nextPosition = transform.position + 0.5f * worldDeltaPosition;

            Visao();
        } else {
            if (!dead) {
                /*if (deadH == false) {
                    anim.SetBool("dead", true);
                    Debug.Log("Dead animation running");
                    deadH = true;
                }*/
                anim.SetTrigger("dead");
                if (!navMeshAgent.isStopped) navMeshAgent.isStopped = true;
                StartCoroutine(die());
                dead = true;
            }
        }
    }

    public void TakeDamage(float ammount) {
        health -= ammount;
    }

    IEnumerator die() {
        yield return new WaitForSeconds(5);
        // Sistema de particulas
        PV.RPC("RPC_DestroyMe", RpcTarget.AllBuffered, PV.ViewID);
    }

    [PunRPC]
    public void RPC_DestroyMe (int id) {
        GameObject del = PhotonView.Find(id).gameObject;
        GameObject.Destroy(del);
    }

    public void MoveToNextWaypoint() {
        currentTarget = (currentTarget + 1) % waypoints.Length;
        navMeshAgent.SetDestination(waypoints[currentTarget].position);
        if (eatingEnabled) {
            if (eatingWaypoints[currentTarget]) isEating = true;
            else isEating = false;

            anim.SetBool("Eating", isEating);
        }
    }

    public void OnAnimatorMove() {
        Vector3 position = anim.rootPosition;
        position.y = navMeshAgent.nextPosition.y;
        transform.position = position;
    }

    public void Visao() {
        Debug.DrawRay(transform.position + Vector3.up, transform.forward * maxDistancetoCheck, Color.green);
        RaycastHit hit;
        if (Physics.SphereCast(transform.position + Vector3.up, 1, transform.forward, out hit, maxDistancetoCheck)) {
            if (hit.rigidbody != null && hit.rigidbody.tag == "Player") {
                Debug.DrawRay(transform.position + Vector3.up, transform.forward * maxDistancetoCheck, Color.red);

                anim.SetBool("Saw", true);
            }

        }
    }


}
