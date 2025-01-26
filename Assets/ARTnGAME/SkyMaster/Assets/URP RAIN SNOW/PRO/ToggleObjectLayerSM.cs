using System.Collections;
using System;
using UnityEngine;

namespace Artngame.SKYMASTER {

	public class ToggleObjectLayerSM : MonoBehaviour {


	public Color mouseOverColor = Color.blue;
	private Color originalColor ;

        public bool moveObject = false;
        public string layerNameA = "Default";
        public string layerNameB = "Background";

    void Start()
    {
        //originalColor = GetComponent<Renderer>().sharedMaterial.color;
    }
	void OnMouseEnter() {
		//GetComponent<Renderer>().material.color = mouseOverColor;
	}

	void OnMouseExit() {
		//GetComponent<Renderer>().material.color = originalColor;
	}
        void Update()
        {
            OnMouseDownA();
        }
	void OnMouseDownA()
    {
            //IEnumerator  OnMouseDown() {
            Vector3 screenSpace = Camera.main.WorldToScreenPoint(transform.position);
		Vector3 offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z));
		if (Input.GetMouseButtonDown(0))// GetMouseButton(0))
		{
			Vector3 curScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z);
			Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenSpace) + offset;

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                //Debug.Log("IN1");
                if (Physics.Raycast(ray, out hit, 10011))
                {
                    //Debug.Log("IN11");
                    //Debug.Log(hit.transform.gameObject.name);
                    if (LayerMask.LayerToName(hit.transform.gameObject.layer) == layerNameA)
                    {
                        //Debug.Log("IN111");
                        hit.transform.gameObject.layer = LayerMask.NameToLayer(layerNameB);
                    }
                    else
                    {
                        hit.transform.gameObject.layer = LayerMask.NameToLayer(layerNameA);
                    }
                }

                if (moveObject)
                {
                    transform.position = curPosition;
                }
                
               
			//yield return 1;
		}
	}

}

}