using UnityEngine;

public class BubbleSizer : MonoBehaviour
{
	[SerializeField] private Transform Bubble;
	[SerializeField] private float stressGainPerSecBubble;
	[SerializeField] private float stressGainPerSecNightmare;
	[SerializeField] private float maxStress;
	[SerializeField] private float minStress;
	[Range(0f, 1f)][SerializeField] private float Threshold;
	[SerializeField] private float minScale;
	[SerializeField] private float maxScale;
	private float currentStress;
	private float percent;
	private Dimension currentDimension = Dimension.BubbleWorld;


	private void Start()
	{
		currentStress = maxStress;
		percent = 1f;
		UpdateBubbleScale();
	}

	private void Update()
	{
		if (currentDimension == Dimension.BubbleWorld)
		{
			currentStress -= stressGainPerSecBubble * Time.deltaTime;
			percent = currentStress / maxStress;
			if (percent < Threshold)
				DimensionManager.Instance.SetDimension(Dimension.Nightmare);
		}
		else // if (currentDimension == Dimension.BubbleWorld)
		{
			currentStress -= stressGainPerSecNightmare * Time.deltaTime;
			percent = currentStress / maxStress;
			if (percent > Threshold)
				DimensionManager.Instance.SetDimension(Dimension.BubbleWorld);
		}
		UpdateBubbleScale();
	}

	private void UpdateBubbleScale()
	{
		Bubble.localScale = Mathf.Lerp(minScale, maxScale, percent) * Vector3.one;
	}

	public void OnDimensinChanged(Dimension newDimention) => currentDimension = newDimention;
}
