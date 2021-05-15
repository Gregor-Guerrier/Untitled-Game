using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawns : MonoBehaviour
{
    public Transform zombiePrefab;
    public Transform[] zombieNode;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(2)){
            int randomSpawn = Random.Range(0, zombieNode.Length-1);
            Instantiate(zombiePrefab, zombieNode[randomSpawn], zombieNode[randomSpawn]);
        }
    }
}
