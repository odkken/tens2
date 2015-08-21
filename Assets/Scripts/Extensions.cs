using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Code.GameSpecific.Tens;
using Assets.Scripts.Player;

namespace Assets.Scripts
{
    public static class Extensions
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng)
        {
            var elements = source.ToArray();
            // Note i > 0 to avoid final pointless iteration
            for (var i = elements.Length - 1; i > 0; i--)
            {
                // Swap element "i" with a random earlier element it (or itself)
                var swapIndex = rng.Next(i + 1);
                var tmp = elements[i];
                elements[i] = elements[swapIndex];
                elements[swapIndex] = tmp;
            }
            // Lazily yield (avoiding aliasing issues etc)
            foreach (var element in elements)
            {
                yield return element;
            }
        }

        public static T Next<T>(this List<T> source, T current)
        {
            var indexOfCurrent = source.IndexOf(current);
            if (indexOfCurrent == source.Count - 1)
                return source[0];
            return source[indexOfCurrent + 1];
        }

        public static IPlayer GetFrom(this List<IPlayer> players, IPlayer fromPlayer, int distance)
        {
            var resultPlayer = fromPlayer;
            for (var i = 0; i < distance; i++)
            {
                resultPlayer = RuleHelpers.GetNextPlayer(resultPlayer.Position, players);
            }
            return resultPlayer;
        }
    }
}