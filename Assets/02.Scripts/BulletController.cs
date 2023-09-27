using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float damage = 20.0f;
    public float force = 1500.0f;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        //this.rb = this.GetComponent<Rigidbody>();
        //this.rb.AddForce(transform.forward * force);
        //로컬좌표기준
        GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * force);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
