using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class DetectionArea : MonoBehaviour
{
	[SerializeField] Collider area;
	public UnityEvent<Collider> OnAreaEntered;
	public UnityEvent<Collider> OnAreaLeft;
	private void OnTriggerEnter(Collider other) => OnAreaEntered.Invoke(other);
	private void OnTriggerExit(Collider other) => OnAreaLeft.Invoke(other);
	private void Start()
	{
		area.isTrigger = true;
	}
}
