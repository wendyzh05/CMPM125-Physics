using UnityEngine;
using UnityEngine.Splines;

public class SplineRoadBuilder : MonoBehaviour
{
    public SplineContainer splineContainer;
    public GameObject roadPrefab;

    [Tooltip("Distance between each tile")]
    public float spacing = 2f;

    [Tooltip("Scale applied to each road tile (X = width, Y = height, Z = length)")]
    [SerializeField] private Vector3 tileScale = new Vector3(3f, 1f, 3f);

    private void Start()
    {
        BuildRoad();
    }

    public void BuildRoad()
    {
        if (splineContainer == null || roadPrefab == null)
            return;

        Spline spline = splineContainer.Spline;

        float length = spline.GetLength();
        int count = Mathf.FloorToInt(length / spacing);

        for (int i = 0; i <= count; i++)
        {
            float t = i / (float)count;

            Vector3 position = spline.EvaluatePosition(t);
            Vector3 forward = spline.EvaluateTangent(t);

            GameObject tile = Instantiate(roadPrefab, position, Quaternion.identity, transform);

            // ✅ Apply scaling here
            tile.transform.localScale = tileScale;

            // ✅ Align tile to spline direction
            if (forward != Vector3.zero)
            {
                tile.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
            }
        }
    }
}