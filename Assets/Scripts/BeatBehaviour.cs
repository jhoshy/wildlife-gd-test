using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatBehaviour : MonoBehaviour
{
	public bool invokeBeatEvent;
	public float beatDelay;
	public Transform target;

	private Vector3 startPosition;
	private float startTime;
	private float endTime;
	public Action OnWillDestroy;

	private Image image;

	private void Awake()
	{
		image = this.GetComponent<Image>();
	}

	public void OnEnable()
	{
		enabled = true;
		beatDelay = AudioController.Singleton.CurrentBeatDelay * 4;

		startPosition = this.transform.position;

		startTime = Time.time;
		endTime = startTime + beatDelay + (Game.Singleton.exitHitDelayOffset);//taxa de aceitacao
		currentTime = Time.time - startTime;

		Player.Singleton.OnMoved += CheckBeat;
		RestartObject();
	}

	public void RestartObject()
	{
		transform.position = Vector3.one * 5000;//Vector3.Lerp(startPosition, target.position, currentTime / beatDelay);

		var color = Color.white;
		color.a = 0.7f;

		image.color = color;
	}

	public void OnDisable()
	{
		Player.Singleton.OnMoved -= CheckBeat;		
	}

	private float currentTime;
	private bool enabled;

	void Update()
    {
		currentTime = Time.time - startTime;
		if (enabled)
		{
			//var currentTime = Time.time - startTime;
			transform.position = Vector3.Lerp(startPosition, target.position, currentTime / beatDelay);
		}

		if(enabled && Time.time > endTime)
		{
			if (invokeBeatEvent)
			{
				// Perdeu A NOTA
				Game.Singleton.OnMissedBeat?.Invoke();
			}

			enabled = false;

			if (!Game.Singleton.IsLevelComplete)
			{
				image.color = Color.red;
				this.transform.DOMove(this.transform.position + Vector3.down * 15f, 0.4f);
			}

			image.DOFade(0f, 0.4f).OnComplete(() =>
			{
				//OnWillDestroy?.Invoke();
				RestartObject();
				SimplePool.Despawn(this.gameObject);
				//Destroy(this.gameObject);
			});
		}
		//transform.position += this.transform.right * beatDelay * Time.deltaTime;

		//clone.transform.DOMove(target.position, AudioController.Singleton.CurrentBeatDelay * 4).SetEase(Ease.Linear).OnComplete(() => {
		//	Destroy(clone);
		//});
	}

	public void CheckBeat()
	{
		// se tiver dentro do valor esperado de aceitação de beat, ta CERTO, se nao ta errado.
		if(enabled && (currentTime / beatDelay) >= Game.Singleton.entryHitAcceptanceOffset)
		{
			enabled = false;
			RightBeat();
			if (invokeBeatEvent)
			{
				Game.Singleton.OnRightBeat?.Invoke();
			}
		}
	}

	public void RightBeat()
	{
		this.transform.DOMove(this.transform.position + Vector3.up * 50f, 0.4f);

		var image = this.GetComponent<Image>();
		image.color = Color.yellow;
		image.DOFade(0f,0.2f).OnComplete(() =>
		{
			//OnWillDestroy?.Invoke();
			RestartObject();
			SimplePool.Despawn(this.gameObject);
			//Destroy(this.gameObject);
		});
	}
}