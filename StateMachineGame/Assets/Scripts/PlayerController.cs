using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5;

    private Rigidbody2D rb;
    private Vector2 moveVelocity;

    public GameObject particle;
    public Camera camera;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        moveVelocity = moveInput.normalized * speed;

         if (Input.GetButtonDown("Fire1"))
        {
                GameObject clone;
                clone = Instantiate(particle, transform.position, transform.rotation);
                Vector2 mousePos = Input.mousePosition;
                mousePos = camera.ScreenToWorldPoint(mousePos);
                float angle = Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x);
                clone.GetComponent<Rigidbody2D>().AddForce(new Vector2 (Mathf.Cos(angle) * 1000f, Mathf.Sin(angle) * 1000f));

        }
    }

    void FixedUpdate(){
        rb.MovePosition(rb.position + moveVelocity * Time.fixedDeltaTime);
    }
}
