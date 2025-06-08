using UniRx;

namespace Code.Gameplay.Money
{
    public class CurrencyModel
    {
        private readonly ReactiveProperty<int> _money;

        public IReactiveProperty<int> Money => _money;

        public CurrencyModel(int value = 0)
        {
            _money = new ReactiveProperty<int>(value);
        }
    }
}