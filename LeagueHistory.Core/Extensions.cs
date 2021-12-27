using System.Runtime.CompilerServices;
using LeagueHistory.Core.Enums;

namespace LeagueHistory.Core
{
    public static class Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Platform ToPlatform(this Region region)
            => (Platform) region;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Region ToRegion(this Platform region)
            => (Region) region;
    }
}