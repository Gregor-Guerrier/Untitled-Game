using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shoot : MonoBehaviour {
    
	[Header("Gun Parts")]
    public Transform barrel;
	public Transform sight;
    public Transform bullet;

	[Header("Attachments")]
	public Transform aBarrel;
	[Header("Stats")]
    public float bulletVelocity;
	public float maxRange;
    public int magazineSize;
    public int magazineAmount;
    public float roundsPerMinute;
	public float projectilesPerShot;
	public float spread;
	private float originalSpread;
	public string[] modes;
	private int selectedMode;
	public Vector2 xRecoil;
	public Vector2 yRecoil;

	[Header("Damage")]
	public int damage;
	public float damageRange;
	public float damageDropoff;
	[Header("Multipliers")]
	public float Head;
	public float Thorax;
	public float Arms;
	public float Legs;

    private float timePassed;
    public float reloadDelay;
    private float reloadPassed;
    private int ammunition;
    private int amountInMag;
    
	[Header("Audio")]
    public AudioSource _as;
    public AudioClip reload;

	private bool burst;
    private bool aiming;
	private Camera camera;
	private float time;
	private Vector3 originalGunPosition;
	private Vector3 originalCameraPosition;
	private MouseLook mouseLook;
	private bool recoil;
	private float recoilTime;
	public RectTransform hipFire;
	public Transform gunModel;
	private Vector3 recoilPosition;
	private float bloomEffect;
	[Range(1,5)]
	public float maxBloom;

	private bool isShot;
	// Use this for initialization
	void Start () {
		camera = GetComponentInParent<Camera>();
		mouseLook = GetComponentInParent<MouseLook>();
		originalCameraPosition = camera.transform.localPosition;
		originalGunPosition = transform.localPosition;
		originalSpread = spread;
		ammunition = magazineSize * magazineAmount;
		amountInMag = magazineSize;
		timePassed = 60/roundsPerMinute;
		reloadPassed = reloadDelay;
		hipFire.sizeDelta = new Vector2(9*spread, 9*spread);
		recoilPosition = gunModel.localPosition;
		
	}
	
	// Update is called once per frame
	void Update () {
		bloomEffect -= Time.deltaTime*5;
		if(bloomEffect < 1){
			bloomEffect = 1;
		} else if(bloomEffect > maxBloom){
			bloomEffect = maxBloom;
		}
		recoilTime -= Time.deltaTime;
	    timePassed -= Time.deltaTime;
	    reloadPassed -= Time.deltaTime;
		if(projectilesPerShot == 1){
			NotShotgun();
		} else {
			Shotgun();
		}
		if (Input.GetKeyDown(KeyCode.R) && ammunition > 0 && reloadPassed <= 0){
			Reload();		    
		}

		if(Input.GetKeyDown(KeyCode.B) && modes.Length > 1){
			Switch();
		}
		if(Input.GetMouseButton(1) || Input.GetAxis("Aiming") > .1f){
			spread = 0;
			bloomEffect = 1;
			if(time < 1){
				time += Time.deltaTime * 3;
				transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0,  0-sight.localPosition.y/10, -sight.localPosition.z * 1.25f), time);
				hipFire.sizeDelta = Vector2.Lerp(hipFire.sizeDelta, new Vector2(9*spread, 9*spread), Time.deltaTime);
			}
			if(time > .35f){
				for(int i = 0; i < hipFire.gameObject.GetComponentsInChildren<Image>().Length; i++){
					hipFire.gameObject.GetComponentsInChildren<Image>()[i].color = Vector4.Lerp(new Vector4(0, 0, 0, 165), new Vector4(0, 0, 0, 0), Time.deltaTime);
				}
			}
			if(time > .2f){
				hipFire.gameObject.SetActive(false);
			}
			
			
		} else if(!Input.GetMouseButton(1) || Input.GetAxis("Aiming") < .1f){
			if(time > 0){
				time -= Time.deltaTime * 3;
				transform.localPosition = Vector3.Lerp(transform.localPosition, originalGunPosition, 1-time);
				hipFire.sizeDelta = Vector2.Lerp(hipFire.sizeDelta, new Vector2(9*spread, 9*spread), time);
			} 
			if(time < .2f){
				hipFire.gameObject.SetActive(true);
				for(int i = 0; i < hipFire.gameObject.GetComponentsInChildren<Image>().Length; i++){
					hipFire.gameObject.GetComponentsInChildren<Image>()[i].color = Vector4.Lerp(new Vector4(0, 0, 0, 0), new Vector4(0, 0, 0, 165), ((.45f-time)*10));
				}
				spread = originalSpread * bloomEffect;
				hipFire.sizeDelta = Vector2.Lerp(hipFire.sizeDelta, new Vector2(9*spread, 9*spread), Time.deltaTime);
			}
			
		}
		if(recoilTime > 0){
			mouseLook.xRotation -= Random.Range(xRecoil.x, xRecoil.y) * Time.deltaTime*4;
			mouseLook.playerBody.Rotate(Vector3.up * Random.Range(yRecoil.x, yRecoil.y) * Time.deltaTime*4);
			gunModel.localPosition = Vector3.Lerp(recoilPosition, new Vector3(recoilPosition.x, recoilPosition.y, recoilPosition.z - .07f), Time.deltaTime*50);
		} else if(recoilTime < 0){
			gunModel.localPosition = Vector3.Lerp(gunModel.localPosition, recoilPosition, Time.deltaTime*50);
		} 

		if(Input.GetAxis("Shooting") < .1f){
			isShot = false;
		}
	}

	
	//For Rifles, Subs, and Others
	private void NotShotgun(){
		if (Input.GetButton("Fire1") && amountInMag > 0 && timePassed <= 0 && modes[selectedMode].Split(' ')[0] == "auto" && burst == false || Input.GetAxis("Shooting") > .1f && amountInMag > 0 && timePassed <= 0 && modes[selectedMode].Split(' ')[0] == "auto" && burst == false){
			var rotationX = barrel.rotation.eulerAngles.x;
			rotationX += Random.insideUnitCircle.x * Random.Range(-spread, spread);
			var rotationY = barrel.rotation.eulerAngles.y;
			rotationY += Random.insideUnitCircle.y * Random.Range(-spread, spread);
			var rot = Quaternion.Euler(rotationX, rotationY, 0);

			Transform b = Instantiate(bullet, barrel.position, rot);
			Instantiate(_as.transform, aBarrel.position, aBarrel.rotation, aBarrel);
            Projectile p = b.GetComponent<Projectile>();
			p.muzzleVelocity = bulletVelocity;
			p.maxRange = maxRange;
			p.damageMultiplier = new Vector4(Head, Thorax, Arms, Legs);
			p.damage = damage;
			p.damageDropoff = damageDropoff;
			p.damageRange = damageRange;
		    amountInMag--;
			bloomEffect += 2f;

			recoilTime = .025f;
		    timePassed = 60/roundsPerMinute;
		}

		if(Input.GetButtonDown("Fire1") && amountInMag > 0 && timePassed <= 0 && modes[selectedMode].Split(' ')[0] == "semi" && burst == false|| Input.GetAxis("Shooting") > .1f && isShot == false && amountInMag > 0 && timePassed <= 0 && modes[selectedMode].Split(' ')[0] == "semi" && burst == false){
			var rotationX = barrel.rotation.eulerAngles.x;
			rotationX += Random.insideUnitCircle.x * Random.Range(-spread, spread);
			var rotationY = barrel.rotation.eulerAngles.y;
			rotationY += Random.insideUnitCircle.y * Random.Range(-spread, spread);
			var rot = Quaternion.Euler(rotationX, rotationY, 0);

			Transform b = Instantiate(bullet, barrel.position, rot);
			Instantiate(_as.transform, aBarrel.position, aBarrel.rotation, aBarrel);
            Projectile p = b.GetComponent<Projectile>();
			p.muzzleVelocity = bulletVelocity;
			p.maxRange = maxRange;
			p.damageMultiplier = new Vector4(Head, Thorax, Arms, Legs);
			p.damage = damage;
			p.damageDropoff = damageDropoff;
			p.damageRange = damageRange;
		    amountInMag--;
			bloomEffect += 2f;
			recoilTime = .025f;
			isShot = true;
		    timePassed = 60/roundsPerMinute;
		}

		if(Input.GetButtonDown("Fire1") && amountInMag > 0 && timePassed <= 0 && modes[selectedMode].Split(' ')[0] == "burst" && burst == false || Input.GetAxis("Shooting") > .1f && isShot == false && amountInMag > 0 && timePassed <= 0 && modes[selectedMode].Split(' ')[0] == "burst" && burst == false){
			StartCoroutine(Burst());
		}
	}
	//Burst firemode
	private IEnumerator Burst(){
		print(modes[selectedMode].Split(' ')[1]);
		for (int i = 0; i < int.Parse(modes[selectedMode].Split(' ')[1]); i++)
    	{
			burst = true;
			if(amountInMag > 0){
				var rotationX = barrel.rotation.eulerAngles.x;
				rotationX += Random.insideUnitCircle.x * Random.Range(-spread, spread);
				var rotationY = barrel.rotation.eulerAngles.y;
				rotationY += Random.insideUnitCircle.y * Random.Range(-spread, spread);
				var rot = Quaternion.Euler(rotationX, rotationY, 0);

				Transform b = Instantiate(bullet, barrel.position, rot);
				Instantiate(_as.transform, aBarrel.position, aBarrel.rotation, aBarrel);
				Projectile p = b.GetComponent<Projectile>();
				p.muzzleVelocity = bulletVelocity;
				p.maxRange = maxRange;
				p.damageMultiplier = new Vector4(Head, Thorax, Arms, Legs);
				p.damage = damage;
				p.damageDropoff = damageDropoff;
				p.damageRange = damageRange;
				amountInMag--;
				bloomEffect += 2f;
				recoilTime = .025f;
				isShot = true;
				yield return new WaitForSeconds(60/roundsPerMinute);
				
			}

		}
		timePassed = .2f;
		burst = false;
		
	}
	//For Shotguns ONLY
	private void Shotgun(){
		if (Input.GetButton("Fire1") && amountInMag > 0 && timePassed <= 0 && modes[selectedMode].Split(' ')[0] == "auto" && burst == false  || Input.GetAxis("Shooting") > .1f && amountInMag > 0 && timePassed <= 0 && modes[selectedMode].Split(' ')[0] == "auto" && burst == false){
			for(int i = 0; i < projectilesPerShot; i++){
				var rotationX = barrel.rotation.eulerAngles.x;
				rotationX += Random.insideUnitCircle.x * Random.Range(-spread, spread);
				var rotationY = barrel.rotation.eulerAngles.y;
				rotationY += Random.insideUnitCircle.y * Random.Range(-spread, spread);
				var rot = Quaternion.Euler(rotationX, rotationY, 0);
				
				Transform b = Instantiate(bullet, barrel.position, rot);
				Instantiate(_as.transform, aBarrel.position, aBarrel.rotation, aBarrel);
				Projectile p = b.GetComponent<Projectile>();
				p.muzzleVelocity = bulletVelocity;
				p.maxRange = maxRange;
				p.damageMultiplier = new Vector4(Head, Thorax, Arms, Legs);
				p.damage = damage;
				p.damageDropoff = damageDropoff;
				p.damageRange = damageRange;
				recoilTime = .025f;
				timePassed = 60/roundsPerMinute;
			}
			amountInMag--;
			bloomEffect += 2f;
		} 

		if(Input.GetButtonDown("Fire1") && amountInMag > 0 && timePassed <= 0 && modes[selectedMode].Split(' ')[0] == "semi" && burst == false || Input.GetAxis("Shooting") > .1f && isShot == false && amountInMag > 0 && timePassed <= 0 && modes[selectedMode].Split(' ')[0] == "semi" && burst == false){
			for(int i = 0; i < projectilesPerShot; i++){
				var rotationX = barrel.rotation.eulerAngles.x;
				rotationX += Random.insideUnitCircle.x * Random.Range(-spread, spread);
				var rotationY = barrel.rotation.eulerAngles.y;
				rotationY += Random.insideUnitCircle.y * Random.Range(-spread, spread);
				var rot = Quaternion.Euler(rotationX, rotationY, 0);

				Transform b = Instantiate(bullet, barrel.position, rot);
				Instantiate(_as.transform, aBarrel.position, aBarrel.rotation, aBarrel);
				Projectile p = b.GetComponent<Projectile>();
				p.muzzleVelocity = bulletVelocity;
				p.maxRange = maxRange;
				p.damageMultiplier = new Vector4(Head, Thorax, Arms, Legs);
				p.damage = damage;
				p.damageDropoff = damageDropoff;
				p.damageRange = damageRange;
				recoilTime = .025f;
				timePassed = 60/roundsPerMinute;
			}
			amountInMag--;
			isShot = true;
			bloomEffect += 2f;
		}
		
		if(Input.GetButtonDown("Fire1") && amountInMag > 0 && timePassed <= 0 && modes[selectedMode].Split(' ')[0] == "burst" && burst == false || Input.GetAxis("Shooting") > .1f && isShot == false && amountInMag > 0 && timePassed <= 0 && modes[selectedMode].Split(' ')[0] == "burst" && burst == false){
			StartCoroutine(BurstShotgun());
		}
	}

	private IEnumerator BurstShotgun(){
		print(modes[selectedMode].Split(' ')[1]);
		for (int i = 0; i < int.Parse(modes[selectedMode].Split(' ')[1]); i++)
    	{
			burst = true;
			if(amountInMag > 0){
				for(int j = 0; j < projectilesPerShot; j++){
				var rotationX = barrel.rotation.eulerAngles.x;
				rotationX += Random.insideUnitCircle.x * Random.Range(-spread, spread);
				var rotationY = barrel.rotation.eulerAngles.y;
				rotationY += Random.insideUnitCircle.y * Random.Range(-spread, spread);
				var rot = Quaternion.Euler(rotationX, rotationY, 0);

				Transform b = Instantiate(bullet, barrel.position, rot);
				Instantiate(_as.transform, aBarrel.position, aBarrel.rotation, aBarrel);
				Projectile p = b.GetComponent<Projectile>();
				p.muzzleVelocity = bulletVelocity;
				p.maxRange = maxRange;
				p.damageMultiplier = new Vector4(Head, Thorax, Arms, Legs);
				p.damage = damage;
				p.damageDropoff = damageDropoff;
				p.damageRange = damageRange;
				recoilTime = .025f;
				isShot = true;
				timePassed = 60/roundsPerMinute;
				
			}
			amountInMag--;
			bloomEffect += 2f;
				yield return new WaitForSeconds(60/roundsPerMinute);
			}

		}
		timePassed = .2f;
		burst = false;
		
	}

	//Reloadings
	private void Reload(){
		if(ammunition >= magazineSize){
			ammunition = ammunition - (magazineSize - amountInMag);
			amountInMag = amountInMag + (magazineSize - amountInMag);
			reloadPassed = reloadDelay;
			
		} else if (ammunition < magazineSize){
			amountInMag = ammunition;
			ammunition = ammunition - (magazineSize - amountInMag);
			reloadPassed = reloadDelay;
		}
	}

	private void Switch(){
		selectedMode++;
		if(selectedMode > modes.Length - 1){
			selectedMode = 0;
		}
	}

	private void Recoil(float xAxisRecoil, float yAxisRecoil){
		print("Part 4");
		recoilTime = .025f;
		while(recoilTime > 0){
			print("Part 5");
			
		}
	}

}