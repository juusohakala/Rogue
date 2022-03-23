using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum WeaponType
//{
//    Melee,
//    ProjectileLauncher
//}

[Serializable]
public class Action
{
    public bool AutoTrigger;
    public float Cooldown;
    public float RangeDistance;
    public float RangeAngle;
    public int MaxTargets;
    public Projectile Projectile;
    public Effect Effect;

    private float CooldownTimer;
    private bool IsTriggered;

    private const float RangeDistance360 = 0.1f;

    public void Trigger()
    {
        IsTriggered = true;
    }

    void Start()
    {

    }


    public void UpdateSingle(Character target, Team userTeam, Vector3 userPosition, Vector3 actDirection)
    {
        if (Projectile == null)
        {
            target.TakeEffect(Effect);
        }
        else
        {
            var projectile = Projectile;
            projectile.Effect = Effect;
            projectile.Team = userTeam;
            projectile.Direction = (target.transform.position - userPosition).normalized;
            UnityEngine.Object.Instantiate(projectile, userPosition, Quaternion.identity);
        }

    }

    public void Update(Team userTeam, Vector3 userPosition, Vector3 actDirection)
    {
        CooldownTimer -= Time.deltaTime;

        if (CooldownTimer < 0 && (IsTriggered || AutoTrigger))
        {
            IsTriggered = false;
            CooldownTimer = Cooldown;

            var allCharacters = UnityEngine.Object.FindObjectsOfType<Character>();
            var targetsCounter = 0;
            foreach (var character in allCharacters)
            {
                if (character.Team != userTeam)
                {
                    var distanceToTarget = Vector3.Distance(character.transform.position, userPosition);
                    var directionToTarget = (character.transform.position - userPosition).normalized;
                    var angleDifference = Vector3.Angle(directionToTarget, actDirection);

                    if ((distanceToTarget < RangeDistance && angleDifference < RangeAngle / 2) || distanceToTarget < RangeDistance360)
                    {
                        UpdateSingle(character, userTeam, userPosition, actDirection);
                        targetsCounter++;
                        if (targetsCounter >= MaxTargets) break;
                    }
                }
            }

            
        }
    }
}
