using TMPro;
using UniRx;
using UnityEngine;

namespace Code.UI.Money
{
    public class MoneyView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;

        public void Initialize(CurrencyScreenModel currencyScreenModel)
        {
            currencyScreenModel
                .Currency
                .Subscribe(value => _text.text = $"Баланс: {value}$")
                .AddTo(this);
        }
    }
}