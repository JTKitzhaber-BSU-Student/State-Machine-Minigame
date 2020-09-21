using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTest : MonoBehaviour
{
    public Animator grif;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            grif.Play("GrifIdle");
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            grif.Play("GrifPrepSlash");
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            grif.Play("GrifSlash");
        }
    }
}
