using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
	public static Player Singleton;

	private int totalHealth;
	private int health = 10;
	public float GetHealthPercent => (float)health / (float)totalHealth;
	public Action<float> OnDamageTaken;
	public Action OnPlayerDied;

	public float walkUnit;

	public Vector3 nextPosition;

	public List<Vector3> movementQueue = new List<Vector3>();

	public List<Follower> followers = new List<Follower>();

	public Action OnMoved;

	public AnimationCurve jumpCurve;
	private Coroutine moveRoutine;

	public bool canMove;
	public bool isDead;

	private void Awake()
	{
		Singleton = this;
		totalHealth = health;
	}

	private void Start()
	{
		this.transform.position = Game.SnapPosition(this.transform.position);
		nextPosition = this.transform.position;

		AddToQueue(nextPosition);

		canMove = true;
		isDead = false;
	}

	void Update()
	{
		if (isDead)
			return;

		var inputRight = Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || SwipeManager.IsSwipingRight();
		var inputLeft = Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || SwipeManager.IsSwipingLeft();
		var inputUp = Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || SwipeManager.IsSwipingUp();
		var inputDown = Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || SwipeManager.IsSwipingDown();

		var direction = Vector3.zero;

		if (inputRight) direction = Vector3.right;
		if (inputLeft) direction = Vector3.left;
		if (inputUp) direction = Vector3.forward;
		if (inputDown) direction = Vector3.back;


		if ((canMove || Game.Singleton.IsLevelComplete) && direction != Vector3.zero)
		{
			if (!Game.Singleton.hasStarted)
			{
				Game.Singleton.hasStarted = true;
			}

			Game.Singleton.OnPlayerPressedBeat?.Invoke();

			// Movement stuff
			{
				var possiblePosition = nextPosition + direction.normalized * walkUnit;

				// Se nao for uma parede do grid, pode ir.
				if (Jessinho.Grrrid.Singleton.GetNodeFromWorldPoint(possiblePosition).walkable)
				{
					OnMoved?.Invoke();
					nextPosition = possiblePosition;
					AddToQueue(nextPosition);

					this.transform.DOKill();
					this.transform.localScale = Vector3.one;

					JumpTo(nextPosition);

					this.transform.DOPunchScale(-Vector3.one * Game.Singleton.landingForce, Game.Singleton.landingDuration).SetDelay(Game.Singleton.moveDuration * Game.Singleton.landingDelay);
				}
				else
				{
					// Se é uma parede, da um pulinho pra cima na mesma posicao
					JumpTo(this.transform.position);
				}

			}
		}

		//this.transform.position = Vector3.MoveTowards(this.transform.position, nextPosition, Time.deltaTime * jumpPower);
	}

	public void TakeDamage()
	{
		health = Mathf.Max(health - 1, 0);
		OnDamageTaken.Invoke(GetHealthPercent);

		if (health == 0)
		{
			isDead = true;
			OnPlayerDied?.Invoke();
			// PLAYER DIED. RESTART LEVEL? POPUP?
			//Debug.Log("Player MORREU");

		}
	}

	void JumpTo(Vector3 target)
	{
		canMove = false;
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

			//yield return null;
			canMove = true;
		}
	}


	internal void UpdateIndexes()
	{
		for (int i = 0; i < followers.Count; i++)
		{
			var follower = followers[i];
			follower.index = i;
		}
	}

	public void AddToQueue(Vector3 position)
	{
		movementQueue.Insert(0, position);
		if (movementQueue.Count > 40)
		{
			movementQueue.RemoveAt(movementQueue.Count - 1);
		}
	}

}