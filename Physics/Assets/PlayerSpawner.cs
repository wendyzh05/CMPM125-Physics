using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Tooltip("Tag used to find the player GameObject.")]
    [SerializeField] private string playerTag = "Player";

    [Tooltip("PlayerPrefs key prefix used by the checkpoints.")]
    [SerializeField] private string prefsKeyPrefix = "LastCheckpoint";

    [Tooltip("Where the player should spawn if no checkpoint is active.")]
    [SerializeField] private Transform initialSpawnPoint;

    private void Start()
    {
        RespawnPlayer();
    }

    // 🔥 This is the method your spikes will call
    public void RespawnPlayer()
    {
        GameObject player = null;

        if (!string.IsNullOrEmpty(playerTag))
            player = GameObject.FindWithTag(playerTag);

        if (player == null)
            player = GameObject.Find("Player");

        if (player == null)
        {
            Debug.LogWarning("PlayerSpawner: No player found with tag '" + playerTag + "' or name 'Player'.");
            return;
        }

        Vector3 spawnPos;

        // Use saved checkpoint if it exists
        if (PlayerPrefs.HasKey($"{prefsKeyPrefix}_X"))
        {
            Vector3 savedPos = check_script.GetSavedCheckpointPosition(prefsKeyPrefix);
            spawnPos = GetSafeSpawnPosition(player, savedPos);
        }
        // Otherwise use initial spawn
        else if (initialSpawnPoint != null)
        {
            spawnPos = initialSpawnPoint.position;
        }
        else
        {
            Debug.LogWarning("PlayerSpawner: No saved checkpoint and no initial spawn point assigned.");
            return;
        }

        MovePlayerToPosition(player, spawnPos);
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