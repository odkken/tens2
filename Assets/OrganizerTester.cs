using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Code.MonoBehavior.GameSpecific.Tens;
using Assets.Scripts.Card;
using Assets.Scripts.Initialization;
using Assets.Scripts.Player;

public class OrganizerTester : MonoBehaviour
{
    public GameObject CardPrefab;

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
        var organizer = new CardOrganizer(Position.West, true, 1, .5f);
        organizer.OrganizeHandCards(hands[0]);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
