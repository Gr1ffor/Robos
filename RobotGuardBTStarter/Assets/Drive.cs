using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drive : MonoBehaviour {
    // variaveis
	float speed = 20.0F;
    float rotationSpeed = 120.0F;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    void Update() {

        // movimentação do tanque
        float translation = Input.GetAxis("Vertical") * speed;
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;
        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);

        // se aperta space, atira
        if(Input.GetKeyDown("space"))
        {
            // cria o tiro
            GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
            // adiciona força ao tiro
            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward*2000);
        }
    }
}
