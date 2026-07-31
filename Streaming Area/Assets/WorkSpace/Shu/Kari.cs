using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{

    public GameObject bulletPrefab; 
    public Transform muzzle;        
    public float bulletSpeed = 20f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        { 
            Shoot();
        }
    }
    void Shoot()
    {
        GameObject bullet =
            Instantiate(bulletPrefab, muzzle.position, muzzle.rotation);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = muzzle.forward * bulletSpeed;
        }
    }
}
