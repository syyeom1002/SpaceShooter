using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public List<Transform> points=new List<Transform>();
    [SerializeField]
    private GameObject monsterPrefab;
    public float createTime = 3.0f;
    //몬스터를 미리 생성해 저장할 리스트 자료형
    public List<GameObject> monsterPool = new List<GameObject>();
    //오브젝트 풀에 생성할 몬스터의 최대 개수
    public int maxMonsters = 10;
    //캐릭터가 사망하면 몬스터 생성 중지시켜야함
    private bool isGameOver;
    
    //프로퍼티
    public bool IsGameOver
    {
        get//외부에서 프로퍼티 읽을 때 실행되는 영역
        {
            return isGameOver;
        }
        set//값을 대입할 때 실행되는 영역
        {
            isGameOver = value;//대입된 값은 value키워드를 통해 전달
            if (isGameOver)
            {
                CancelInvoke("CreateMonster");
            }
        }
    }
    //싱글턴 인스턴스 선언
    public static GameManager instance = null;

    public TMP_Text scoreText;
    private int totalScore = 0;

    private void Awake()
    {
        if (GameManager.instance == null)//처음 실행하는거라면
        {
            GameManager.instance = this;
        }
        else if (GameManager.instance != this)//씬 전환했다가 다시 돌아오면 static 변수라 이미 값 들어있음
        {
            Destroy(this.gameObject);//두번째 생성된 인스턴스 삭제
        }
        DontDestroyOnLoad(this.gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        CreateMonsterPool();

        Transform spawnPointGroup = GameObject.Find("SpawnPointGroup")?.transform;

        foreach(Transform point in spawnPointGroup)
        {
            points.Add(point);
        }
        InvokeRepeating("CreateMonster", 2.0f, createTime);//함수,대기시간,호출간격--> 실행되고 2초뒤에 함수 실행하고, 그뒤로 3초 간격으로 함수를 반복해서 호출?
        totalScore = PlayerPrefs.GetInt("TOTAL_SCORE",0);
        DisplayScore(0);
    }
    void CreateMonster()
    {
        int idx = Random.Range(0, points.Count);
        Transform point = this.points[idx];
        GameObject monster = GetMonsterInPool();
        monster?.transform.SetPositionAndRotation(point.position, point.rotation);
        monster?.SetActive(true);
    }
    //오브젝트 풀에 몬스터 생성
    void CreateMonsterPool()
    {
        for(int i=0; i < maxMonsters; i++)
        {
            GameObject monster = Instantiate(this.monsterPrefab);
            monster.name = $"Monster_{i:00}";//생성되는 gameobject이름 
            monster.SetActive(false);
            this.monsterPool.Add(monster);
        }
    }
    //오브젝트 풀에서 사용 가능한 몬스터를 추출해 반환
    public GameObject GetMonsterInPool()
    {
        foreach(var monster in monsterPool)//순회
        {
            if (monster.activeSelf == false)//비활성화여부 판단, 비활성화된 몬스터 프리팹 반환
            {
                //Debug.LogFormat("<color=red>isDie:{0}</color>",MonsterController.isDie);
                return monster;
            }
        }
        return null;
    }
    public void DisplayScore(int score)
    {
        totalScore += score;
        scoreText.text = $"<color=green>SCORE : </color><color=red>{totalScore:#,##0}</color>";
        PlayerPrefs.SetInt("TOTAL_SCORE", totalScore);
    }
}
