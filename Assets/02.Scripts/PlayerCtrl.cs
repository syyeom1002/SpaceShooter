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
    //���� ����
    public float currHp;
    
    // Start is called before the first frame update
    void Start()
    {
        this.tr = GetComponent<Transform>();
        this.anim = this.GetComponent<Animation>();
        //���1
        //anim.Play("Idle");
        //���2
        this.anim.clip = this.anim.GetClip(eAnimState.Idle.ToString());
        this.anim.Play();
        this.currHp = this.initHp;
    }

    // Update is called once per frame
    void Update()
    {
        //����
        float h = Input.GetAxis("Horizontal");
        //����
        float v = Input.GetAxis("Vertical");
        float r = Input.GetAxis("Mouse X");

        //Debug.Log("h="+h);
        //Debug.Log("v="+ v);

        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
        this.tr.Translate(moveDir.normalized* Time.deltaTime * moveSpeed);//v�� Ű���� �Է°�
        //tr.Rotate(Vector3.up * turnSpeed * Time.deltaTime * r);

        //���콺 �巡���Ѹ�ŭ ȸ���ϱ�
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
                var rotDir = Mathf.Sign(r);//��ȣ��ȯ�Լ� 
                this.transform.Rotate(Vector3.up, rotDir * Time.deltaTime * this.turnSpeed);//y��������� ȸ��
                this.downPosition = Input.mousePosition;
            }
        }

        this.PlayerAnim(h, v);
    }

    void PlayerAnim(float h,float v)
    {
        if (v >= 0.1f)
        {
            anim.CrossFade("RunF", 0.25f);//0.25f ���� �ִϸ��̼��� ����
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
        Debug.Log("�÷��̾� ����߽��ϴ�");
        //�÷��̾� ����ϸ� monster�±װ� �޸� ��� ���ӿ�����Ʈ���� ã��
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("MONSTER");
        //��� ������ onPlayerDie �Լ��� ���������� ȣ���Ѵ�. 
        foreach(GameObject monster in monsters)
        {
            monster.SendMessage("OnPlayDie", SendMessageOptions.DontRequireReceiver);
        }
    }
}
