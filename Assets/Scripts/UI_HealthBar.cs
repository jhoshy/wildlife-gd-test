using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UI_HealthBar : MonoBehaviour
{
	public static UI_HealthBar Singleton;

	public Image currentHealth;
	public Image delayHealth;

	public Gradient healthGradient;
	public Gradient delayGradient;

	//public GameObject group;

	public float delay;

	private void Awake()
	{
		Singleton = this;
		
	}
	public void Start()
	{
		Player.Singleton.OnDamageTaken += UpdateHealth;		
	}

	private void OnDestroy()
	{
		Player.Singleton.OnDamageTaken -= UpdateHealth;
	}

	public void UpdateHealth(float value)
	{
		currentHealth.fillAmount = value;

		currentHealth.DOKill();
		currentHealth.color = Color.white;
		currentHealth.DOColor(healthGradient.Evaluate(value), 0.3f);
		//currentHealth.color = healthGradient.Evaluate(value);

		delayHealth.color = delayGradient.Evaluate(value);

		delayHealth.DOKill();
		delayHealth.DOFillAmount(value, delay);

		//group.SetActive(value > 0);
	}

	//public void SpawnDamageFloatingNumber(float value)
	//{
	//	var clone = Instantiate(damageTextTemplate.gameObject, damageTextTemplate.transform.position + Vector3.up * 130f, damageTextTemplate.transform.rotation);
	//	clone.transform.SetParent(UI_GameplayHUD.Singleton.transform);
	//	clone.SetActive(true);
	//	var text = clone.GetComponent<Text>();
	//	clone.transform.DOMoveY(clone.transform.position.y + 50f, 0.4f).SetEase(Ease.InFlash);
	//	text.DOFade(0, 0.25f).SetDelay(0.2f).OnComplete(()=>{
	//		Destroy(clone);
	//	});
	//	text.text = value.ToString();
	//
	//	if (target.IsDead)
	//	{
	//		text.fontSize = 43;
	//		text.color = Color.red;
	//		//mudar cor e pa
	//	}
	//}

	//public static void CreateHealthBar(HealthStatus target)
	//{
	//	var clone = Instantiate(UI_GameplayHUD.Singleton.healthBarPrefab, UI_GameplayHUD.Singleton.transform);
	//	var uiHealth = clone.GetComponent<UI_HealthBar>();
	//	uiHealth.SetTarget(target);
	//}
}