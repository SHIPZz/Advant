using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class BusinessView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _incomeText;
        [SerializeField] private Slider _progressBar;
        [SerializeField] private Button _levelUpButton;
        [SerializeField] private TMP_Text _levelUpPriceText;
        [SerializeField] private List<UpgradeBusinessView> _upgradeBusinessViews;

        public void Initialize(BusinessScreenModel model)
        {
            model.Level
                .Subscribe(level => _levelText.text = $"LVL: \n{level}" )
                .AddTo(this);

            model.Name
                .Subscribe(targetName => _nameText.text = targetName)
                .AddTo(this);

            model.Progress
                .Subscribe(progress => _progressBar.value = progress)
                .AddTo(this);

            _levelUpButton.OnClickAsObservable()
                .Subscribe(_ => model.OnLevelUpButtonClicked())
                .AddTo(this);

            model.LevelUpPrice
                .Subscribe(price => _levelUpPriceText.text = $"LVL UP: \n{price}$")
                .AddTo(this);

            model.Income
                .Subscribe(income => _incomeText.text = $"ДОХОД: \n{income}$")
                .AddTo(this);

            InitUpgradeViews(model);
        }

        private void InitUpgradeViews(BusinessScreenModel model)
        {
            for (int i = 0; i < _upgradeBusinessViews.Count; i++)
            {
                UpgradeBusinessScreenModel upgradeBusinessScreenModel = model.UpgradeBusinessScreenModels[i];

                _upgradeBusinessViews[i].Initialize(upgradeBusinessScreenModel);
            }
        }
    }
}