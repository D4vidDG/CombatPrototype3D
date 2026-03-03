using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Respawner : MonoBehaviour
{
    [SerializeField] Vector2 timeToRespawn;
    [SerializeField] GameObject[] targets;

    Dictionary<Health, Vector3> positionByTarget;

    private void Awake()
    {
        positionByTarget = new Dictionary<Health, Vector3>();
    }

    void Start()
    {
        foreach (GameObject target in targets)
        {
            Health health = target.GetComponent<Health>();
            positionByTarget.Add(health, target.transform.position);
            health.OnDead.AddListener(() =>
            {
                StartCoroutine(Respawn(health));
            });
        }
    }

    private IEnumerator Respawn(Health target)
    {
        yield return new WaitForSeconds(Random.Range(timeToRespawn.x, timeToRespawn.y));
        target.gameObject.SetActive(true);
        target.transform.position = positionByTarget[target];
        target.Revive();
    }

}
