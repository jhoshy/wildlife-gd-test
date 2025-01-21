using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ZombieType
{
	RedBrain,
	YellowCap,
}

public class Trap : MonoBehaviour
{
	public ZombieType zombieType;

	public TrapObserver parent;
	public GameObject explosionPrefab;

	public int trigger;
	public bool IsTriggered => trigger > 0;

	public Follower target;
	private bool dead;

	public GameObject head1, head2;

	// Start is called before the first frame update
	void Start()
	{
		parent = GetComponentInParent<TrapObserver>();
		parent.totalTraps++;
		parent.observedTraps.Add(this);

		SimplePool.Preload(explosionPrefab, 6);

		head1.SetActive(zombieType == ZombieType.RedBrain);
		head2.SetActive(zombieType == ZombieType.YellowCap);

		this.transform.position = Game.SnapPosition(this.transform.position);
	}

	private void OnValidate()
	{
		head1.SetActive(zombieType == ZombieType.RedBrain);
		head2.SetActive(zombieType == ZombieType.YellowCap);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (dead)
			return;

		// TODO: Checar a condicao de se for o zumbi certo na trap certa...
		if (IsRightType(other))
		{
			var follower = other.GetComponent<Follower>();
			target = follower;
			trigger++;
		}
	}
	private void OnTriggerExit(Collider other)
	{
		if (dead)
			return;

		if (IsRightType(other))
		{
			if (trigger == 1)
				target = null;
			trigger--;
		}
	}

	public bool IsRightType(Collider other)
	{
		var follower = other.GetComponent<Follower>();
		if (follower && follower.zombieType == zombieType)
		{
			return true;
		}
		return false;
	}

	public void Die()
	{
		dead = true;

		var myTarget = target;

		StartCoroutine(Routine());
		IEnumerator Routine()
		{
			yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));
			myTarget.Die();

			SimplePool.Spawn(explosionPrefab, this.transform.position, explosionPrefab.transform.rotation);

			Destroy(this.gameObject);
		}
	}
}