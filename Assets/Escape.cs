using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Escape : StateMachineBehaviour
{
    private NPC_Animal npc;
    private Transform playerPos;
    private float speed = 7f;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        npc = animator.GetComponent<NPC_Animal>();
        playerPos = npc.GetClosestPlayerDistance().transform;
        //playerPos = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        float distance = Vector3.Distance(playerPos.position, animator.bodyPosition);
        if (distance > 10f)
            animator.SetBool("Saw", false);
        else if (distance < 2f) {
            // Atacar
            //animator.SetBool("Saw", false);
        }
        animator.transform.rotation = Quaternion.LookRotation(animator.transform.position - playerPos.position);
        Vector3 runTo = animator.transform.position + animator.transform.forward * 3f;

        NavMeshHit hit;
        NavMesh.SamplePosition(runTo, out hit, 5, 1 << NavMesh.GetNavMeshLayerFromName("Default"));

        npc.navMeshAgent.SetDestination(hit.position);

        //animator.transform.LookAt(playerPos, Vector3.up);
        //animator.transform.eulerAngles = new Vector3(0, animator.transform.eulerAngles.y, 0);
        //npc.navMeshAgent.SetDestination(playerPos.position);
        
        //animator.transform.position = Vector3.MoveTowards(animator.transform.position, playerPos.position, speed * Time.deltaTime);
    }
}
