using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class bombScript : NetworkBehaviour
{
    public bombSpawner bombSpawner;
    public GameObject bombEffectPrefab;
    private void OnCollisionEnter(Collision collision)
    {
        if (!IsOwner) return;
        if (collision.gameObject.tag == "Player")
        {
            ulong networkObjectID = GetComponent<NetworkObject>().NetworkObjectId;
            SpawnBombEffect();
            bombSpawner.DestroyServerRpc(networkObjectID);
        }
    }
    private void SpawnBombEffect()
    {
        GameObject bombEffect = Instantiate(bombEffectPrefab, transform.position, Quaternion.identity);
        bombEffect.GetComponent<NetworkObject>().Spawn();
    }
    private void Update()
    {
        if (!IsOwner) return;
        StartCoroutine(delaybeforeDestroy());

        
    }
    //Auto destroy bullet if not hiiting anything
    IEnumerator delaybeforeDestroy()
    {
        yield return new WaitForSeconds(5);
        ulong networkObjectID = GetComponent<NetworkObject>().NetworkObjectId;
        bombSpawner.DestroyServerRpc(networkObjectID);
    }
}