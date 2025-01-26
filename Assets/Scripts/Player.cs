using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
	[SerializeField] private float playerSpeed = 8f;
	[SerializeField] private float jumpHeight = 2f;
	[SerializeField] private float gravityValue = -20f;
	[SerializeField] private float bufferJumpTime = 0.1f;
	[SerializeField] private GameObject model;
	[SerializeField] private Animator animator;
	[SerializeField] private CinemachineCamera cinemachineCamera;
	private CharacterController controller;
	private Vector3 playerVelocity;
	private Vector2 moveDirection;
	private bool isGrounded;
	private bool isJumping;
	private bool jumpBuffered;
	private float jumpBufferCounter;

	private InputAction move;
	private InputAction jump;
	private InputAction attack;

	void Start()
	{
		controller = gameObject.GetComponent<CharacterController>();

		// Get InputAction references from Project-wide input actions.
		move = InputSystem.actions.FindAction("Player/Move");
		jump = InputSystem.actions.FindAction("Player/Jump");
		attack = InputSystem.actions.FindAction("Player/Attack");

		// Add event listeners
		jump.started += JumpKeyDown;
		jump.canceled += JumpKeyUp;
		attack.started += AttackKeyDown;
		attack.canceled += AttackKeyUp;
		DimensionManager.Instance.OnDimensionChange.AddListener(OnDimensionChange);
	}

	private void OnDimensionChange(Dimension dimension)
	{
		Debug.Log("Dimension changed to " + dimension);
		animator.SetBool("IsNightmare", dimension == Dimension.Nightmare);
	}

	private void JumpKeyDown(InputAction.CallbackContext context)
	{
		if (isGrounded)
		{
			isJumping = true;
			jumpBufferCounter = 0;
		}
		else
		{
			jumpBuffered = true;
			jumpBufferCounter = bufferJumpTime;
		}
	}

	private void JumpKeyUp(InputAction.CallbackContext context)
	{
		isJumping = false;
	}

	private void ApplyJumpBuffer()
	{
		if (jumpBuffered)
		{
			jumpBufferCounter -= Time.deltaTime;
			if (jumpBufferCounter <= 0)
			{
				jumpBuffered = false;
			}
			if (isGrounded)
			{
				Jump();
				jumpBuffered = false;
			}
		}
	}

	private void Jump()
	{
		if (isJumping || jumpBuffered)
		{
			playerVelocity.y += Mathf.Sqrt(jumpHeight * -2f * gravityValue);
			jumpBuffered = false;
		}
	}

	private void Move()
	{
		// Apply gravity
		playerVelocity.y += gravityValue * Time.deltaTime;
		moveDirection = move.ReadValue<Vector2>();

		animator.SetFloat("HorizontalMovement", moveDirection.x);
		animator.SetFloat("VerticalMovement", moveDirection.y);
		bool isMoving = moveDirection != Vector2.zero;
		animator.SetBool("IsWalking", isMoving);

		if (!isMoving)
			return;

		var cameraForward = cinemachineCamera.transform.forward;
		var cameraRight = cinemachineCamera.transform.right;
		// Keep the camera's y rotation only
		cameraForward.y = 0;
		cameraRight.y = 0;
		var movement = (cameraForward * moveDirection.y + cameraRight * moveDirection.x).normalized;

		if (moveDirection.y > 0)
		{
			transform.rotation = Quaternion.Slerp(model.transform.rotation, Quaternion.LookRotation(movement), Time.deltaTime * 10f);
		}

		controller.Move(playerSpeed * Time.deltaTime * movement);
	}

	private void AttackKeyDown(InputAction.CallbackContext context)
	{
		animator.SetBool("IsAttacking", true);
	}

	private void AttackKeyUp(InputAction.CallbackContext context)
	{
		animator.SetBool("IsAttacking", false);
	}

	void Update()
	{
		isGrounded = controller.isGrounded;
		animator.SetBool("IsGrounded", isGrounded);
		if (isGrounded && playerVelocity.y < 0f)
		{
			playerVelocity.y = 0f;
		}
		ApplyJumpBuffer();
		Move();

		// Apply gravity
		controller.Move(playerVelocity * Time.deltaTime);
	}
}
