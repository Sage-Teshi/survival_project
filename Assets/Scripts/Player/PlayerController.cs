using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]                    // 이동에 필요한 변수들
    public float moveSpeed;                 // 이동 속도
    public float jumpPower;                 // 점프 파워
    private Vector2 curMovementInput;       // 현재 누른 키
    public LayerMask groundLayerMask;

    [Header("Look")]                        // 회전에 필요한 변수들
    public Transform cameraContainer;
    public float minXLook;                  // 회전 범위 최소값 (최소 시야각)
    public float maxXLook;                  // 회전 범위 최대값 (최대 시야각)
    private float camCurXRot;               // 인풋엑션에서 마우스의 델타값을 저장
    public float lookSensitivity;           // 회전 민감도
    private Vector2 mouseDelta;             // 마우스 델타값 (마우스 변화값)
    public bool canLook = true;

    public Action inventory;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // 마우스 커서 보이지 않게 숨기기
    }

    
    void FixedUpdate()
    {
        Move();
    }
    
    private void LateUpdate()
    {
        if(canLook)
        {
            CameraLook();
        }
       
    }


    void Move() // 실제로 이동을 시켜주는 함수
    {
        // 방향 추출하기 (W, S키 (Y값))
        // 현재 입력의 y 값은 z 축 (forward, 앞뒤)에 곱한다.
        // 현재 입력의 x 값은 x 축 (right, 좌우)에 곱한다. 
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        dir *= moveSpeed;   // 방향에 속력을 곱해주기
        dir.y = _rigidbody.velocity.y; // y 값은 velocity(변화량)의 y 값을 넣어준다.

        // 방향 넣어주기 
        _rigidbody.velocity = dir;  // 연산된 속도를 velocity(변화량)에 넣어준다. 
    }

    // 실제로 카메라를 돌리는 함수
    void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);  // 원드 좌표가 아닌 월드 좌표를 넣어주기 

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0); //캐릭터 상하 각도;

    }


    public void OnMove(InputAction.CallbackContext context) // 이벤트 등록
    {   // phase는 분기점이라는 뜻, phase의 상태 보기
        if (context.phase == InputActionPhase.Performed)    // 키를 누르고 있는 동안 값을 받기 
        {
            // context에 정보가 닮겨 있다. 
            curMovementInput = context.ReadValue<Vector2>();
        }
        // 키를 땠을 때 
        else if (context.phase == InputActionPhase.Canceled)
        {
            // 가만히 있기
            curMovementInput = Vector2.zero;
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }


    public void OnJump(InputAction.CallbackContext context)
    {
        // 눌렀을 때 (주의: 눌리고 있을 때가 아님)
        if (context.phase == InputActionPhase.Started && IsGrounded())
        {
            _rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
        }
    }

    bool IsGrounded() // 플레이어가 그라운드인지 판별 (공중에서 점프 방지) (Ray를 그라운드로 쏘기)
    // 4개의 Ray를 만든다.
    // 플레이어(transfrom)을 기준으로 앞뒤좌우 0.2씩 떨어트려서.
    // 0.01 정도 살짝 위로 울린다. 
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up *0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up *0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up *0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right* 0.2f) + (transform.up *0.01f), Vector3.down),
        };

        // 4개의 Ray 중 groundLayerMask에 해당하는 오브젝트가 충돌했는지 조회한다.
        for(int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask))
            {
                return true;
            }
        }

        return false;
    }

    public void OnInventory(InputAction.CallbackContext context) 
    { 
        if (context.phase == InputActionPhase.Started)
        {
            inventory?.Invoke();
            ToggleCursor();
        }
    }

    void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;

    }

}
