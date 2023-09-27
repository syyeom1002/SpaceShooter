using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelController : MonoBehaviour
{
    //����ȿ�� ��ƼŬ
    public GameObject expEffect;
    public Texture[] textures;
    private new MeshRenderer renderer;
    //���� �ݰ�
    public float radius = 10.0f;
    //���� ��ġ
    private Transform tr;
    private Rigidbody rb;
    private int hitCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();

        renderer = GetComponentInChildren<MeshRenderer>();
        //���� �߻�
        int idx = Random.Range(0, textures.Length);
        renderer.material.mainTexture = textures[idx];
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("BULLET"))
        {
            if (++hitCount == 3)//�ε����� 1�� ����
            {
                this.ExpBarrel();
            }
        }
    }
    //�巳�� ���߽�ų �Լ�
    void ExpBarrel()
    {
        //���� ��ƼŬ ������Ŵ
        GameObject exp = Instantiate(expEffect, this.tr.position, Quaternion.identity);
        //���� ��ƼŬ ����
        Destroy(exp,5.0f);

        //mass�� ���� 20�̾��µ� 1�� �ٿ��� ������ ��
        //rb.mass = 1.0f;
        //���� �ڱ�ġ�鼭 ���󰡴� ȿ��
        //rb.AddForce(Vector3.up * 1500.0f);
        IndirectDamage(tr.position);

        //�巳������
        Destroy(this.gameObject, 3.0f);
    }
    void IndirectDamage(Vector3 pos)
    {
        Collider[] colls = Physics.OverlapSphere(pos, radius, 1 << 3);//3�� ���̾ ����

        foreach(var coll in colls)
        {
            //�������� ���Ե� �巳���� rigidbody ����
            rb = coll.GetComponent<Rigidbody>();
            rb.mass = 1.0f;
            //freezerotation�ߴ��� ����
            rb.constraints = RigidbodyConstraints.None;
            rb.AddExplosionForce(1500.0f, pos, radius, 1200.0f);//Ⱦ���߷�(����), ���߿���, ���߹ݰ�, �� ���߷�(���� �ڱ�ġ����)
        }
    }
}
