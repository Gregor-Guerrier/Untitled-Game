using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{   
    public int health = 100;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public bool Damage(int damage, Vector4 multipliers, string location)
    {
        print(location);
        if(location == "Head"){
            health -= (int)(damage * multipliers.x);
            if(health <= 0){
                Die();
                return true;
            }
            return false;
        } else if(location == "Thorax"){
            health -= (int)(damage * multipliers.y);
            if(health <= 0){
                Die();
                return true;
            }
            return false;           
        } else if(location == "Arms"){
            health -= (int)(damage * multipliers.z);
            if(health <= 0){
                Die();
                return true;
            }
            return false;            
        } else if(location == "Legs"){
            health -= (int)(damage * multipliers.w);
            if(health <= 0){
                Die();
                return true;
            }
            return false;            
        } else {
            health -= (int)(damage);
            if(health <= 0){
                Die();
                return true;
            }
            return false;            
        }

        
        
    }

    private void Die(){
        Destroy(gameObject);
    }
}
