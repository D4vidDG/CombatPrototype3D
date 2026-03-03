using System.Collections.Generic;
using ExtensionMethods;
using UnityEngine;
using static UnityEngine.ParticleSystem;

[ExecuteInEditMode]
public class Shockwave : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] MeshCollider meshCollider;
    [SerializeField] ParticleSystem particles;
    [SerializeField] float maxRadius;
    [SerializeField] float startRadius;
    [SerializeField] float speed;
    [SerializeField] float angle;
    [SerializeField] float collisionHeight = 1f;
    [SerializeField] LayerMask targetLayer;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] int lineResolution;
    [SerializeField] TickDamage tickDamage;

    float radius;

    List<Health> targetsInShockwave;

    void Start()
    {

        if (!Application.isPlaying) return;

        radius = startRadius;
        float lifetime = (maxRadius - startRadius) / speed;

        MainModule main = particles.main;
        main.duration = lifetime;
        main.startLifetime = lifetime;
        main.startSpeed = speed;
        main.loop = false;

        ShapeModule shape = particles.shape;
        shape.radius = radius;
        shape.arc = angle - 5f;
        shape.rotation = new Vector3(shape.rotation.x, shape.rotation.y, -shape.arc / 2);

        particles.Play();

        PositionOnFloor();

        targetsInShockwave = new List<Health>();

    }

    private void PositionOnFloor()
    {
        bool hit = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, 100f, groundLayer);
        if (hit && Vector3.Angle(Vector3.up, hitInfo.normal) < 60f)
        {
            transform.position = hitInfo.point;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.isEditor && !Application.isPlaying)
        {
            DebugShockwave();
            return;
        }

        if (radius >= maxRadius)
        {
            meshCollider.enabled = false;
            lineRenderer.enabled = false;
            particles.Stop();
            return;
        }

        radius = Mathf.Min(maxRadius, radius + speed * Time.deltaTime);
        UpdateShockwave(radius);
    }

    private void UpdateShockwave(float radius)
    {
        float deltaAngle = angle / lineResolution;
        float currentAngle = -angle / 2;

        Vector3[] points = new Vector3[lineResolution];

        for (int i = 0; i < lineResolution; i++)
        {
            points[i] = (Vector3)VectorExtensions.PolarToVector(radius, currentAngle);
            currentAngle += deltaAngle;
        }

        lineRenderer.positionCount = lineResolution;
        lineRenderer.SetPositions(points);

        CreateColliderMesh();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (LayerMaskExtensions.IsInLayerMask(targetLayer, other.gameObject))
        {
            Health target = other.GetComponentInParent<Health>();
            if (target != null && !targetsInShockwave.Contains(target))
            {
                targetsInShockwave.Add(target);
                target.TakeDamage(damage);
                tickDamage.ApplyTickDamage(target);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (LayerMaskExtensions.IsInLayerMask(targetLayer, other.gameObject))
        {
            Health target = other.GetComponentInParent<Health>();
            if (target != null && targetsInShockwave.Contains(target))
            {
                targetsInShockwave.Remove(target);
            }
        }
    }


    private void CreateColliderMesh()
    {
        Mesh mesh = new Mesh();
        lineRenderer.BakeMesh(mesh, false);

        Vector3[] vertices = mesh.vertices;
        List<Vector3> terrainVertices = new();
        foreach (var vert in vertices)
        {
            terrainVertices.Add(vert);
        }

        List<Vector3> verticesList = new(terrainVertices);
        List<Vector3> verticesExtrudedList = new();
        List<int> indices = new();

        for (int i = 0; i < verticesList.Count; i++)
        {
            verticesExtrudedList.Add(new Vector3(verticesList[i].x, verticesList[i].y, verticesList[i].z + collisionHeight));
        }

        //add the extruded parts to the end of verteceslist
        verticesList.AddRange(verticesExtrudedList);
        for (int i = 0; i < terrainVertices.Count; i++)
        {
            int N = terrainVertices.Count;
            int i1 = i;
            int i2 = (i1 + 1) % N;
            int i3 = i1 + N;
            int i4 = i2 + N;
            indices.Add(i1);
            indices.Add(i3);
            indices.Add(i4);
            indices.Add(i1);
            indices.Add(i4);
            indices.Add(i2);
        }

        //var mesh = meshFilter.mesh;
        Mesh extrudedMesh = new Mesh();
        extrudedMesh.Clear();
        extrudedMesh.vertices = verticesList.ToArray();
        extrudedMesh.triangles = indices.ToArray();
        extrudedMesh.RecalculateNormals();
        extrudedMesh.RecalculateBounds();
        extrudedMesh.Optimize();

        meshCollider.sharedMesh = extrudedMesh;
    }


    private void DebugShockwave()
    {
        ShapeModule shape = particles.shape;
        shape.arc = angle - 5f;
        shape.rotation = new Vector3(shape.rotation.x, shape.rotation.y, -shape.arc / 2);
        shape.radius = maxRadius;

        UpdateShockwave(maxRadius);
    }
}
