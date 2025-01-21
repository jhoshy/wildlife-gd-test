using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapObserver : MonoBehaviour
{
	public int totalTraps;
	public int triggeredTraps;

	public List<Trap> observedTraps;

	private float explosionDelay = 0.01f;
	public float timer = 0;

	public bool exploded;

	public void Update()
	{
		if (exploded)
			return;

		foreach (var item in observedTraps)
		{
			if (!item.IsTriggered)
			{
				timer = Time.time;
				return;
			}
		}

		if(Time.time > timer + explosionDelay)
		{
			exploded = true;
			Debug.Log("Expliode geral pls");

			foreach (var item in observedTraps)
			{
				item.Die();
			}
		}
	}
}
