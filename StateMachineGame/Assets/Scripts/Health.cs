using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    public int numOfHearts;
    public GameObject[] hearts;
    public bool canTakeDamge;
    SpriteRenderer spriteRenderer;


    void Start(){
        canTakeDamge = true;
        spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    void Update(){
        //Debug.Log(canTakeDamge);
        for(int i = 0; i < hearts.Length; i++)
        {
            if(i < numOfHearts )
            {
                hearts[i].SetActive(true);
            }
            else 
            {
                hearts[i].SetActive(false);
            }
        }

        if(numOfHearts <= 0)
        {
            SceneManager.LoadScene("DeathScene");
        }
    }

    public void TakeDamage(){
        if(canTakeDamge)
        {
            numOfHearts--;
            canTakeDamge = false;
            
            Color tmp =  spriteRenderer.color;
            tmp.a = 0.5f;
            spriteRenderer.color = tmp;
            StartCoroutine(Immunity());
        }

    }

    IEnumerator Immunity()
    {
        Debug.Log("should be able to take damge");
        yield return new WaitForSeconds(1f);
        Color tmp =  spriteRenderer.color;
        tmp.a = 1f;
        spriteRenderer.color = tmp;
        canTakeDamge = true;
    }

}
