using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // Added since we're using a navmesh.

public class State
{
    // 'States' that the NPC could be in.
    public enum STATE
    {
        IDLE, PATROL, PURSUE, ATTACK, SLEEP, RUNAWAY
    };

    // 'Events' - where we are in the running of a STATE.
    public enum EVENT
    {
        ENTER, UPDATE, EXIT
    };

    public STATE name; // To store the name of the STATE.
    protected EVENT stage; // To store the stage the EVENT is in.
    protected GameObject npc; // To store the NPC game object.
    protected SpriteRenderer spriteRenderer;
    protected Transform player; // To store the transform of the player. This will let the guard know where the player is, so it can face the player and know whether it should be shooting or chasing (depending on the distance).
    protected State nextState; // This is NOT the enum above, it's the state that gets to run after the one currently running (so if IDLE was then going to PATROL, nextState would be PATROL).

    public Transform target;
    float visDist = 10.0f; // When the player is within a distance of 10 from the NPC, then the NPC should be able to see it...
    float visAngle = 30.0f; // ...if the player is within 30 degrees of the line of sight.
    float shootDist = 7.0f; // When the player is within a distance of 7 from the NPC, then the NPC can go into an ATTACK state.
    public GameObject[] tags;

    // Constructor for State
    public State(GameObject _npc, SpriteRenderer _spriteRenderer, Transform _player)
    {
        npc = _npc;
        spriteRenderer = _spriteRenderer;
        stage = EVENT.ENTER;
        player = _player;
        tags = GameObject.FindGameObjectsWithTag("Checkpoint");
    }

    // Phases as you go through the state.
    public virtual void Enter() { stage = EVENT.UPDATE; } // Runs first whenever you come into a state and sets the stage to whatever is next, so it will know later on in the process where it's going.
    public virtual void Update() { stage = EVENT.UPDATE; } // Once you are in UPDATE, you want to stay in UPDATE until it throws you out.
    public virtual void Exit() { stage = EVENT.EXIT; } // Uses EXIT so it knows what to run and clean up after itself.

    // The method that will get run from outside and progress the state through each of the different stages.
    public State Process()
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
        if(distance < 3)
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
public class Idle : State
{
    public Idle(GameObject _npc, SpriteRenderer _spriteRenderer, Transform _player)
                : base(_npc, _spriteRenderer, _player)
    {
        name = STATE.IDLE; // Set name of current state.
    }

    public override void Enter()
    {
        spriteRenderer.color = new Color(1,.55f,1);
        base.Enter(); // Sets stage to UPDATE.
    }
    public override void Update()
    {
         Debug.Log("Update of idle");
        if(Random.Range(0,100) < 2){
            spriteRenderer.color = new Color(Random.Range(0F,1F), Random.Range(0, 1F), Random.Range(0, 1F));
             Debug.Log("Update of idle delta time");
        }

        if (PlayerIsClose())
        {
            nextState = new Pursue(npc, spriteRenderer, player);
            stage = EVENT.EXIT; // The next time 'Process' runs, the EXIT stage will run instead, which will then return the nextState.
        }
        // The only place where Update can break out of itself. Set chance of breaking out at 10%.
        else if(Random.Range(0,100) < 10)
        {
            nextState = new Patrol(npc, spriteRenderer, player);
            stage = EVENT.EXIT; // The next time 'Process' runs, the EXIT stage will run instead, which will then return the nextState.
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}

// Constructor for Patrol state.
public class Patrol : State
{
    int currentIndex = -1;
    
    public Patrol(GameObject _npc, SpriteRenderer _spriteRenderer, Transform _player)
                : base(_npc, _spriteRenderer, _player)
    {
        name = STATE.PATROL; // Set name of current state.
        
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
        Vector3 curPos = npc.transform.position;
        if(currentIndex == -1)
        {
            currentIndex = 0;
        }
        float distance = Vector3.Distance(curPos, tags[currentIndex].transform.position);
        Debug.Log(distance);
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
            nextState = new Pursue(npc, spriteRenderer, player);
            stage = EVENT.EXIT; // The next time 'Process' runs, the EXIT stage will run instead, which will then return the nextState.
        }

        else if (IsPlayerBehind())
        {
            nextState = new Idle(npc, spriteRenderer, player);
            stage = EVENT.EXIT; // The next time 'Process' runs, the EXIT stage will run instead, which will then return the nextState.
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}

public class Pursue : State
{
    float startTime;
    public Pursue(GameObject _npc, SpriteRenderer _spriteRenderer, Transform _player)
                : base(_npc, _spriteRenderer, _player)
    {
        name = STATE.PURSUE; // State set to match what NPC is doing.
        // agent.speed = 5; // Speed set to make sure NPC appears to be running.
        // agent.isStopped = false; // Set bool to determine NPC is moving.
    }

    public override void Enter()
    {
        startTime = Time.time;
        spriteRenderer.color = new Color(.25f,.75f,1);
        base.Enter();
    }

    public override void Update()
    {
        float curTime = Time.time;
        float step = 2 * Time.deltaTime;
        npc.transform.position = Vector3.MoveTowards(npc.transform.position, player.position, step);
        float timeElasped = Time.time - startTime;
        if(timeElasped > 5)
        {
            nextState = new Idle(npc, spriteRenderer, player);
            stage = EVENT.EXIT;
        }
        if(!PlayerIsClose())
        {
            nextState = new Idle(npc, spriteRenderer, player);
            stage = EVENT.EXIT; // The next time 'Process' runs, the EXIT stage will run instead, which will then return the nextState.
        }
        // agent.SetDestination(player.position);  // Set goal for NPC to reach but navmesh processing might not have taken place, so...
        // if(agent.hasPath)                       // ...check if agent has a path yet.
        // {
        //     if (CanAttackPlayer())
        //     {
        //         nextState = new Idle(npc, agent, spriteRenderer, player); // If NPC can attack player, set correct nextState.
        //         stage = EVENT.EXIT; // Set stage correctly as we are finished with Pursue state.
        //     }
        //     // If NPC can't see the player, switch back to Patrol state.
        //     else if (!CanSeePlayer())
        //     {
        //         nextState = new Patrol(npc, agent, spriteRenderer, player); // If NPC can't see player, set correct nextState.
        //         stage = EVENT.EXIT; // Set stage correctly as we are finished with Pursue state.
        //     }
        // }
    }

    public override void Exit()
    {
        base.Exit();
    }
}

// public class Attack : State
// {
//     float rotationSpeed = 2.0f; // Set speed that NPC will rotate around to face player.
//     AudioSource shoot; // To store the AudioSource component.
//     public Attack(GameObject _npc, NavMeshAgent _agent, SpriteRenderer _spriteRenderer, Transform _player)
//                 : base(_npc, _agent, _spriteRenderer, _player)
//     {
//         name = STATE.ATTACK; // Set name to correct state.
//         shoot = _npc.GetComponent<AudioSource>(); // Get AudioSource component for shooting sound.
//     }

//     public override void Enter()
//     {
//         spriteRenderer.color = new Color(.33f,.99f,.87f);
//         agent.isStopped = true; // Stop NPC so he can shoot.
//         shoot.Play(); // Play shooting sound.
//         base.Enter();
//     }

//     public override void Update()
//     {
//         // Calculate direction and angle to player.
//         Vector3 direction = player.position - npc.transform.position; // Provides the vector from the NPC to the player.
//         float angle = Vector3.Angle(direction, npc.transform.forward); // Provide angle of sight.
//         direction.y = 0; // Prevent character from tilting.

//         // Rotate NPC to always face the player that he's attacking.
//         npc.transform.rotation = Quaternion.Slerp(npc.transform.rotation,
//                                             Quaternion.LookRotation(direction),
//                                             Time.deltaTime * rotationSpeed);

//         if(!CanAttackPlayer())
//         {
//             nextState = new Idle(npc, agent, spriteRenderer, player); // If NPC can't attack player, set correct nextState.
//             stage = EVENT.EXIT; // Set stage correctly as we are finished with Attack state.
//         }
//     }

//     public override void Exit()
//     {
//         shoot.Stop(); // Stop shooting sound.
//         base.Exit();
//     }
// }

// public class RunAway : State
// {
    
//     public RunAway(GameObject _npc, NavMeshAgent _agent, SpriteRenderer _spriteRenderer, Transform _player)
//                 : base(_npc, _agent, _spriteRenderer, _player)
//     {
//         name = STATE.RUNAWAY; // Set name to correct state.
//     }

//     public override void Enter()
//     {
//         spriteRenderer.color = new Color(1,1,1);
//         agent.isStopped = false; // Set bool to determine NPC is moving.
//         agent.speed = 6; // Set speed slightly fsater than when running towards player.
//         base.Enter();
//     }

//     public override void Update()
//     {
//         // When the NPC hits the top of the cube, return to the Idle state that has a 10% chance of going into Patrol state.
//         if (agent.remainingDistance < 1)
//         {
//             nextState = new Idle(npc, agent, spriteRenderer, player); // If NPC can't attack player, set correct nextState.
//             stage = EVENT.EXIT; // Set stage correctly as we are finished with Runaway state.
//         }
//     }

//     public override void Exit()
//     {
//         base.Exit();
//     }
// }