using Code.Gameplay.Money;
using UniRx;

namespace Code.UI.Money
{
    public class CurrencyScreenModel
    {
        private readonly CurrencyModel _currencyModel;

        public IReadOnlyReactiveProperty<int> Currency => _currencyModel.Money;

        public CurrencyScreenModel(CurrencyModel currencyModel)
        {
            _currencyModel = currencyModel;
        }
    }
}