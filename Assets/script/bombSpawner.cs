using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class bombSpawner : NetworkBehaviour
{
    public GameObject BombPrefab;
    private List<GameObject> spawnedBomb = new List<GameObject>();
    private OwnerNetworkAnimator ownerNetworkAnimator;
    private bool delay = false;

    private void Start()
    {
        ownerNetworkAnimator = GetComponent<OwnerNetworkAnimator>();
    }

    void Update()
    {
        if (!IsOwner) return;
        if (Input.GetButtonDown("Fire1") && delay == false)
        {
            delay = true;
            SpawnBombServerRpc();
            StartCoroutine(Shooting());

        }
    }

    IEnumerator Shooting()
    {
        yield return new WaitForSeconds(0.5f);
        delay = false;
    }

    [ServerRpc]
    void SpawnBombServerRpc()
    {
        Vector3 spawnPos = transform.position + (transform.forward * 1.5f) + (transform.up * 1.2f);
        Quaternion spawnRot = transform.rotation;

        GameObject Bomb = Instantiate(BombPrefab, spawnPos, spawnRot);
        spawnedBomb.Add(Bomb);
        Bomb.GetComponent<bombScript>().bombSpawner = this;
        Bomb.GetComponent<NetworkObject>().Spawn();
        Bomb.GetComponent<Rigidbody>().AddRelativeForce(new Vector3
                                                (0, 0, 1500));
    }

    [ServerRpc (RequireOwnership = false)]
    public void DestroyServerRpc(ulong NetworkObjectId)
    {
        GameObject toDestroy = FindBombFromNetworkId(NetworkObjectId);
        if (toDestroy == null)
        {
            return;
        }
        toDestroy.GetComponent<NetworkObject>().Despawn();
        spawnedBomb.Remove(toDestroy);
        Destroy(toDestroy);
    }

    private GameObject FindBombFromNetworkId(ulong networkObjectId)
    {
        foreach(GameObject bomb in spawnedBomb)
        {
            ulong bombId = bomb.GetComponent<NetworkObject>().NetworkObjectId;
            if(bombId == networkObjectId) 
            { 
                return bomb; 
            }
        }
        return null;
    }
}