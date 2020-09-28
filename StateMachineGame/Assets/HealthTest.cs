using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthTest : MonoBehaviour
{
    public enum SCENE
    {
        WOLF, HYDRA, GRIFFIN
    };

    public SCENE currentScene;
    public int maxHealth = 100;
    public int currentHealth;

    public HealthBar healthBar;

    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(20);
        }
        if(currentHealth <= 0)
        {
            LoadTheNextScene();
        }

    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        healthBar.SetHealth(currentHealth);
    }

    private void LoadTheNextScene(){
        if (currentScene == SCENE.WOLF)
        {
            SceneManager.LoadScene("GriffinBattle");
        }
        else if (currentScene == SCENE.GRIFFIN)
        {
            SceneManager.LoadScene("HydraBattle");
        }
        else if (currentScene == SCENE.HYDRA)
        {
            SceneManager.LoadScene("WinScene");
        }
    }
}
