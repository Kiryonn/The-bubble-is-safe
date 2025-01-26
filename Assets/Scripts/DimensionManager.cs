using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DimensionManager : MonoBehaviour
{
    public static DimensionManager Instance;
	public List<DimensionalObject> DimensionalObjects = new();
	public UnityEvent<Dimension> OnDimensionChange; 
	public Dimension currentDimension = Dimension.BubbleWorld;
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

	internal void AddDimensionalObject(DimensionalObject obj)
	{
		DimensionalObjects.Add(obj);
	}

	internal void RemoveDimensionalObjects(DimensionalObject obj)
	{
		DimensionalObjects.Remove(obj);
	}

	public void SetDimension(Dimension dimension)
	{
		foreach (var obj in DimensionalObjects)
			obj.SetDimension(dimension);
		OnDimensionChange.Invoke(dimension);
	}

	public void SetDimension(Dimension dimension, DimensionalObject obj)
	{
		obj.SetDimension(dimension);
	}

	public void SwitchDimension()
	{
		if (currentDimension == Dimension.BubbleWorld)
			currentDimension = Dimension.Nightmare;
		else
			currentDimension = Dimension.BubbleWorld;
		foreach (var dimensionalObject in DimensionalObjects)
			dimensionalObject.SetDimension(currentDimension);
		OnDimensionChange.Invoke(currentDimension);
	}

	public void SwitchDimension(DimensionalObject obj)
	{
		if (obj.CurrentDimension == Dimension.BubbleWorld)
			obj.SetDimension(Dimension.Nightmare);
		else
			obj.SetDimension(Dimension.BubbleWorld);
	}

	public List<DimensionalObject> GetBubbleWorldObjects() => DimensionalObjects.FindAll(dimensionalObject => dimensionalObject.CurrentDimension == Dimension.BubbleWorld);
	public List<DimensionalObject> GetNightmareObjects() => DimensionalObjects.FindAll(dimensionalObject => dimensionalObject.CurrentDimension == Dimension.Nightmare);
}
