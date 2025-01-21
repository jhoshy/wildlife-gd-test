using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
	public float moveSpeed = 2f;
	public float topOffset = -5f;
	public float bottomOffset = 5f;

	private float diff;

	private Transform target;
	private Camera camera;
    
    void Start()
    {
		target = Player.Singleton.transform;
		camera = this.GetComponent<Camera>();
    }

    void LateUpdate()
    {
		 diff = this.transform.position.z - target.position.z;
		var pos = this.transform.position;

		if (diff < topOffset)
		{
			pos.z += topOffset - diff;
		}
		if (diff > bottomOffset)
		{
			pos.z += bottomOffset - diff;
		}

		this.transform.position = Vector3.Lerp(this.transform.position, pos, Time.deltaTime * moveSpeed);
    }
}
