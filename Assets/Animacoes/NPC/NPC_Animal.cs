using Boo.Lang;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NPC_Animal : MonoBehaviour {

    [SerializeField] private Canvas canvas;

    [SerializeField] private Transform[] waypoints;
    public NavMeshAgent navMeshAgent;

    public int currentTarget;
    public float maxDistancetoCheck = 30.0f;

    [SerializeField] private float health = 100.0f;
    public bool dead = false;
    [SerializeField] private bool deadH = false;

    private GameObject playerSeen;

    public Animator anim;
    //Vector3 disttoPlayer;
    private List<Vector3> distToPlayers;
    //Canvas
    
    //public GameObject player;
    [SerializeField] private GameObject[] players;
    RaycastHit hit;
    Vector3 worldDeltaPosition;

    public PhotonView PV;

    void Start() {
        distToPlayers = new List<Vector3>();
        players = GameObject.FindGameObjectsWithTag("Player");
        navMeshAgent = GetComponent<NavMeshAgent>();
        PV = GetComponent<PhotonView>();
        anim = GetComponent<Animator>();
        navMeshAgent.updatePosition = true;
        navMeshAgent.updateRotation = true;
    }

    void Update() {
        if (dead) navMeshAgent.isStopped = true;

        if (health > 0.0f) {
            // Idea: apresentar UI de vida do npc quando o player esta perto
            //disttoPlayer = (transform.position - player.transform.position);
            
            GameObject closestPlayer = GetClosestPlayerDistance();
            Vector3 dist = (transform.position - closestPlayer.transform.position);
            anim.SetFloat("DistToPlayer", dist.magnitude);

            worldDeltaPosition = navMeshAgent.nextPosition - transform.position;

            if (worldDeltaPosition.magnitude > navMeshAgent.radius)
                navMeshAgent.nextPosition = transform.position + 0.5f * worldDeltaPosition;

            Visao();
            canvas.transform.LookAt(-closestPlayer.transform);
        } else {
            if (!dead) {
                anim.SetTrigger("dead");
                if (!navMeshAgent.isStopped) navMeshAgent.isStopped = true;
                StartCoroutine(die());
                dead = true;
            }
        }
        

    }

    public GameObject GetClosestPlayerDistance() {
        float closestPlayerdistance = float.MaxValue;
        GameObject closestPlayer = null;
        for (int i = 0; i < players.Length; ++i) {
            Vector3 ajuda = transform.position - players[i].transform.position;
            distToPlayers.Add(ajuda);
            if (ajuda.magnitude < closestPlayerdistance) {
                closestPlayerdistance = ajuda.magnitude;
                closestPlayer = players[i];
            }
        }
        return closestPlayer;
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

        Vector3 nextPos = RandomNavmeshLocation(4f);
        navMeshAgent.SetDestination(nextPos);

        /*currentTarget = (currentTarget + 1) % waypoints.Length;
        navMeshAgent.SetDestination(waypoints[currentTarget].position);
        if (eatingEnabled) {
            if (eatingWaypoints[currentTarget]) isEating = true;
            else isEating = false;

            anim.SetBool("Eating", isEating);
        }*/
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
                playerSeen = hit.transform.gameObject;
                anim.SetBool("Saw", true);
            }

        }
    }

    public GameObject GetLastSeenPlayer () { return playerSeen; }

    public Vector3 RandomNavmeshLocation(float radius) {
        Vector3 randomDir = Random.insideUnitSphere * radius;
        randomDir += transform.position;

        NavMeshHit hit;
        Vector3 finalPos = Vector3.zero;
        if (NavMesh.SamplePosition(randomDir, out hit, radius, 1))
            finalPos = hit.position;

        return finalPos;
    }


}
