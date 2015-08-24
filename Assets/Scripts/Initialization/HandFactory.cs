using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.GameLogic;
using Assets.Scripts.Player;

namespace Assets.Scripts.Initialization
{
    public class HandFactory : IHandFactory
    {
        private readonly IDeckFactory _deckFactory;
        private readonly List<int> _shuffleSeeds;
        private Random _rng;
        public HandFactory(IDeckFactory deckFactory, List<int> shuffleSeeds)
        {
            _deckFactory = deckFactory;
            _shuffleSeeds = shuffleSeeds;
            _rng = new Random(shuffleSeeds[0]);
        }

        public IHand GetNewHand(List<IPlayer> players, int firstPlayerId, int team1PreviousScore, int team2PreviousScore)
        {
            var deck = _deckFactory.GetTensDeck(_shuffleSeeds.Shuffle(_rng).ToList());
            return new SimpleHand(new RoundFactory(), players, deck, team1PreviousScore, team2PreviousScore, new BidManagerFactory(), firstPlayerId);
        }
    }
}