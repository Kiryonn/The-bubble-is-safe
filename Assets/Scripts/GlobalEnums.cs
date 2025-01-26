using System;

public enum Dimension
{
	BubbleWorld = 1,
	Nightmare = 2,
	InBetween = Nightmare | BubbleWorld,
}

[Serializable]
public enum DamageSource
{
	Player,
	MonsterChest,
}
