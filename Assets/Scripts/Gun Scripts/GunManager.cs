using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunManager : MonoBehaviour {
    
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
	public float adsSpeedMultiplyer;

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
	private PlayerMovement _pm;
	private GameManager gm;
	public Transform reticle;
	// Use this for initialization
	void Start () {
		gm = GameObject.FindObjectOfType<GameManager>();
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
		_pm = transform.parent.GetComponentInParent<PlayerMovement>();
		
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
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

		//Aiming Down Sights
		if(Input.GetKey(gm.keybindManager.aimDownSights.primaryBind) || Input.GetKey(gm.keybindManager.aimDownSights.altBind))
		{
			spread = 0;
			bloomEffect = 1;
			_pm.isAiming = true;
			_pm.adsSpeedMultiplyer = adsSpeedMultiplyer;
			if(time < 1)
			{
				time += Time.deltaTime * 3;
				transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0,  0-sight.localPosition.y/10, -sight.localPosition.z * 1.25f), time);
				camera.transform.localPosition = Vector3.Lerp(new Vector3(0, 1.45f, .35f), new Vector3(-sight.localPosition.z * 1.25f, 1.45f, .35f), time*2f);
				hipFire.sizeDelta = Vector2.Lerp(hipFire.sizeDelta, new Vector2(9*spread, 9*spread), Time.deltaTime);
			}
			if(time > .2f)
			{
				hipFire.gameObject.SetActive(false);
				reticle.parent.GetComponentInChildren<Camera>().cullingMask = reticle.parent.GetComponentInChildren<Camera>().cullingMask | (1 << 11);
			}
			
			
		} else if(!Input.GetKey(gm.keybindManager.aimDownSights.primaryBind) || !Input.GetKeyDown(gm.keybindManager.aimDownSights.altBind))
		{
			spread = originalSpread * bloomEffect;
			if(time > 0)
			{
				
				time -= Time.deltaTime * 3;
				transform.localPosition = Vector3.Lerp(transform.localPosition, originalGunPosition, 1-time);
				camera.transform.localPosition = Vector3.Lerp(new Vector3(-sight.localPosition.z * 1.25f, 1.45f, .35f), new Vector3(0, 1.45f, .35f), 1-time*2f);
				hipFire.sizeDelta = Vector2.Lerp(hipFire.sizeDelta, new Vector2(9*spread, 9*spread), time);
				_pm.isAiming = false;
			}
			if(time < .2f)
			{
				reticle.parent.GetComponentInChildren<Camera>().cullingMask = reticle.parent.GetComponentInChildren<Camera>().cullingMask & ~ (1 << 11);
				hipFire.gameObject.SetActive(true);
				hipFire.sizeDelta = Vector2.Lerp(hipFire.sizeDelta, new Vector2(9*spread, 9*spread), Time.deltaTime);
				_pm.isAiming = false;
				_pm.adsSpeedMultiplyer = adsSpeedMultiplyer;
			}
			
		}
		if(recoilTime > 0)
		{
			mouseLook.xRotation -= Random.Range(xRecoil.x, xRecoil.y) * Time.deltaTime;
			mouseLook.playerBody.Rotate(Vector3.up * Random.Range(yRecoil.x, yRecoil.y) * Time.deltaTime);
			gunModel.localPosition = Vector3.Lerp(new Vector3(gunModel.localPosition.x, gunModel.localPosition.y, recoilPosition.z), new Vector3(recoilPosition.x, recoilPosition.y, recoilPosition.z - .5f), Time.deltaTime*5f);
		} else if(recoilTime < 0)
		{
			gunModel.localPosition = Vector3.Lerp(gunModel.localPosition, new Vector3(gunModel.localPosition.x, gunModel.localPosition.y, recoilPosition.z), Time.deltaTime*5f);
		}
	}

	
	//For Rifles, Subs, and Others
	private void NotShotgun()
	{
		if (Input.GetKey(gm.keybindManager.shoot.primaryBind) && amountInMag > 0 && timePassed <= 0 && modes[selectedMode].Split(' ')[0] == "auto" && burst == false || Input.GetKey(gm.keybindManager.shoot.altBind) && amountInMag > 0 && timePassed <= 0 && modes[selectedMode].Split(' ')[0] == "auto" && burst == false)
		{
			var rotationX = barrel.rotation.eulerAngles.x;
			rotationX += Random.insideUnitCircle.x * Random.Range(-spread, spread);
			var rotationY = barrel.rotation.eulerAngles.y;
			rotationY += Random.insideUnitCircle.y * Random.Range(-spread, spread);
			var rot = Quaternion.Euler(rotationX, rotationY, 0);

			Transform b = Instantiate(bullet, barrel.position, rot);
			Instantiate(_as.transform, aBarrel.position, aBarrel.rotation, aBarrel);
            ProjectileManager p = b.GetComponent<ProjectileManager>();
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

		if(Input.GetKeyDown(gm.keybindManager.shoot.primaryBind) && amountInMag > 0 && timePassed <= 0 && modes[selectedMode].Split(' ')[0] == "semi" && burst == false || Input.GetKeyDown(gm.keybindManager.shoot.altBind) && amountInMag > 0 && timePassed <= 0 && modes[selectedMode].Split(' ')[0] == "semi" && burst == false)
		{
			var rotationX = barrel.rotation.eulerAngles.x;
			rotationX += Random.insideUnitCircle.x * Random.Range(-spread, spread);
			var rotationY = barrel.rotation.eulerAngles.y;
			rotationY += Random.insideUnitCircle.y * Random.Range(-spread, spread);
			var rot = Quaternion.Euler(rotationX, rotationY, 0);

			Transform b = Instantiate(bullet, barrel.position, rot);
			Instantiate(_as.transform, aBarrel.position, aBarrel.rotation, aBarrel);
            ProjectileManager p = b.GetComponent<ProjectileManager>();
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
				Instantiate(_as.transform, aBarrel.position, aBarrel.rotation, aBarrel);
				ProjectileManager p = b.GetComponent<ProjectileManager>();
				p.muzzleVelocity = bulletVelocity;
				p.maxRange = maxRange;
				p.damageMultiplier = new Vector4(Head, Thorax, Arms, Legs);
				p.damage = damage;
				p.damageDropoff = damageDropoff;
				p.damageRange = damageRange;
				amountInMag--;
				bloomEffect += 2f;
				recoilTime = .025f;
				yield return new WaitForSeconds(60/roundsPerMinute);
				
			}

		}
		timePassed = .2f;
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
				Instantiate(_as.transform, aBarrel.position, aBarrel.rotation, aBarrel);
				ProjectileManager p = b.GetComponent<ProjectileManager>();
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

		if(Input.GetKeyDown(gm.keybindManager.shoot.primaryBind) && amountInMag > 0 && timePassed <= 0 && modes[selectedMode].Split(' ')[0] == "semi" && burst == false || Input.GetKeyDown(gm.keybindManager.shoot.altBind) && amountInMag > 0 && timePassed <= 0 && modes[selectedMode].Split(' ')[0] == "semi" && burst == false)
		{
			for(int i = 0; i < projectilesPerShot; i++){
				var rotationX = barrel.rotation.eulerAngles.x;
				rotationX += Random.insideUnitCircle.x * Random.Range(-spread, spread);
				var rotationY = barrel.rotation.eulerAngles.y;
				rotationY += Random.insideUnitCircle.y * Random.Range(-spread, spread);
				var rot = Quaternion.Euler(rotationX, rotationY, 0);

				Transform b = Instantiate(bullet, barrel.position, rot);
				Instantiate(_as.transform, aBarrel.position, aBarrel.rotation, aBarrel);
				ProjectileManager p = b.GetComponent<ProjectileManager>();
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
		
		if(Input.GetKeyDown(gm.keybindManager.shoot.primaryBind) && amountInMag > 0 && timePassed <= 0 && modes[selectedMode].Split(' ')[0] == "semi" && burst == false || Input.GetKeyDown(gm.keybindManager.shoot.altBind) && amountInMag > 0 && timePassed <= 0 && modes[selectedMode].Split(' ')[0] == "semi" && burst == false)
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
				Instantiate(_as.transform, aBarrel.position, aBarrel.rotation, aBarrel);
				ProjectileManager p = b.GetComponent<ProjectileManager>();
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
				yield return new WaitForSeconds(60/roundsPerMinute);
			}

		}
		timePassed = .2f;
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

	private void Recoil(float xAxisRecoil, float yAxisRecoil)
	{
		recoilTime = .025f;
		while(recoilTime > 0){
			
		}
	}

}