using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Tooltip("Tag used to find the player GameObject.")]
    [SerializeField] private string playerTag = "Player";

    [Tooltip("PlayerPrefs key prefix used by the checkpoints.")]
    [SerializeField] private string prefsKeyPrefix = "LastCheckpoint";

    private void Start()
    {
        // If no saved checkpoint, do nothing
        if (!PlayerPrefs.HasKey($"{prefsKeyPrefix}_X"))
            return;

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

        Vector3 savedPos = check_script.GetSavedCheckpointPosition(prefsKeyPrefix);

        // Determine a safe spawn position by raycasting down from the saved position.
        // If there's ground beneath the checkpoint, place the player just above it using the player's collider radius.
        Vector3 spawnPos = savedPos;
        float playerRadius = 0.5f;

        var sphere = player.GetComponent<SphereCollider>();
        if (sphere != null)
        {
            // convert local radius to world radius (handle non-uniform scaling conservatively)
            float maxScale = Mathf.Max(player.transform.lossyScale.x, player.transform.lossyScale.y, player.transform.lossyScale.z);
            playerRadius = sphere.radius * maxScale;
        }

        // Raycast down from slightly above the saved position
        RaycastHit hit;
        float rayStartOffset = Mathf.Max(1f, playerRadius + 1f);
        Vector3 rayStart = savedPos + Vector3.up * rayStartOffset;
        if (Physics.Raycast(rayStart, Vector3.down, out hit, rayStartOffset + 2f))
        {
            spawnPos = hit.point + Vector3.up * (playerRadius + 0.05f);
        }
        else
        {
            // If no ground found, keep savedPos but ensure it's not extremely high above ground by clamping Y (optionally)
            // For now we accept savedPos if no ground was found; you can adjust behavior if needed.
            spawnPos = savedPos;
        }

        // Teleport in a physics-safe way
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
            // Use Rigidbody to set position and clear velocities so physics starts stable
            rb.position = spawnPos;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.WakeUp();
            rb.useGravity = true;
            return;
        }

        // Fallback
        player.transform.position = spawnPos;
    }
}