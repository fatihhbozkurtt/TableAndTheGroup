using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Controllers
{
    public class CounterPanel : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private TextMeshProUGUI counterText;

        [SerializeField] private GameObject image;

        [Header("Debug")] [SerializeField] private bool counterStarted;
        [SerializeField] private float currentTime;
        private const float Duration = 5f;
        private MobController _parentMob;

        private void Start()
        {
            _parentMob = transform.parent.GetComponent<MobController>();
        }

        public void StartCounter()
        {
            transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBounce);
            image.transform.forward = Camera.main.transform.forward;
            currentTime = Duration;
            counterText.text = Mathf.FloorToInt(currentTime).ToString();
            counterStarted = true;
        }

        private void Update()
        {
            if (!counterStarted) return;

            currentTime -= Time.deltaTime;

            if (currentTime <= 0)
            {
                currentTime = 0;
                counterStarted = false;
                EndRoutine();
                return;
            }

            counterText.text = currentTime.ToString("F1");
            // counterText.text = Mathf.FloorToInt(currentTime).ToString();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private async UniTask EndRoutine()
        {
            counterText.text = "GO!";
            await UniTask.Delay(125);
            _parentMob.OnCounterFinished();
            await UniTask.Delay(750);
            transform.DOScale(Vector3.zero, 0.25f)
                .OnComplete(() => Destroy(gameObject));
        }
    }
}