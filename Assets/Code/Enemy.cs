using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    //public GameObject Target;
    private Character Target;

    private float MoveSpeed;
    private float AttackRange;

    void Awake()
    {
        Target = FindObjectOfType<Player>();

        MoveSpeed = 2f;
        AttackRange = 0.5f;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        transform.position = Vector2.MoveTowards(transform.position, Target.transform.position, MoveSpeed * Time.deltaTime);

        var distanceToTarget = Vector3.Distance(Target.transform.position, transform.position);
        if (distanceToTarget < AttackRange)
        {

            Target.GetHit(transform.position, 0);
        }
    }
}
