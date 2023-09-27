using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    public enum eAnimState
    {
        Idle,
        RunB,
        RunF,
        RunL,
        RunR
    }
    private Transform tr;
    public float moveSpeed = 10.0f;
    public float turnSpeed = 80.0f;
    private Animation anim;
    private bool isDown;
    private Vector3 downPosition;
    private float initHp = 100.0f;
    //현재 생명값
    public float currHp;
    
    // Start is called before the first frame update
    void Start()
    {
        this.tr = GetComponent<Transform>();
        this.anim = this.GetComponent<Animation>();
        //방법1
        //anim.Play("Idle");
        //방법2
        this.anim.clip = this.anim.GetClip(eAnimState.Idle.ToString());
        this.anim.Play();
        this.currHp = this.initHp;
    }

    // Update is called once per frame
    void Update()
    {
        //수평
        float h = Input.GetAxis("Horizontal");
        //수직
        float v = Input.GetAxis("Vertical");
        float r = Input.GetAxis("Mouse X");

        //Debug.Log("h="+h);
        //Debug.Log("v="+ v);

        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
        this.tr.Translate(moveDir.normalized* Time.deltaTime * moveSpeed);//v는 키보드 입력값
        //tr.Rotate(Vector3.up * turnSpeed * Time.deltaTime * r);

        //마우스 드래그한만큼 회전하기
        if (Input.GetMouseButtonDown(0))
        {
            this.isDown = true;
            this.downPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            this.isDown = false;
        }

        if (this.isDown)//isDown==true
        {
            if (this.downPosition != Input.mousePosition)
            {
                var rotDir = Mathf.Sign(r);//부호반환함수 
                this.transform.Rotate(Vector3.up, rotDir * Time.deltaTime * this.turnSpeed);//y축방향으로 회전
                this.downPosition = Input.mousePosition;
            }
        }

        this.PlayerAnim(h, v);
    }

    void PlayerAnim(float h,float v)
    {
        if (v >= 0.1f)
        {
            anim.CrossFade("RunF", 0.25f);//0.25f 동안 애니메이션이 변경
        }
        else if (v <= -0.1f)
        {
            anim.CrossFade("RunB", 0.25f);
        }
        else if (h >= 0.1f)
        {
            anim.CrossFade("RunR", 0.25f);
        }
        else if (h <= -0.1f)
        {
            anim.CrossFade("RunL", 0.25f);
        }
        else
        {
            anim.CrossFade("Idle", 0.25f);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (currHp >= 0.0f && other.CompareTag("PUNCH"))
        {
            currHp -= 10.0f;
            Debug.LogFormat($"hp:{currHp/this.initHp}");

            if (currHp <= 0.0f)
            {
                PlayerDie();
            }
        }
    }
    private void PlayerDie()
    {
        Debug.Log("플레이어 사망했습니다");
        //플레이어 사망하면 monster태그가 달린 모든 게임오브젝트들을 찾고
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("MONSTER");
        //모든 몬스터의 onPlayerDie 함수를 순차적으로 호출한다. 
        foreach(GameObject monster in monsters)
        {
            monster.SendMessage("OnPlayDie", SendMessageOptions.DontRequireReceiver);
        }
    }
}
