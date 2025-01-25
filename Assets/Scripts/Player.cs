using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
	[SerializeField] private float playerSpeed = 8f;
	[SerializeField] private float jumpHeight = 2f;
	[SerializeField] private float gravityValue = -20f;
	[SerializeField] private float bufferJumpDistance = 3f;
	private CharacterController controller;
	private Vector3 playerVelocity;
	private Vector2 moveDirection;
	private bool isGrounded;
	private bool isMoving;
	private bool isJumping;
	private bool isJumpKeyPressed;
	private InputAction move;
	private InputAction jump;

	void Start()
	{
		controller = gameObject.GetComponent<CharacterController>();

		// Get InputAction references from Project-wide input actions.
		move = InputSystem.actions.FindAction("Player/Move");
		jump = InputSystem.actions.FindAction("Player/Jump");

		// add event listeners for jump input
		jump.started += JumpKeyDown;
		jump.canceled += JumpKeyUp;
	}

	private bool IsJumping()
	{
		// let the player buffer it's jump if he's near the ground
		return isJumpKeyPressed && (isGrounded || Physics.Raycast(transform.position, Vector3.down, bufferJumpDistance, Physics.DefaultRaycastLayers));
	}
	
	private void JumpKeyDown(InputAction.CallbackContext context)
	{
		isJumpKeyPressed = true;
	}

	private void JumpKeyUp(InputAction.CallbackContext context)
	{
		isJumpKeyPressed = false;
	}

	private void Move()
	{
		moveDirection = move.ReadValue<Vector2>();
		var mv = new Vector3(moveDirection.x, 0, moveDirection.y);
		controller.Move(playerSpeed * Time.deltaTime * mv);
		if (mv != Vector3.zero)
			gameObject.transform.forward = mv;
	}

	void Update()
	{
		isGrounded = controller.isGrounded;
		if (isGrounded)
		{
			if (playerVelocity.y < 0f)
			{
				playerVelocity.y = 0f;
			}
			if (IsJumping())
			{
				playerVelocity.y += Mathf.Sqrt(jumpHeight * -2f * gravityValue);
				isJumpKeyPressed = false;
			}
		}
		playerVelocity.y += gravityValue * Time.deltaTime;
		Move();

		// gravity
		controller.Move(playerVelocity * Time.deltaTime);
	}
}
