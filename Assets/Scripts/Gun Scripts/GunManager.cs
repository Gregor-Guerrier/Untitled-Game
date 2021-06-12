using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunManager : MonoBehaviour {
    
	[Header("Gun Parts")]
    public Transform barrel;
    public Transform bullet;

	[Header("Attachments")]
	public Transform muzzle;
	public Transform sight;
	public Transform underbarrel;
	public Transform rightRail;
	public Transform leftRail;

	[Header("Bullet Stats")]
    public float bulletVelocity;
	public float maxRange;
	public float roundsPerMinute;

	[Header("Magazine Stats")]
    public int magazineSize;
    public int magazineAmount;
	private int ammunition;
    private int amountInMag;


	[Header("Gun Stats")]
	public float projectilesPerShot;
	public float spread;
	[Range(1,5)]
	public float maxBloom;
	private float originalSpread;
	public string[] modes;
	private int selectedMode;
	public float adsSpeedMultiplyer;

	[Header("Recoil Stats")]
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

	//Time Variables
    private float timePassed;
    public float reloadDelay;
    private float reloadPassed;
    
	[Header("Audio")]
    public AudioSource muzzleFire;
	public AudioSource dryFire;
    public AudioClip reload;

	[Header("UI")]
	public RectTransform hipFire;
	public Transform reticle;

	private bool burst;
    private bool aiming;
	private Camera camera;
	private float time;
	private Vector3 originalGunPosition;
	private Vector3 originalCameraPosition;
	private MouseLook mouseLook;
	private bool recoil;
	private float recoilTime;
	public Transform gunModel;
	private Vector3 originalPosition;
	private Vector3 recoilPosition;
	private Vector3 oldPosition;
	private float bloomEffect;
	private PlayerMovement _pm;
	private GameManager gm;
	private Vector3 adsLocation;
	private GunSway gunSway;

	public Camera gunCamera;
	// Use this for initialization
	void Start () {
		gunSway = GetComponentInChildren<GunSway>();
		originalPosition = gunModel.localPosition;
		adsLocation.y = ((originalPosition.y - (originalPosition.y + sight.localPosition.y + GameObject.Find("Perspective").transform.localPosition.y)));
		print(gunCamera.transform.position.z - GameObject.Find("Perspective").transform.position.z);
		adsLocation.z = (transform.localPosition.z + (gunCamera.transform.position.z - GameObject.Find("Perspective").transform.position.z));
		gm = GameObject.FindObjectOfType<GameManager>();
		mouseLook = GetComponentInParent<MouseLook>();
		camera = GetComponentInParent<Camera>();
		originalCameraPosition = camera.transform.localPosition;
		originalGunPosition = transform.localPosition;
		originalSpread = spread;
		ammunition = magazineSize * magazineAmount;
		amountInMag = magazineSize;
		timePassed = 60/roundsPerMinute;
		reloadPassed = reloadDelay;
		hipFire.sizeDelta = new Vector2(9*spread, 9*spread);
		recoilPosition = new Vector3(transform.parent.localPosition.x, transform.parent.localPosition.y, transform.parent.localPosition.z - .1f);
		oldPosition = new Vector3(transform.parent.localPosition.x, transform.parent.localPosition.y, transform.parent.localPosition.z);
		_pm = transform.parent.GetComponentInParent<PlayerMovement>();
		
	}
	
	// Update is called once per frame
	void Update()
	{
		hipFire.gameObject.SetActive(!gm.menuOpen);
		gunSway.menuOpen = gm.menuOpen;
		if(gm.menuOpen == false){
			//Figure out if the gun is a shotgun or not
			if(projectilesPerShot == 1)
			{
				NotShotgun();
			} else 
			{
				Shotgun();
			}

			//Reloading
			if (Input.GetKeyDown(gm.keybindManager.reload.primaryBind) && ammunition > 0 && reloadPassed <= 0)
			{
				Reload();		    
			} else if (Input.GetKeyDown(gm.keybindManager.reload.altBind) && ammunition > 0 && reloadPassed <= 0)
			{
				Reload();	
			}

			//Switch Firemode
			if(Input.GetKeyDown(gm.keybindManager.firemode.primaryBind) && modes.Length > 1)
			{
				Switch();
			} else if(Input.GetKeyDown(gm.keybindManager.firemode.altBind) && modes.Length > 1)
			{
				Switch();
			}

			if(Input.GetKey(gm.keybindManager.controlledHipFire.primaryBind) && _pm.isAiming == false || Input.GetKey(gm.keybindManager.controlledHipFire.altBind) && _pm.isAiming == false)
			{
				_pm.adsSpeedMultiplyer = adsSpeedMultiplyer;
				if(time > 0){time -= Time.deltaTime * 9;}
				transform.localPosition = Vector3.Lerp(originalPosition, adsLocation, time);
				spread = originalSpread*.5f * (((bloomEffect-1)*.5f)+1);
				_pm.isControlledAiming = true;
				hipFire.sizeDelta = Vector2.Lerp(hipFire.sizeDelta, new Vector2(9*spread, 9*spread), Time.deltaTime);
			} else
			{
				spread = originalSpread;
				hipFire.sizeDelta = Vector2.Lerp(hipFire.sizeDelta, new Vector2(9*spread, 9*spread), Time.deltaTime);
				_pm.isControlledAiming = false;
			}
			//Aiming Down Sights
			if(Input.GetKey(gm.keybindManager.aimDownSights.primaryBind) && _pm.isControlledAiming == false || Input.GetKey(gm.keybindManager.aimDownSights.altBind) && _pm.isControlledAiming == false)
			{
				spread = 0;
				bloomEffect = 1;
				_pm.isAiming = true;
				_pm.adsSpeedMultiplyer = adsSpeedMultiplyer;
				if(time < 1)
				{
					time += Time.deltaTime * 9;
					camera.transform.localPosition = Vector3.Lerp(new Vector3(0, 1.45f, .35f), new Vector3(-sight.localPosition.z * 1.25f, 1.45f, .35f), time);
					transform.localPosition = Vector3.Lerp(originalPosition, adsLocation, time);
					hipFire.sizeDelta = Vector2.Lerp(hipFire.sizeDelta, new Vector2(9*spread, 9*spread), Time.deltaTime);
				}
				if(time > .2f)
				{
					hipFire.gameObject.SetActive(false);
					if(reticle != null)
					{
						reticle.parent.GetComponentInChildren<Camera>().cullingMask = reticle.parent.GetComponentInChildren<Camera>().cullingMask | (1 << 11);
					}
				}
				
				
			} else if(!Input.GetKey(gm.keybindManager.aimDownSights.primaryBind) && _pm.isControlledAiming == false || !Input.GetKeyDown(gm.keybindManager.aimDownSights.altBind) && _pm.isControlledAiming == false)
			{
				spread = originalSpread * bloomEffect;
				if(time > 0)
				{
					
					time -= Time.deltaTime * 9;
					transform.localPosition = Vector3.Lerp(originalPosition, adsLocation, time);
					camera.transform.localPosition = Vector3.Lerp(new Vector3(-sight.localPosition.z * 1.25f, 1.45f, .35f), new Vector3(0, 1.45f, .35f), 1-time);
					hipFire.sizeDelta = Vector2.Lerp(hipFire.sizeDelta, new Vector2(9*spread, 9*spread), time);
					_pm.isAiming = false;
				}
				if(time < .2f)
				{
					if(reticle != null)
					{
						reticle.parent.GetComponentInChildren<Camera>().cullingMask = reticle.parent.GetComponentInChildren<Camera>().cullingMask & ~ (1 << 11);
					}
					hipFire.gameObject.SetActive(true);
					hipFire.sizeDelta = Vector2.Lerp(hipFire.sizeDelta, new Vector2(9*spread, 9*spread), Time.deltaTime);
					_pm.isAiming = false;
					_pm.adsSpeedMultiplyer = adsSpeedMultiplyer;
				}
				
			}
			
			if(Input.GetKey(gm.keybindManager.reload.primaryBind) || Input.GetKey(gm.keybindManager.aimDownSights.altBind))
			{
				Reload();
			}
		}
	}
	void FixedUpdate () 
	{
		if(gm.menuOpen == false){
			print(spread);
			//Bloom Effect
			bloomEffect -= Time.deltaTime*5;
			if(bloomEffect < 1)
			{
				bloomEffect = 1;
			} else if(bloomEffect > maxBloom)
			{
				bloomEffect = maxBloom;
			}
			bloomEffect += Mathf.Abs(Input.GetAxis("Horizontal")/7.5f);
			bloomEffect += Mathf.Abs(Input.GetAxis("Vertical")/7.5f);
			
			//Reducing Time
			recoilTime -= Time.deltaTime;
			timePassed -= Time.deltaTime;
			reloadPassed -= Time.deltaTime;


			if(recoilTime > 0)
			{
				mouseLook.xRotation -= Random.Range(xRecoil.x, xRecoil.y) * Time.deltaTime;
				mouseLook.playerBody.Rotate(Vector3.up * Random.Range(yRecoil.x, yRecoil.y) * Time.deltaTime);
				transform.parent.localPosition = Vector3.Lerp(transform.parent.localPosition, recoilPosition, Time.deltaTime*5);
			} else if(recoilTime < 0)
			{
				
				transform.parent.localPosition = Vector3.Lerp(transform.parent.localPosition, oldPosition, Time.deltaTime*5);
			}
		}
	}

	
	//For Rifles, Subs, and Others
	private void NotShotgun()
	{	
		if (Input.GetKeyDown(gm.keybindManager.shoot.primaryBind) && amountInMag == 0 && timePassed <= 0 || Input.GetKeyDown(gm.keybindManager.shoot.altBind) && amountInMag == 0 && timePassed <= 0)
		{
			Instantiate(dryFire.transform, muzzle.position, muzzle.rotation, muzzle);
		}
		if (Input.GetKey(gm.keybindManager.shoot.primaryBind) && amountInMag > 0 && timePassed <= 0 && modes[selectedMode].Split(' ')[0] == "auto" && burst == false || Input.GetKey(gm.keybindManager.shoot.altBind) && amountInMag > 0 && timePassed <= 0 && modes[selectedMode].Split(' ')[0] == "auto" && burst == false)
		{
			var rotationX = barrel.rotation.eulerAngles.x;
			rotationX += Random.insideUnitCircle.x * Random.Range(-spread, spread);
			var rotationY = barrel.rotation.eulerAngles.y;
			rotationY += Random.insideUnitCircle.y * Random.Range(-spread, spread);
			var rot = Quaternion.Euler(rotationX, rotationY, 0);

			Transform b = Instantiate(bullet, barrel.position, rot);
			Instantiate(muzzleFire.transform, muzzle.position, muzzle.rotation, muzzle);
            ProjectileManager p = b.GetComponent<ProjectileManager>();
			p.muzzleVelocity = bulletVelocity;
			p.maxRange = maxRange;
			p.damageMultiplier = new Vector4(Head, Thorax, Arms, Legs);
			p.damage = damage;
			p.damageDropoff = damageDropoff;
			p.damageRange = damageRange;
		    amountInMag--;
			bloomEffect += 2f;

			recoilTime = .05f;
		    timePassed = 60/roundsPerMinute;
		}

		if(Input.GetKeyDown(gm.keybindManager.shoot.primaryBind) && amountInMag > 0 && timePassed <= 0 && modes[selectedMode].Split(' ')[0] == "semi" && burst == false || Input.GetKeyDown(gm.keybindManager.shoot.altBind) && amountInMag > 0 && timePassed <= 0 && modes[selectedMode].Split(' ')[0] == "semi" && burst == false)
		{
			var rotationX = barrel.rotation.eulerAngles.x;
			rotationX += Random.insideUnitCircle.x * Random.Range(-spread, spread);
			var rotationY = barrel.rotation.eulerAngles.y;
			rotationY += Random.insideUnitCircle.y * Random.Range(-spread, spread);
			var rot = Quaternion.Euler(rotationX, rotationY, 0);

			Transform b = Instantiate(bullet, barrel.position, rot);
			Instantiate(muzzleFire.transform, muzzle.position, muzzle.rotation, muzzle);
            ProjectileManager p = b.GetComponent<ProjectileManager>();
			p.muzzleVelocity = bulletVelocity;
			p.maxRange = maxRange;
			p.damageMultiplier = new Vector4(Head, Thorax, Arms, Legs);
			p.damage = damage;
			p.damageDropoff = damageDropoff;
			p.damageRange = damageRange;
		    amountInMag--;
			bloomEffect += 2f;

			recoilTime = .05f;
		    timePassed = 60/roundsPerMinute;
		}

		if(Input.GetKeyDown(gm.keybindManager.shoot.primaryBind) && amountInMag > 0 && timePassed <= 0 && modes[selectedMode].Split(' ')[0] == "burst" && burst == false || Input.GetKeyDown(gm.keybindManager.shoot.altBind) && amountInMag > 0 && timePassed <= 0 && modes[selectedMode].Split(' ')[0] == "burst" && burst == false)
		{
			StartCoroutine(Burst());
		}
	}
	//Burst firemode
	private IEnumerator Burst()
	{
		print(modes[selectedMode].Split(' ')[1]);
		for (int i = 0; i < int.Parse(modes[selectedMode].Split(' ')[1]); i++)
    	{
			burst = true;
			if(amountInMag > 0)
			{
				var rotationX = barrel.rotation.eulerAngles.x;
				rotationX += Random.insideUnitCircle.x * Random.Range(-spread, spread);
				var rotationY = barrel.rotation.eulerAngles.y;
				rotationY += Random.insideUnitCircle.y * Random.Range(-spread, spread);
				var rot = Quaternion.Euler(rotationX, rotationY, 0);

				Transform b = Instantiate(bullet, barrel.position, rot);
				Instantiate(muzzleFire.transform, muzzle.position, muzzle.rotation, muzzle);
				ProjectileManager p = b.GetComponent<ProjectileManager>();
				p.muzzleVelocity = bulletVelocity;
				p.maxRange = maxRange;
				p.damageMultiplier = new Vector4(Head, Thorax, Arms, Legs);
				p.damage = damage;
				p.damageDropoff = damageDropoff;
				p.damageRange = damageRange;
				amountInMag--;
				bloomEffect += 2f;
	
				recoilTime = .05f;
				yield return new WaitForSeconds(60/roundsPerMinute);
				
			}

		}
		timePassed = 120/roundsPerMinute;
		burst = false;
		
	}
	//For Shotguns ONLY
	private void Shotgun(){
		if (Input.GetKey(gm.keybindManager.shoot.primaryBind) && amountInMag > 0 && timePassed <= 0 && modes[selectedMode].Split(' ')[0] == "auto" && burst == false || Input.GetKey(gm.keybindManager.shoot.altBind) && amountInMag > 0 && timePassed <= 0 && modes[selectedMode].Split(' ')[0] == "auto" && burst == false)
		{
			for(int i = 0; i < projectilesPerShot; i++){
				var rotationX = barrel.rotation.eulerAngles.x;
				rotationX += Random.insideUnitCircle.x * Random.Range(-spread, spread);
				var rotationY = barrel.rotation.eulerAngles.y;
				rotationY += Random.insideUnitCircle.y * Random.Range(-spread, spread);
				var rot = Quaternion.Euler(rotationX, rotationY, 0);
				
				Transform b = Instantiate(bullet, barrel.position, rot);
				Instantiate(muzzleFire.transform, muzzle.position, muzzle.rotation, muzzle);
				ProjectileManager p = b.GetComponent<ProjectileManager>();
				p.muzzleVelocity = bulletVelocity;
				p.maxRange = maxRange;
				p.damageMultiplier = new Vector4(Head, Thorax, Arms, Legs);
				p.damage = damage;
				p.damageDropoff = damageDropoff;
				p.damageRange = damageRange;
	
				recoilTime = .05f;
				timePassed = 60/roundsPerMinute;
			}
			amountInMag--;
			bloomEffect += 2f;
		} 

		if(Input.GetKeyDown(gm.keybindManager.shoot.primaryBind) && amountInMag > 0 && timePassed <= 0 && modes[selectedMode].Split(' ')[0] == "semi" && burst == false || Input.GetKeyDown(gm.keybindManager.shoot.altBind) && amountInMag > 0 && timePassed <= 0 && modes[selectedMode].Split(' ')[0] == "semi" && burst == false)
		{
			for(int i = 0; i < projectilesPerShot; i++){
				var rotationX = barrel.rotation.eulerAngles.x;
				rotationX += Random.insideUnitCircle.x * Random.Range(-spread, spread);
				var rotationY = barrel.rotation.eulerAngles.y;
				rotationY += Random.insideUnitCircle.y * Random.Range(-spread, spread);
				var rot = Quaternion.Euler(rotationX, rotationY, 0);

				Transform b = Instantiate(bullet, barrel.position, rot);
				Instantiate(muzzleFire.transform, muzzle.position, muzzle.rotation, muzzle);
				ProjectileManager p = b.GetComponent<ProjectileManager>();
				p.muzzleVelocity = bulletVelocity;
				p.maxRange = maxRange;
				p.damageMultiplier = new Vector4(Head, Thorax, Arms, Legs);
				p.damage = damage;
				p.damageDropoff = damageDropoff;
				p.damageRange = damageRange;
	
				recoilTime = .05f;
				timePassed = 60/roundsPerMinute;
			}
			amountInMag--;
			bloomEffect += 2f;
		}
		
		if(Input.GetKeyDown(gm.keybindManager.shoot.primaryBind) && amountInMag > 0 && timePassed <= 0 && modes[selectedMode].Split(' ')[0] == "burst" && burst == false || Input.GetKeyDown(gm.keybindManager.shoot.altBind) && amountInMag > 0 && timePassed <= 0 && modes[selectedMode].Split(' ')[0] == "burst" && burst == false)
		{
			StartCoroutine(BurstShotgun());
		}
	}

	private IEnumerator BurstShotgun()
	{
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
				Instantiate(muzzleFire.transform, muzzle.position, muzzle.rotation, muzzle);
				ProjectileManager p = b.GetComponent<ProjectileManager>();
				p.muzzleVelocity = bulletVelocity;
				p.maxRange = maxRange;
				p.damageMultiplier = new Vector4(Head, Thorax, Arms, Legs);
				p.damage = damage;
				p.damageDropoff = damageDropoff;
				p.damageRange = damageRange;
	
				recoilTime = .05f;
				timePassed = 60/roundsPerMinute;
				
			}
			amountInMag--;
			bloomEffect += 2f;
				yield return new WaitForSeconds(60/roundsPerMinute);
			}

		}
		timePassed = 120/roundsPerMinute;
		burst = false;
		
	}

	//Reloadings
	private void Reload()
	{
		if(ammunition >= magazineSize){
			ammunition = ammunition - (magazineSize - amountInMag);
			amountInMag = amountInMag + (magazineSize - amountInMag);
			reloadPassed = reloadDelay;
			
		} else if (ammunition < magazineSize)
		{
			amountInMag = ammunition;
			ammunition = ammunition - (magazineSize - amountInMag);
			reloadPassed = reloadDelay;
		}
	}

	private void Switch()
	{
		selectedMode++;
		if(selectedMode > modes.Length - 1)
		{
			selectedMode = 0;
		}
	}

}