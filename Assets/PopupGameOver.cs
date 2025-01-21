using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PopupGameOver : MonoBehaviour
{
	public static PopupGameOver Singleton;

	public GameObject popupBackground;
	public GameObject popupContainer;
	public Text stageText;
	public Text beatHitText;
	public Slider beatHitSlider;
	public Button closeButton, retryButton, restartButton;

	private void Awake()
	{
		Singleton = this;

		restartButton.onClick.AddListener(Game.Singleton.RetryLevel);
	}

	public void ShowPopup()
	{
		closeButton.onClick.AddListener(Game.Singleton.CloseLevel);
		retryButton.onClick.AddListener(Game.Singleton.RetryLevel);

		popupBackground.SetActive(true);
		popupContainer.SetActive(true);
		var pos = popupContainer.transform.position;
		popupContainer.transform.position = pos + Vector3.up * 1920;
		var moveDelay = 0.5f;
		popupContainer.transform.DOMove(pos, moveDelay);

		stageText.transform.DOPunchScale(Vector3.one * 0.25f, 0.2f).SetDelay(moveDelay);

		beatHitText.text = "0%";
		var totalCount = Game.Singleton.missedBeatCounter + Game.Singleton.rightBeatCounter;
		float percent = (float)Game.Singleton.rightBeatCounter / (float)totalCount;
		Debug.Log("total " + totalCount + " right " + Game.Singleton.rightBeatCounter + " percent " + percent);
		beatHitSlider.value = 0f;
		beatHitSlider.DOValue(percent, 1.5f).SetDelay(moveDelay).OnUpdate(()=> {
			beatHitText.text = (Mathf.Round(beatHitSlider.value * 100)) + "%";
		});

		stageText.text = Game.Singleton.currentLevel.ToString();
	}
}