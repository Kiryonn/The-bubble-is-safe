using System.Linq;
using UnityEngine;

public class Monster : MonoBehaviour
{
	[SerializeField] private DetectionSphere detectionSphere;
	[SerializeField] private Animator animator;

	private void OnEnable()
	{
		detectionSphere.OnAreaEntered.AddListener(DetectedSomething);
		detectionSphere.OnAreaLeft.AddListener(LostDetection);
	}

	private void OnDisable()
	{
		detectionSphere.OnAreaEntered.RemoveListener(DetectedSomething);
		detectionSphere.OnAreaLeft.RemoveListener(LostDetection);
	}

	private void DetectedSomething(Collider something)
	{
		if (something.TryGetComponent(out Player player))
		{
			if (animator.parameters.Any(e => e.name == "EnemyDetected"))
				animator.SetBool("EnemyDetected", true);
		}
	}

	private void LostDetection(Collider something)
	{
		if (something.TryGetComponent(out Player player))
		{
			if (animator.parameters.Any(e => e.name == "EnemyDetected"))
				animator.SetBool("EnemyDetected", false);
		}
	}
}
