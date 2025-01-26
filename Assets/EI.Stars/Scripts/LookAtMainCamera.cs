using UnityEngine;

public class LookAtMainCamera : MonoBehaviour
{
	void Update() 
	{
        Quaternion targetRotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime);        
    }
}