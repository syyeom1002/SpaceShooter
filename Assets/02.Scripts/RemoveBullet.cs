using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBullet : MonoBehaviour
{
    public GameObject sparkEffect;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag( "BULLET"))
        {
            ContactPoint cp = collision.GetContact(0);//첫번째 충돌지점
            Quaternion rot = Quaternion.LookRotation(-cp.normal);//normal은 충돌지점의 법선
            GameObject spark=Instantiate(this.sparkEffect, cp.point,rot);//회전없이생성
            Destroy(spark, 0.5f);
            Destroy(collision.gameObject);
        }
    }
}
