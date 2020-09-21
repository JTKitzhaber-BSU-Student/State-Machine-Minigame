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
    public GameObject griffin;

 

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
        currentState = currentState.Process();
        float dist = Vector2.Distance(player.position, this.transform.position);
        if(dist <= 0.5f)
        {
            grifAttack.Enter();
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
