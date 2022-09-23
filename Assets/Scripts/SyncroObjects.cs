using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SyncroObjects : NetworkBehaviour
{
    public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();
    [SerializeField]
    private GameObject m_cam;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            Vector3 tmp = new Vector3(19.63382f, 5.8f, 0.05f);
            Move(tmp);
        }
        else
            m_cam.SetActive(false);
    }

    public void Move(Vector3 pos)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            //var tmp = GetPositionOnPlane();
            transform.position = pos;
            Position.Value = pos;
        }
        else
        {
            SubmitPositionRequestServerRpc(pos);
        }
    }

    [ServerRpc]
    void SubmitPositionRequestServerRpc(Vector3 p_pos, ServerRpcParams rpcParams = default)
    {
        Position.Value = p_pos;
    }

    static Vector3 GetPositionOnPlane()
    {
        return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
    }

    void Update()
    {
        transform.position = Position.Value;
    }
}
