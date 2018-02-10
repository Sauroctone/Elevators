using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public List<string> playerOneInputs;
    public List<string> playerTwoInputs;

    public Rounds round;
    Coroutine roundCorWait;
    Coroutine roundCorFight;
    Coroutine playerTurns;

    [Header("Round : Wait")]
    public float flipDelay;
    public float peepTime;
    public float turnTime;
    public float roundTimeWait;
    public Turns turn;

//    [Header("Round : Fight")]
    

    [Header("References")]
    public CardController player1;
    public CardController player2;
    x360_Gamepad gamepad1;
    x360_Gamepad gamepad2;
    GamepadManager gamepadMan;

    void Start()
    {
        gamepadMan = GamepadManager.Instance;
        gamepad1 = gamepadMan.GetGamepad(1);
        gamepad2 = gamepadMan.GetGamepad(2);
    }

    void Update()
    {
        if (round == Rounds.Startup)
        {
            if (gamepad1.GetButtonDown("Start") || gamepad2.GetButtonDown("Start"))
            {
                round = Rounds.Wait;
                roundCorWait = StartCoroutine(WaitCor());
            }
        }
    }

    public void EndTurn()
    {
        if (playerTurns != null)
        {
            StopCoroutine(playerTurns);
        }

        if (player1.gameObject.activeSelf)
        {
            player1.gameObject.SetActive(false);
            player2.gameObject.SetActive(true);
            print("Player2");
        }

        else
        {
            player2.gameObject.SetActive(false);
            player1.gameObject.SetActive(true);
            print("Player1");
        }

        playerTurns = StartCoroutine(PlayerTurns());
    }

    IEnumerator WaitCor ()
    {
        yield return new WaitForSeconds(1);

        //Show cards

        foreach (CardBehaviour card in player1.cardOrder)
        {
            card.Flip();
            yield return new WaitForSeconds(flipDelay);
        }

        foreach (CardBehaviour card in player2.cardOrder)
        {
            card.Flip();
            yield return new WaitForSeconds(flipDelay);
        }

        yield return new WaitForSeconds(peepTime);

        //Hide cards

        foreach (CardBehaviour card in player1.cardOrder)
        {
            card.Flip();
            yield return new WaitForSeconds(flipDelay);
        }

        foreach (CardBehaviour card in player2.cardOrder)
        {
            card.Flip();
            yield return new WaitForSeconds(flipDelay);
        }

        //Start turns

        playerTurns = StartCoroutine(PlayerTurns());
        player1.gameObject.SetActive(true);
        print("Player1");
        yield return new WaitForSeconds(roundTimeWait);
        
        //End turns

        StopCoroutine(playerTurns);
        print("End of round");
        player1.gameObject.SetActive(false);
        player2.gameObject.SetActive(false);

        yield return new WaitForSeconds(1);
        round = Rounds.Fight;
        Fight();
    }

    void Fight()
    {
        player1.cardOrder[1].Resolve();
     //   player2.cardOrder[0].Resolve();
    }

    IEnumerator PlayerTurns ()
    {
        yield return new WaitForSeconds(turnTime);
        EndTurn();
    }
}
