using UnityEngine;
using UnityEngine.Events;

public class GodotTimer : MonoBehaviour
{
	public float WaitTime = 1.0f;
	public bool Autostart = false;
	public bool OneShot = false;
	private float _timeLeft;
	private bool _isRunning = false;

	public UnityEvent Timeout;

	void Start()
	{
		if (Autostart)
		{
			StartTimer();
		}
	}

	private void Update()
	{
		if (_isRunning)
		{
			_timeLeft -= Time.deltaTime;
			if (_timeLeft <= 0)
			{
				_isRunning = false;
				Timeout?.Invoke();
				if (!OneShot)
				{
					StartTimer();
				}
			}
		}
	}

	public void StartTimer()
	{
		_timeLeft = WaitTime;
		_isRunning = true;
	}

	public void StopTimer()
	{
		_isRunning = false;
	}

	public void SetWaitTime(float time)
	{
		WaitTime = time;
	}

	public bool IsRunning()
	{
		return _isRunning;
	}
}
