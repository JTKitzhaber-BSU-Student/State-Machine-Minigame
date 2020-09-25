using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTest : MonoBehaviour
{
    public Animator grif;
    public Animator wolf;
    public Animator hydra;
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
            wolf.Play("WolfIdle");
            hydra.Play("HydraIdle");
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            grif.Play("GrifPrepSlash");
            wolf.Play("WolfPrep");
            hydra.Play("HydraTailAttack");
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            grif.Play("GrifSlash");
            wolf.Play("WolfAttack");
            hydra.Play("HydraFireAttack");
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            grif.Play("GrifPrepWing");
            wolf.Play("WolfSpin");
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            grif.Play("GrifWing");
        }
    }
}
