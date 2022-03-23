using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float MoveSpeed;
    public float RangeDistance;

    [HideInInspector]
    public Effect Effect;
    [HideInInspector]
    public Team Team;
    [HideInInspector]
    public Vector3 Direction;

    private Vector3 Velocity;

    public bool Rotate;

    private const float RotateSpeed = 1f;

    void Start()
    {
        Velocity = Direction * MoveSpeed;
    }

    void Update()
    {
        transform.position += Velocity * Time.deltaTime;
        if(Rotate) transform.Rotate(new Vector3(0, 0, RotateSpeed));

        var allCharacters = FindObjectsOfType<Character>();
        foreach (var character in allCharacters)
        {
            if (character.Team != Team)
            {
                var distanceToTarget = Vector3.Distance(character.transform.position, transform.position);
                if (distanceToTarget < RangeDistance)
                {
                    character.TakeEffect(Effect);
                    break;
                }
            }
        }
    }
}
