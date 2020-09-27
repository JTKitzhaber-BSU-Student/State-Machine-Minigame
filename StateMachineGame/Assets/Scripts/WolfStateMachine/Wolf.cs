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

    // Start is called before the first frame update
    void Start()
    {
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
        if(currentState.name == WolfState.WOLFSTATE.PURSUE){
            currentState = new WolfCharge(this.gameObject, spriteRenderer, anim, player);
        }
    }
}
