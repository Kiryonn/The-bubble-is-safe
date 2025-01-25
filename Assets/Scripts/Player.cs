using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
	[SerializeField] private float playerSpeed = 8.0f;
	[SerializeField] private float jumpHeight = 2.5f;
	[SerializeField] private float gravityValue = -20f;
	private CharacterController controller;
	private Vector3 playerVelocity;
	private Vector2 moveDirection;
	private bool isGrounded;
	private bool isMoving;
	private bool hasPressedJump;
	private InputAction move;
	private InputAction jump;

	void Start()
	{
		controller = gameObject.GetComponent<CharacterController>();

		// Get InputAction references from Project-wide input actions.
		move = InputSystem.actions.FindAction("Player/Move");
		jump = InputSystem.actions.FindAction("Player/Jump");

		// Subscribe to the InputAction events.
		jump.performed += ctx => Jump();
	}

	private void Jump()
	{
		hasPressedJump = true;
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
		playerVelocity.y += gravityValue * Time.deltaTime;
		if (isGrounded)
		{
			if (playerVelocity.y < 0f)
			{
				playerVelocity.y = 0f;
			}
			if (hasPressedJump)
			{
				playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
				hasPressedJump = false;
			}
		}
		Move();

		// gravity
		controller.Move(playerVelocity * Time.deltaTime);
	}
}
