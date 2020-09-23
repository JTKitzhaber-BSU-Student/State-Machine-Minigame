using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int shootRate;

    // Start is called before the first frame update
    void Start()
    {
     
        //Invoke("killBullet", 3.0f);
        StartCoroutine(KillBullet());
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            
            KillBullet();

            //Call Enemy Damage function
        }
    }

    IEnumerator KillBullet()
    {
        yield return new WaitForSeconds(1f);
        //Debug.Log("Bullet Destroyed");
        Destroy(gameObject);
    }

    //after two seconds reset the shoot rate
    IEnumerator ResetShoot()
    {
        yield return new WaitForSeconds(2f);
        shootRate = 0;
    }

    void CountShots()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            shootRate++;
            if(shootRate == 3)
            {
                ResetShoot();
            }
        }
    }
}
