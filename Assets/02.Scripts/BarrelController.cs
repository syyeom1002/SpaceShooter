using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelController : MonoBehaviour
{
    //폭발효과 파티클
    public GameObject expEffect;
    public Texture[] textures;
    private new MeshRenderer renderer;
    //폭발 반경
    public float radius = 10.0f;
    //베럴 위치
    private Transform tr;
    private Rigidbody rb;
    private int hitCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();

        renderer = GetComponentInChildren<MeshRenderer>();
        //난수 발생
        int idx = Random.Range(0, textures.Length);
        renderer.material.mainTexture = textures[idx];
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("BULLET"))
        {
            if (++hitCount == 3)//부딪히면 1씩 증가
            {
                this.ExpBarrel();
            }
        }
    }
    //드럼통 폭발시킬 함수
    void ExpBarrel()
    {
        //폭발 파티클 생성시킴
        GameObject exp = Instantiate(expEffect, this.tr.position, Quaternion.identity);
        //폭발 파티클 제거
        Destroy(exp,5.0f);

        //mass를 원래 20이었는데 1로 줄여서 가볍게 함
        //rb.mass = 1.0f;
        //위로 솟구치면서 날라가는 효과
        //rb.AddForce(Vector3.up * 1500.0f);
        IndirectDamage(tr.position);

        //드럼통제거
        Destroy(this.gameObject, 3.0f);
    }
    void IndirectDamage(Vector3 pos)
    {
        Collider[] colls = Physics.OverlapSphere(pos, radius, 1 << 3);//3번 레이어만 추출

        foreach(var coll in colls)
        {
            //범위내에 포함된 드럼통의 rigidbody 추출
            rb = coll.GetComponent<Rigidbody>();
            rb.mass = 1.0f;
            //freezerotation했던거 해제
            rb.constraints = RigidbodyConstraints.None;
            rb.AddExplosionForce(1500.0f, pos, radius, 1200.0f);//횡폭발력(가로), 폭발원점, 폭발반경, 종 폭발력(위로 솟구치는힘)
        }
    }
}
