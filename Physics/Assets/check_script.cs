using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class check_script : MonoBehaviour
{
    [Header("Activation")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private Color activeColor = Color.green;
    [SerializeField] private Color inactiveColor = Color.red;

    [Header("Persistence")]
    [SerializeField] private string prefsKeyPrefix = "LastCheckpoint";

    [Header("Events")]
    [SerializeField] private UnityEvent onActivate;

    private Renderer[] _renderers;
    private bool _isActive;

    private void Awake()
    {
        // Ensure the collider is a trigger so OnTriggerEnter is called
        var col = GetComponent<Collider>();
        col.isTrigger = true;

        _renderers = GetComponentsInChildren<Renderer>();
        _isActive = false;
        SetColor(inactiveColor);

        // If this checkpoint matches the last saved checkpoint, restore its active state visually
        if (PlayerPrefs.HasKey($"{prefsKeyPrefix}_Name"))
        {
            string savedName = PlayerPrefs.GetString($"{prefsKeyPrefix}_Name");
            if (savedName == name)
            {
                _isActive = true;
                SetColor(activeColor);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
            return;

        Activate();
    }

    public void Activate()
    {
        // Deactivate other checkpoints in the scene
        var all = FindObjectsOfType<check_script>();
        foreach (var cp in all)
        {
            if (cp != this)
                cp.Deactivate();
        }

        _isActive = true;
        SetColor(activeColor);
        SaveCheckpoint();
        onActivate?.Invoke();
        Debug.Log($"Checkpoint activated: {name}");
    }

    public void Deactivate()
    {
        if (!_isActive) return;
        _isActive = false;
        SetColor(inactiveColor);
    }

    private void SetColor(Color c)
    {
        if (_renderers == null) return;
        foreach (var r in _renderers)
        {
            // Use material property if available
            if (r.sharedMaterial != null && r.sharedMaterial.HasProperty("_Color"))
                r.material.color = c;
        }
    }

    private void SaveCheckpoint()
    {
        var pos = transform.position;
        PlayerPrefs.SetFloat($"{prefsKeyPrefix}_X", pos.x);
        PlayerPrefs.SetFloat($"{prefsKeyPrefix}_Y", pos.y);
        PlayerPrefs.SetFloat($"{prefsKeyPrefix}_Z", pos.z);
        PlayerPrefs.SetString($"{prefsKeyPrefix}_Name", name);
        PlayerPrefs.Save();
    }

    // Utility: get last saved position (other classes can call this)
    public static Vector3 GetSavedCheckpointPosition(string prefsKeyPrefix = "LastCheckpoint")
    {
        if (!PlayerPrefs.HasKey($"{prefsKeyPrefix}_X"))
            return Vector3.zero;

        float x = PlayerPrefs.GetFloat($"{prefsKeyPrefix}_X");
        float y = PlayerPrefs.GetFloat($"{prefsKeyPrefix}_Y");
        float z = PlayerPrefs.GetFloat($"{prefsKeyPrefix}_Z");
        return new Vector3(x, y, z);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = _isActive ? activeColor : inactiveColor;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}
