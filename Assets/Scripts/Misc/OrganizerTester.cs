using System.Collections.Generic;
using Assets.Scripts.Card;
using Assets.Scripts.Initialization;
using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.Misc
{
    public class OrganizerTester : MonoBehaviour
    {
        public GameObject CardPrefab;
        public Position Position;
        public float HandDistance;
        public float HandWidth;
        public bool IsMine;
        public float HandTilt;
        // Use this for initialization
        void Start()
        {
            var deck = new DeckFactory(new MBCardFactory(CardPrefab, false)).GetTensDeck(new List<int> { 0, 1, 2 });
            var hands = new List<List<ICard>>();
            for (int i = 0; i < 4; i++)
            {
                var thisHand = new List<ICard>();
                while (thisHand.Count < 10)
                {
                    thisHand.Add(deck.PopCard());
                }
                hands.Add(thisHand);
            }
            var organizer = new CardOrganizer(IsMine, 1, HandWidth, HandDistance, HandTilt);
            for (int i = 0; i < 4; i++)
            {
                organizer.OrganizeHandCards(hands[i], (Position)i);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
