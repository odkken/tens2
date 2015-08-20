using System.Collections.Generic;
using Assets.Scripts.Player;

namespace Assets.Scripts.Initialization
{
    public interface IPlayerFactory
    {
        List<IPlayer> CreateTwoPlayers(IDeck deck);
        List<IPlayer> CreateFourPlayers(IDeck deck);
    }
}