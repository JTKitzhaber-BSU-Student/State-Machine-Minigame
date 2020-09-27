using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hydra : MonoBehaviour
{
    Animator anim;
    public Transform player;
    public GameObject hydraProjectile;
    public GameObject hydraSlam;
    public float shotStrength = 250f;
    HydraState currentState;

    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();
        currentState = new HydraIdle(this.gameObject, anim, player);
        Debug.Log(currentState);
    }

    // Update is called once per frame
    void Update()
    {
        currentState = currentState.Process();
    }

    public void SpawnShot(float angle)
    {
        GameObject shot = Instantiate(hydraProjectile, transform.position, transform.rotation);
        shot.GetComponent<Rigidbody2D>().rotation = angle;
        shot.GetComponent<Rigidbody2D>().AddForce(new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle)) * shotStrength);
        anim.Play("HydraFireAttack");
    }
    public void SpawnSlam()
    {
        anim.Play("HydraPrep");
        StartCoroutine(Slam());
    }

    IEnumerator Slam()
    {
        yield return new WaitForSeconds(.5f);
        GameObject slam = Instantiate(hydraSlam, transform.position, transform.rotation);
    }
}
