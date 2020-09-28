using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HydraBullet : MonoBehaviour
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
        if (collision.gameObject.tag == "Player")
        {
            killBullet();

            collision.gameObject.GetComponent<Health>().TakeDamage();
        }
    }

    void killBullet()
    {
        //Debug.Log("Bullet Destroyed");
        Destroy(gameObject);
    }
}
