using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{ 
    [Range(0,100)]
    public int bulletHoleThreshold = 15;

    public List<GameObject> bulletholes = new List<GameObject>();
    public List<GameObject> muzzleFlashes = new List<GameObject>();
    public List<GameObject> hitMarkers = new List<GameObject>(); 
    float hitmarkerTime;
    float bulletholeTime;
    void Start () {
       
    }
    void Update () {
        hitmarkerTime+=Time.deltaTime;
        bulletholeTime+=Time.deltaTime;
        bulletholes = new List<GameObject>(GameObject.FindGameObjectsWithTag("Bullet Hole"));
        if(bulletholes.Count > bulletHoleThreshold){
            if(bulletholeTime > .01f){
                for(int i = bulletholes.Count; i > bulletHoleThreshold; i--){
                    Destroy(bulletholes[0].gameObject);
                    bulletholes.RemoveAt(0);
                    bulletholeTime = 0;
                }
            }
            
        }

        muzzleFlashes = new List<GameObject>(GameObject.FindGameObjectsWithTag("Muzzle Flash"));
        if(muzzleFlashes[0].GetComponent<AudioSource>().isPlaying == false){
            Destroy(muzzleFlashes[0]);
            muzzleFlashes.RemoveAt(0);
        }

        hitMarkers = new List<GameObject>(GameObject.FindGameObjectsWithTag("Hitmarker"));
        
    }

    void FixedUpdate(){
        if (hitmarkerTime > .5f){
            print("yo");
            for(int i = 0; i < hitMarkers.Count; i++){
                Destroy(hitMarkers[i]);
            }
            hitMarkers = new List<GameObject>();
            hitmarkerTime = 0;
        }
    }
}
