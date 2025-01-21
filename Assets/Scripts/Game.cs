using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
	public static Game Singleton;

	public int currentLevel {
		get
		{
			var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
			var nextNumber = int.Parse(activeScene.name.Substring(5, 1));
			return nextNumber;
		}
	}

	public bool hasStarted;

	public Action OnPlayerPressedBeat;

	public Action OnRightBeat;
	public Action OnMissedBeat;

	public Action OnLevelComplete;

	public int rightBeatCounter;
	public int missedBeatCounter;

	public List<Follower> enemies = new List<Follower>();
	public bool IsLevelComplete => enemies.Count == 0;

	[Header("References")]
	public GameObject beatBackground;

	[Header("Movement Animations")]
	public float moveDuration = 0.17f;
	public float jumpPower = 0.8f;
	public float landingDuration = 0.2f;
	public float landingDelay = 0.63f;
	public float landingForce = 0.1f;

	public float entryHitAcceptanceOffset = 0.87f;
	public float exitHitDelayOffset = 0.2f;

	private void Awake()
	{
		Singleton = this;

		rightBeatCounter = 0;
		missedBeatCounter = 0;

		hasStarted = false;

		OnRightBeat += AddRightBeat;
		OnMissedBeat += AddMissedBeat;
	}

	void Start()
	{
		Player.Singleton.OnPlayerDied += HideBGAndFadeMusic;
		Player.Singleton.OnPlayerDied += ShowGameOverScreen;
		var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();		
	}

	private void OnDestroy()
	{
		OnRightBeat -= AddRightBeat;
		OnMissedBeat -= AddMissedBeat;
		Player.Singleton.OnPlayerDied -= HideBGAndFadeMusic;
		Player.Singleton.OnPlayerDied -= ShowGameOverScreen;
	}

	public void GoToNextLevel()
	{		
		var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
		var nextNumber = int.Parse(activeScene.name.Substring(5, 1)) + 1;

		if (nextNumber < 7)
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene("Level" + nextNumber);
		}
		else
		{
			ShowGameOverScreen();
		}		
	}

	public void CloseLevel()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
	}

	public void RetryLevel()
	{
		var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
		UnityEngine.SceneManagement.SceneManager.LoadScene(activeScene.name);
	}

	public void AddRightBeat()
	{
		rightBeatCounter++;
	}
	public void AddMissedBeat()
	{
		missedBeatCounter++;
	}

	public static Vector3 SnapPosition(Vector3 position)
	{
		//var modX = (position.x % 2);
		//var modY = (position.y % 2);
		//var modZ = (position.z % 2);
		//position.x -= modX;
		//position.y -= modY;
		//position.z -= modZ;
		var scale = 2f;
		float x = Mathf.Round(position.x / scale) * scale;
		float z = Mathf.Round(position.z / scale) * scale;
		position = new Vector3(x, 0, z);

		return position; //.Round(2);
	}

	private bool beatBgIsHidden;

	private bool levelCompleteWasCalled;
	private void Update()
	{
		if (!levelCompleteWasCalled && IsLevelComplete)
		{
			levelCompleteWasCalled = true;
			OnLevelComplete?.Invoke();
		}

		if (!beatBgIsHidden && IsLevelComplete)
		{
			HideBGAndFadeMusic();
		}
	}

	public void ShowGameOverScreen()
	{
		StartCoroutine(Routine());
		IEnumerator Routine()
		{
			yield return new WaitForSeconds(0.75f);
			PopupGameOver.Singleton.ShowPopup();
		}
	}

	public void HideBGAndFadeMusic()
	{
		beatBgIsHidden = true;
		foreach (Transform item in beatBackground.transform)
		{
			item.gameObject.SetActive(false);
		}
		beatBackground.transform.DOMove(beatBackground.transform.position + Vector3.up * 300f, 2f);
		AudioController.Singleton.source.DOFade(0.2f, 2f);
	}
}

static class ExtensionMethods
{
	/// <summary>
	/// Rounds Vector3.
	/// </summary>
	/// <param name="vector3"></param>
	/// <param name="decimalPlaces"></param>
	/// <returns></returns>
	public static Vector3 Round(this Vector3 vector3, float multiplier, int decimalPlaces = 2)
	{
		//float multiplier = 1;
		for (int i = 0; i < decimalPlaces; i++)
		{
			multiplier *= 10f;
		}
		return new Vector3(
			Mathf.Round(vector3.x * multiplier) / multiplier,
			Mathf.Round(vector3.y * multiplier) / multiplier,
			Mathf.Round(vector3.z * multiplier) / multiplier);
	}
}

public class MathParabola
{
	public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t, bool zAxisHeight = false)
	{
		Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

		var mid = Vector3.Lerp(start, end, t);

		if (zAxisHeight)
			return new Vector3(mid.x, mid.y, f(t) + Mathf.Lerp(start.z, end.z, t));
		else
			return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
	}

	public static Vector2 Parabola(Vector2 start, Vector2 end, float height, float t)
	{
		Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

		var mid = Vector2.Lerp(start, end, t);

		return new Vector2(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t));
	}

	public static Vector3 SampleParabola(Vector3 start, Vector3 end, float height, float t)
	{
		float parabolicT = t * 2 - 1;
		if (Mathf.Abs(start.y - end.y) < 0.1f)
		{
			//start and end are roughly level, pretend they are - simpler solution with less steps
			Vector3 travelDirection = end - start;
			Vector3 result = start + t * travelDirection;
			result.y += (-parabolicT * parabolicT + 1) * height;
			return result;
		}
		else
		{
			//start and end are not level, gets more complicated
			Vector3 travelDirection = end - start;
			Vector3 levelDirecteion = end - new Vector3(start.x, end.y, start.z);
			Vector3 right = Vector3.Cross(travelDirection, levelDirecteion);
			Vector3 up = Vector3.Cross(right, travelDirection);
			if (end.y > start.y) up = -up;
			Vector3 result = start + t * travelDirection;
			result += ((-parabolicT * parabolicT + 1) * height) * up.normalized;
			return result;
		}
	}
	public static Vector3 SampleParabola(Vector3 start, Vector3 end, float height, float t, bool zAxisHeight = true)
	{
		float parabolicT = t * 2 - 1;
		if (Mathf.Abs(start.z - end.z) < 0.1f)
		{
			//start and end are roughly level, pretend they are - simpler solution with less steps
			Vector3 travelDirection = end - start;
			Vector3 result = start + t * travelDirection;
			result.z += (-parabolicT * parabolicT + 1) * height;
			return result;
		}
		else
		{
			//start and end are not level, gets more complicated
			Vector3 travelDirection = end - start;
			Vector3 levelDirecteion = end - new Vector3(start.x, start.y, end.z);
			Vector3 right = Vector3.Cross(travelDirection, levelDirecteion);
			Vector3 up = Vector3.Cross(right, travelDirection);
			if (end.z > start.z) up = -up;
			Vector3 result = start + t * travelDirection;
			result += ((-parabolicT * parabolicT + 1) * height) * up.normalized;
			return result;
		}
	}
}