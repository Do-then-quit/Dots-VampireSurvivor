using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    // 필드에 스폰할 카드 프리팹
    public GameObject cardPrefab;

    // 덱에 있는 카드 리스트 (예: Card는 카드 정보를 담은 클래스)
    public List<CardData> deck;


    // left top spawn position, righr bottom spawn position.
    // let's make map finite then.
    private Vector2 LeftTopSpawnBoxPosition = new Vector2(-140, -70);
    private Vector2 RightBottomSpawnPosition = new Vector2(140, 70);

    // 스폰 위치의 범위
    private Vector2 spawnAreaMin = new Vector2(-140, -70); // 최소 좌표
    private Vector2 spawnAreaMax = new Vector2(140, 70); // 최대 좌표

    // 매 라운드 시작 시 호출
    public void SpawnCardsOnField()
    {
        foreach (var card in deck)
        {
            // 랜덤 위치 계산
            Vector3 spawnPosition = new Vector3(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                Random.Range(spawnAreaMin.y, spawnAreaMax.y),
                0f // 2D 필드라면 Z는 고정
            );
            cardPrefab.GetComponent<CardItem>().cardData = card;
            // 카드 프리팹 생성
            Instantiate(cardPrefab, spawnPosition, Quaternion.identity);
            // 카드 정보 업데이트
            //Debug.Log(spawnedCard);
            //spawnedCard.GetComponent<CardItem>().Initialize(card);

        }
    }
    
    public static List<CardData> CreateStandardDeck()
    {
        List<CardData> tempdeck = new List<CardData>();

        foreach (Suit suit in System.Enum.GetValues(typeof(Suit)))
        {
            for (int value = 1; value <= 13; value++) // 카드 값: 1~13 (Ace~King)
            {
                tempdeck.Add(new CardData(value, suit));
            }
        }

        return tempdeck;
    }
    
    private void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int randomIndex = Random.Range(0, deck.Count);
            (deck[i], deck[randomIndex]) = (deck[randomIndex], deck[i]);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        // create deck.
        deck = CreateStandardDeck();
        ShuffleDeck();
        // spawn cards . 
        SpawnCardsOnField();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
