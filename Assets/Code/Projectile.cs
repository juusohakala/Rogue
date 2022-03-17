using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [HideInInspector]
    public Vector3 Velocity;


    private Character[] Targets;

    void Start()
    {
        
    }

    void Update()
    {
        transform.position += Velocity * Time.deltaTime;

        Targets = FindObjectsOfType<Enemy>();
        foreach (var target in Targets)
        {
            var distanceToTarget = Vector3.Distance(target.transform.position, transform.position);
            if (distanceToTarget < 0.5f)
            {
                target.GetHit(transform.position, 1f);
            }
        }
    }
}
