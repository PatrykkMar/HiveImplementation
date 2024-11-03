using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MinorInformationText : MonoBehaviour
{
    [SerializeField] private Text informationText;

    private void OnEnable()
    {
        ServiceLocator.Services.EventAggregator.MinorInformationTextReceived += UpdateInformationText;
        informationText.text = "";
    }

    private void OnDisable()
    {
        ServiceLocator.Services.EventAggregator.MinorInformationTextReceived -= UpdateInformationText;
    }

    private void UpdateInformationText(string text, float? delay)
    {
        if (informationText != null)
        {
            informationText.text = text;
            if(delay.HasValue)
                StartCoroutine(ClearTextAfterDelay(delay.Value));
        }
    }

    private IEnumerator ClearTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        informationText.text = "";
    }
}