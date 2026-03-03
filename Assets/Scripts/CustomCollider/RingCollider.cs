using EasyButtons;
using ExtensionMethods;
using Unity.VisualScripting;
using UnityEngine;

public class RingCollider : MonoBehaviour
{
    [SerializeField] float radius;
    [SerializeField] float width;
    [SerializeField] float angle;
    [SerializeField] float overlap;
    [SerializeField] int resolution;

    [Button("Create Collider")]
    void CreateCollider()
    {
        GameObject[] old = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            old[i] = transform.GetChild(i).gameObject;
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            DestroyImmediate(old[i]);
        }

        float deltaAngle = angle / resolution;
        float currentAngle = -angle / 2;
        float colliderHeight = deltaAngle * Mathf.Deg2Rad * radius + overlap / 2;

        for (int i = 0; i < resolution + 1; i++)
        {
            GameObject gameObject = new GameObject();
            gameObject.transform.parent = this.transform;
            gameObject.transform.localRotation = Quaternion.AngleAxis(currentAngle, gameObject.transform.forward);
            gameObject.transform.localScale = Vector3.one;

            CapsuleCollider capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
            capsuleCollider.direction = 1; //y-axis
            capsuleCollider.radius = width;

            if (i == 0 || i == resolution)
            {
                float angle = currentAngle;
                float correction = colliderHeight / 2 / radius * Mathf.Rad2Deg;

                if (i == 0) angle += correction;
                else angle -= correction;

                gameObject.transform.localPosition = (Vector3)VectorExtensions.PolarToVector(radius, angle);
                capsuleCollider.height = colliderHeight / 2;
            }
            else
            {
                gameObject.transform.localPosition = (Vector3)VectorExtensions.PolarToVector(radius, currentAngle);
                capsuleCollider.height = colliderHeight;
            }

            capsuleCollider.isTrigger = true;

            currentAngle += deltaAngle;
        }
    }


}

