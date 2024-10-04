using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageFont : MonoBehaviour
{
	private TextMeshPro _damageText;

	public void SetInfo(Vector2 pos, float damage = 0, Transform parent = null, bool isCritical = false)
	{
		_damageText = GetComponent<TextMeshPro>();
		_damageText.sortingOrder = SortingLayers.PROJECTILE;

		transform.position = pos;

		if (damage < 0)
		{
			_damageText.color = Util.HexToColor("4EEE6F");
		}
		else if (isCritical)
		{
			_damageText.color = Util.HexToColor("EFAD00");
		}
		else
		{
			_damageText.color = Color.red;
		}

		_damageText.text = $"{Mathf.Abs(damage)}";
		_damageText.alpha = 1;

		if (parent != null)
			GetComponent<MeshRenderer>().sortingOrder = SortingLayers.DAMAGE_FONT;

		DoAnimation();
	}

	private void DoAnimation()
	{
		Sequence seq = DOTween.Sequence();

		transform.localScale = new Vector3(0, 0, 0);

		seq.Append(transform.DOScale(1.3f, 0.3f).SetEase(Ease.InOutBounce)).
			Join(transform.DOMove(transform.position + Vector3.up, 0.3f).SetEase(Ease.Linear))
			.Append(transform.DOScale(1.0f, 0.3f).SetEase(Ease.InOutBounce))
			.Join(transform.GetComponent<TMP_Text>().DOFade(0, 0.3f).SetEase(Ease.InQuint))
			.OnComplete(() =>
			{
				Managers.Resource.Destroy(gameObject);
			});
	}
}
