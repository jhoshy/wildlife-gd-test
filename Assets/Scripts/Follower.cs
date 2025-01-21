using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jessinho;

public class Follower : MonoBehaviour
{
	public ZombieType zombieType;

	private Player leader;
	public float searchTargetRange = 4f;

	public Vector3 nextPosition;
	public Vector3 targetPosition;

	public int index = 1;
	private Coroutine moveRoutine;
	private Vector3 moveDirection;

	public GameObject warningSign;
	public GameObject attackHitPrefab;

	void Start()
	{
		warningSign.SetActive(false);
		this.transform.position = Game.SnapPosition(this.transform.position);
		nextPosition = this.transform.position;

		Game.Singleton.OnMissedBeat += HandleFinishedTurnMissedBeat;
		Player.Singleton.OnMoved += Move;

		Game.Singleton.enemies.Add(this);

		currentNode = Grrrid.Singleton.GetNodeFromWorldPoint(this.transform.position);

		SimplePool.Preload(attackHitPrefab);
	}
	private void OnDestroy()
	{
		Game.Singleton.OnMissedBeat -= HandleFinishedTurnMissedBeat;
		Player.Singleton.OnMoved -= Move;

		Game.Singleton.enemies.Remove(this);
	}

	public void HandleFinishedTurnMissedBeat()
	{
		if (this.transform.position != targetPosition)
		{
			Move();
		}
		else
		{
			if (index == 0)
			{
				//Debug.Log("PODE ATACAR");
				//var targetPos = this.transform.position + (leader.transform.position - this.transform.position).normalized * leader.walkUnit;
				//JumpTo(targetPos);
				Attack();
			}
			//else
			{
				JumpTo(this.transform.position);
			}
		}
	}

	void Update()
	{
		if (!leader)
			return;

		var direction = Vector3.zero;

		//this.transform.position = Vector3.MoveTowards(this.transform.position, nextPosition, Time.deltaTime * speed);
	}

	public void Attack()
	{
		var clone = SimplePool.Spawn(attackHitPrefab, this.transform.position + (leader.transform.position- this.transform.position).normalized * 1.0f + Vector3.up * 5f, this.transform.rotation);

		Player.Singleton.TakeDamage();
	}

	Node currentNode;
	public void Move()
	{
		// Encontrar o player
		if (!leader)
		{
			var cols = Physics.OverlapSphere(this.transform.position, searchTargetRange);
			foreach (var item in cols)
			{
				var player = item.GetComponent<Player>();
				if (player)
				{
					leader = player;
					leader.followers.Add(this);
					player.UpdateIndexes();

					warningSign.SetActive(true);
					warningSign.transform.DOPunchScale(Vector3.one * 0.20f, 0.2f);
					warningSign.transform.DOPunchRotation(Vector3.one * 0.35f, 0.4f);
					warningSign.transform.DOScale(Vector3.zero, 0.2f).SetDelay(0.4f).OnComplete(() =>
					{
						warningSign.SetActive(false);
					});

					//StartCoroutine(Routine());
					//IEnumerator Routine()
					//{
					//	warningSign.SetActive(true);
					//	yield return new WaitForSeconds(0.7f);
					//	warningSign.SetActive(false);
					//}
					break;
				}
			}
			return;
		}

		if (leader.movementQueue.Count > index)
			targetPosition = leader.movementQueue[index];

		// IS ON PLACE
		if(this.transform.position == targetPosition)
		{
			Debug.Log("Meio que ja ta onde devia ne");			
		}

		moveDirection = targetPosition - this.transform.position;		
		nextPosition += moveDirection.normalized * leader.walkUnit;		
		nextPosition = Game.SnapPosition(nextPosition);

		JumpTo(nextPosition);
		//this.transform.DOJump(nextPosition, Game.Singleton.jumpPower, 1, Game.Singleton.moveDuration);
		
	}

	void JumpTo(Vector3 target)
	{
		this.transform.DOKill();

		if (moveRoutine != null)
			StopCoroutine(moveRoutine);
		moveRoutine = StartCoroutine(Routine());

		IEnumerator Routine()
		{

			var start = this.transform.position;
			var end = target;

			var total = Game.Singleton.moveDuration;

			var current = 0f;

			while (current < total)
			{
				current = Mathf.Min(current + Time.deltaTime, total);// current / total;

				var pos = MathParabola.Parabola(start, end, Game.Singleton.jumpPower, current / total, true);
				this.transform.position = pos;

				yield return null;
			}
			this.transform.position = end;
		}
		this.transform.localScale = Vector3.one;
		this.transform.DOPunchScale(-Vector3.one * Game.Singleton.landingForce, Game.Singleton.landingDuration).SetDelay(Game.Singleton.moveDuration * Game.Singleton.landingDelay);
	}

	/*
//if (this.name.Equals("Follower1"))
{
	//direction.y = Mathf.Abs(direction.y) > Mathf.Abs(direction.x) ? direction.y : 0;
	//direction.x = Mathf.Abs(direction.x) > Mathf.Abs(direction.y) ? direction.x : 0;

	Debug.Log("Dir: " + direction);
	Debug.Log("AbX: " + Mathf.Abs(direction.x));
	Debug.Log("AbY: " + Mathf.Abs(direction.z));

	if (Mathf.Abs(direction.z) < Mathf.Abs(direction.x))
	{
		direction = new Vector3(direction.x, 0, 0);
	}
	else
	{
		direction = new Vector3(0, 0, direction.z);
	}
	//Mathf.Abs(direction.x) > Mathf.Abs(direction.y) ? direction.x : 0;
}
*/

	public void Die()
	{
		leader.followers.Remove(this);
		leader.UpdateIndexes();

		Destroy(this.gameObject);
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(this.transform.position, searchTargetRange);
	}

	private void OnTriggerEnter(Collider other)
	{
		//var player = other.GetComponent<Player>();
		//
		//if (player)
		//{
		//	player.TakeDamage();
		//}
	}
}