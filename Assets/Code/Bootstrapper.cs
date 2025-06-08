using System.Collections.Generic;
using Code.Common.Services;
using Code.Configs;
using Code.Gameplay.Business;
using Code.Gameplay.Money;
using Code.Systems;
using Code.Systems.Business;
using Code.Systems.Hero;
using Code.Systems.Money;
using Code.UI.Business;
using Code.UI.Money;
using Leopotam.EcsLite;
using UnityEngine;

namespace Code
{
    public class Bootstrapper : MonoBehaviour
    {
        [SerializeField] private Transform _mainUi;
        [SerializeField] private Transform _businessUIParent;
        [SerializeField] private MoneyView _moneyView;

        private EcsWorld _world;
        private IEcsSystems _systems;
        private IIdentifierService _identifierService;

        private void Start()
        {
            _world = new EcsWorld();
            _systems = new EcsSystems(_world);
            _identifierService = new IdentifierService();

            StaticDataService staticData = LoadStaticData();
            BusinessUpgradeNamesConfig businessUpgradeNamesConfig = staticData.GetBusinessUpgradeNamesConfig();
            BusinessConfig businessConfig = staticData.GetBusinessConfig();

            CurrencyModel currencyModel = new CurrencyModel();

            IMoneyService moneyService = new HeroMoneyService(currencyModel, _world);
            moneyService.Initialize();

            BusinessService businessService = new BusinessService(_world, moneyService);
            businessService.Initialize();

            _moneyView.Initialize(new CurrencyScreenModel(currencyModel));

            CreateBusinessViews(businessConfig.GetBusinessDatas(), staticData, businessService,
                businessUpgradeNamesConfig);

            InitSystems(businessService, moneyService, businessUpgradeNamesConfig, businessConfig);
        }

        private void Update()
        {
            _systems?.Run();
        }

        private void OnDestroy()
        {
            _systems?.Destroy();
            _systems = null;
            _world?.Destroy();
            _world = null;
        }

        private void InitSystems(BusinessService businessService, IMoneyService heroMoneyService,
            BusinessUpgradeNamesConfig businessUpgradeNamesConfig, BusinessConfig businessConfig)
        {
            _systems
                .Add(new HeroInitSystem(_identifierService))
                .Add(new BusinessInitSystem(businessUpgradeNamesConfig, _identifierService, businessConfig,businessService))
                .Add(new CalculateIncomeCooldownSystem())
                .Add(new CalculateBusinessProgressSystem(businessService))
                .Add(new CreateMoneyUpdateRequestOnIncomeCooldownUpSystem())
                .Add(new UpdateMoneyOnRequestSystem())
                .Add(new UpdateHeroMoneySystem(heroMoneyService))
                .Add(new UpdateBusinessOnRequestSystem(businessService,heroMoneyService))
                .Add(new CleanupBusinessRequestsSystem())
                .Add(new CleanupMoneyRequestsSystem())
                .Init();
        }

        private void CreateBusinessViews(IReadOnlyList<BusinessData> businessDatas, StaticDataService staticData,
            BusinessService businessService, BusinessUpgradeNamesConfig businessUpgradeNamesConfig)
        {
            int lastBusinessId = 0;

            for (int i = 0; i < businessDatas.Count; i++)
            {
                BusinessView
                    businessView = Instantiate(staticData.GetBusinessView, _businessUIParent); //todo: move to factory

                BusinessData businessData = businessDatas[i];

                List<UpgradeBusinessScreenModel> upgradeBusinessScreenModels =
                    CreateUpgradeScreenModels(businessUpgradeNamesConfig, i, businessData, businessService);

                businessView.Initialize(new BusinessScreenModel(businessService, lastBusinessId++,
                    upgradeBusinessScreenModels));
            }
        }

        private static List<UpgradeBusinessScreenModel> CreateUpgradeScreenModels(
            BusinessUpgradeNamesConfig businessUpgradeNamesConfig, int businessId, BusinessData businessData,
            BusinessService businessService)
        {
            List<UpgradeBusinessScreenModel> upgradeBusinessScreenModels = new List<UpgradeBusinessScreenModel>();
            List<string> upgradeNames = businessUpgradeNamesConfig.BusinessUpgradeNameDatas[businessId].UpgradeNames;

            for (int i = 0; i < businessData.Upgrades.Length; i++)
            {
                string targetName = upgradeNames[i];

                bool purchased = businessData.Upgrades[i].Purchased;

                int cost = businessData.Upgrades[i].Cost;
                float incomeMultiplier = businessData.Upgrades[i].IncomeMultiplier;

                UpgradeBusinessScreenModel upgradeBusinessScreenModel = new(purchased, incomeMultiplier, cost,
                    targetName, i, businessId, businessService);
                upgradeBusinessScreenModels.Add(upgradeBusinessScreenModel);
            }

            return upgradeBusinessScreenModels;
        }

        private static StaticDataService LoadStaticData()
        {
            var staticDataService = new StaticDataService();

            staticDataService.LoadAll();

            return staticDataService;
        }
    }
}