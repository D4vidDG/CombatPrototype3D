using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum ChainDirection
{
    Default,
    Forward,
    Right,
    Up
}

[ExecuteInEditMode]
public class Chain : MonoBehaviour
{
    [SerializeField] public Transform startPoint;
    [SerializeField] public Transform endPoint;
    [SerializeField] LineRenderer chainLine;


    [Header("Adjustments")]
    [SerializeField] ChainDirection chainDirection;
    [SerializeField] bool negDirection;
    [SerializeField] public float curveSizeMultiplier = 1;
    [Range(0.02f, 0.2f)][SerializeField] float curve_Smoothing = 0.1f;

    [Header("Debugging")]
    [SerializeField] Transform attackCenter;
    [SerializeField] float attackRadius;
    [SerializeField] bool simulateOnEditor = false;


    List<Vector3> newPoints = new List<Vector3>();
    List<Vector3> finalPoints = new List<Vector3>();

    private void LateUpdate()
    {
        if ((Application.isEditor && simulateOnEditor) || Application.isPlaying)
        {
            CalculatePoints();
            UpdateChainRenderer();
        }
    }

    void CalculatePoints()
    {
        if (curve_Smoothing < 0.02f)
        {
            Debug.LogError("Curve Smoothing Can't be Lower than 0.02f");
            return;
        }

        newPoints.Clear();

        Vector3 firstPoint = startPoint.position;
        newPoints.Add(firstPoint);

        Vector3 secondPoint = firstPoint + GetCurveDirection(startPoint) * startPoint.localScale.x * curveSizeMultiplier;
        newPoints.Add(secondPoint);

        Vector3 finalPoint = endPoint.position;

        Vector3 thirdPoint = finalPoint - GetCurveDirection(endPoint) * endPoint.localScale.x * curveSizeMultiplier;
        newPoints.Add(thirdPoint);

        newPoints.Add(finalPoint);

        SubdividePoints();
    }

    Vector3 GetCurveDirection(Transform point)
    {
        Vector3 curveDirection = point.right;
        if (chainDirection != ChainDirection.Default)
        {
            curveDirection = chainDirection == ChainDirection.Forward ? point.forward :
                            chainDirection == ChainDirection.Right ? point.right :
                            chainDirection == ChainDirection.Up ? point.up : point.right;
        }
        if (negDirection) curveDirection = -curveDirection;
        return curveDirection;
    }


    void SubdividePoints()
    {
        finalPoints.Clear();
        for (int i = 0; i < newPoints.Count - 1; i += 3)
        {
            for (float j = 0; j < 1; j += curve_Smoothing)
            {
                Vector3 points = Bezier.CubicBezierCurve(newPoints[i], newPoints[i + 1], newPoints[i + 2], newPoints[i + 3], j);
                finalPoints.Add(points);
            }
        }
        finalPoints[0] = startPoint.position;
        finalPoints[finalPoints.Count - 1] = newPoints[newPoints.Count - 1];
    }

    void UpdateChainRenderer()
    {
        if (finalPoints.Count <= 0) return;
        //weaponModel.position = newPoints[newPoints.Count - 1];

        chainLine.positionCount = finalPoints.Count;
        for (int i = 0; i < finalPoints.Count; i++)
        {
            chainLine.SetPosition(i, finalPoints[i]);
        }
    }

    void OnDrawGizmos()
    {

        if (newPoints.Count <= 0) return;
        Gizmos.color = Color.yellow;
        for (int i = 0; i < newPoints.Count; i++)
        {
            Gizmos.DrawSphere(newPoints[i], 0.03f);
            if (i < newPoints.Count - 1) Gizmos.DrawLine(newPoints[i], newPoints[i + 1]);
        }

        if (finalPoints.Count <= 0) return;
        Gizmos.color = Color.red;
        for (int i = 0; i < finalPoints.Count; i++)
        {
            Gizmos.DrawSphere(finalPoints[i], 0.01f);
            if (i < finalPoints.Count - 1) Gizmos.DrawLine(finalPoints[i], finalPoints[i + 1]);
        }

        if (attackCenter != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackCenter.position + Vector3.up * 0.1f, attackRadius);
        }
    }

}
