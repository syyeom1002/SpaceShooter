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

    //이벤트 연결
    private void OnEnable()
    {
        PlayerCtrl.OnPlayerDie += this.OnPlayerDie;
    }
    //이벤트 해지
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
    //일정한 간격으로 몬스터 상태체크
    IEnumerator CheckMonsterState()
    {
        while (!isDie)//죽지 않는 동안 상태체크, 죽으면 상태체크 x
        {
            //0.3초 동안 중지하는 동안 제어권을 메시지 루프에 양보
            yield return new WaitForSeconds(0.3f);

            //죽었으면 여기서 코루틴 종료 
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
                    //추적 중지
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

            //총알 맞은 부위에 혈흔 이펙트 생성하기
            Vector3 pos = collision.GetContact(0).point;
            Quaternion rot = Quaternion.LookRotation(-collision.GetContact(0).normal);//맞은 부위의 법선벡터
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
    //거리 표시 원그리기
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
        //죽으면 총을 맞아도 혈흔 효과 일어나지 않음 
        GetComponent<CapsuleCollider>().enabled = false;
    }
    void OnPlayerDie()
    {
        StopAllCoroutines();//몬스터의 상태를 체크하는 코루틴 함수를 모두 정지시킴

        agent.isStopped = true;
        anim.SetFloat(hashSpeed, Random.Range(0.8f, 1.2f));//몬스터 강남스타일 애니메이션 속도 조절
        anim.SetTrigger(hashPlayerDie);
    }
}
