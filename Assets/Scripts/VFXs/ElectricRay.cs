using UnityEngine;

public class ElectricRay : MonoBehaviour
{
    [SerializeField] int numberOfPoints;
    [SerializeField] float spread;
    [SerializeField] float timeToUpdate;
    LineRenderer lineRenderer;

    float timer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        timer = 0;
    }

    private void Update()
    {
        if (!lineRenderer.enabled) return;
        timer += Time.deltaTime;
        if (timer > timeToUpdate)
        {
            RandomizeRay(lineRenderer.GetPosition(0), lineRenderer.GetPosition(lineRenderer.positionCount - 1));
            timer = 0;
        }
    }

    public void Enable(bool enable)
    {
        lineRenderer.enabled = enable;
    }

    public void SetStartPoint(Vector3 startPoint)
    {
        lineRenderer.SetPosition(0, startPoint);
    }

    public void SetEndPoint(Vector3 endPoint)
    {
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, endPoint);
    }

    private void RandomizeRay(Vector3 startPoint, Vector3 endPoint)
    {
        lineRenderer.positionCount = numberOfPoints;
        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, endPoint);

        float rayLength = Vector3.Distance(startPoint, endPoint);
        Vector3 rayDirection = (endPoint - startPoint).normalized;

        for (int i = 1; i < numberOfPoints - 1; i++)
        {
            Vector3 positionOnRay = rayDirection * (i + 1) / numberOfPoints * rayLength;

            Vector3 yDirection = Vector3.Cross(Random.insideUnitSphere, rayDirection).normalized;
            while (yDirection == Vector3.zero)
            {
                yDirection = Vector3.Cross(Random.insideUnitSphere, rayDirection).normalized;

            }
            Vector3 xDirection = Vector3.Cross(yDirection, rayDirection).normalized;
            Vector2 randomUnitVector = Random.insideUnitCircle;
            Vector3 randomSpread = (xDirection * randomUnitVector.x + yDirection * randomUnitVector.y) * spread;


            Vector3 nextPosition = startPoint + positionOnRay + randomSpread;
            lineRenderer.SetPosition(i, nextPosition);
        }

        lineRenderer.enabled = true;
    }
}
