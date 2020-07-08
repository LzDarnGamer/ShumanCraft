using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chase : StateMachineBehaviour {
    private NPC_Animal npc;
    private Transform playerPos;
    private float speed = 7f;

    private GameObject raio;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        npc = animator.GetComponent<NPC_Animal>();
        playerPos = npc.GetLastSeenPlayer().transform;
        npc.ShowIconGameObject(true);
        //playerPos = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        float distance = Vector3.Distance(playerPos.position, animator.bodyPosition);
        if (distance > 10f)
            animator.SetBool("Saw", false);
        else if (distance < 2f) {
            int id = playerPos.gameObject.GetComponent<PhotonView>().ViewID;
            npc.GetComponent<AnimalAux>().RunRPC(id);
        }

        animator.transform.LookAt(playerPos, Vector3.up);
        animator.transform.eulerAngles = new Vector3(0, animator.transform.eulerAngles.y, 0);
        npc.navMeshAgent.SetDestination(playerPos.position);

        if (raio != null) raio.SetActive(true);


        /*
        if (npc.navMeshAgent.remainingDistance <= 2.0f) {
            int id = playerPos.gameObject.GetComponent<PhotonView>().ViewID;
            npc.GetComponent<AnimalAux>().RunRPC(id);
        }*/
        
        //animator.transform.position = Vector3.MoveTowards(animator.transform.position, playerPos.position, speed * Time.deltaTime);
    }


}
