using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Gun : NetworkBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float bulletSpeed = 10f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (IsServer)
                    shoot(bulletSpawnPoint.position, bulletSpawnPoint.rotation, bulletSpawnPoint.forward);
                else
                    SubmitRequestShotServerRpc(bulletSpawnPoint.position, bulletSpawnPoint.rotation, bulletSpawnPoint.forward);
            }
        }
    }

    private void shoot (Vector3 SpawnPosition, Quaternion SpawnRotation, Vector3 SpawnDirection)
    {
        var bullet = Instantiate(bulletPrefab, SpawnPosition, SpawnRotation);
        bullet.GetComponent<NetworkObject>().Spawn();
        bullet.GetComponent<Rigidbody>().velocity = SpawnDirection * bulletSpeed;
    }
    
    [ServerRpc]
    private void SubmitRequestShotServerRpc(Vector3 p_SpawnPosition, Quaternion p_SpawnRotation, Vector3 p_SpawnDirection, ServerRpcParams rpcParams = default)
    {
        Debug.Log("in submit request");
        shoot(p_SpawnPosition, p_SpawnRotation, p_SpawnDirection);
    }
}
