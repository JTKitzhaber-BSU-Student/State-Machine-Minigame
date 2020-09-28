using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Griffin : MonoBehaviour
{
    public Animator anim;
    public Transform player;
 //   public GriffinState.GRIFFIN_STATE currentState;
    public Sprite grifAttack;
    public Sprite grifIdle;
    public GameObject griffin;
    private Rigidbody2D rb;
    public float viewDist = 3f;
    public float attackDist = 1f;
 

    public float playerDist;

    int health = 100;

    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();
        rb = this.GetComponent<Rigidbody2D>();
       // grifIdle = new GriffinIdle(this.gameObject, anim, player);
     //   currentState = GriffinState.GRIFFIN_STATE.IDLE;

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = player.position - this.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rb.rotation = angle - 90;

       // currentState = currentState.Process();
        float playerDist = Vector2.Distance(player.position, this.transform.position);
        if(playerDist <= viewDist)
        {
       //     currentState = GriffinState.GRIFFIN_STATE.IDLE;
        }

        if (attackDist <= viewDist)
        {
          //  currentState = GriffinState.GRIFFIN_STATE.ATTACK;
        }

        anim.ResetTrigger("GriffinIdleAnim");
    }

    void OnTriggerStay2D(Collider2D coll){
        if(coll.gameObject.tag == "Player"){
            // currentState = new WolfCharge(this.gameObject, spriteRenderer, anim, player);
            // Debug.Log("player should take damage");
            coll.gameObject.GetComponent<Health>().TakeDamage();
        }
       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Debug.Log("player hit");
            collision.gameObject.GetComponent<Health>().TakeDamage();
        }

        if(collision.transform.tag == "Enemy")
        {
            health -= 5;

            if(health <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

}
