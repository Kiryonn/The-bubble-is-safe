[System.Serializable]
public enum Dimension
{
	BubbleWorld = 1,
	Nightmare = 2,
	InBetween = Nightmare | BubbleWorld,
}
