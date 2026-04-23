using UnityEngine;

public class colliderRespawn : MonoBehaviour
{
    private PlayerSpawner spawner;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        spawner = FindObjectOfType<PlayerSpawner>();
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (spawner != null)
            {
                spawner.RespawnPlayer();
            }
        }
    }
}
