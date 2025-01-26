using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Hitbox : MonoBehaviour
{
	[SerializeField] private float damage = 2f;
	private void OnTriggerEnter(Collider other)
	{
		if (other.TryGetComponent(out Hurtbox hurtBox))
		{
			float dmg = damage;
			foreach (var item in GetComponents<DamageModifier>())
				dmg += damage * item.DamagePercent + item.DamageFlat;
			AttackInfo context = new()
			{
				Damage = dmg
			};
			hurtBox.Hurt(context);
		}
	}
}
