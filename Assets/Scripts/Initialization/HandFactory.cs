using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Card;
using Assets.Scripts.GameLogic;
using Assets.Scripts.Player;

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
            
            foreach (var player in players)
            {
                var theirCards = new List<ICard>();
                while (theirCards.Count < 10)
                {
                    theirCards.Add(deck.PopCard());
                }
                player.GiveCards(theirCards);
            }

            return new SimpleHand(new RoundFactory(), players, previousScores, new BidManagerFactory(), firstPlayerId);
        }
    }
}