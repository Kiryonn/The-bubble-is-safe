using System.Collections.Generic;
using UnityEngine;

public class DimensionManager : MonoBehaviour
{
    public static DimensionManager Instance;
	public List<DimensionalObject> DimensionalObjects = new();

	public Skybox bubbleWorldSkybox;
	public Skybox nightmareSkybox;
	public Skybox InBetweenSkybox;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
			return;
		}
	}

	internal void AddDimensionalObject(DimensionalObject dimensionalObject)
	{
		DimensionalObjects.Add(dimensionalObject);
	}

	internal void RemoveDimensionalObjects(DimensionalObject dimensionalObject)
	{
		DimensionalObjects.Remove(dimensionalObject);
	}

	public void SetDimension(Dimension dimension)
	{
		foreach (var dimensionalObject in DimensionalObjects)
			dimensionalObject.SetDimension(dimension);
	}

	public void SetDimension(Dimension dimension, DimensionalObject dimensionalObject)
	{
		dimensionalObject.SetDimension(dimension);
	}

	public void SwitchDimension()
	{
		foreach (var dimensionalObject in DimensionalObjects)
		{
			if (dimensionalObject.CurrentDimension == Dimension.BubbleWorld)
				dimensionalObject.SetDimension(Dimension.Nightmare);
			else
				dimensionalObject.SetDimension(Dimension.BubbleWorld);
		}
	}

	public void SwitchDimension(DimensionalObject dimensionalObject)
	{
		if (dimensionalObject.CurrentDimension == Dimension.BubbleWorld)
			dimensionalObject.SetDimension(Dimension.Nightmare);
		else
			dimensionalObject.SetDimension(Dimension.BubbleWorld);
	}

	public List<DimensionalObject> GetBubbleWorldObjects() => DimensionalObjects.FindAll(dimensionalObject => dimensionalObject.CurrentDimension == Dimension.BubbleWorld);
	public List<DimensionalObject> GetNightmareObjects() => DimensionalObjects.FindAll(dimensionalObject => dimensionalObject.CurrentDimension == Dimension.Nightmare);
}
