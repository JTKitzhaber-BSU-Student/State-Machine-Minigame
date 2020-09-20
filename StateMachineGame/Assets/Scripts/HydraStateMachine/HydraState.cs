using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HydraState
{
   public enum HYDRA_STATE
    {
        IDLE, RANGE_ATTACK, CLOSE_ATTACK
    };
    public enum EVENT
    {
        ENTER, UPDATE, EXIT
    };

    public HYDRA_STATE stateName;
    protected EVENT stage;
    protected Transform player;
    protected Animator anim;
    protected GameObject npcEnemy;
    protected HydraState nextState;

    public float slamDist = 3.5f;
    public float shootDist = 8.0f;

    public HydraState(GameObject _npcEnemy, Animator _anim, Transform _player)
    {
        npcEnemy = _npcEnemy;
        anim = _anim;
        stage = EVENT.ENTER;
        player = _player;
    }

    public virtual void Enter() { stage = EVENT.UPDATE; }
    public virtual void Update() { }
    public virtual void Exit() { stage = EVENT.EXIT; }

    public HydraState Process()
    {
        if (stage == EVENT.ENTER) Enter();
        if (stage == EVENT.UPDATE) Update();
        if (stage == EVENT.EXIT)
        {
            Exit();
            return nextState;
        }
        return this;
    }
}

public class HydraIdle : HydraState
{
    public HydraIdle(GameObject _npcEnemy, Animator _anim, Transform _player) : base(_npcEnemy, _anim, _player)
    {
        stateName = HYDRA_STATE.IDLE;
    }

    double timeDelay = 2.0f; // 2 seconds

    public override void Enter()
    {
        Debug.Log("Entered IDLE State");
        anim.SetTrigger("isIdle");
        timeDelay = Random.Range(2.0f, 5.0f);
        base.Enter();
    }
    public override void Update()
    {
        timeDelay -= Time.deltaTime; 
        if (timeDelay <= 0f)
        {
            float xDist = player.position.x - npcEnemy.transform.position.x;
            float yDist = player.position.y - npcEnemy.transform.position.y;
            double distanceFromPlayer = Mathf.Sqrt(Mathf.Pow(xDist, 2.0f) + Mathf.Pow(yDist, 2.0f));
            Debug.Log("Checking Player Distance: " + distanceFromPlayer);

            if (distanceFromPlayer < slamDist)
            {
                nextState = new HydraCloseAttack(npcEnemy, anim, player);
                stage = EVENT.EXIT;
            } else if (distanceFromPlayer < shootDist)
            {
                nextState = new HydraRangedAttack(npcEnemy, anim, player);
                stage = EVENT.EXIT;
            } else
            {
                timeDelay = 2.0f;
            }
        }
        base.Update();
    }
    public override void Exit()
    {
        anim.ResetTrigger("isIdle");
        base.Exit();
    }
}

public class HydraRangedAttack : HydraState
{
    public HydraRangedAttack(GameObject _npcEnemy, Animator _anim, Transform _player) : base(_npcEnemy, _anim, _player)
    {
        stateName = HYDRA_STATE.RANGE_ATTACK;
    }

    public float projectileCount = 5f;
    public float multiShotAngle = 12.0f; // degrees

    public override void Enter()
    {
        Debug.Log("Entered Ranged Attack");
        anim.SetTrigger("isAttack");
        base.Enter();
    }
    public override void Update()
    {
        float xDist = player.position.x - npcEnemy.transform.position.x;
        float yDist = player.position.y - npcEnemy.transform.position.y;
        float angleToPlayer = Mathf.Atan2(yDist, xDist) * Mathf.Rad2Deg;
        Hydra theHydra = GameObject.Find("Hydra_Enemy").GetComponent<Hydra>();
        //Debug.Log("Angle to Player: " + angleToPlayer);
        for (int i = 0; i < projectileCount; i++)
        {
            theHydra.SpawnShot(angleToPlayer + (multiShotAngle * (projectileCount - (i + Mathf.FloorToInt(projectileCount/2)))));
            //Debug.Log("Shot Angle: " + (angleToPlayer + (multiShotAngle * (projectileCount - i))));
        }

        nextState = new HydraIdle(npcEnemy, anim, player);
        stage = EVENT.EXIT;

        base.Update();
    }
    public override void Exit()
    {
        anim.ResetTrigger("isAttack");
        base.Exit();
    }
}

public class HydraCloseAttack : HydraState
{
    public HydraCloseAttack(GameObject _npcEnemy, Animator _anim, Transform _player) : base(_npcEnemy, _anim, _player)
    {
        stateName = HYDRA_STATE.CLOSE_ATTACK;
    }

    public override void Enter()
    {
        anim.SetTrigger("isAttack");
        base.Enter();
    }
    public override void Update()
    {
        Hydra theHydra = GameObject.Find("Hydra_Enemy").GetComponent<Hydra>();
        theHydra.SpawnSlam();

        nextState = new HydraIdle(npcEnemy, anim, player);
        stage = EVENT.EXIT;

        base.Update();
    }
    public override void Exit()
    {
        anim.ResetTrigger("isAttack");
        base.Exit();
    }
}