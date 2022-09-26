using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Gun : NetworkBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float bulletSpeed = 10;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            /*if (IsServer)
            {
                Debug.Log("tir Serv");
                shoot();
            }
            else
            {
                Debug.Log("tir client");
                SubmitRequestShotServerRpc();
            }*/
        }
    }

    private void shoot ()
    {
        var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        bullet.GetComponent<NetworkObject>().Spawn();
        bullet.GetComponent<Rigidbody>().velocity = bulletSpawnPoint.forward * bulletSpeed;
    }

    [ServerRpc]
    private void SubmitRequestShotServerRpc(ServerRpcParams rpcParams = default)
    {
        Debug.Log("in submit request");
        shoot();
    }
}
