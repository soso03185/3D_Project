using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController),typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float runSpeed = 5.0f;
    [SerializeField] float walkSpeed = 2.5f;
    [SerializeField] float jumpSpeed = 8.0f;
    [SerializeField] float gravity = 20.0f;
    [SerializeField] float rotationSpeed = 5f;

    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform barrelTransfrom;
    [SerializeField] Transform bulletParent;
    [SerializeField] float bulletHitMissDistance = 25f;

    [SerializeField] float animationSmoothTime = 0.1f;
    [SerializeField] float animationPlayTransition = 0.15f;

    public PlayerUI playerUI;

    private float speed;
    private Vector3 moveDirection = Vector3.zero;
    private Transform cameraTransform;
    private PlayerInput playerInput;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction shootAction;
    private InputAction aimAction;
    private InputAction reloadAction;

    private Animator animator;
    int moveXAnimationParameterId;
    int moveZAnimationParameterId;
    int jumpAnimation;
    int strafeAnimation;
    int moveAnimation;

    Vector2 currentAnimationBlendVector;
    Vector2 animationVelocity;

    private void Awake()
    {
        speed = runSpeed;
        cameraTransform = Camera.main.transform;

        // Player Input
        playerInput = GetComponent<PlayerInput>();
        moveAction  = playerInput.actions["Move"];
        jumpAction  = playerInput.actions["Jump"];
        shootAction = playerInput.actions["Shoot"];
        aimAction   = playerInput.actions["Aim"];
        reloadAction = playerInput.actions["Reload"];

        // Lock Cursor
        Cursor.lockState = CursorLockMode.Locked;

        // Anim
        animator = GetComponent<Animator>();
        jumpAnimation = Animator.StringToHash("Jump");
        strafeAnimation = Animator.StringToHash("Strafe");
        moveAnimation = Animator.StringToHash("Move");

        // Move Anim Parameter
        moveXAnimationParameterId = Animator.StringToHash("MoveX");
        moveZAnimationParameterId = Animator.StringToHash("MoveZ");
    }

    private void OnEnable()
    {       
        shootAction.performed += _ => ShootGun();
        aimAction.performed += _ => Aim();
        aimAction.canceled += _ => AimCancel();
        reloadAction.performed += _ => playerUI.Reload();
    }

    private void OnDisable()
    {
        shootAction.performed -= _ => ShootGun();
        aimAction.performed -= _ => Aim();
        aimAction.canceled -= _ => AimCancel();
        reloadAction.performed -= _ => playerUI.Reload();
    }

    private void ShootGun()  // Fire Button
    {
        if (playerUI.bullets <= 0) return;
        playerUI.Fire();

        RaycastHit hit;
        GameObject bullet = GameObject.Instantiate(bulletPrefab, barrelTransfrom.position, Quaternion.identity, bulletParent);
        BulletController bulletController = bullet.GetComponent<BulletController>();
        Debug.DrawRay(cameraTransform.position, cameraTransform.forward * bulletHitMissDistance, Color.red);

        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity))
        {
            bulletController.target = hit.point;
            bulletController.hit = true;
        }
        else
        {
            bulletController.target = cameraTransform.position + cameraTransform.forward * bulletHitMissDistance;
            bulletController.hit = false;
        }

    }

    private void Aim() // Aim Button
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Jump")) return;
            
        speed = walkSpeed;
        animator.CrossFade(strafeAnimation, animationPlayTransition);
    }

    private void AimCancel()
    {
        speed = runSpeed;
        animator.CrossFade(moveAnimation, animationPlayTransition);
    }

    void Update()
    {
        CharacterController controller = GetComponent<CharacterController>();
        Vector2 input = moveAction.ReadValue<Vector2>();

        if (controller.isGrounded)
        {
            currentAnimationBlendVector = Vector2.SmoothDamp(currentAnimationBlendVector, input, ref animationVelocity, animationSmoothTime);

            moveDirection = new Vector3(currentAnimationBlendVector.x, 0, currentAnimationBlendVector.y);
            moveDirection *= speed;
            moveDirection = moveDirection.x * cameraTransform.right.normalized + moveDirection.z * cameraTransform.forward.normalized;
            moveDirection.y = 0f;

            if (jumpAction.triggered)
            {
                moveDirection.y = jumpSpeed;
                animator.CrossFade(jumpAnimation, animationPlayTransition);
            }
        }

        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);

        animator.SetFloat(moveXAnimationParameterId, currentAnimationBlendVector.x);
        animator.SetFloat(moveZAnimationParameterId, currentAnimationBlendVector.y);

        // Rotate towards [player direction --> camera direction]
        Quaternion targetRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}