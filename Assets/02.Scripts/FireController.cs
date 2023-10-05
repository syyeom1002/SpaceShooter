using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour
{
    //총알 프리팹
    public GameObject bullet;
    public Transform firePos;

    private MeshRenderer muzzleFlash;
    private RaycastHit hit;
    // Start is called before the first frame update
    void Start()
    {
        muzzleFlash = firePos.GetComponentInChildren<MeshRenderer>();
        //처음 시작할때 비활성화
        muzzleFlash.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(firePos.position, firePos.forward * 10.0f, Color.green);

        if (Input.GetMouseButtonDown(0))
        {
            Fire();

            if(Physics.Raycast(firePos.position,firePos.forward,out hit,10.0f,1<<7))
            {
                Debug.Log($"Hit={hit.transform.name}");
                hit.transform.GetComponent<MonsterController>()?.OnDamage(hit.point, hit.normal);
            }
        }
    }
    //앞으로 나가는건 bulletController에서 하고 있으니 생성만 하면됨
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
