using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Griffin : MonoBehaviour
{
    public Animator anim;
    public Transform player;
    public GriffinState currentState;
    public GriffinAttack grifAttack;
    public GriffinIdle grifIdle;
    public GameObject griffin;
    public float viewDist = 3f;
    public float attackDist = 1f;
 

    public float playerDist;

    int health = 100;

    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();
        currentState = new GriffinIdle(this.gameObject, anim, player);

    }

    // Update is called once per frame
    void Update()
    {
       // currentState = currentState.Process();
        float playerDist = Vector2.Distance(player.position, this.transform.position);
        if(playerDist <= viewDist)
        {
            currentState = grifIdle;
        }

        if (attackDist <= viewDist)
        {
            currentState = grifAttack;
        }

        anim.ResetTrigger("GriffinIdleAnim");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(gameObject.tag == "Player")
        {
            Debug.Log("playet hit");
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
