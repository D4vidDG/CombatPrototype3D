using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [SerializeField] Health target;
    [SerializeField] float damage;
    [SerializeField] KeyCode key;



    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            target.TakeDamage(damage);
        }
    }
}
