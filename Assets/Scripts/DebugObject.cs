using UnityEngine;

public class DebugObject : MonoBehaviour
{
	#if !UNITY_EDITOR
	private void Awake()
	{
		DestroyImmediate(gameObject);
	}
	#endif
}
