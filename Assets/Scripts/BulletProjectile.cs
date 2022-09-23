using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    private Rigidbody bulletRigidBody;

    private void Awake()
    {
        bulletRigidBody = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        float speed = 10f;
        bulletRigidBody.velocity = Vector3.forward;
    }
    private void OnTriggerEnter(Collider other)
    {
         Destroy(gameObject);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //Coroutine explosion
            Destroy(gameObject);
        }

    }
}
