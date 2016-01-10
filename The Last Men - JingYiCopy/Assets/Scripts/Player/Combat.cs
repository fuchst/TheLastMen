using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Combat : MonoBehaviour {

    Weapon[] weapons = new Weapon[2];
    public GameObject[] weaponModels;

    public Transform cameraTransform;

    protected int activeWeaponIdx = 0;
    protected GameObject currWeaponModel;

    void Awake()
    {
        //JingYi: not optimal but atleast we dont have errors anymore.
        weapons[0] = gameObject.AddComponent<Pistol>();
        weapons[1] = gameObject.AddComponent<Shotgun>();

        //spawn weapon model
        Vector3 gunpos = new Vector3(cameraTransform.position.x + 0.3F, cameraTransform.position.y-0.2F, cameraTransform.position.z + 1.0F);
        currWeaponModel = Instantiate(weaponModels[activeWeaponIdx], gunpos, cameraTransform.rotation) as GameObject;
        currWeaponModel.transform.localScale = new Vector3(0.1F, 0.1F, 0.1F);
        currWeaponModel.transform.parent = cameraTransform;
    }

    void OnHit(int dmg)
    {
        s_GameManager.Instance.healthpointsCur -= dmg;
        if (s_GameManager.Instance.healthpointsCur <= 0)
        {
             Debug.Log("Player died");
        }
    }

    void Update()
    {
        
        if (CrossPlatformInputManager.GetButtonDown("Shoot"))
        {
            Transform frame = this.transform.GetChild(0);

            weapons[activeWeaponIdx].shootNVI(frame);
        }

        if (CrossPlatformInputManager.GetButtonDown("Reload"))
        {
            weapons[activeWeaponIdx].reload();
        }

        if (CrossPlatformInputManager.GetButtonDown("NextWeapon"))
        {
            activeWeaponIdx = (activeWeaponIdx + 1) % weapons.Length;
            //switch weapon model
            Destroy(currWeaponModel);
            Vector3 gunpos = new Vector3(cameraTransform.position.x + 0.3F, cameraTransform.position.y, cameraTransform.position.z + 1.5F);
            currWeaponModel = Instantiate(weaponModels[activeWeaponIdx], gunpos, cameraTransform.rotation) as GameObject;
            currWeaponModel.transform.parent = cameraTransform;
            if(activeWeaponIdx == 1)
            {
                currWeaponModel.transform.Rotate(-90, 0, 0);
                currWeaponModel.transform.localScale = new Vector3(0.5F, 0.5F, 0.5F);
                currWeaponModel.transform.localPosition = new Vector3(0.3F, -0.2F, 0.6F);
            }
            else
            {
                currWeaponModel.transform.localScale = new Vector3(0.1F, 0.1F, 0.1F);
                currWeaponModel.transform.localPosition = new Vector3(0.3F, -0.2F, 1.0F);
            }
          

        }

        if (CrossPlatformInputManager.GetButtonDown("PrevWeapon"))
        {
            activeWeaponIdx = (activeWeaponIdx + weapons.Length - 1) % weapons.Length;
            //switch weapon model
            Destroy(currWeaponModel);
            Vector3 gunpos = new Vector3(cameraTransform.position.x + 0.3F, cameraTransform.position.y, cameraTransform.position.z + 1.5F);
            currWeaponModel = Instantiate(weaponModels[activeWeaponIdx], gunpos, cameraTransform.rotation) as GameObject;
            currWeaponModel.transform.parent = cameraTransform;
            if (activeWeaponIdx == 1)
            {
                currWeaponModel.transform.Rotate(-90, 0, 0);
                currWeaponModel.transform.localScale = new Vector3(0.5F, 0.5F, 0.5F);
                currWeaponModel.transform.localPosition = new Vector3(0.3F, -0.2F, 0.6F);
            }
            else
            {
                currWeaponModel.transform.localScale = new Vector3(0.1F, 0.1F, 0.1F);
                currWeaponModel.transform.localPosition = new Vector3(0.3F, -0.2F, 1.0F);
            }
            
        }
    }
}
