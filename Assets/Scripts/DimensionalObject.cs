using UnityEngine;

public class DimensionalObject : MonoBehaviour
{
	[SerializeField] private GameObject BubbleWorldObject;
	[SerializeField] private GameObject NightmareObject;
	private Dimension currentDimension;
	public Dimension CurrentDimension { get => currentDimension; set => SetDimension(value); }

	private void Start()
	{
		DimensionManager.Instance.AddDimensionalObject(this);
		SetDimension(Dimension.BubbleWorld);
	}

	private void OnDestroy()
	{
		DimensionManager.Instance.RemoveDimensionalObjects(this);
	}

	public void SetDimension(Dimension dimension)
	{
		currentDimension = dimension;
		if (BubbleWorldObject != null)
			BubbleWorldObject.SetActive(dimension == Dimension.BubbleWorld);
		if (NightmareObject != null)
			NightmareObject.SetActive(dimension == Dimension.Nightmare);
	}

	public void ToggleDimension()
	{
		SetDimension(currentDimension == Dimension.BubbleWorld ? Dimension.Nightmare : Dimension.BubbleWorld);
	}
}
