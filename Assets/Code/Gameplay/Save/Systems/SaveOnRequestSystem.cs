using Code.Common.Components;
using Code.Gameplay.Business.Components;
using Code.Gameplay.Hero.Components;
using Code.Gameplay.Money.Components;
using Code.Gameplay.Save.Components;
using Code.Gameplay.Save.Models;
using Leopotam.EcsLite;

namespace Code.Gameplay.Save.Systems
{
    public class SaveOnRequestSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly ISaveService _saveService;
        private EcsWorld _world;
        
        private EcsFilter _heroFilter;
        private EcsFilter _businessFilter;
        private EcsFilter _saveRequestFilter;
        
        private EcsPool<MoneyComponent> _moneyPool;
        private EcsPool<LevelComponent> _levelPool;
        private EcsPool<BusinessIdComponent> _businessIdPool;
        private EcsPool<ProgressComponent> _progressPool;
        private EcsPool<IncomeСooldownLeftComponent> _cooldownLeftPool;
        private EcsPool<IncomeComponent> _incomePool;
        private EcsPool<LevelUpPriceComponent> _levelUpPricePool;
        private EcsPool<PurchasedComponent> _purchasedPool;
        private EcsPool<IncomeСooldownAvailableComponent> _cooldownAvailablePool;
        private EcsPool<UpdateBusinessModifiersComponent> _updateBusinessModifiersPool;
        private EcsPool<IncomeСooldownComponent> _cooldownPool;

        public SaveOnRequestSystem(ISaveService saveService)
        {
            _saveService = saveService;
        }

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            
            _heroFilter = _world.Filter<HeroComponent>().End();
            _businessFilter = _world.Filter<BusinessIdComponent>().End();
            _saveRequestFilter = _world.Filter<SaveRequestComponent>().End();
            
            _moneyPool = _world.GetPool<MoneyComponent>();
            _levelPool = _world.GetPool<LevelComponent>();
            _businessIdPool = _world.GetPool<BusinessIdComponent>();
            _progressPool = _world.GetPool<ProgressComponent>();
            _cooldownLeftPool = _world.GetPool<IncomeСooldownLeftComponent>();
            _incomePool = _world.GetPool<IncomeComponent>();
            _levelUpPricePool = _world.GetPool<LevelUpPriceComponent>();
            _purchasedPool = _world.GetPool<PurchasedComponent>();
            _cooldownAvailablePool = _world.GetPool<IncomeСooldownAvailableComponent>();
            _updateBusinessModifiersPool = _world.GetPool<UpdateBusinessModifiersComponent>();
            _cooldownPool = _world.GetPool<IncomeСooldownComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var request in _saveRequestFilter)
            {
                SaveGame();
                _world.DelEntity(request);
            }
        }

        private void SaveGame()
        {
            var saveData = new GameSaveModel
            {
                Hero = new HeroSaveModel()
            };

            foreach (var hero in _heroFilter)
            {
                saveData.Hero.Money = _moneyPool.Get(hero).Value;
            }

            foreach (var business in _businessFilter)
            {
                float totalCooldown = _cooldownPool.Get(business).Value;
                float currentCooldown = _cooldownLeftPool.Get(business).Value;
                float progress = 1f - (currentCooldown / totalCooldown);

                var businessSave = new BusinessSaveModel
                {
                    Id = _businessIdPool.Get(business).Value,
                    Level = _levelPool.Get(business).Value,
                    Progress = progress,
                    Cooldown = currentCooldown,
                    Income = _incomePool.Get(business).Value,
                    LevelUpPrice = _levelUpPricePool.Get(business).Value,
                    IsPurchased = _purchasedPool.Has(business) && _purchasedPool.Get(business).Value,
                    IsCooldownAvailable = _cooldownAvailablePool.Has(business) && _cooldownAvailablePool.Get(business).Value
                };

                if (_updateBusinessModifiersPool.Has(business))
                {
                    var upgradeDatas = _updateBusinessModifiersPool.Get(business).Value;
                    for (int i = 0; i < upgradeDatas.Count; i++)
                    {
                        businessSave.Upgrades.Add(new UpgradeSaveModel
                        {
                            Id = i,
                            IsPurchased = upgradeDatas[i].Purchased
                        });
                    }
                }

                saveData.Businesses.Add(businessSave);
            }

            _saveService.SaveGame(saveData);
        }
    }
} 