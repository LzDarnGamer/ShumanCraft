using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC_Animal : MonoBehaviour {

    public Transform[] waypoints;
    public NavMeshAgent navMeshAgent;

    public int currentTarget;
    public float maxDistancetoCheck = 30.0f;

    public Animator anim;
    Vector3 disttoPlayer;
    public GameObject player, cameraPlayer;
    RaycastHit hit;
    Vector3 worldDeltaPosition;

    [SerializeField] private bool isEating;
    [SerializeField] private bool eatingEnabled = false;
    [SerializeField] private bool[] eatingWaypoints;

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        navMeshAgent.updatePosition = true;
        navMeshAgent.updateRotation = true;
    }

    void Update() {

        // Idea: apresentar UI de vida do npc quando o player esta perto
        disttoPlayer = (transform.position - player.transform.position);

        anim.SetFloat("DistToPlayer", disttoPlayer.magnitude);

        worldDeltaPosition = navMeshAgent.nextPosition - transform.position;

        if (worldDeltaPosition.magnitude > navMeshAgent.radius)
            navMeshAgent.nextPosition = transform.position + 0.5f * worldDeltaPosition;

        Visao();
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

    public void OnAnimatorMove () {
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
