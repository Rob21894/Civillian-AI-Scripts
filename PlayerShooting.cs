using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour {

    // Use this for initialization
    public GameObject bullet;
    [Range(0, 50)]
    public float bulletSpeed;
    public Transform bulletSpawn;
    PlayerControl playerControl;
    private MakeNoise makeNoise;
    // OBJECT POOLING VARIABLES

    public int amountToSpawn;
    public List<GameObject> spawnedBullets;
	void Start ()
    {
        playerControl = GetComponent<PlayerControl>();
        makeNoise = GetComponent<MakeNoise>();
        for (int i = 0; i < amountToSpawn; i++)
        {
            GameObject _spawnedBullets = Instantiate(bullet, bulletSpawn.position, bulletSpawn.transform.rotation);
            _spawnedBullets.SetActive(false);
            spawnedBullets.Add(_spawnedBullets);
        }


	}
	
	// Update is called once per frame
	void Update ()
    {
        //Debug.Log("Rotation.y: " + bulletSpawn.rotation.y);
        // Debug.Log("Local Rotation.y: " + bulletSpawn.localRotation.y);
        if (playerControl.playerStates == PlayerControl.PlayerStates.Unholstered) // more consistent in update loop
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                GameObject newBullet = CheckForAvailableBullet();
                Vector3 rot = bulletSpawn.transform.rotation.eulerAngles;
                newBullet.SetActive(true);
                newBullet.transform.position = bulletSpawn.transform.position;
                newBullet.transform.rotation = Quaternion.Euler(90.0f, rot.y, rot.z);
                newBullet.GetComponent<Rigidbody>().AddForce(transform.forward * bulletSpeed, ForceMode.Impulse);
                makeNoise.noiseName = MakeNoise.NoiseName.gun;
                makeNoise.makeNoise = true;

            }
        }

    }

    public GameObject CheckForAvailableBullet()
    {
        GameObject bulletObj = null;
        for (int i = 0; i < spawnedBullets.Count; i++)
        {
            if (!spawnedBullets[i].activeInHierarchy)
            {
             
                bulletObj = spawnedBullets[i];
               // Debug.Log(i);
                break;
            }
        }

        return bulletObj as GameObject;

    }

}
