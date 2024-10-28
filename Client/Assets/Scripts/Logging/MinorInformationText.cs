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

    private void UpdateInformationText(string text)
    {
        if (informationText != null)
        {
            informationText.text = text;
            StartCoroutine(ClearTextAfterDelay(5f));
        }
    }

    private IEnumerator ClearTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        informationText.text = "";
    }
}