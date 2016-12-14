using System.Collections;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Gamelogic.Chat
{
    [EngineType(EnginePlatform.Client)]
    public class NotificationController : MonoBehaviour
    {
        private Coroutine timeoutCoroutine;

        [SerializeField] private Canvas baseCanvas;
        [SerializeField] private Text notificationText;

        public void ShowNotification(string text)
        {
            CancelTimeout();

            notificationText.text = text;
            baseCanvas.gameObject.SetActive(true);

            timeoutCoroutine = StartCoroutine(TimeoutNotification(2.0f));
        }

        public void HideNotification()
        {
            CancelTimeout();
            baseCanvas.gameObject.SetActive(false);
            notificationText.text = "";
        }

        private void CancelTimeout()
        {
            if (timeoutCoroutine != null)
            {
                StopCoroutine(timeoutCoroutine);
                timeoutCoroutine = null;
            }
        }

        private IEnumerator TimeoutNotification(float duration)
        {
            yield return new WaitForSeconds(duration);
            HideNotification();
        }
    }
}