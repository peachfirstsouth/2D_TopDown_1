using UnityEngine;
using TMPro;
using DG.Tweening;

public class DamageIndicator : MonoBehaviour
{
    private TextMeshProUGUI textMesh;

    void Awake()
    {
        textMesh = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Setup(int damageAmount)
    {
        if(damageAmount > 800) {textMesh.color = Color.red; textMesh.fontSize = 60f;}
        else {textMesh.color = Color.white; textMesh.fontSize = 36f;}
        textMesh.text = damageAmount.ToString();

        transform.DOMoveY(transform.position.y + 1f, 0.5f).SetEase(Ease.OutQuad);

        transform.DOPunchScale(new Vector3(0.3f, 0.3f, 0.3f), 0.2f);

        textMesh.DOFade(0f, 0.5f)
            .SetDelay(0.2f)
            .OnComplete(() => Destroy(gameObject));
    }
}