using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour {

    public List<Transform> cardList = new List<Transform>();
    [Range(1, 2)]
    public int player;
    string inputSelect;
    string inputHorizontal;
    bool canChangeCards = true;
    public float horCooldown;

    [Header("References")]
    public GameManager gameMan;
    Transform selectedCard;
    Transform hoveredCard;

    void Start()
    {
        hoveredCard = cardList[0];

        if (player == 1)
        {
            inputSelect = gameMan.playerOneInputs[0];
            inputHorizontal = gameMan.playerOneInputs[1];
        }

        else
        {
            inputSelect = gameMan.playerTwoInputs[0];
            inputHorizontal = gameMan.playerTwoInputs[1];
        }
    }

    void Update()
    {
        if (canChangeCards)
        {
            if (Input.GetAxisRaw(inputHorizontal) == -1)
            {
                int cardIndex = cardList.IndexOf(hoveredCard) - 1;

                if (cardIndex < 0)
                {
                    hoveredCard = cardList[cardList.Count - 1];
                }

                else
                    hoveredCard = cardList[cardIndex];

                print(hoveredCard.name);

                canChangeCards = false;
                StartCoroutine(HorizontalCooldown());
            }

            if (Input.GetAxisRaw(inputHorizontal) == 1)
            {
                int cardIndex = cardList.IndexOf(hoveredCard) + 1;

                if (cardIndex == cardList.Count)
                {
                    hoveredCard = cardList[0];
                }

                else
                    hoveredCard = cardList[cardIndex];

                print(hoveredCard.name);

                canChangeCards = false;
                StartCoroutine(HorizontalCooldown());
            }
        }
    }

    IEnumerator HorizontalCooldown()
    {
        yield return new WaitForSeconds(horCooldown);
        canChangeCards = true;
    }

}
