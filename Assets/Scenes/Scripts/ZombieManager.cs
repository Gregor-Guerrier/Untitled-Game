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

    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake(){
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    void FixedUpdate()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if(!playerInSightRange && !playerInAttackRange) Patrolling();
        if(playerInSightRange && !playerInSightRange) ChasePlayer();
        if(playerInAttackRange && playerInSightRange) AttackPlayer();
    }
    private void Patrolling(){
        if(!walkPointSet) SearchWalkPoint();

        if(walkPointSet) agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if(distanceToWalkPoint.magnitude < 1f) walkPointSet = false;
    }

    private void SearchWalkPoint(){
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround)){
            walkPointSet = true;
        }
    }
    private void ChasePlayer(){
        agent.SetDestination(player.position);
    }
    private void AttackPlayer(){
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if(!alreadyAttacked){
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack(){
        alreadyAttacked = false;
    }

    public bool Damage(int damage, Vector4 multipliers, string location)
    {
        print(location);
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
}
