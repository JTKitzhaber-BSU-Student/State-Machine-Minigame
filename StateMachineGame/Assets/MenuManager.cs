using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject[] panels;
    
    public void GoTo(string name)
    {
        foreach (GameObject p in panels)
        {
            if (p.name == name)
            {
                p.SetActive(true);
            }
            else
            {
                p.SetActive(false);
            }
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("WolfBattle");
    }
}
