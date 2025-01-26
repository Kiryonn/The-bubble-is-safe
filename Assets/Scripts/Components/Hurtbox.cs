using UnityEngine;


[RequireComponent(typeof(Collider))]
public class Hurtbox : MonoBehaviour
{
	[SerializeField] private Health health;

	public void Hurt(AttackInfo context)
	{
		Debug.Log("Take damage");
		health.TakeDamage(context.Damage);
	}
}
