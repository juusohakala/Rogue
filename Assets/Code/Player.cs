using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : Character
{
    public Projectile Projectile;

    private const float AttackInterval = 0.6f;

    private float AttackIntervalTimer;

    private Character[] Targets;

    private float MoveSpeed;
    private float AttackRange;

    // Start is called before the first frame update
    void Start()
    {
        MoveSpeed = 6f;
        AttackRange = 5f;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();


        AttackIntervalTimer -= Time.deltaTime;
        if (AttackIntervalTimer < 0)
        {
            AttackIntervalTimer = AttackInterval;

            Targets = FindObjectsOfType<Enemy>();
            foreach (var target in Targets)
            {
                var distanceToTarget = Vector3.Distance(target.transform.position, transform.position);
                if (distanceToTarget < AttackRange)
                {
                    var projectile = Projectile;
                    var direction = (target.transform.position - transform.position);
                    //projectile.Velocity = Vector2.MoveTowards(transform.position, target.transform.position, 10);
                    projectile.Velocity = direction * 3;

                    Instantiate(projectile, transform.position, Quaternion.FromToRotation(target.transform.position, transform.position));
                    //Instantiate(projectile, transform.position, Quaternion.Euler(direction));
                    break;

                }
            }
        }


        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        transform.Translate(new Vector3(horizontalInput, verticalInput, 0) * MoveSpeed * Time.deltaTime);


        //if (Input.GetButton("Attack")) GetHit();
    }

}
