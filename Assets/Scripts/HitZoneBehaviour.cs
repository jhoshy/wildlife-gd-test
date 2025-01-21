using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class HitZoneBehaviour : MonoBehaviour
{
	//public float beatTempo = 120f;
	//
	private float beatDelay = 0.4f;
	private float lastBeatTime;
	private Image image;

	public Material floorMaterial;


	public void Start()
	{
		//beatDelay = beatTempo / 60f;	
		image = GetComponent<Image>();

		beatDelay = AudioController.Singleton.CurrentBeatDelay;

		Game.Singleton.OnPlayerPressedBeat += HitEffect;

		Game.Singleton.beatBackground = this.transform.parent.gameObject;
	}

	//0.05f 0.301f
	bool switchFloor;
	public void Update()
	{
		if (Time.time > lastBeatTime + beatDelay)
		{
			lastBeatTime = Time.time;

			floorMaterial.mainTextureOffset = new Vector2(switchFloor? 0.05f : 0.301f,-0.11f);
			switchFloor = !switchFloor;
			//floorMaterial.mainTextureOffset = new Vector2(0.301f, -0.11f);
		}
	}

	public void HitEffect()
	{
		image.DOKill();
		var color = image.color;
		color.a = 1f;
		image.color = color;
		image.DOFade(0.1f, beatDelay);
	}

	public void OnDestroy()
	{
		Game.Singleton.OnPlayerPressedBeat -= HitEffect;
	}
}