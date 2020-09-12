using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;  // Added since we're using a navmesh.

public class AI : MonoBehaviour
{
    // Variables to handle what we need to send through to our state.
    SpriteRenderer spriteRenderer;
    public Transform player;  // To store the transform of the player. This will let the guard know where the player is, so it can face the player and know whether it should be shooting or chasing (depending on the distance).
    State currentState;

    void Start()
    {
        spriteRenderer = this.GetComponent<SpriteRenderer>(); // Grab agents Animator component.
        currentState = new Idle(this.gameObject, spriteRenderer, player); // Create our first state.
    }

    void Update()
    {
        currentState = currentState.Process(); // Calls Process method to ensure correct state is set.
    }
}
