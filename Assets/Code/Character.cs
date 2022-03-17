using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{

    public SpriteRenderer Sprite;
    public int MaxHealth;

    private const float GotHitBlinkTime = 0.05f;

    private float Health;
    private bool JustGotHit;
    private float GotHitBlinkTimeTimer;

    protected virtual void Start()
    {
        Health = MaxHealth;
    }

    protected virtual void Update()
    {
        if (Health < 0) Destroy(gameObject);


        GotHitBlinkTimeTimer -= Time.deltaTime;

        if (JustGotHit)
        {
            GotHitBlinkTimeTimer = GotHitBlinkTime;
            JustGotHit = false;
        }

        if (GotHitBlinkTimeTimer > 0)
        {
            Sprite.color = Color.red;
        }
        else
        {
            Sprite.color = Color.white;
        }

    }


    public void GetHit(Vector3 attackerPosition, float damage)
    {
        var hitDirection = attackerPosition - transform.position;

        Health -= damage;


        //transform.position = Vector2.MoveTowards(transform.position, attackerPosition, -1000 * Time.deltaTime);

        JustGotHit = true;

    }
}
