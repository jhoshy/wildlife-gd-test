using UnityEngine;
using UnityEngine.SceneManagement;

public class Fade : MonoBehaviour {
    public static Fade Singleton;

    private Texture2D texturea;
	public Color color;

    private static float fadeSpeed = 1.0f;
    private static int fadeDir = -1;

    private int drawDepth = -1000;
    private static float alpha = 1;

	public static void Out(float speed)
	{
		alpha = 1;		
		fadeDir = -1;
		Fade.fadeSpeed = speed;
	}
	public static void In(float speed)
	{
		alpha = 0;
		fadeDir = 1;
		Fade.fadeSpeed = speed;
	}

	void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
		Out(1f);
    }

    void Awake() {
        Singleton = this;
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
		texturea = Texture2D.whiteTexture;
    }
	private void OnDestroy()
	{
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	}

	void OnGUI() {
        alpha += fadeDir * fadeSpeed * Time.deltaTime;
        alpha = Mathf.Clamp01(alpha);

        GUI.color = new Color(color.r, color.g, color.b, alpha);
        GUI.depth = drawDepth;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texturea);
    }

#if UNITY_EDITOR
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F1))
		{
			In(0.5f);
		}
		if (Input.GetKeyDown(KeyCode.F2))
		{
			Out(0.5f);
		}
	}
#endif
}