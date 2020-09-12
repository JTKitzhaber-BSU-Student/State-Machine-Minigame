using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleStateMachine : MonoBehaviour
{
    public enum Cs { Idle, Walking};

    public Transform player;
    Animator anim;
    public float speed;
    public Cs currentS;


    void Start()
    {
        anim = GetComponent<Animator>();
        currentS = Cs.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentS == Cs.Idle)
        {
            Idle();
        }
        else if (currentS == Cs.Walking)
        {
            Walking();
        }
    }

    void Idle()
    {
        anim.SetTrigger("isIdle");
        //Check for exit conditions to eneter a new state
        if (Vector3.Distance(transform.position, player.position) < 3){
            //call the state you want to go to on this condition
            anim.ResetTrigger("isIdle");
            currentS = Cs.Walking;
        }
        //main actictivity of the state that is looping
    }
    void Walking()
    {
        anim.SetTrigger("isWalking");
        if(Vector3.Distance(transform.position, player.position) > 4){
            anim.ResetTrigger("isWalking");
            currentS = Cs.Idle;
        }
        //main activity
        float speedMod = speed * Time.deltaTime;
        Vector3 playerxzPos = player.position;
        playerxzPos.y = 0;
        Vector3 catxzPos = transform.position;
        catxzPos.y = 0;
        transform.position = Vector3.Lerp(catxzPos, playerxzPos, speedMod);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(player.transform.position - transform.position), 1);
    }
}
