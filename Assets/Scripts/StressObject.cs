using UnityEngine;


public class StressObject : MonoBehaviour
{
	[SerializeField] private SphereCollider sphereCollider;
	public float Radius { get => sphereCollider.radius; set => sphereCollider.radius = value; }
	public float Stress = 1f;
	public float Delay = 1f;
	private float timer = 0f;
	private bool isPlayerInside = false;
	private void Update()
	{
		if (!isPlayerInside)
		{
			timer = 0f;
			return;
		}
		timer += Time.deltaTime;
		if (timer >= Delay)
		{
			timer -= Delay;
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		isPlayerInside = true;
	}

	private void OnCollisionExit(Collision other)
	{
		isPlayerInside = false;
	}
}
