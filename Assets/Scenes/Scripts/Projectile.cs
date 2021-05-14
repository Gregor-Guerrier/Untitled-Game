using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Projectile : MonoBehaviour
{
    public float muzzleVelocity = 715.0f;
    public float maxRange = 715.0f;
    public int predictionStepsPerFrame = 6;
    public Vector3 bulletVelocity;
    private Vector3 originalLocation;
    private Transform[] bulletHolePrefabs;
    public int damage;
    public float damageRange;
    public float damageDropoff;
    public Vector4 damageMultiplier;

    private TrailRenderer trailRenderer;
    // Start is called before the first frame update
    void Start()
    {
        bulletHolePrefabs = Resources.LoadAll<Transform>("Bulletholes");
        bulletVelocity = this.transform.forward * muzzleVelocity;
        originalLocation = this.transform.position;
        trailRenderer = GetComponent<TrailRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(originalLocation, this.transform.position) > maxRange){
            Destroy(this.gameObject);
        }

        if(Vector3.Distance(originalLocation, this.transform.position) > 5){
            trailRenderer.enabled = true;
        } else {
            trailRenderer.enabled = false;
        }

        if(Vector3.Distance(originalLocation, this.transform.position) > damageRange){
            damage = (int)(damage * damageDropoff);
        }
        Vector3 point1 = this.transform.position;
        float stepSize = 1.0f / predictionStepsPerFrame;
        for(float step = 0; step < 1; step += stepSize){
            bulletVelocity += Physics.gravity * stepSize * Time.deltaTime;
            Vector3 point2 = point1 + bulletVelocity * stepSize * Time.deltaTime;
            Ray ray = new Ray(point1, point2 - point1);
            RaycastHit hitInfo;
            if(Physics.Raycast(ray, out hitInfo, (point2 - point1).magnitude)){
                if(hitInfo.collider.tag == "Environment"){
                    print(hitInfo.collider.material.name.Replace(" (Instance)", ""));
                    Transform bullethole = Instantiate(bulletHolePrefabs[Random.Range(0,bulletHolePrefabs.Length)], hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                    bullethole.GetComponentInChildren<Renderer>().material = Resources.Load<Material>("Bulletholes/Materials/" + hitInfo.collider.material.name.Replace(" (Instance)", ""));
                }
                if(hitInfo.collider.gameObject.layer == 7){
                    Transform marker = Instantiate(Resources.Load<Transform>("Hitmarker"), GameObject.FindGameObjectWithTag("Center").transform.position, Resources.Load<Transform>("Hitmarker").rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
                    print(hitInfo.collider.material.name.Replace(" (Instance)", ""));
                    PlayerManager _pm = hitInfo.collider.gameObject.GetComponent<PlayerManager>(); 
                    if(_pm.Damage(damage, damageMultiplier, hitInfo.collider.material.name.Replace(" (Instance)", "")) == true){
                        for(int i = 0; i < marker.GetComponentsInChildren<Image>().Length; i++){
                            marker.GetComponentsInChildren<Image>()[i].color = Color.red;
                        }
                    }              
                }
                Destroy(this.gameObject);
            }
            point1 = point2;
            this.transform.position = point1;
        }
    }

    void OnDrawGizmos(){
        Gizmos.color = Color.red;
        Vector3 point1 = this.transform.position;
        Vector3 predictedBulletVelocity = bulletVelocity;
        float stepSize = 0.01f;
        for(float step = 0; step < 1; step += stepSize){
            predictedBulletVelocity += Physics.gravity * stepSize;
            Vector3 point2 = point1 + predictedBulletVelocity * stepSize;
            Gizmos.DrawLine(point1,point2);
            point1 = point2;
        }
    }
}
