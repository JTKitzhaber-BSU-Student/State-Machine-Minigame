using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("killBullet", 3.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            killBullet();

            //Call Enemy Damage function
        }
    }

    void killBullet()
    {
        //Debug.Log("Bullet Destroyed");
        Destroy(gameObject);
    }
}
