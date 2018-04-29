using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebCamFeed : MonoBehaviour {

	public RawImage rawimage;
	bool setSize = false;
	
	void Start () 
	{
			WebCamTexture webcamTexture = new WebCamTexture();
			rawimage.texture = webcamTexture;
			webcamTexture.Play();
	}
	
	void Update()
	{
		// rawimage.texture.width
		rawimage.SetNativeSize();
	}
}