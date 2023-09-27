using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour
{
    //�Ѿ� ������
    public GameObject bullet;
    public Transform firePos;

    private MeshRenderer muzzleFlash;
    // Start is called before the first frame update
    void Start()
    {
        muzzleFlash = firePos.GetComponentInChildren<MeshRenderer>();
        //ó�� �����Ҷ� ��Ȱ��ȭ
        muzzleFlash.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }
    }
    //������ �����°� bulletController���� �ϰ� ������ ������ �ϸ��
    void Fire()
    {
        Instantiate(bullet, firePos.position, firePos.rotation);
        StartCoroutine(ShowMuzzleFlash());
    }

    IEnumerator ShowMuzzleFlash()
    {
        Vector2 offset = new Vector2(Random.Range(0, 2), Random.Range(0, 2)) * 0.5f;
        muzzleFlash.material.mainTextureOffset = offset;

        float angle = Random.Range(0, 360);
        muzzleFlash.transform.localRotation = Quaternion.Euler(0, 0, angle);

        float scale = Random.Range(1.0f, 2.0f);
        muzzleFlash.transform.localScale = Vector3.one * scale;

        muzzleFlash.enabled = true;
        yield return new WaitForSeconds(0.2f);
        muzzleFlash.enabled = false;
    }
}
