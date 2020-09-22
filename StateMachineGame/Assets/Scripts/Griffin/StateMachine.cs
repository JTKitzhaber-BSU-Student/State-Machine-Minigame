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
    public SpriteRenderer spriteRend;

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

    }

    public void Update()
    {
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
            Attack();   
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
            Debug.Log("attacking");
            spriteRend.sprite = attackGrif;
            anim.SetTrigger("isAttacking");
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


    /* private bool InterruptState
     {
         get{ return interruptState; }
         set{ interruptState = value; }

     }*/

    /* bool PlayerInRange()
     {
         if(Vector2.Distance(enemyPos.position, playerPos.position) < viewDistance)
         {
             return true;
         }
         else
         {
             return false;
         }*/
    //}
}






//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class GriffinState : MonoBehaviour
//{

//    public enum GRIFFIN_STATE
//    {
//        IDLE,  ATTACK
//    }

//    public enum EVENT
//    {
//        ENTER, UPDATE, EXIT
//    }

//    public GRIFFIN_STATE state;
//    public EVENT grifEvent;
//    public Animator anim;
//    public Transform player;
//    public GriffinState nextState;
//    public GameObject enemy;

//    public float playerDist;
//    public float attackDist = 1.0f;

//    // Start is called before the first frame update
//    public GriffinState(GameObject _enemy, Animator _anim, Transform _player)
//    {

//        enemy = _enemy;
//        anim = _anim;
//        player = _player;

//    }

//    public virtual void Enter() { grifEvent = EVENT.UPDATE; }
//    public virtual void Update() { }
//    public virtual void Exit() { grifEvent = EVENT.EXIT; }

//    public GriffinState Process()
//    {
//        if (grifEvent == EVENT.ENTER) Enter();
//        if (grifEvent == EVENT.UPDATE) Update();
//        if (grifEvent == EVENT.EXIT)
//        {
//            Exit();
//            return nextState;
//        }
//        return this;
//    }
//}

//public class GriffinIdle : GriffinState
//{

//    public GriffinIdle(GameObject _enemy, Animator _anim, Transform _player) : base(_enemy, _anim, _player)
//    {
//        state = GRIFFIN_STATE.IDLE;
//        Debug.Log("Idle working");
//    }

//    double timeDelay = 2.0f;

//    public override void Enter()
//    {
//        Debug.Log("Entered IDLE State");
//        anim.SetTrigger("isIdle");
//        timeDelay = Random.Range(2.0f, 5.0f);
//        base.Enter();
//    }
//    public override void Update()
//    {
//        enemy.transform.position = Vector3.Lerp(enemy.transform.position, player.position, Time.deltaTime);
//        Debug.Log(enemy.transform.position);
//        timeDelay -= Time.deltaTime;
//        if (timeDelay <= 0f)
//        {
//            float xDist = player.position.x - enemy.transform.position.x;
//            float yDist = player.position.y - enemy.transform.position.y;
//            double distanceFromPlayer = Mathf.Sqrt(Mathf.Pow(xDist, 2.0f) + Mathf.Pow(yDist, 2.0f));
//            Debug.Log("Checking Player Distance: " + distanceFromPlayer);

//            if (distanceFromPlayer < attackDist)
//            {
//                nextState = new GriffinAttack(enemy, anim, player);
//                grifEvent = EVENT.EXIT;
//            }

//        }
//        base.Update();
//    }
//    public override void Exit()
//    {
//        anim.ResetTrigger("isIdle");
//        base.Exit();
//    }


//}



//public class GriffinAttack : GriffinState
//{

//    public GriffinAttack(GameObject _enemy, Animator _anim, Transform _player): base(_enemy, _anim, _player)
//    {
//        {
//            state = GRIFFIN_STATE.ATTACK;

//        }
//    }
//    public override void Enter()
//    {
//        Debug.Log("Entered attack state");
//        state = GRIFFIN_STATE.ATTACK;
//        anim.SetTrigger("isAttacking");
//        base.Enter();
//    }

//    public override void Update()
//    {
//        playerDist = Vector2.Distance(enemy.transform.position, player.transform.position);
//        if (playerDist <= attackDist) { 
//        nextState = new GriffinIdle(enemy, anim, player);
//        grifEvent = EVENT.EXIT;
//        }

//        base.Update();
//    }

//    public override void Exit()
//    {
//        anim.ResetTrigger("isIdle");
//        base.Exit();
//    }


//}



