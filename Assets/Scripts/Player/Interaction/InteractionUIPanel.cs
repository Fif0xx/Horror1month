using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionUIPanel : MonoBehaviour
{
    [SerializeField] private Image progressBar;
    [SerializeField] private TextMeshProUGUI tooltipText;

    private void Awake()
    {
        progressBar.gameObject.SetActive(true);
    }

    public void SetToolTip(string tooltip)
    {
        tooltipText.SetText(tooltip);
    }

    public void UpdateProgressBar(float fillAmount)
    {
        progressBar.fillAmount = fillAmount;
    }

    public void ResetUI()
    {
        progressBar.fillAmount = 0;
        tooltipText.SetText("");
    }
}
