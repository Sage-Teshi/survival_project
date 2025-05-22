using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]                    // �̵��� �ʿ��� ������
    public float moveSpeed;                 // �̵� �ӵ�
    public float jumpPower;                 // ���� �Ŀ�
    private Vector2 curMovementInput;       // ���� ���� Ű
    public LayerMask groundLayerMask;

    [Header("Look")]                        // ȸ���� �ʿ��� ������
    public Transform cameraContainer;
    public float minXLook;                  // ȸ�� ���� �ּҰ� (�ּ� �þ߰�)
    public float maxXLook;                  // ȸ�� ���� �ִ밪 (�ִ� �þ߰�)
    private float camCurXRot;               // ��ǲ���ǿ��� ���콺�� ��Ÿ���� ����
    public float lookSensitivity;           // ȸ�� �ΰ���
    private Vector2 mouseDelta;             // ���콺 ��Ÿ�� (���콺 ��ȭ��)
    public bool canLook = true;

    public Action inventory;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // ���콺 Ŀ�� ������ �ʰ� �����
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


    void Move() // ������ �̵��� �����ִ� �Լ�
    {
        // ���� �����ϱ� (W, SŰ (Y��))
        // ���� �Է��� y ���� z �� (forward, �յ�)�� ���Ѵ�.
        // ���� �Է��� x ���� x �� (right, �¿�)�� ���Ѵ�. 
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        dir *= moveSpeed;   // ���⿡ �ӷ��� �����ֱ�
        dir.y = _rigidbody.velocity.y; // y ���� velocity(��ȭ��)�� y ���� �־��ش�.

        // ���� �־��ֱ� 
        _rigidbody.velocity = dir;  // ����� �ӵ��� velocity(��ȭ��)�� �־��ش�. 
    }

    // ������ ī�޶� ������ �Լ�
    void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);  // ���� ��ǥ�� �ƴ� ���� ��ǥ�� �־��ֱ� 

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0); //ĳ���� ���� ����;

    }


    public void OnMove(InputAction.CallbackContext context) // �̺�Ʈ ���
    {   // phase�� �б����̶�� ��, phase�� ���� ����
        if (context.phase == InputActionPhase.Performed)    // Ű�� ������ �ִ� ���� ���� �ޱ� 
        {
            // context�� ������ ��� �ִ�. 
            curMovementInput = context.ReadValue<Vector2>();
        }
        // Ű�� ���� �� 
        else if (context.phase == InputActionPhase.Canceled)
        {
            // ������ �ֱ�
            curMovementInput = Vector2.zero;
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }


    public void OnJump(InputAction.CallbackContext context)
    {
        // ������ �� (����: ������ ���� ���� �ƴ�)
        if (context.phase == InputActionPhase.Started && IsGrounded())
        {
            _rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
        }
    }

    bool IsGrounded() // �÷��̾ �׶������� �Ǻ� (���߿��� ���� ����) (Ray�� �׶���� ���)
    // 4���� Ray�� �����.
    // �÷��̾�(transfrom)�� �������� �յ��¿� 0.2�� ����Ʈ����.
    // 0.01 ���� ��¦ ���� �︰��. 
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up *0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up *0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up *0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right* 0.2f) + (transform.up *0.01f), Vector3.down),
        };

        // 4���� Ray �� groundLayerMask�� �ش��ϴ� ������Ʈ�� �浹�ߴ��� ��ȸ�Ѵ�.
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
