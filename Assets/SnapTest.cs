using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapTest : MonoBehaviour
{

	public Vector3 snappedPosition;

	private void OnDrawGizmos()
	{
		snappedPosition = Game.SnapPosition(this.transform.position);

		//if (Jessinho.Grrrid.Singleton)
		//{
		//	var node = Jessinho.Grrrid.Singleton.GetNodeFromWorldPoint(this.transform.position);
		//	snappedPosition = node.worldPosition;
		//}


		Gizmos.color = Color.red;
		Gizmos.DrawSphere(snappedPosition, 0.3f);
	}
}
