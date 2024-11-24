using UnityEngine;
using UnityEngine.UI;

public class MinorInformationText : MonoBehaviour
{
    [SerializeField] private Text informationText;

    private float clearTextTimer = 0f;
    private float clearTextDelay = 0f;
    private bool isClearTimerActive = false;

    private void OnEnable()
    {
        ServiceLocator.Services.EventAggregator.MinorInformationTextReceived += UpdateInformationText;
        informationText.text = "";
    }

    private void OnDisable()
    {
        ServiceLocator.Services.EventAggregator.MinorInformationTextReceived -= UpdateInformationText;
    }

    private void Update()
    {
        if (isClearTimerActive)
        {
            clearTextTimer += Time.deltaTime;

            if (clearTextTimer >= clearTextDelay)
            {
                informationText.text = "";
                clearTextTimer = 0f;
                isClearTimerActive = false;
            }
        }
    }

    private void UpdateInformationText(string text, float? delay)
    {
        if (informationText != null)
        {
            informationText.text = text;

            if (delay.HasValue)
            {
                clearTextDelay = delay.Value;
                clearTextTimer = 0f;
                isClearTimerActive = true;
            }
            else
            {
                isClearTimerActive = false;
            }
        }
    }
}