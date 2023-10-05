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
    public eState state=eState.IDLE;
    [SerializeField]
    private float attackRange = 2.0f;
    [SerializeField]
    private float traceRange = 10.0f;
    public  bool isDie = false;
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
    private void OnEnable()//게임 오브젝트가 비활성화된 상태에서 다시 활성화될때마다 발생하는 유니티 콜백 함수
    {
        PlayerCtrl.OnPlayerDie += this.OnPlayerDie;

        this.StartCoroutine(this.CheckMonsterState());
        this.StartCoroutine(this.MonsterAction());
    }

    //이벤트 해지
    private void OnDisable()
    {
        PlayerCtrl.OnPlayerDie -= this.OnPlayerDie;
    }

    // Start is called before the first frame update
    void Awake()
    {
        this.monsterTr = this.gameObject.GetComponent<Transform>();
        this.playerTr = GameObject.FindWithTag("PLAYER").GetComponent<Transform>();
        this.agent = this.GetComponent<NavMeshAgent>();
        //navMeshAgent 의 자동 회전 기능 비활성화
        this.agent.updateRotation = false;
        this.anim = this.GetComponent<Animator>();
        this.bloodEffectPrefab = Resources.Load<GameObject>("BloodSprayEffect");
        //agent.destination = playerTr.position;
    }
    private void Update()
    {
        if (agent.remainingDistance >= 2.0f)
        {
            //에이전트 이동 방향
            Vector3 direction = agent.desiredVelocity;//내비게이션 시스템이 계산한 에이전트의 목표 속도와 방향,장애물 회피를 고려한 이동방향
            Quaternion rot = Quaternion.LookRotation(direction);
            monsterTr.rotation = Quaternion.Slerp(monsterTr.rotation, rot, Time.deltaTime * 10.0f);
        }

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
        while (isDie==false)
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
                    yield return this.Die();
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
        }
    }
    public void OnDamage(Vector3 pos, Vector3 normal)
    {
        anim.SetTrigger(hashHit);

        //총알 맞은 부위에 혈흔 이펙트 생성하기
        Quaternion rot = Quaternion.LookRotation(normal);//맞은 부위의 법선벡터
        ShowBloodEffect(pos, rot);

        this.monsterHp -= 20;
        Debug.LogFormat("<color=lime>monster hp :{0}</color>", this.monsterHp);
        if (monsterHp == 0)
        {
            state = eState.DIE;
            GameManager.instance.DisplayScore(50);
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

    private IEnumerator Die()
    {
        isDie = true;
        this.agent.isStopped = true;
        anim.SetTrigger(hashDie);
        //죽으면 총을 맞아도 혈흔 효과 일어나지 않음 
        GetComponent<CapsuleCollider>().enabled = false;

        //일정 시간 대기 후 오브젝트 풀링으로 환원
        yield return new WaitForSeconds(3.0f);
        //리셋
        monsterHp = 100;
        isDie = false;
        Debug.LogFormat("isDie:{0},hp:{1}", isDie, monsterHp);
        GetComponent<CapsuleCollider>().enabled = true;
        this.gameObject.SetActive(false);
    }

    void OnPlayerDie()
    {
        StopAllCoroutines();//몬스터의 상태를 체크하는 코루틴 함수를 모두 정지시킴

        agent.isStopped = true;
        anim.SetFloat(hashSpeed, Random.Range(0.8f, 1.2f));//몬스터 강남스타일 애니메이션 속도 조절
        anim.SetTrigger(hashPlayerDie);
    }
}
