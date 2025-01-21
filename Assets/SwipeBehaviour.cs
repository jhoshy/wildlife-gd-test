using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeBehaviour : MonoBehaviour
{
	public GameObject handImage;
	public Vector3 offset;
	private bool finishedTutorial;

	void Start()
    {
		handImage.gameObject.SetActive(true);
		handImage.transform.DOMove(this.transform.position + this.transform.right + offset, 1.0f).SetLoops(-1, LoopType.Restart);
	}
    void Update()
    {
		if (!finishedTutorial && Game.Singleton.hasStarted)
		{
			finishedTutorial = true;
			handImage.SetActive(false);
			handImage.transform.DOKill();
		}
    }
}
