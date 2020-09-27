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

    public Animator anim;

    private bool shotCoolDown = true;
    public float shotDelay = 2.0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Fire();
        Look();
    }

    void FixedUpdate(){
        rb.MovePosition(rb.position + moveVelocity * Time.fixedDeltaTime);
    }

    void Look()
    {
        //Get the Screen positions of the object
        Vector2 positionOnScreen = Camera.main.WorldToViewportPoint(transform.position);

        //Get the Screen position of the mouse
        Vector2 mouseOnScreen = (Vector2)Camera.main.ScreenToViewportPoint(Input.mousePosition);

        //Get the angle between the points
        float angle = AngleBetweenTwoPoints(positionOnScreen, mouseOnScreen);

        //Ta Daaa
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle + 90));
    }
    float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }

    void Fire()
    {
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        moveVelocity = moveInput.normalized * speed;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
                anim.Play("PlayerWalk");
        }

        if (Input.GetButtonDown("Fire1"))
        {
            if (shotCoolDown)
            {
                shotCoolDown = false;
                Invoke("ResetShot", shotDelay);
                anim.Play("PlayerShoot");
                GameObject clone;
                clone = Instantiate(particle, transform.position, transform.rotation);
                Vector2 mousePos = Input.mousePosition;
                mousePos = camera.ScreenToWorldPoint(mousePos);
                float angle = Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x);
                clone.GetComponent<Rigidbody2D>().AddForce(new Vector2(Mathf.Cos(angle) * 1000f, Mathf.Sin(angle) * 1000f));
            }
        }
    }

    private void ResetShot()
    {
        shotCoolDown = true;
    }
}
