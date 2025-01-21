using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissedBeadFeedback : MonoBehaviour
{

	public Transform myText;
	public Vector3 offset;

	private Camera camera;
    void Start()
    {
		camera = Camera.main;

		Game.Singleton.OnMissedBeat += HandleMissedBeat;
	}

	private void HandleMissedBeat()
	{
		myText.DOKill();
		myText.gameObject.SetActive(true);

		myText.position = camera.WorldToScreenPoint(Player.Singleton.transform.position + offset, Camera.MonoOrStereoscopicEye.Mono);
		myText.DOMove(myText.position + Vector3.up * 100f, 0.8f).OnComplete(()=>{
			myText.gameObject.SetActive(false);
		});
		//myText.DOMove(myText.position + Vector3.up * 100f, 0.8f);

	}
	private void OnDestroy()
	{
		Game.Singleton.OnMissedBeat -= HandleMissedBeat;
	}
}
