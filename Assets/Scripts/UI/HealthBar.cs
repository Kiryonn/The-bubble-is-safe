using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
	[SerializeField] private Health health;
	[SerializeField] private Scrollbar healthBar;

	private void OnEnable()
	{
		UpdateHealthBar(health.CurrentHealth);
		health.OnHealthChanged.AddListener(UpdateHealthBar);
	}

	private void OnDisable()
	{
		health.OnHealthChanged.RemoveListener(UpdateHealthBar);
	}

	private void UpdateHealthBar(float currentHealth)
	{
		healthBar.size = currentHealth / health.MaxHealth;
	}
}
