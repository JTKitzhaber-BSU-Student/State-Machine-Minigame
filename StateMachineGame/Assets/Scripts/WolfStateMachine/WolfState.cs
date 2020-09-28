using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfState
{
    // 'States' that the NPC could be in.
    public enum WOLFSTATE
    {
        IDLE, PATROL, PURSUE, PREP, ATTACK, SLEEP, RUNAWAY, CHARGE
    };

    // 'Events' - where we are in the running of a STATE.
    public enum EVENT
    {
        ENTER, UPDATE, EXIT
    };

    public WOLFSTATE name; // To store the name of the STATE.
    protected EVENT stage; // To store the stage the EVENT is in.
    protected GameObject npc; // To store the NPC game object.
    protected SpriteRenderer spriteRenderer;
    protected Transform player; // To store the transform of the player. This will let the guard know where the player is, so it can face the player and know whether it should be shooting or chasing (depending on the distance).
    protected WolfState nextState; // This is NOT the enum above, it's the state that gets to run after the one currently running (so if IDLE was then going to PATROL, nextState would be PATROL).
    protected Animator anim;

    public Transform target;
    public float lastCharge;
    float visDist = 10.0f; // When the player is within a distance of 10 from the NPC, then the NPC should be able to see it...
    float visAngle = 30.0f; // ...if the player is within 30 degrees of the line of sight.
    float shootDist = 7.0f; // When the player is within a distance of 7 from the NPC, then the NPC can go into an ATTACK state.
    public GameObject[] tags;

    // Constructor for State
    public WolfState(GameObject _npc, SpriteRenderer _spriteRenderer, Animator _anim, Transform _player)
    {
        npc = _npc;
        spriteRenderer = _spriteRenderer;
        stage = EVENT.ENTER;
        player = _player;
        tags = GameObject.FindGameObjectsWithTag("Checkpoint");
        anim = _anim;
        lastCharge = Time.time;
    }

    // Phases as you go through the state.
    public virtual void Enter() { stage = EVENT.UPDATE; } // Runs first whenever you come into a state and sets the stage to whatever is next, so it will know later on in the process where it's going.
    public virtual void Update() { stage = EVENT.UPDATE; } // Once you are in UPDATE, you want to stay in UPDATE until it throws you out.
    public virtual void Exit() { stage = EVENT.EXIT; } // Uses EXIT so it knows what to run and clean up after itself.

    // The method that will get run from outside and progress the state through each of the different stages.
    public WolfState Process()
    {
        if (stage == EVENT.ENTER) Enter();
        if (stage == EVENT.UPDATE) Update();
        if (stage == EVENT.EXIT)
        {
            Exit();
            return nextState; // Notice that this method returns a 'state'.
        }
        return this; // If we're not returning the nextState, then return the same state.
    }

    // Can the NPC see the player, using a simple Line Of Sight calculation?
    public bool PlayerIsClose()
    {
        Vector3 curPos = npc.transform.position;
        Vector3 targetPos = player.position;
        
        float distance = Vector2.Distance(curPos, targetPos);
        if(distance < 5)
        {
            return true; // NPC CAN see the player.
        }


        // Vector3 direction = player.position - npc.transform.position; // Provides the vector from the NPC to the player.
        // float angle = Vector3.Angle(direction, npc.transform.forward); // Provide angle of sight.

        // // If player is close enough to the NPC AND within the visible viewing angle...
        // if(direction.magnitude < visDist && angle < visAngle)
        // {
        //     return true; // NPC CAN see the player.
        // }
        return false; // NPC CANNOT see the player.
    }

    public bool IsPlayerBehind()
    {
        Vector3 direction = npc.transform.position - player.position; // Provides the vector from the player to the NPC.
        float angle = Vector3.Angle(direction, npc.transform.forward); // Provide angle of sight.

        // If player is close enough to the NPC AND within the visible viewing angle...
        if (direction.magnitude < 2 && angle < 30)
        {
            return true; // Player IS behind the NPC.
        }
        return false; // Player IS NOT behind the NPC.
    }

    public bool CanAttackPlayer()
    {
        Vector3 direction = player.position - npc.transform.position; // Provides the vector from the NPC to the player.
        if(direction.magnitude < shootDist)
        {
            return true; // NPC IS close enough to the player to attack.
        }
        return false; // NPC IS NOT close enough to the player to attack.
    }
}

// Constructor for Idle state.
public class WolfIdle : WolfState
{
    Rigidbody2D rb;
    public WolfIdle(GameObject _npc, SpriteRenderer _spriteRenderer, Animator _anim, Transform _player)
                : base(_npc, _spriteRenderer, _anim, _player)
    {
        name = WOLFSTATE.IDLE; // Set name of current state.
        spriteRenderer.color = new Color(1,1,1);
        rb = npc.GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;

    }

    public override void Enter()
    {
        spriteRenderer.color = new Color(1,1,1);
        rb.velocity = Vector2.zero;
        base.Enter(); // Sets stage to UPDATE.
    }
    public override void Update()
    {
        // if(Random.Range(0,100) < 2){
        //     spriteRenderer.color = new Color(Random.Range(0F,1F), Random.Range(0, 1F), Random.Range(0, 1F));
        // }

        // if (PlayerIsClose())
        // {
        //     nextState = new WolfPursue(npc, spriteRenderer, anim, player);
        //     stage = EVENT.EXIT; // The next time 'Process' runs, the EXIT stage will run instead, which will then return the nextState.
        // }
        // // The only place where Update can break out of itself. Set chance of breaking out at 10%.
        // else if(Random.Range(0,100) < 10)
        // {
        //     nextState = new WolfPatrol(npc, spriteRenderer, anim, player);
        //     stage = EVENT.EXIT; // The next time 'Process' runs, the EXIT stage will run instead, which will then return the nextState.
        // }
        if(Random.Range(0,100) < 5)
        {
            nextState = new WolfPursue(npc, spriteRenderer, anim, player);
            stage = EVENT.EXIT; // The next time 'Process' runs, the EXIT stage will run instead, which will then return the nextState.
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}

// Constructor for Patrol state.
public class WolfPatrol : WolfState
{
    Rigidbody2D rb;
    int currentIndex = -1;
    
    public WolfPatrol(GameObject _npc, SpriteRenderer _spriteRenderer, Animator _anim, Transform _player)
                : base(_npc, _spriteRenderer, _anim, _player)
    {
        name = WOLFSTATE.PATROL; // Set name of current state.
        rb = npc.GetComponent<Rigidbody2D>();
        
    }

    public override void Enter()
    {
        float lastDist = Mathf.Infinity; // Store distance between NPC and waypoints.

        // Calculate closest waypoint by looping around each one and calculating the distance between the NPC and each waypoint.
        for (int i = 0; i < tags.Length; i++)
        {
            GameObject thisWP = tags[i];
            float distance = Vector3.Distance(npc.transform.position, thisWP.transform.position);
            if(distance < lastDist)
            {
                currentIndex = i - 1; // Need to subtract 1 because in Update, we add 1 to i before setting the destination.
                lastDist = distance;
            }
        }
        spriteRenderer.color = new Color(1,.55f,1);
        base.Enter();
    }

    public override void Update()
    {
        if(currentIndex == -1)
        {
            currentIndex = 0;
        }

        // Direction
        Vector3 direction = tags[currentIndex].transform.position - npc.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rb.rotation = angle - 90;

        // Postion
        Vector3 curPos = npc.transform.position;
        
        float distance = Vector3.Distance(curPos, tags[currentIndex].transform.position);
        if(distance < 0.25f)
        {
            // If agent has reached end of waypoint list, go back to the first one, otherwise move to the next one.
            if (currentIndex >= tags.Length - 1)
            {
                currentIndex = 0;
            }
            else
            {
                currentIndex++;
            }
        }
        float step = 2 * Time.deltaTime;
        npc.transform.position = Vector3.MoveTowards(npc.transform.position, tags[currentIndex].transform.position, step);

        if (PlayerIsClose())
        {
            nextState = new WolfPursue(npc, spriteRenderer, anim, player);
            stage = EVENT.EXIT; // The next time 'Process' runs, the EXIT stage will run instead, which will then return the nextState.
        }

        else if (IsPlayerBehind())
        {
            nextState = new WolfIdle(npc, spriteRenderer, anim, player);
            stage = EVENT.EXIT; // The next time 'Process' runs, the EXIT stage will run instead, which will then return the nextState.
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}

public class WolfPursue : WolfState
{
    float startTime;
    Rigidbody2D rb;
    
    public WolfPursue(GameObject _npc, SpriteRenderer _spriteRenderer, Animator _anim, Transform _player)
                : base(_npc, _spriteRenderer, _anim, _player)
    {
        name = WOLFSTATE.PURSUE; 
        rb = npc.GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
    }

    public override void Enter()
    {
        anim.SetTrigger("IdleTrigger");
        startTime = Time.time;

        spriteRenderer.color = new Color(1,1,1);
        base.Enter();
    }


    public override void Update()
    {
        // Collider2D[] hit = Physics2D.OverlapCircleAll(npc.transform.position, 2, 9);
        // // Direction    
        // foreach(Collider2D h in hit){
        //    Debug.Log("Hit" + h.name);
        // }
        Vector3 direction = player.position - npc.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rb.rotation = angle - 90;

        // Position
        float curTime = Time.time;
        float step = 2 * Time.deltaTime;
        npc.transform.position = Vector3.MoveTowards(npc.transform.position, player.position, step);
        if(Time.time > lastCharge + 5)
        {
            nextState = new WolfCharge(npc, spriteRenderer, anim, player);
            stage = EVENT.EXIT; 
        }

        // if(!PlayerIsClose())
        // {
        //     nextState = new WolfIdle(npc, spriteRenderer, anim, player);
        //     stage = EVENT.EXIT; 
        // }
    }

    

    public override void Exit()
    {
        base.Exit();
    }
}

public class WolfPrep : WolfState
{
    float startTime;
    Rigidbody2D rb;
    
    public WolfPrep(GameObject _npc, SpriteRenderer _spriteRenderer, Animator _anim, Transform _player)
                : base(_npc, _spriteRenderer, _anim, _player)
    {
        name = WOLFSTATE.PREP; 
        rb = npc.GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        spriteRenderer.color = new Color(.55f,.33f,.82f);
    }

    public override void Enter()
    {
        rb.velocity = Vector2.zero;
        anim.ResetTrigger("IdleTrigger");
        anim.SetTrigger("PrepTrigger");
    
        startTime = Time.time;
        
        base.Enter();
    }

    public override void Update()
    {

        Vector3 direction = player.position - npc.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rb.rotation = angle - 90;

        // Position
        float curTime = Time.time;
        float step = 1.25f * Time.deltaTime;
        npc.transform.position = Vector3.MoveTowards(npc.transform.position, player.position, step);

        if(Time.time > startTime + 1f)
        {
            Debug.Log("Entered Attack");
            nextState = new WolfAttack(npc, spriteRenderer, anim, player);
            stage = EVENT.EXIT; 
        }

    }

    public override void Exit()
    {
        base.Exit();
    }
}

public class WolfAttack : WolfState
{
    float startTime;
    Rigidbody2D rb;
    
    public WolfAttack(GameObject _npc, SpriteRenderer _spriteRenderer, Animator _anim, Transform _player)
                : base(_npc, _spriteRenderer, _anim, _player)
    {
        name = WOLFSTATE.ATTACK; 
        rb = npc.GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
    }

    public override void Enter()
    {
        rb.velocity = Vector2.zero;
        anim.ResetTrigger("PrepTrigger");
        anim.SetTrigger("AttackTrigger");
        startTime = Time.time;
        
        base.Enter();
    }

    public override void Update()
    {
        if(Random.Range(0,100) < 2){
            spriteRenderer.color = new Color(255, 0, 0);
        }

        Vector3 direction = player.position - npc.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rb.rotation = angle - 90;

        // Position
        float curTime = Time.time;
        float step = 1.25f * Time.deltaTime;
        npc.transform.position = Vector3.MoveTowards(npc.transform.position, player.position, step);

        if(Time.time > startTime + 0.5f)
        {
            nextState = new WolfPursue(npc, spriteRenderer, anim, player);
            stage = EVENT.EXIT; 
        }

    }

    public override void Exit()
    {
        anim.ResetTrigger("AttackTrigger");
        base.Exit();
    }
}

public class WolfCharge : WolfState
{
    float startTime;
    Rigidbody2D rb;
    
    public WolfCharge(GameObject _npc, SpriteRenderer _spriteRenderer, Animator _anim, Transform _player)
                : base(_npc, _spriteRenderer, _anim, _player)
    {
        name = WOLFSTATE.CHARGE; 
        rb = npc.GetComponent<Rigidbody2D>();
        anim.ResetTrigger("IdleTrigger");
        anim.SetTrigger("SpinTrigger");
    }

    public override void Enter()
    {
        startTime = Time.time;
        spriteRenderer.color = new Color(.55f,.33f,.82f);
        base.Enter();
        float angle = Mathf.Atan2(player.position.y - npc.transform.position.y, player.position.x - npc.transform.position.x);
        npc.GetComponent<Rigidbody2D>().AddForce(new Vector2 (Mathf.Cos(-angle) * 1000f, Mathf.Sin(-angle) * 1000f));
    }

    public override void Update()
    {
        if(Random.Range(0,100) < 2){
            spriteRenderer.color = new Color(Random.Range(0F,1F), Random.Range(0, 1F), Random.Range(0, 1F));
        }
        Vector2 v = rb.velocity;
        float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        angle = angle - 90;
        npc.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        //npc.transform.rotation = Quaternion.LookRotation(npc.GetComponent<Rigidbody2D>().velocity);

        if(Time.time > startTime + 5f)
        {
            nextState = new WolfPursue(npc, spriteRenderer, anim, player);
            stage = EVENT.EXIT; 
        }
        

    }

    public override void Exit()
    {
        rb.velocity = Vector2.zero;
        lastCharge = Time.time;
        anim.ResetTrigger("SpinTrigger");
        base.Exit();
    }
}