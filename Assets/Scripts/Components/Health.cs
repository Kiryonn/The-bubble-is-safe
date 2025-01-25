using UnityEngine;
using UnityEngine.Events;

public class Health: MonoBehaviour
{
	public float MaxHealth = 100f;
	public float CurrentHealth { get; private set; }
	public UnityEvent<float> OnHealthChanged;
	public UnityEvent OnDeath;

	private void Start()
	{
		CurrentHealth = MaxHealth;
		OnHealthChanged?.Invoke(CurrentHealth);
	}

	public void TakeDamage(float damage)
	{
		CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0f, MaxHealth);
		OnHealthChanged?.Invoke(CurrentHealth);
		if (CurrentHealth <= 0f)
		{
			OnDeath?.Invoke();
		}
	}
}
