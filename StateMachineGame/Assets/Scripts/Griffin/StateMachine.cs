using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.AI;
using UnityEngine.Animations;

public enum theStates
{
 IDLE,
 ATTACK
}

public class StateMachine : MonoBehaviour
{

    public GameObject player;
    public GameObject enemy;
    public Animator anim;
    public Sprite idleGrif;
    public Sprite attackGrif;
    public Sprite wingGrif;
    public SpriteRenderer spriteRend;
    private Rigidbody2D rb;
    
    //TESTING CODE
    //public Color attackCol = Color.red;
   // public Color normalCol = Color.blue;

    //TEMPORARY NUMBER. FEEL FREE TO CHANGE LATER
    public int health = 20;

    //CHECK ENEMY AI FORUM FOR CODE BY Eiznek
    //STATES
    public theStates currentState = theStates.IDLE;
    public theStates nextState = theStates.IDLE;


    //CHECK ENEMY AI FORUM FOR CODE BY Eiznek
    public float viewDistance = 3f;
    public float attackDistance = 1f;
    public float playerDist;
    //THE RADIUS WHERE THE ENEMY WILL SEE YOU
    public float radius = 1f;
    public float speed = 2f;

    //TRANSFORMS/VECTOR3s
    private Transform playerPos;
    private Transform enemyPos;
  //private Quaternion lookRotation;

    //CHECK ENEMY AI FORUM FOR CODE BY Eiznek
    //BOOLS
    private bool isStateFinished = true;
    private bool interruptState = false;
    private bool inRange = false;

    protected void Start()
    {

        anim = GetComponent<Animator>();

        playerPos = player.transform;

        enemy = GameObject.FindGameObjectWithTag("Enemy");

        enemyPos = enemy.transform;

        rb = GetComponent<Rigidbody2D>();

    }

    public void Update()
    {
        Vector3 direction = player.transform.position - this.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rb.rotation = angle + 90;

        playerDist = Vector2.Distance(enemyPos.position, playerPos.position);
        //CHECK ENEMY AI FORUM BY Eiznek
        if (currentState != nextState && (isStateFinished || interruptState))
        {
            currentState = nextState;
            isStateFinished = false;
            switch (currentState)
            {
                case theStates.IDLE:
                    Idle();
                    break;
                case theStates.ATTACK:
                    Attack();
                    break;
                default:
                    break;
            }
        }

        //if player is within attack distance
        //then the enemy will attack
        if (playerDist <= attackDistance)
        {
            nextState = theStates.ATTACK;
               
        }
        else if(playerDist <= viewDistance)
        {
            nextState = theStates.IDLE;
            Idle();
        }

        
    }

    //GetRandomDir method from Simple Public Enemy AI video
    //Generates random normalized direction
    private static Vector3 GetRandomDir()
    {
        return new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
    }

    //GetRoamingPos method from Simple Public Enemy AI video

    // private Vector3 GetRoamingPos()
    //{
    // return enemyPos + GetRandomDir() * Random.Range(10f, 50f);

    //}

    //moving state of our creatures
    protected void Idle()
    {
        //if you see the player
        //then follow them
        FollowTarget();
        spriteRend.sprite = idleGrif;
        isStateFinished = true;

        anim.SetTrigger("isIdle");

    }

    //for the defensive or scared state of our creatures
    protected void Attack()
    {
        if (enemy.tag == "Enemy")
        {
            float randFloat = Random.value;
        //do slashing attack or wing attack
        if (randFloat >= 0.5f)
        {
            Debug.Log("attacking");
            spriteRend.sprite = attackGrif;
            anim.SetTrigger("isSlashing");
        }
        else
        {
            Debug.Log("winging it");
            spriteRend.sprite = wingGrif;
            anim.SetTrigger("wingingIt");
        }

            randFloat = Random.value;           
           
            isStateFinished = true;
            Debug.Log(spriteRend.sprite);
        }

        anim.ResetTrigger("isIdle");
    }

    public void FollowTarget()
    {
        //find player position
        //follow the player
        GameObject.FindGameObjectsWithTag("Player");
        Vector3.Lerp(enemyPos.position, playerPos.position, Time.deltaTime);
       // Quaternion.Slerp(enemy.transform.rotation, player.transform.rotation, Time.deltaTime);
        //transform.LookAt(playerPos.position);
        transform.position = Vector2.MoveTowards(enemyPos.position, playerPos.position, Time.deltaTime);
        Debug.Log("Following player");
      //  Debug.Log(enemyPos.position);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("hit");
        GameObject.Find("Main Camera").GetComponent<HealthTest>().TakeDamage(5);

    }
}



