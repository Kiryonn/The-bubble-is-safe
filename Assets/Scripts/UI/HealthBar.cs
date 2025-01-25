using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
	[SerializeField] private Health health;
	[SerializeField] private Scrollbar healthBar;

	private void OnEnable()
	{
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
