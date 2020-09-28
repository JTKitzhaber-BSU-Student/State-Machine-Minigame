using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GriffinColl : MonoBehaviour
{
    public GameObject Player;
    void OnTriggerStay2D(Collider2D coll){
        if(coll.gameObject.tag == "Enemy"){
            // currentState = new WolfCharge(this.gameObject, spriteRenderer, anim, player);
            // Debug.Log("player should take damage");
            Player.GetComponent<Health>().TakeDamage();
        }
       
    }
}
