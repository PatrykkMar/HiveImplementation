using UnityEngine;
using UnityEngine.UI;

public class InformationText : MonoBehaviour
{
    [SerializeField] private Text informationText;

    private void OnEnable()
    {
        ServiceLocator.Services.EventAggregator.InformationTextReceived += UpdateInformationText;
    }

    private void OnDisable()
    {
        ServiceLocator.Services.EventAggregator.InformationTextReceived -= UpdateInformationText;
    }

    private void UpdateInformationText(string text)
    {
        informationText.text = text;
    }
}