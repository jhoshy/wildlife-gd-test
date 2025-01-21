using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BeatSpawner : MonoBehaviour
{
	public Transform target;
	public GameObject beatHitPrefab;
	private float lastBeatTime;
	private Transform canvasTransform;
	public bool invokeBeatEvent;

	private void Start()
	{
		canvasTransform = GetComponentInParent<Canvas>().transform;
		SimplePool.Preload(beatHitPrefab,10);
	}


	void Update()
	{

		var delay = AudioController.Singleton.CurrentBeatDelay;
		if (Time.time > lastBeatTime + delay)
		{
			lastBeatTime = Time.time;

			if (Game.Singleton.hasStarted)
			{
				SpawnBeat();
			}
		}
	}

	public void SpawnBeat()
	{		
		//var clone = Instantiate(beatHitPrefab, this.transform.position, this.transform.rotation);
		var clone = SimplePool.Spawn(beatHitPrefab, this.transform.position, this.transform.rotation);
		clone.transform.SetParent(this.transform);
		var beat = clone.GetComponent<BeatBehaviour>();
		beat.target = this.target;
		beat.invokeBeatEvent = invokeBeatEvent;
		//beat.OnWillDestroy = () =>
		//{
		//	SimplePool.Despawn(,);
		//};

		//clone.transform.DOMove(target.position, AudioController.Singleton.CurrentBeatDelay * 4).SetEase(Ease.Linear).OnComplete(() => {
		//	Destroy(clone);
		//});
	}
}
