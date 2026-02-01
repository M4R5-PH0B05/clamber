using UnityEngine;

public class GrappleLineRenderer : MonoBehaviour
{
    [SerializeField] private LineRenderer line;
    [SerializeField] private Transform origin;

    private bool isActive;
    private Vector3 endPoint;

    void Awake()
    {
        if (!line) line = GetComponent<LineRenderer>();
        line.positionCount = 2;
        line.enabled = false;
    }

    public void SetActive(bool active)
    {
        isActive = active;
        line.enabled = active;
    }

    public void SetEndPoint(Vector3 worldPoint)
    {
        endPoint = worldPoint;
    }

    void LateUpdate()
    {
        if (!isActive) return;

        line.SetPosition(0, origin.position);
        line.SetPosition(1, endPoint);
    }
}

