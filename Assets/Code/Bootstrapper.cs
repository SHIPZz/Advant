using System.Collections.Generic;
using Code.Common.Services;
using Code.Components;
using Code.Configs;
using Code.Gameplay.Business;
using Code.Gameplay.Money;
using Code.Systems;
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
        [SerializeField] private MoneyView moneyView;

        private EcsWorld _world;
        private IEcsSystems _systems;
        private IIdentifierService _identifierService;
        private int _lastBusinessId;

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

            BusinessService businessService = new BusinessService(businessConfig, _world, businessUpgradeNamesConfig, moneyService);
            businessService.Initialize();

            moneyView.Initialize(new CurrencyScreenModel(currencyModel));

            int heroEntity = CreateHero();

            CreateBusinessEntitiesFromConfig(businessConfig, heroEntity);
            CreateBusinessViews(businessConfig.GetBusinessDatas(), staticData, businessService,
                businessUpgradeNamesConfig);

            InitSystems(businessService, moneyService);
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

        private void InitSystems(BusinessService businessService, IMoneyService heroMoneyService)
        {
            _systems
                .Add(new CalculateIncomeCooldownSystem())
                .Add(new CalculateBusinessProgressSystem(businessService))
                .Add(new CreateMoneyUpdateRequestOnIncomeCooldownUpSystem(businessService))
                .Add(new UpdateMoneyOnRequestSystem())
                .Add(new UpdateHeroMoneySystem(heroMoneyService))
                .Add(new UpdateBusinessOnRequestSystem())
                .Add(new CleanupBusinessRequestsSystem())
                .Add(new CleanupMoneyRequestsSystem())
                .Init();
        }

        private void CreateBusinessViews(IReadOnlyList<BusinessData> businessDatas, StaticDataService staticData,
            BusinessService businessService, BusinessUpgradeNamesConfig businessUpgradeNamesConfig)
        {
            for (int i = 0; i < businessDatas.Count; i++)
            {
                BusinessView
                    businessView = Instantiate(staticData.GetBusinessView, _businessUIParent); //todo: move to factory

                BusinessData businessData = businessDatas[i];

                List<UpgradeBusinessScreenModel> upgradeBusinessScreenModels =
                    CreateUpgradeScreenModels(businessUpgradeNamesConfig, i, businessData, businessService);

                businessView.Initialize(new BusinessScreenModel(businessService, _lastBusinessId++,
                    upgradeBusinessScreenModels));
            }
        }

        private static List<UpgradeBusinessScreenModel> CreateUpgradeScreenModels(
            BusinessUpgradeNamesConfig businessUpgradeNamesConfig, int businessId, BusinessData businessData,
            BusinessService businessService)
        {
            List<UpgradeBusinessScreenModel> upgradeBusinessScreenModels = new List<UpgradeBusinessScreenModel>();
            var upgradeNames = businessUpgradeNamesConfig.BusinessUpgradeNameDatas[businessId].UpgradeNames;

            for (int i = 0; i < businessData.Upgrades.Length; i++)
            {
                var targetName = upgradeNames[i];

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

        private void CreateBusinessEntitiesFromConfig(BusinessConfig businessConfig, int hero)
        {
            IReadOnlyList<BusinessData> list = businessConfig.GetBusinessDatas();

            EcsPool<IdComponent> idPool = _world.GetPool<IdComponent>();
            int ownerId = idPool.Get(hero).Value;

            for (var index = 0; index < list.Count; index++)
            {
                int business = _world.NewEntity();

                BusinessData businessData = list[index];

                _world.GetPool<BusinessComponent>()
                        .Add(business)
                        .Value = true
                    ;

                if (index == 0)
                    _world.GetPool<PurchasedComponent>()
                        .Add(business)
                        .Value = true;

                _world.GetPool<LevelComponent>()
                    .Add(business)
                    ;

                _world.GetPool<OwnerIdComponent>()
                    .Add(business)
                    .Value = ownerId;

                _world.GetPool<LevelUpPriceComponent>()
                    .Add(business)
                    .Value = 0;

                _world.GetPool<IncomeСooldownComponent>()
                    .Add(business)
                    .Value = businessData.IncomeDelay;

                if (index == 0)
                    _world.GetPool<IncomeСooldownAvailableComponent>()
                        .Add(business)
                        .Value = true;

                _world.GetPool<IncomeСooldownLeftComponent>()
                    .Add(business)
                    .Value = businessData.IncomeDelay;

                _world.GetPool<IncomeComponent>()
                    .Add(business)
                    ;

                _world.GetPool<IncomeСooldownUpComponent>()
                    .Add(business)
                    .Value = false;

                _world.GetPool<BusinessIdComponent>()
                    .Add(business)
                    .Value = index;

                _world.GetPool<IdComponent>()
                    .Add(business)
                    .Value = _identifierService.Next();
            }
        }

        private int CreateHero()
        {
            int hero = _world.NewEntity();

            _world.GetPool<HeroComponent>()
                .Add(hero)
                .Value = true;

            _world.GetPool<MoneyComponent>()
                .Add(hero)
                .Value = 10000;

            _world.GetPool<IdComponent>()
                .Add(hero)
                .Value = _identifierService.Next();

            return hero;
        }
    }
}