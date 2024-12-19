using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEngine;


[System.Serializable]
public class CardData
{
    public int Value; // 1~13 (A~K)
    public string Suit; // "spades", "hearts", "diamonds", "clubs"
    public bool IsJoker;

    public CardData(int value, string suit, bool isJoker = false)
    {
        Value = value;
        Suit = suit;
        IsJoker = isJoker;
    }
}

public class CardItem : ItemBase
{
    public CardData cardData;
    private SpriteRenderer _spriteRenderer;

    protected override IEnumerator Start()
    {
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        UpdateVisuals();
        return base.Start();
    }

    public void Initialize(CardData data)
    {
        cardData = data;
        UpdateVisuals();
    }
    
    private void UpdateVisuals()
    {
        if (cardData.IsJoker)
        {
            _spriteRenderer.sprite = Resources.Load<Sprite>("Cards (large)/card_joker_red");
        }
        else
        {
            string cardValueString = "";
            switch (cardData.Value)
            {
                case 1:
                    cardValueString = "A";
                    break;
                case 11:
                    cardValueString = "J";
                    break;
                case 12:
                    cardValueString = "Q";
                    break;
                case 13:
                    cardValueString = "K";
                    break;
                case 10:
                    cardValueString = cardData.Value.ToString();
                    break;
                default:
                    cardValueString = "0" + cardData.Value.ToString();
                    break;
            }
            string texturePath = $"Cards (large)/card_{cardData.Suit}_{cardValueString}";
            _spriteRenderer.sprite = Resources.Load<Sprite>(texturePath);
        }
    }
    protected override void OnItemCollected()
    {
        //Debug.Log("Card Collected");
        var playerHand = WorldEntityManager.GetComponentData<PlayerHand>(PlayerEntity);
        if (playerHand.Cards.Length >= 5)
        {
            Debug.Log("Full Hand");
        }
        else
        {
            var newPokerCard = new PokerCard(cardData.Value, cardData.Suit);
            playerHand.Cards.Add(newPokerCard);
            
            List<PokerCard> pokerCardsList = new List<PokerCard>();
            for (int i = 0; i < playerHand.Cards.Length; i++)
            {
                pokerCardsList.Add(playerHand.Cards[i]);
            }
            
            var debughandtype = PokerHandEvaluator.EvaluateHand(pokerCardsList);
            Debug.Log(debughandtype);
            playerHand.HandType = PokerHandEvaluator.EvaluateHand(pokerCardsList);
            
            WorldEntityManager.SetComponentData(PlayerEntity, playerHand);

        }
    }
}
