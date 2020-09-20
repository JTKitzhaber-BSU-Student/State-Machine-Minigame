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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //Call Player Damage function
        }
    }

    void killSlam()
    {
        //Debug.Log("Bullet Destroyed");
        Destroy(gameObject);
    }
}
