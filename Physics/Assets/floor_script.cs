using UnityEngine;

public class floor_script : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string prefsKeyPrefix = "LastCheckpoint";
    [SerializeField] private Transform initialSpawnPoint;

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag(playerTag))
            return;

        RespawnPlayer(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
            return;

        RespawnPlayer(other.gameObject);
    }

    private void RespawnPlayer(GameObject player)
    {
        Vector3 respawnPos;

        // Use checkpoint if one exists
        if (PlayerPrefs.HasKey($"{prefsKeyPrefix}_X"))
        {
            Vector3 savedPos = check_script.GetSavedCheckpointPosition(prefsKeyPrefix);
            respawnPos = GetSafeSpawnPosition(player, savedPos);
        }
        // Otherwise use initial spawn point
        else if (initialSpawnPoint != null)
        {
            respawnPos = initialSpawnPoint.position;
        }
        else
        {
            Debug.LogWarning("floor_script: No saved checkpoint and no initial spawn point assigned.");
            return;
        }

        MovePlayerToPosition(player, respawnPos);
        Debug.Log("Player respawned at: " + respawnPos);
    }

    private Vector3 GetSafeSpawnPosition(GameObject player, Vector3 savedPos)
    {
        Vector3 spawnPos = savedPos;
        float playerRadius = 0.5f;

        var sphere = player.GetComponent<SphereCollider>();
        if (sphere != null)
        {
            float maxScale = Mathf.Max(
                player.transform.lossyScale.x,
                player.transform.lossyScale.y,
                player.transform.lossyScale.z
            );
            playerRadius = sphere.radius * maxScale;
        }

        RaycastHit hit;
        float rayStartOffset = Mathf.Max(1f, playerRadius + 1f);
        Vector3 rayStart = savedPos + Vector3.up * rayStartOffset;

        if (Physics.Raycast(rayStart, Vector3.down, out hit, rayStartOffset + 2f))
        {
            spawnPos = hit.point + Vector3.up * (playerRadius + 0.05f);
        }

        return spawnPos;
    }

    private void MovePlayerToPosition(GameObject player, Vector3 spawnPos)
    {
        var characterController = player.GetComponent<CharacterController>();
        if (characterController != null)
        {
            characterController.enabled = false;
            player.transform.position = spawnPos;
            characterController.enabled = true;
            return;
        }

        var rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.position = spawnPos;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.WakeUp();
            rb.useGravity = true;
            return;
        }

        player.transform.position = spawnPos;
    }
}