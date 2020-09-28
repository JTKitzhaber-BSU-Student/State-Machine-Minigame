using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HydraMelee : MonoBehaviour
{
    void Start()
    {
        Invoke("killSlam", 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //Call Player Damage function
            collision.gameObject.GetComponent<Health>().TakeDamage();
        }
    }

    void killSlam()
    {
        //Debug.Log("Bullet Destroyed");
        Destroy(gameObject);
    }
}
