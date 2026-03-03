using UnityEngine;

public class ElectricSpreadEffect : MonoBehaviour
{
    [SerializeField] public float lifetime;

    ElectricRay[] electricRays;

    float lifeTimer;
    bool activated = false;

    private void Awake()
    {
        electricRays = GetComponentsInChildren<ElectricRay>();
    }

    private void Update()
    {
        if (!activated) return;
        lifeTimer += Time.deltaTime;
        if (lifeTimer > lifetime)
        {
            Destroy(this.gameObject);
        }
    }

    public void Activate(Vector3 startPoint, Vector3 endPoint)
    {
        foreach (ElectricRay electricRay in electricRays)
        {
            electricRay.Enable(true);
            electricRay.SetStartPoint(startPoint);
            electricRay.SetEndPoint(endPoint);
        }

        lifeTimer = 0;
        activated = true;
    }


}
