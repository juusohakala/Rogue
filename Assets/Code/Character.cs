using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Control
{
    Player1,
    Player2,
    Npc
}

public enum Team
{
    Players,
    Enemies
}


public class Character : MonoBehaviour
{
    public Control Control;
    public Team Team;

    public SpriteRenderer Sprite;
    public int MaxHealth;
    public float MoveSpeed;
    //public Weapon[] Weapons;

    private const float TookDamageBlinkTime = 0.05f;
    private const float TookDamageFreezeTime = 0.5f;

    private float Health;
    private float TookDamageTimer;

    private Vector3 Velocity;


    //NPC specific:
    private Character Target;

    //Player specific:
    public Action[] Actions;

    public Dictionary<string, Action> Actions2;

    void Start()
    {
        TookDamageTimer = 999;


        var allCharacters = FindObjectsOfType<Character>();
        foreach(var character in allCharacters)
        {
            if(character.Control == Control.Player1)
            {
                Target = character;
            }
        }

        Health = MaxHealth;

        //foreach(var weapon in Weapons)
        //{
        //    weapon.Team = Team;
        //    Instantiate(weapon, transform.position, Quaternion.identity);
        //}
    }

    void Update()
    {
        UpdateControls();
        UpdateActions();


        if (Health < 0) Destroy(gameObject);



        // Blink the sprite when taking damge
        TookDamageTimer += Time.deltaTime;
        if (TookDamageTimer < TookDamageBlinkTime)
        {
            Sprite.color = Color.red;
        }
        else
        {
            Sprite.color = Color.white;
        }
        if (TookDamageTimer < TookDamageFreezeTime)
        {
            Velocity = Vector3.zero;
        }



        transform.Translate(Velocity * MoveSpeed * Time.deltaTime);

    }

    void UpdateControls()
    {
        if(Control == Control.Player1)
        {
            var horizontalInput = Input.GetAxis("Horizontal");
            var verticalInput = Input.GetAxis("Vertical");
            Velocity = new Vector3(horizontalInput, verticalInput, 0);

            if (Input.GetButtonUp("Dash"))
            {
                Velocity += Velocity.normalized * 200f;
            }

            if (Input.GetButtonUp("Attack1"))
            {
                if(Actions[0] != null)
                {
                    Actions[0].Trigger();
                }
            }
            if (Input.GetButton("Attack1"))
            {
                TookDamageTimer =  0.3f; //POISTA TÄMÄ!
            }
        }
        else if(Control == Control.Player2)
        {

        }
        else if (Control == Control.Npc)
        {

            //transform.position = Vector2.MoveTowards(transform.position, Target.transform.position, MoveSpeed * Time.deltaTime);

            Velocity = (Target.transform.position - transform.position).normalized;
        }
    }

    void UpdateActions()
    {
        foreach(var action in Actions)
        {
            action.Update(Team, transform.position, Velocity.normalized);
        }
    }


    public void TakeEffect(Effect effect)
    {
        Health -= effect.Damage;


        //transform.position += hitDirection * 2;

        TookDamageTimer = 0;

    }
}
