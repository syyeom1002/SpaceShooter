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
    //���͸� �̸� ������ ������ ����Ʈ �ڷ���
    public List<GameObject> monsterPool = new List<GameObject>();
    //������Ʈ Ǯ�� ������ ������ �ִ� ����
    public int maxMonsters = 10;
    //ĳ���Ͱ� ����ϸ� ���� ���� �������Ѿ���
    private bool isGameOver;
    
    //������Ƽ
    public bool IsGameOver
    {
        get//�ܺο��� ������Ƽ ���� �� ����Ǵ� ����
        {
            return isGameOver;
        }
        set//���� ������ �� ����Ǵ� ����
        {
            isGameOver = value;//���Ե� ���� valueŰ���带 ���� ����
            if (isGameOver)
            {
                CancelInvoke("CreateMonster");
            }
        }
    }
    //�̱��� �ν��Ͻ� ����
    public static GameManager instance = null;

    public TMP_Text scoreText;
    private int totalScore = 0;

    private void Awake()
    {
        if (GameManager.instance == null)//ó�� �����ϴ°Ŷ��
        {
            GameManager.instance = this;
        }
        else if (GameManager.instance != this)//�� ��ȯ�ߴٰ� �ٽ� ���ƿ��� static ������ �̹� �� �������
        {
            Destroy(this.gameObject);//�ι�° ������ �ν��Ͻ� ����
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
        InvokeRepeating("CreateMonster", 2.0f, createTime);//�Լ�,���ð�,ȣ�Ⱓ��--> ����ǰ� 2�ʵڿ� �Լ� �����ϰ�, �׵ڷ� 3�� �������� �Լ��� �ݺ��ؼ� ȣ��?
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
    //������Ʈ Ǯ�� ���� ����
    void CreateMonsterPool()
    {
        for(int i=0; i < maxMonsters; i++)
        {
            GameObject monster = Instantiate(this.monsterPrefab);
            monster.name = $"Monster_{i:00}";//�����Ǵ� gameobject�̸� 
            monster.SetActive(false);
            this.monsterPool.Add(monster);
        }
    }
    //������Ʈ Ǯ���� ��� ������ ���͸� ������ ��ȯ
    public GameObject GetMonsterInPool()
    {
        foreach(var monster in monsterPool)//��ȸ
        {
            if (monster.activeSelf == false)//��Ȱ��ȭ���� �Ǵ�, ��Ȱ��ȭ�� ���� ������ ��ȯ
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
