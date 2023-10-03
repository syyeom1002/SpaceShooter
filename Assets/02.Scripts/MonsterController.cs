using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{
    public enum eState
    {
        IDLE,TRACE,ATTACK,DIE
    }
    public eState state;
    [SerializeField]
    private float attackRange = 2.0f;
    [SerializeField]
    private float traceRange = 10.0f;
    public bool isDie = false;
    private int monsterHp = 100;

    private Transform monsterTr;
    private Transform playerTr;
    private NavMeshAgent agent;
    private Animator anim;

    private readonly int hashTrace = Animator.StringToHash("IsTrace");
    private readonly int hashAttack = Animator.StringToHash("IsAttack");
    private readonly int hashHit = Animator.StringToHash("Hit");
    private readonly int hashPlayerDie = Animator.StringToHash("PlayerDie");
    private readonly int hashSpeed = Animator.StringToHash("Speed");
    private readonly int hashDie = Animator.StringToHash("Die");

    private GameObject bloodEffectPrefab;

    //�̺�Ʈ ����
    private void OnEnable()
    {
        PlayerCtrl.OnPlayerDie += this.OnPlayerDie;
    }
    //�̺�Ʈ ����
    private void OnDisable()
    {
        PlayerCtrl.OnPlayerDie -= this.OnPlayerDie;
    }
    // Start is called before the first frame update
    void Start()
    {
        this.monsterTr = this.gameObject.GetComponent<Transform>();
        this.playerTr = GameObject.FindWithTag("PLAYER").GetComponent<Transform>();
        this.agent = this.GetComponent<NavMeshAgent>();
        this.anim = this.GetComponent<Animator>();
        this.bloodEffectPrefab = Resources.Load<GameObject>("BloodSprayEffect");
        //agent.destination = playerTr.position;

        this.StartCoroutine(this.CheckMonsterState());
        this.StartCoroutine(this.MonsterAction());
    }
    //������ �������� ���� ����üũ
    IEnumerator CheckMonsterState()
    {
        while (!isDie)//���� �ʴ� ���� ����üũ, ������ ����üũ x
        {
            //0.3�� ���� �����ϴ� ���� ������� �޽��� ������ �纸
            yield return new WaitForSeconds(0.3f);

            //�׾����� ���⼭ �ڷ�ƾ ���� 
            if (state == eState.DIE) yield break;

            float distance = Vector3.Distance(this.playerTr.position, this.monsterTr.position);

            if (distance <= attackRange)
            {
                state = eState.ATTACK;
            }
            else if (distance <= traceRange)
            {
                state = eState.TRACE;
            }
            else
            {
                state = eState.IDLE;
            }
        }
    }

    IEnumerator MonsterAction()
    {
        while (!isDie)
        {
            switch (state)
            {
                case eState.IDLE:
                    //���� ����
                    this.Idle();
                    break;
                case eState.TRACE:
                    this.Trace();
                    break;
                case eState.ATTACK:
                    this.Attack();
                    break;
                case eState.DIE:
                    this.Die();
                    break;
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("BULLET"))
        {
            Destroy(collision.gameObject);
            anim.SetTrigger(hashHit);

            //�Ѿ� ���� ������ ���� ����Ʈ �����ϱ�
            Vector3 pos = collision.GetContact(0).point;
            Quaternion rot = Quaternion.LookRotation(-collision.GetContact(0).normal);//���� ������ ��������
            ShowBloodEffect(pos, rot);

            this.monsterHp -= 10;
            if (monsterHp <= 0)
            {
                state = eState.DIE;
            }
        }
    }
    private void ShowBloodEffect(Vector3 pos,Quaternion rot)
    {
        GameObject bloodGo = Instantiate<GameObject>(bloodEffectPrefab, pos, rot, monsterTr);
        Destroy(bloodGo, 1.0f);
    }
    //�Ÿ� ǥ�� ���׸���
    private void OnDrawGizmos()
    {
        if (state == eState.TRACE)
        {
            Gizmos.color = Color.blue;
            GizmosExtensions.DrawWireArc(this.transform.position, this.transform.forward,360,this.traceRange);
        }
        else if (state == eState.ATTACK)
        {
            Gizmos.color = Color.red;
            GizmosExtensions.DrawWireArc(this.transform.position, this.transform.forward, 360, this.attackRange);
        }
    }
    private void Idle()
    {
        this.agent.isStopped = true;
        anim.SetBool(hashTrace, false);
    }
    private void Attack()
    {
        anim.SetBool(hashAttack, true);
    }
    private void Trace()
    {
        agent.SetDestination(this.playerTr.position);
        this.agent.isStopped = false;
        anim.SetBool(hashTrace, true);
        anim.SetBool(hashAttack, false);
    }
    private void Die()
    {
        isDie = true;
        this.agent.isStopped = true;
        anim.SetTrigger(hashDie);
        //������ ���� �¾Ƶ� ���� ȿ�� �Ͼ�� ���� 
        GetComponent<CapsuleCollider>().enabled = false;
    }
    void OnPlayerDie()
    {
        StopAllCoroutines();//������ ���¸� üũ�ϴ� �ڷ�ƾ �Լ��� ��� ������Ŵ

        agent.isStopped = true;
        anim.SetFloat(hashSpeed, Random.Range(0.8f, 1.2f));//���� ������Ÿ�� �ִϸ��̼� �ӵ� ����
        anim.SetTrigger(hashPlayerDie);
    }
}
