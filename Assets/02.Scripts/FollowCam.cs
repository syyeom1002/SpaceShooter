using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    //카메라가 따라갈 타겟의 Transform
    public Transform targetTr;
    //카메라 자신의 Transform
    private Transform camTr;

    //따라갈 대상으로부터 떨어질 거리
    [Range(2.0f, 20.0f)]//다음라인에 선언한 변수(distance)의 입력범위 최소 최대값, 인스펙터뷰에 슬라이드바를 표시
    public float distance = 10.0f;

    [Range(0.0f, 10.0f)]
    public float height = 2.0f;
    //반응 속도
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
