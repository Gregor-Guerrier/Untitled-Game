using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{   
    //Values
    public int energyReductionTime;
    public int hydrationReductionTime;
    public int staminaReductionModifier;
    public float weightLimit;

    private int health = 100;
    private int energy = 100;
    private int hydration = 100;
    public float stamina = 100;
    public float weight = 0;
    private bool overWeight = false;
    public bool canSprint = true;
    public bool isSprinting = false;
    
    private float timeUntilNext;

    private PlayerMovement playerMovement;
    private float originalSpeed, originalSprint, originalJump;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        originalSpeed = playerMovement.speed;
        originalSprint = playerMovement.sprintMultiplier;
        originalJump = playerMovement.jumpHeight;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if(isSprinting || !playerMovement.isGrounded){timeUntilNext = 1.5f;} else if(!isSprinting && playerMovement.isGrounded){timeUntilNext -= Time.deltaTime;}
        if(timeUntilNext <= 0 && stamina <= 100){RechargeStamina();}
        if(energy <= 0){NoEnergy();}
        if(hydration <= 0){NoHydration();}
        if(stamina >= 100){stamina = 100;}
        if(weight > weightLimit){overWeight = true;} else if (weight <= weightLimit){overWeight = false;}

        if(overWeight == true)
        {
            if(weight > weightLimit * 1.25f)
            {
                playerMovement.speed = originalSpeed * (1 - ((weight - weightLimit * 1.25f)/(weightLimit * .4f)));
                playerMovement.jumpHeight = originalJump * (1 - ((weight - weightLimit * 1.25f)/(weightLimit * .4f)));
                playerMovement.sprintMultiplier = 1;
                if(playerMovement.speed <= 1){playerMovement.speed = 1;}
                if(playerMovement.jumpHeight <= 1f){playerMovement.jumpHeight = 0;}
            } else
            {
                playerMovement.speed = originalSpeed;
                playerMovement.jumpHeight = originalJump;
                playerMovement.sprintMultiplier = 1 + ((.25f*((weightLimit * 1.25f - weightLimit) - (weight - weightLimit)))/(weightLimit * 1.25f - weightLimit));
                if(playerMovement.sprintMultiplier <= 1.05f){playerMovement.sprintMultiplier = 1.05f;}
            }
        } else if(overWeight == false)
        {
            playerMovement.speed = originalSpeed;
            playerMovement.jumpHeight = originalJump;
            playerMovement.sprintMultiplier = originalSprint;
        }
    }
    public bool Damage(int damage, Vector4 multipliers, string location)
    {
        print(location);
        if(location == "Head")
        {
            health -= (int)(damage * multipliers.x);
            if(health <= 0)
            {
                Die();
                return true;
            }
            return false;
        } else if(location == "Thorax")
        {
            health -= (int)(damage * multipliers.y);
            if(health <= 0)
            {
                Die();
                return true;
            }
            return false;           
        } else if(location == "Arms")
        {
            health -= (int)(damage * multipliers.z);
            if(health <= 0)
            {
                Die();
                return true;
            }
            return false;            
        } else if(location == "Legs")
        {
            health -= (int)(damage * multipliers.w);
            if(health <= 0)
            {
                Die();
                return true;
            }
            return false;            
        } else 
        {
            health -= (int)(damage);
            if(health <= 0)
            {
                Die();
                return true;
            }
            return false;            
        }

        
        
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    public void Sprinting(){if(stamina <= 0){canSprint = false;} else {stamina -= Time.deltaTime * 15 * staminaReductionModifier;}}
    public void Jumping(){stamina -= 20 * staminaReductionModifier;}

    private void RechargeStamina()
    {
        if(stamina < 10)
        {
            canSprint = false;
        } else if (stamina > 10)
        {
            canSprint = true;
        }
        stamina += Time.deltaTime * 10 * 1 + (1-staminaReductionModifier);
    }
    public void Eat(int addEnergy){energy += addEnergy;}
    public void Drink(int addHydration){hydration += addHydration;}
    private void NoEnergy()
    {

    }
    private void NoHydration()
    {

    }
}
