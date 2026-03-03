using System.Collections.Generic;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class AimingLine : MonoBehaviour
{
    const double LINE_TOLERANCE = 0.1;

    [SerializeField] float distancePerPoint;
    [SerializeField] public float maxLineLength;
    [SerializeField] GameObject dotPrefab;
    [SerializeField] LayerMask obstacleMask;
    [SerializeField] bool debug;
    [SerializeField] Color debugColor;


    Stack<GameObject> activeDots;
    int NumberOfDots => (int)(maxLineLength / distancePerPoint) + 5;
    int NumberOfActiveDots => activeDots.Count;
    GameObject NextInactiveDot => transform.GetChild(NumberOfActiveDots).gameObject;
    GameObject LastActiveDot => activeDots.Peek();
    float LineLength
    {
        get
        {
            if (NumberOfActiveDots == 0) return 0;
            else return Vector3.Distance(LastActiveDot.transform.position, transform.position);
        }
    }

    void Awake()
    {
        activeDots = new Stack<GameObject>();
        for (int i = 0; i < NumberOfDots; i++)
        {
            GameObject dot = Instantiate(dotPrefab, this.transform);
            dot.SetActive(false);
        }
    }

    void Update()
    {
        float distanceToTarget = CalculateDistanceToTarget();
        ArrangeDots(distanceToTarget);
    }

    public Vector3 GetEndPoint()
    {
        if (NumberOfActiveDots > 0)
        {
            return LastActiveDot.transform.position;
        }
        else
        {
            return transform.position;
        }
    }


    private float CalculateDistanceToTarget()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        bool obstacleHit = Physics.Raycast(
            ray: ray,
            out RaycastHit obstacleHitInfo,
            maxDistance: maxLineLength,
            layerMask: obstacleMask
            );

        if (obstacleHit)
        {
            return Vector3.Distance(obstacleHitInfo.point, transform.position);
        }
        else
        {
            return maxLineLength;
        }
    }

    private void ArrangeDots(float distanceToTarget)
    {
        //if line is shorther
        if (LineLength < distanceToTarget)
        {
            //correct position of last dot
            if (0 < NumberOfActiveDots)
            {
                LastActiveDot.transform.position = transform.position + NumberOfActiveDots * distancePerPoint * transform.forward;
            }

            //increase line length
            while (LineLength < distanceToTarget - LINE_TOLERANCE)
            {
                GameObject dot = NextInactiveDot;
                dot.gameObject.SetActive(true);
                activeDots.Push(dot.gameObject);
                dot.transform.position = transform.position + NumberOfActiveDots * distancePerPoint * transform.forward;
            }

            //place last active dot in the target position
            LastActiveDot.transform.position = transform.position + transform.forward * distanceToTarget;
        }
        //else if line is larger
        else if (distanceToTarget + LINE_TOLERANCE < LineLength)
        {
            //decrease line
            while (distanceToTarget < LineLength)
            {
                activeDots.Pop().SetActive(false);
            }

            //activate dot and place it in target position
            NextInactiveDot.transform.position = transform.position + transform.forward * distanceToTarget;
            NextInactiveDot.SetActive(true);
            activeDots.Push(NextInactiveDot);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (debug)
        {
            Gizmos.color = debugColor;
            for (int i = 0; i < NumberOfDots; i++)
            {
                Gizmos.DrawSphere(transform.position + (i + 1) * distancePerPoint * transform.forward, 0.1f);
            }
        }
    }
}
