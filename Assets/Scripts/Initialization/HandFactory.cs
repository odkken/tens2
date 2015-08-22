using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Card;
using Assets.Scripts.GameLogic;
using Assets.Scripts.Player;
using UnityEngine;
using Random = System.Random;

namespace Assets.Scripts.Initialization
{
    public class HandFactory : IHandFactory
    {
        private readonly IDeckFactory _deckFactory;
        private readonly List<int> _shuffleSeeds;
        private Random rng;
        public HandFactory(IDeckFactory deckFactory, List<int> shuffleSeeds)
        {
            _deckFactory = deckFactory;
            _shuffleSeeds = shuffleSeeds;
            rng = new Random(shuffleSeeds[0]);
        }

        public IHand GetNewHand(List<IPlayer> players, int firstPlayerId, Dictionary<int, int> previousScores)
        {
            var deck = _deckFactory.GetTensDeck(_shuffleSeeds.Shuffle(rng).ToList());
            return new SimpleHand(new RoundFactory(), players, deck, previousScores, new BidManagerFactory(), firstPlayerId);
        }
    }
}