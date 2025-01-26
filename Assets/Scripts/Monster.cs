using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
	[SerializeField] private DetectionArea detectionArea;
	[SerializeField] private DetectionArea attackRange;
	[SerializeField] private GodotTimer attackCooldown;
	[SerializeField] private GodotTimer attackDuration;
	[SerializeField] private Animator animator;
	[SerializeField] private Hitbox attackHit;
	[SerializeField] private Hurtbox hurtbox;
	[SerializeField] private Health health;
	private readonly HashSet<string> animationParametersAvailable = new();
	private Transform target;
	private bool inAttackRange = false;
	private bool canAttack = true;

	private void Start()
	{
		foreach (var item in animator.parameters)
			animationParametersAvailable.Add(item.name);

		health.OnDeath.AddListener(OnDeath);
		health.OnHealthChanged.AddListener(OnDamageTaken);
	}

	private void Update()
	{
		if (target != null && !inAttackRange)
		{
			Move();
		}
	}

	private void OnEnable()
	{
		detectionArea.OnAreaEntered.AddListener(DetectedSomething);
		detectionArea.OnAreaLeft.AddListener(LostDetection);
		attackRange.OnAreaEntered.AddListener(OnAttackRangeEntered);
		attackRange.OnAreaLeft.AddListener(OnAttackRangeLeft);
		animator.Play("Idle");
	}

	private void OnDisable()
	{
		detectionArea.OnAreaEntered.RemoveListener(DetectedSomething);
		detectionArea.OnAreaLeft.RemoveListener(LostDetection);
		attackRange.OnAreaEntered.RemoveListener(OnAttackRangeEntered);
		attackRange.OnAreaLeft.RemoveListener(OnAttackRangeLeft);
	}

	private void OnAttackCooldownEnd()
	{
		canAttack = true;
	}

	private void OnAttackAnimationEnd()
	{
		attackHit.enabled = false;
	}

	private void Attack()
	{
		if (canAttack && inAttackRange)
		{
			animator.SetTrigger("Attack");
			canAttack = false;
			attackHit.enabled = true;
			attackCooldown.StartTimer();
			attackDuration.StartTimer();
		}
	}

	private void Move()
	{
		// Move enemy toward player when in range
		Vector3 direction = (target.position - transform.position).normalized;
		transform.position += direction * Time.deltaTime; // Adjust speed as needed

		if (animationParametersAvailable.Contains("HorizontalMovement"))
			animator.SetFloat("HorizontalMovement", direction.x);
		if (animationParametersAvailable.Contains("VerticalMovement"))
			animator.SetFloat("VerticalMovement", direction.y);
	}

	private void DetectedSomething(Collider something) => SetDetection(something, true);
	private void LostDetection(Collider something) => SetDetection(something, false);
	private void SetDetection(Collider something, bool detected)
	{
		if (something.TryGetComponent(out Player player))
		{
			if (animationParametersAvailable.Contains("EnemyDetected"))
				animator.SetBool("EnemyDetected", detected);
			target = detected ? player.transform : null;
		}
	}

	private void OnAttackRangeEntered(Collider something) => SetInRange(something, true);
	private void OnAttackRangeLeft(Collider something) => SetInRange(something, false);
	private void SetInRange(Collider something, bool inRange)
	{
		if (something.TryGetComponent(out Player player))
		{
			inAttackRange = inRange;
		}
	}

	private void OnDeath()
	{
		if (animationParametersAvailable.Contains("Died"))
			animator.SetTrigger("Died");
	}

	private void OnDamageTaken(float amount)
	{
		if (animationParametersAvailable.Contains("TakeDamage"))
			animator.SetTrigger("TakeDamage");
	}
}
