using Code.Extensions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class UpgradeBusinessView : MonoBehaviour
    {
        [SerializeField] private Button _upgradeButton;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _incomeMultiplierText;
        [SerializeField] private TMP_Text _priceText;
        [SerializeField] private TMP_Text _purchasedText;

        public void Initialize(UpgradeBusinessScreenModel model)
        {
            _nameText.text = model.Name.Value;
            _incomeMultiplierText.text = $"Доход: + {model.IncomeMultiplier.Value.ToPercentage()}%";

            model.Purchased
                .Subscribe(SetupAfterPurchasing)
                .AddTo(this);

            model.PurchaseAvailable.CombineLatest(model.Purchased,
                (purchaseAvailable, isPurchased) => purchaseAvailable && !isPurchased)
                .Subscribe(isInteractable => _upgradeButton.interactable = isInteractable)
                .AddTo(this);

            if (_priceText.gameObject.activeSelf)
                _priceText.text = $"Цена: {model.Price}$";

            _upgradeButton.OnClickAsObservable()
                .Subscribe(_ => model.OnUpgradeClicked())
                .AddTo(this);
        }

        private void SetupAfterPurchasing(bool purchased)
        {
            if(purchased)
                _upgradeButton.interactable = false;

            _priceText.gameObject.SetActive(!purchased);
            _purchasedText.gameObject.SetActive(purchased);
        }
    }
}