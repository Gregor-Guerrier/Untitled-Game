using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieManager : MonoBehaviour
{
    public int health;
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;

    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    public float timeBetweenAttacks;
    bool alreadyAttacked;

    public float sightRange, attackRange, hearRange;
    public bool playerInSightRange, playerInAttackRange;
    private float time;
    public bool targeted;
    private void Awake(){
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    void FixedUpdate()
    {  
        if(Vector3.Distance(transform.position, player.position) < sightRange){
            playerInSightRange = true;
        } else if(Vector3.Distance(transform.position, player.position) > sightRange){
            playerInSightRange = false;
        }
        if(Vector3.Distance(transform.position, player.position) < attackRange){
            playerInAttackRange = true;
        } else if(Vector3.Distance(transform.position, player.position) > attackRange){
            playerInAttackRange = false;
        }
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        if(!playerInSightRange && !playerInAttackRange) Patrolling();
        if(playerInSightRange) ChasePlayer();

        if(targeted){
            agent.SetDestination(player.position);
            GameObject[] Zombies = GameObject.FindGameObjectsWithTag("Zombie");
            for(int i = 0; i < Zombies.Length; i++){
                if(Vector3.Distance(transform.position, Zombies[i].transform.position) < sightRange * 1.5f){
                    Zombies[i].GetComponent<ZombieManager>().targeted = true;
                }
            }
            agent.SetDestination(player.position);
        }
        if(targeted == true && Vector3.Distance(transform.position, player.position) > sightRange * 3f){
            float randomZ = Random.Range(-walkPointRange, walkPointRange);
            float randomX = Random.Range(-walkPointRange, walkPointRange);
            walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
            targeted = false;
        }
        
    }

    private void Patrolling(){
        time += Time.deltaTime;
        
        if(!walkPointSet) SearchWalkPoint();

        if(walkPointSet){
            agent.SetDestination(walkPoint);
            walkPointSet = false;
            
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if(distanceToWalkPoint.magnitude < 1f) walkPointSet = false;
    }

    private void SearchWalkPoint(){
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        if (time > 10){
            walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
            time = 0;
        }
        if (Physics.Raycast(walkPoint, -transform.up, 2.6f, whatIsGround)){
            walkPointSet = true;
        }
    }
    private void ChasePlayer(){
        if(CheckPath(transform.position, player.position) == true){
            agent.SetDestination(player.position);
            targeted = true;
        }

    }

    public bool Damage(int damage, Vector4 multipliers, string location)
    {
        if(location == "Head"){
            health -= (int)(damage * multipliers.x * 1.1f);
            if(health <= 0){
                Die();
                return true;
            }
            return false;
        } else if(location == "Thorax"){
            health -= (int)(damage * multipliers.y * .95f);
            if(health <= 0){
                Die();
                return true;
            }
            return false;           
        } else if(location == "Arms"){
            health -= (int)(damage * multipliers.z * .5f);
            if(health <= 0){
                Die();
                return true;
            }
            return false;            
        } else if(location == "Legs"){
            health -= (int)(damage * multipliers.w * .5f);
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

    private bool CheckPath(Vector3 position, Vector3 target)
    {
        bool result = true;
        Quaternion rotation = Quaternion.LookRotation(target - position);
        Vector3 direction = target - position;
        float distance = Vector3.Distance(position, target);

        var relativePoint = transform.InverseTransformPoint(target);
        RaycastHit[] rhit = Physics.RaycastAll(position, direction, distance);
        Debug.DrawLine(position, target, Color.yellow);
        print(relativePoint.z);
        if(rhit.Length > 0 || relativePoint.z < 0){
            result = false;
        }

        Vector3 center = Vector3.Lerp(position, target, 0.5f);
        // Debug.DrawRay(position, direction, result ? Color.green : Color.red);
        return result;
    }
}
