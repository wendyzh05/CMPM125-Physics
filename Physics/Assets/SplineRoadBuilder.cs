using UnityEngine;
using UnityEngine.Splines;

public class SplineRoadBuilder : MonoBehaviour
{
    public SplineContainer splineContainer;
    public GameObject roadPrefab;

    [Tooltip("Distance between each tile")]
    public float spacing = 2f;

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

            if (forward != Vector3.zero)
            {
                tile.transform.rotation = Quaternion.LookRotation(forward);
            }
        }
    }
}