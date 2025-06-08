using Code.Configs;
using Code.UI.Business;

namespace Code.Common.Services
{
    public interface IStaticDataService
    {
        BusinessConfig GetBusinessConfig();
        void LoadAll();
        BusinessUpgradeNamesConfig GetBusinessUpgradeNamesConfig();
        BusinessView GetBusinessView { get; }
    }
} 