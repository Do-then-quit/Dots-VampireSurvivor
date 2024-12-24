using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public struct PokerCard
{
    public int Number; // 1 ~ 13
    public Suit Suit;  // 문양 (Spades, Hearts, Diamonds, Clubs)
    //public bool IsCalculated;

    public PokerCard(int number, string suitString)
    {
        Number = number;
        //IsCalculated = false;
        // "spades", "hearts", "diamonds", "clubs"
        switch (suitString)
        {
            case "spades":
                Suit = Suit.Spades;
                break;
            case "hearts":
                Suit = Suit.Hearts;
                break;
            case "diamonds":
                Suit = Suit.Diamonds;
                break;
            case "clubs":
                Suit = Suit.Clubs;
                break;
            default:
                Suit = Suit.Clubs;
                break;
        }
    }
}

public enum Suit
{
    Spades,
    Hearts,
    Diamonds,
    Clubs
}
public enum PokerHandType
{
    HighCard,
    OnePair,
    TwoPair,
    ThreeOfAKind,
    Straight,
    Flush,
    FullHouse,
    FourOfAKind,
    StraightFlush
}

public class PokerHandEvaluator
{
    public static PokerHandType EvaluateHand(List<PokerCard> hand)
    {
        // TODO : Later Think how to deal with no card.
        if (hand.Count() == 0) return PokerHandType.HighCard;
        
        var ranks = hand.Select(card => card.Number).OrderBy(rank => rank).ToList();
        var suits = hand.Select(card => card.Suit).ToList();

        bool isFlush = hand.Count() == 5 && 
                       suits.Distinct().Count() == 1;
        bool isStraight =  hand.Count() == 5 && 
                           ranks.Distinct().Count() == 5 &&
                           (ranks.Max() - ranks.Min() == 4 || 
                            ranks.SequenceEqual(new List<int> { 1, 10, 11, 12, 13 })); // Ace-high straight

        var rankGroups = ranks.GroupBy(rank => rank).ToDictionary(g => g.Key, g => g.Count());

        if (isStraight && isFlush)
            return PokerHandType.StraightFlush;
        if (rankGroups.ContainsValue(4))
            return PokerHandType.FourOfAKind;
        if (rankGroups.ContainsValue(3) && rankGroups.ContainsValue(2))
            return PokerHandType.FullHouse;
        if (isFlush)
            return PokerHandType.Flush;
        if (isStraight)
            return PokerHandType.Straight;
        if (rankGroups.ContainsValue(3))
            return PokerHandType.ThreeOfAKind;
        if (rankGroups.Count(kv => kv.Value == 2) == 2)
            return PokerHandType.TwoPair;
        if (rankGroups.ContainsValue(2))
            return PokerHandType.OnePair;

        return PokerHandType.HighCard;
    }
}