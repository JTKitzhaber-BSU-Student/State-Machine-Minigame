using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GriffinState
{


    public enum GRIFFIN_STATE
    {
        IDLE,  ATTACK
    }

    public enum EVENT
    {
        ENTER, UPDATE, EXIT
    }

    public GRIFFIN_STATE state;
    public EVENT grifEvent;
    public Animator anim;
    public Transform player;
    public GriffinState nextState;
    public GameObject enemy;

    public float attackDist = 1.0f;

    // Start is called before the first frame update
    public GriffinState(GameObject _enemy, Animator _anim, Transform _player)
    {

        enemy = _enemy;
        anim = _anim;
        player = _player;

    }

    public virtual void Enter() { grifEvent = EVENT.UPDATE; }
    public virtual void Update() { }
    public virtual void Exit() { grifEvent = EVENT.EXIT; }

    public GriffinState Process()
    {
        if (grifEvent == EVENT.ENTER) Enter();
        if (grifEvent == EVENT.UPDATE) Update();
        if (grifEvent == EVENT.EXIT)
        {
            Exit();
            return nextState;
        }
        return this;
    }
}

public class GriffinIdle : GriffinState
{

    public GriffinIdle(GameObject _npcEnemy, Animator _anim, Transform _player) : base(_npcEnemy, _anim, _player)
    {
        state = GRIFFIN_STATE.IDLE;
    }

    double timeDelay = 2.0f;

    public override void Enter()
    {
        Debug.Log("Entered IDLE State");
        anim.SetTrigger("GriffinIdleAnim");
        timeDelay = Random.Range(2.0f, 5.0f);
        base.Enter();
    }
    public override void Update()
    {
        timeDelay -= Time.deltaTime;
        if (timeDelay <= 0f)
        {
            float xDist = player.position.x - enemy.transform.position.x;
            float yDist = player.position.y - enemy.transform.position.y;
            double distanceFromPlayer = Mathf.Sqrt(Mathf.Pow(xDist, 2.0f) + Mathf.Pow(yDist, 2.0f));
            Debug.Log("Checking Player Distance: " + distanceFromPlayer);

            if (distanceFromPlayer < attackDist)
            {
                nextState = new GriffinAttack(enemy, anim, player);
                grifEvent = EVENT.EXIT;
            }

        }
        base.Update();
    }
    public override void Exit()
    {
        anim.ResetTrigger("GriffinIdleAnim");
        base.Exit();
    }

    
}

public class GriffinAttack : GriffinState
{

    public GriffinAttack(GameObject _enemy, Animator _anim, Transform _player): base(_enemy, _anim, _player)
    {
        {
            state = GRIFFIN_STATE.ATTACK;

        }
    }
    public override void Enter()
    {
        Debug.Log("Entered attack state");
        state = GRIFFIN_STATE.ATTACK;
        anim.SetTrigger("GriffinAttackAnim");
        base.Enter();
    }

    public override void Update()
    {

        
        nextState = new GriffinIdle(enemy, anim, player);
        grifEvent = EVENT.EXIT;
        base.Update();
    }

    public override void Exit()
    {
        anim.ResetTrigger("GriffinIdleAnim");
        base.Exit();
    }

    
}



