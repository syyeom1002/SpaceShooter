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
            ContactPoint cp = collision.GetContact(0);//ù��° �浹����
            Quaternion rot = Quaternion.LookRotation(-cp.normal);//normal�� �浹������ ����
            GameObject spark=Instantiate(this.sparkEffect, cp.point,rot);//ȸ�����̻���
            Destroy(spark, 0.5f);
            Destroy(collision.gameObject);
        }
    }
}
