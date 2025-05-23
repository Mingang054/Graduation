﻿using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance { get; private set; } // 싱글톤 인스턴스

    [SerializeField] private Animator animator;

    // 임시 변수들
    private float temp = 50f; // 임시 달리기용 자원
    private float tempMax = 100f;
    private float tempRegain = 1f;
    private float tempCost = 5;

    // sprint 플래그
    private bool isSprinting = false;
    private bool isSprintable = true;
    private bool isSprintReset = true;

    // 이동 관련 변수
    private float walkSpeed = 4f;
    public float WalkSpeed
    {
        get { return walkSpeed; }
        private set { walkSpeed = Mathf.Clamp(value, 0f, 20f); }
    }

    private float sprintSpeed = 7.5f;
    public float SprintSpeed
    {
        get { return sprintSpeed; }
        private set { sprintSpeed = Mathf.Clamp(value, 0f, 20f); }
    }


    private Vector2 moveInput;
    private Rigidbody2D playerRigidbody2D;
    private Camera mainCamera;

    //private float previousDirection = 1f; // 이전 방향 저장
    private Vector3 previousMousePosition;
    private bool wasWalking = false;
    private bool wasSprinting = false;

    private void Awake()
    {
        playerRigidbody2D = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main; // 메인 카메라 참조

        // 싱글톤 초기화
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 중복 인스턴스 제거
        }


    }

    public void OnMove(InputValue value)            // WASD nomalVector 반환
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnSprint(InputValue value)
    {
        if (value.isPressed)
        {
            // Sprint 버튼을 다시 누르기 위해 먼저 떼야 한다.
            if (!isSprintReset)
                return;

            isSprinting = true;
        }
        else
        {
            // Sprint 버튼이 떼어질 때 상태 초기화
            isSprinting = false;
            isSprintReset = true;
        }
    }

    private void Update()
    {
        //OptimizeFlipObject(); // 마우스 위치 변화 시에만 반전 처리
    }

    void FixedUpdate()
    {
        OptimizeControlMovement(); // 이동 관련 최적화 처리
        OptimizeUpdateAnimator(); // 애니메이션 상태 변경 시에만 업데이트
    }

    private void OptimizeControlMovement()
    {
        if (moveInput == Vector2.zero) // 이동하지 않을 때는 조기 반환
        {
            RegainTemp();
            return;
        }

        bool isTempEnough = temp > 0;

        // 스프린트 가능 여부 업데이트
        if (!isTempEnough)
        {
            isSprintable = false;
            isSprinting = false; // 스프린트 중단
        }
        else if (!isSprinting)
        {
            isSprintable = true; // 스프린트가 아닌 경우 회복 가능
        }

        if (isSprinting && isSprintable && isTempEnough)
        {
            isSprintReset = false;
            temp -= tempCost * Time.fixedDeltaTime;
            MovePlayer(sprintSpeed);
        }
        else
        {
            RegainTemp();
            MovePlayer(walkSpeed);
        }
    }

    private void MovePlayer(float speed)
    {
        Vector2 movement = moveInput * speed * Time.fixedDeltaTime;
        playerRigidbody2D.MovePosition(playerRigidbody2D.position + movement);
    }

    private void RegainTemp()
    {
        temp = Mathf.Clamp(temp + tempRegain * Time.fixedDeltaTime, 0, tempMax);
    }

    public void SetMoveSpeed(float newSpeed)
    {
        WalkSpeed = newSpeed; // 속성의 private set을 통해 값 설정
    }


    /*private void OptimizeFlipObject()    //PlayerShooter로 이관
    {
        // 마우스 위치를 월드 좌표로 변환
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = mainCamera.ScreenToWorldPoint(mousePosition);

        // 플레이어와 마우스 간의 거리 계산
        Vector3 direction = mousePosition - transform.position;
        float currentDirection = Mathf.Sign(direction.x);

        // 방향이 바뀌었을 때만 처리
        if (currentDirection != previousDirection)
        {
            previousDirection = currentDirection;

            Vector3 scale = playerToFlip.localScale;
            scale.x = Mathf.Abs(scale.x) * currentDirection;
            playerToFlip.localScale = scale;
        }
    }*/


    private void OptimizeUpdateAnimator()
    {
        // Stand -> Walk -> Sprint 전환을 위한 애니메이션 상태 설정
        bool isWalking = moveInput != Vector2.zero && !isSprinting; // 걷는 중인지 확인
        bool isSprintingNow = isSprinting && moveInput != Vector2.zero; // 달리는 중인지 확인

        if (isWalking != wasWalking)
        {
            animator.SetBool("isWalk", isWalking);
            wasWalking = isWalking;
        }

        if (isSprintingNow != wasSprinting)
        {
            animator.SetBool("isSprint", isSprintingNow);
            wasSprinting = isSprintingNow;
        }
    }
}



