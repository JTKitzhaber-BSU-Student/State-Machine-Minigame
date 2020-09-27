using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    public Transform player;
    private Rigidbody2D rb;
    private Animator anim;
    
    public float shotStrength = 250f;
    WolfState currentState;
    private bool onAttack;

    // Start is called before the first frame update
    void Start()
    {
        onAttack = false;
        anim = this.GetComponent<Animator>();
        spriteRenderer = this.GetComponent<SpriteRenderer>(); // Grab agents Animator component.
        currentState = new WolfIdle(this.gameObject, spriteRenderer, anim, player);
    }

    // Update is called once per frame
    void Update()
    {
        currentState = currentState.Process();

    }

    void OnTriggerEnter2D(Collider2D coll){
        if(currentState.name == WolfState.WOLFSTATE.PURSUE && coll.gameObject.tag != "Bullet" && onAttack == false){
            // currentState = new WolfCharge(this.gameObject, spriteRenderer, anim, player);
            anim.SetTrigger("PrepTrigger");
            onAttack = true;
            StartCoroutine(Attack());
        }
        if(coll.gameObject.tag == "Bullet")
        {
            GameObject.Find("Main Camera").GetComponent<HealthTest>().TakeDamage(5);
            // HealthTest.TakeDamage(5);
            Destroy(coll.gameObject);
        }
    }

    void OnTriggerStay2D(Collider2D coll){
        if(currentState.name == WolfState.WOLFSTATE.ATTACK && coll.gameObject.tag != "Bullet"){
            // currentState = new WolfCharge(this.gameObject, spriteRenderer, anim, player);
            Debug.Log("player should take damage");
        }
       
    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(1f);
        onAttack = false;
        Debug.Log("Should persue or soemthing");
        // WolfState.stage = WolfState.EVENT.EXIT; 
        currentState = new WolfAttack(this.gameObject, spriteRenderer, anim, player);
    }
}
