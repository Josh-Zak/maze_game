using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIAnimation : MonoBehaviour
{
    public Animator animator;
    public NavMeshAgent agent;

    private void Update()
    {
        animator.SetFloat("Speed", agent.speed);
    }
}
