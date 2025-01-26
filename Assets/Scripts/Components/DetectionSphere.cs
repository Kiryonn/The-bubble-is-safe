using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SphereCollider))]
public class DetectionSphere : MonoBehaviour
{
	[SerializeField] SphereCollider sphereCollider;
	public float Range {get => sphereCollider.radius; set => sphereCollider.radius = value; }
	public UnityEvent<Collider> OnAreaEntered;
	public UnityEvent<Collider> OnAreaLeft;
	private void OnTriggerEnter(Collider other) => OnAreaEntered.Invoke(other);
	private void OnTriggerExit(Collider other) => OnAreaLeft.Invoke(other);
}
