using System.Collections.Generic;
using System.Linq;
using Code.Common.Components;
using Code.Common.Services;
using Code.Gameplay.Business.Components;
using Code.Gameplay.Business.Configs;
using Leopotam.EcsLite;

namespace Code.Gameplay.Business.Factory
{
    public class BusinessFactory
    {
        private readonly IIdentifierService _identifierService;
        private readonly EcsWorld _world;

        private readonly EcsPool<BusinessComponent> _businessPool;
        private readonly EcsPool<BusinessIdComponent> _businessIdPool;
        private readonly EcsPool<NameComponent> _namePool;
        private readonly EcsPool<IdComponent> _idPool;
        private readonly EcsPool<OwnerIdComponent> _ownerIdPool;
        private readonly EcsPool<IncomeComponent> _incomePool;
        private readonly EcsPool<BaseIncomeComponent> _baseIncomePool;
        private readonly EcsPool<IncomeСooldownComponent> _incomeCooldownPool;
        private readonly EcsPool<IncomeСooldownLeftComponent> _incomeCooldownLeftPool;
        private readonly EcsPool<IncomeСooldownUpComponent> _incomeCooldownUpPool;
        private readonly EcsPool<IncomeСooldownAvailableComponent> _incomeCooldownAvailablePool;
        private readonly EcsPool<LevelComponent> _levelPool;
        private readonly EcsPool<BaseCostComponent> _baseCostPool;
        private readonly EcsPool<LevelUpPriceComponent> _levelUpPricePool;
        private readonly EcsPool<PurchasedComponent> _purchasedPool;
        private readonly EcsPool<ProgressComponent> _progressPool;
        private readonly EcsPool<UpdateBusinessModifiersComponent> _modifiersPool;
        private readonly EcsPool<TotalIncomeComponent> _totalIncomePool;
        private readonly EcsPool<AccumulatedModifierComponent> _accumulatedModifiersPool;

        public BusinessFactory(EcsWorld world, IIdentifierService identifierService)
        {
            _world = world;
            _identifierService = identifierService;

            _businessPool = world.GetPool<BusinessComponent>();
            _businessIdPool = world.GetPool<BusinessIdComponent>();
            _namePool = world.GetPool<NameComponent>();
            _idPool = world.GetPool<IdComponent>();
            _ownerIdPool = world.GetPool<OwnerIdComponent>();
            _incomePool = world.GetPool<IncomeComponent>();
            _baseIncomePool = world.GetPool<BaseIncomeComponent>();
            _incomeCooldownPool = world.GetPool<IncomeСooldownComponent>();
            _incomeCooldownLeftPool = world.GetPool<IncomeСooldownLeftComponent>();
            _incomeCooldownUpPool = world.GetPool<IncomeСooldownUpComponent>();
            _totalIncomePool = world.GetPool<TotalIncomeComponent>();
            _incomeCooldownAvailablePool = world.GetPool<IncomeСooldownAvailableComponent>();
            _levelPool = world.GetPool<LevelComponent>();
            _baseCostPool = world.GetPool<BaseCostComponent>();
            _levelUpPricePool = world.GetPool<LevelUpPriceComponent>();
            _purchasedPool = world.GetPool<PurchasedComponent>();
            _progressPool = world.GetPool<ProgressComponent>();
            _modifiersPool = world.GetPool<UpdateBusinessModifiersComponent>();
            _accumulatedModifiersPool = world.GetPool<AccumulatedModifierComponent>();
        }

        public int CreateBusiness(BusinessData businessData, BusinessUpgradeNameData businessNameData,
            int businessIndex, int ownerId)
        {
            int entity = _world.NewEntity();

            AddBasicComponents(entity, businessIndex, businessNameData.Name, ownerId);
            AddIncomeComponents(entity, businessData);
            AddLevelComponents(entity, businessData, businessIndex);
            AddProgressComponents(entity);
            AddUpgradeModifiers(entity, businessData);
            AddAccumulatedModifiers(entity);

            return entity;
        }

        private void AddBasicComponents(int entity, int businessIndex, string name, int ownerId)
        {
            ref var business = ref _businessPool.Add(entity);
            business.Value = true;

            ref var businessId = ref _businessIdPool.Add(entity);
            businessId.Value = businessIndex;

            ref var nameComponent = ref _namePool.Add(entity);
            nameComponent.Value = name;

            ref var id = ref _idPool.Add(entity);
            id.Value = _identifierService.Next();

            ref var ownerIdComponent = ref _ownerIdPool.Add(entity);
            ownerIdComponent.Value = ownerId;
        }

        private void AddIncomeComponents(int entity, BusinessData businessData)
        {
            ref var income = ref _incomePool.Add(entity);
            income.Value = businessData.BaseIncome;

            ref var baseIncome = ref _baseIncomePool.Add(entity);
            baseIncome.Value = businessData.BaseIncome;
            
            ref var totalIncome = ref _totalIncomePool.Add(entity);
            totalIncome.Value = 0;

            ref var incomeСooldown = ref _incomeCooldownPool.Add(entity);
            incomeСooldown.Value = businessData.IncomeDelay;

            ref var incomeСooldownLeft = ref _incomeCooldownLeftPool.Add(entity);
            incomeСooldownLeft.Value = businessData.IncomeDelay;

            ref var incomeСooldownUp = ref _incomeCooldownUpPool.Add(entity);
            incomeСooldownUp.Value = false;
        }

        private void AddLevelComponents(int entity, BusinessData businessData, int businessIndex)
        {
            ref var level = ref _levelPool.Add(entity);
            level.Value = businessIndex == 0 ? 1 : 0;

            ref var baseCost = ref _baseCostPool.Add(entity);
            baseCost.Value = businessData.BaseCost;

            ref var levelUpPrice = ref _levelUpPricePool.Add(entity);
            levelUpPrice.Value = (level.Value + 1) * baseCost.Value;

            if (level.Value > 0)
            {
                ref var incomeСooldownAvailable = ref _incomeCooldownAvailablePool.Add(entity);
                incomeСooldownAvailable.Value = true;
            }

            ref var purchased = ref _purchasedPool.Add(entity);
            purchased.Value = level.Value > 0;
        }

        private void AddProgressComponents(int entity)
        {
            ref var progress = ref _progressPool.Add(entity);
            progress.Value = 0f;
        }

        private void AddUpgradeModifiers(int entity, BusinessData businessData)
        {
            ref var modifiers = ref _modifiersPool.Add(entity);
            modifiers.Value = new List<UpgradeData>(businessData.Upgrades.ToList());
        }

        private void AddAccumulatedModifiers(int entity)
        {
            ref var accumulatedModifiers = ref _accumulatedModifiersPool.Add(entity);
            accumulatedModifiers.Value = new List<AccumulatedModifiersData>(2); 
        }
    }
}