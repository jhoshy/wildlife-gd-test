using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelExit : MonoBehaviour
{
	public GameObject portal;

    void Start()
    {
		portal.gameObject.SetActive(false);

		Game.Singleton.OnLevelComplete += ShowLevelExit;
    }

	private void OnDestroy()
	{
		Game.Singleton.OnLevelComplete -= ShowLevelExit;
	}

	public void ShowLevelExit()
	{
		portal.gameObject.SetActive(true);		
	}

	private void OnTriggerEnter(Collider other)
	{
		Player.Singleton.isDead = true;
		Game.Singleton.GoToNextLevel();
	}
}