using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SlashVFXLineController : MonoBehaviour
{
    [SerializeField] Transform center;
    [SerializeField] float minDistance;
    [SerializeField] float maxDistance;
    [SerializeField] float pointLifetime;

    private LineRenderer lineRenderer;

    LinkedList<Vector3> points;
    Queue<float> times;

    Vector3 CurrentPosition => transform.position;
    Vector3 LastPosition => points.Last.Value;
    float LastTime => times.Peek();

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        points = new LinkedList<Vector3>();
        times = new Queue<float>();
    }

    void LateUpdate()
    {
        DeleteExpiredPoints();

        if (points.Count > 0)
        {
            float distance = Vector3.Distance(LastPosition, CurrentPosition);
            if (distance < minDistance)
            {
                return;
            }
            else if (minDistance <= distance && distance <= maxDistance)
            {
                int numberOfPoints = (int)Mathf.Floor(distance / minDistance);

                Vector3 localCurrentPosition = CurrentPosition - center.position;
                Vector3 localLastPosition = LastPosition - center.position;
                float currentRadius = localCurrentPosition.magnitude;
                float lastRadius = localLastPosition.magnitude;
                float currentTime = Time.time;

                for (int i = 0; i < numberOfPoints; i++)
                {
                    bool isLastPoint = i == numberOfPoints - 1;
                    if (isLastPoint)
                    {
                        EnqueuePoint(CurrentPosition, currentTime);
                    }
                    else
                    {
                        float percentage = (float)(i + 1) / numberOfPoints;
                        float time = Mathf.Lerp(LastTime, currentTime, percentage);
                        float radius = Mathf.Lerp(lastRadius, currentRadius, percentage);
                        Vector3 newLocal = Vector3.Lerp(localLastPosition, localCurrentPosition, percentage).normalized * radius;
                        Vector3 newPoint = center.position + newLocal;
                        EnqueuePoint(newPoint, time);
                    }
                }

                SetLinePositions();
            }
            else if (maxDistance < distance)
            {
                return;
            }

        }
        else
        {
            EnqueuePoint(CurrentPosition, Time.time - pointLifetime + Time.deltaTime);
            SetLinePositions();
        }
    }

    private void EnqueuePoint(Vector3 point, float time)
    {
        points.AddLast(point);
        times.Enqueue(time);
    }

    private void DequeuePoint()
    {
        points.RemoveFirst();
        times.Dequeue();
    }


    private void DeleteExpiredPoints()
    {
        if (points.Count < 1) return;
        while (times.Count > 0 && Time.time - times.Peek() >= pointLifetime)
        {
            DequeuePoint();
        }
        SetLinePositions();
    }

    private void SetLinePositions()
    {
        Vector3[] pointsArray = points.ToArray();
        lineRenderer.positionCount = pointsArray.Length;
        lineRenderer.SetPositions(pointsArray);
    }

}
