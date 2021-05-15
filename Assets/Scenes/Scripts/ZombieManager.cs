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
    public bool playerInSightRange, playerInHearRange, playerInAttackRange;
    private float time;

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
        if(playerInAttackRange && playerInSightRange) AttackPlayer();

        
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

        if (Physics.Raycast(walkPoint, -transform.up, 2.6f, whatIsGround)){
            walkPointSet = true;
        }
    }
    private void ChasePlayer(){
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        if(playerInSightRange == true){
            Debug.Log("Path cleared: " + CheckPath(transform.position, player.position));
        } else if (time > 10){
            walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
            time = 0;
        }
        if(CheckPath(transform.position, player.position) == true) agent.SetDestination(player.position);
        print(agent.destination);
    }
    private void AttackPlayer(){
        if(CheckPath(transform.position, player.position) == true) agent.SetDestination(player.position);
        print(agent.destination);
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

    private bool CheckPath(Vector3 position, Vector3 target)
    {
        bool result = true;
        Quaternion rotation = Quaternion.LookRotation(target - position);
        Vector3 direction = target - position;
        float distance = Vector3.Distance(position, target);

        RaycastHit[] rhit = Physics.RaycastAll(position, direction, distance);
        print("ok" + rhit);
        Debug.DrawLine(position, target, Color.yellow);
        if(rhit.Length > 0){
            result = false;
        }

        Vector3 center = Vector3.Lerp(position, target, 0.5f);
        // Debug.DrawRay(position, direction, result ? Color.green : Color.red);
        return result;
    }
}
