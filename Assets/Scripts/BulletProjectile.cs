using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BulletProjectile : NetworkBehaviour
{
    private Rigidbody bulletRigidBody;
    public NetworkVariable<Vector3> Position = new(writePerm: NetworkVariableWritePermission.Owner);
    public NetworkVariable<Quaternion> Rotation = new(writePerm: NetworkVariableWritePermission.Owner);
    public float m_speed = 10;

    private void Awake()
    {
        bulletRigidBody = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        float speed = 10f;
        bulletRigidBody.velocity = Vector3.forward;
    }

    private void Update()
    {
       // bulletRigidBody.velocity = Vector3.forward;
        transform.position = transform.position + transform.forward * m_speed * Time.deltaTime;
        synchro();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }

    public void synchro()
    {
        if (IsOwner)
        {
            Position.Value = transform.position;
            Rotation.Value = transform.rotation;
        }
        else
        {
            transform.position = Position.Value;
            transform.rotation = Rotation.Value;
        }
    }
}
