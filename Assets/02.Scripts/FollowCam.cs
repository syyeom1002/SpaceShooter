using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    //ī�޶� ���� Ÿ���� Transform
    public Transform targetTr;
    //ī�޶� �ڽ��� Transform
    private Transform camTr;

    //���� ������κ��� ������ �Ÿ�
    [Range(2.0f, 20.0f)]//�������ο� ������ ����(distance)�� �Է¹��� �ּ� �ִ밪, �ν����ͺ信 �����̵�ٸ� ǥ��
    public float distance = 10.0f;

    [Range(0.0f, 10.0f)]
    public float height = 2.0f;
    //���� �ӵ�
    public float damping = 10.0f;

    public float targetOffset = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        camTr = GetComponent<Transform>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 pos = targetTr.position + (-targetTr.forward * distance) + (Vector3.up * height);
        camTr.position = Vector3.Slerp(camTr.position, pos, Time.deltaTime * damping);
        camTr.LookAt(targetTr.position+(targetTr.up*targetOffset));
    }
}
