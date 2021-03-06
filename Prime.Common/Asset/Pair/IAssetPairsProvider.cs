using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IAssetPairsProvider : IDescribesAssets
    {
        Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context);
    }
}