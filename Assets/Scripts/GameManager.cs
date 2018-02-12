using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public List<string> playerOneInputs;
    public List<string> playerTwoInputs;
    public List<Cards> cardTypes = new List<Cards>();

    public Rounds round;
    Coroutine roundCorWait;
    Coroutine roundCorFight;
    Coroutine playerTurns;

    [Header("Round : Wait")]
    public float flipDelay;
    public float peepTime;
    public float turnTime;
    public float roundTimeWait;

    [Header("Round : Fight")]
    public int health1;
    public int health2;   

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

        if (round == Rounds.Fight)
        {
            if (health1 == 0)
            {
                round = Rounds.End;
                StartCoroutine(EndGame(player2));
            }

            if (health2 == 0)
            {
                round = Rounds.End;
                StartCoroutine(EndGame(player1));
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
        player1.Randomize();
        player2.Randomize();
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
        roundCorFight = StartCoroutine(FightCor());
    }

    IEnumerator FightCor()
    {
        CardBehaviour previousCard1 = null;
        CardBehaviour previousCard2 = null;

        for (int i = 0; i < 3; i++)
        {
            CardBehaviour card1 = player1.cardOrder[i];
            CardBehaviour card2 = player2.cardOrder[i];
            bool breaksShield1 = false;
            bool breaksShield2 = false;
            bool leaps1 = false;
            bool leaps2 = false;

            card1.Flip();
            card2.Flip();
            yield return new WaitForSeconds(1);

            if (card1.cardNature == Cards.Attack)
            {
                //Check attack's special properties

                if (previousCard1 != null && previousCard1.cardNature == Cards.Backstep)
                {
                    breaksShield1 = true;
                }

                if (previousCard1 != null && previousCard1.cardNature == Cards.Shield)
                {
                    leaps1 = true;
                }

                //Compare to other card

                if (card2.cardNature == Cards.Attack)
                {
                    player1.agent.Resolve(Cases.BlockedByAttack);
                    player2.agent.Resolve(Cases.BlockedByAttack);
                }

                if (card2.cardNature == Cards.Backstep)
                {
                    if (leaps1)
                    {
                        player1.agent.Resolve(Cases.Attack);
                        player2.agent.Resolve(Cases.Hurt);
                        health2--;
                    }

                    else
                    {
                        player1.agent.Resolve(Cases.Attack);
                        player2.agent.Resolve(Cases.BackstepDodge);
                    }
                }

                if (card2.cardNature == Cards.Shield)
                {
                    if (breaksShield1)
                    {
                        player1.agent.Resolve(Cases.Attack);
                        player2.agent.Resolve(Cases.Hurt);
                        health2--;
                    }

                    else
                    {
                        player1.agent.Resolve(Cases.BlockedByShield);
                        player2.agent.Resolve(Cases.Block);
                    }
                }
            }

            if (card1.cardNature == Cards.Backstep)
            {
                //Compare to other card

                if (card2.cardNature == Cards.Attack)
                {
                    if (previousCard2 != null && previousCard2.cardNature == Cards.Shield)
                    {
                        leaps2 = true;
                    }

                    if (leaps2)
                    {
                        player1.agent.Resolve(Cases.Hurt);
                        player2.agent.Resolve(Cases.Attack);
                        health1--;
                    }

                    else
                    {
                        player1.agent.Resolve(Cases.BackstepDodge);
                        player2.agent.Resolve(Cases.Attack); 
                    }
                }

                if (card2.cardNature == Cards.Backstep)
                {
                    player1.agent.Resolve(Cases.BackstepPose);
                    player2.agent.Resolve(Cases.BackstepPose);
                }

                if (card2.cardNature == Cards.Shield)
                {
                    player1.agent.Resolve(Cases.BackstepPose);
                    player2.agent.Resolve(Cases.Block);
                }
            }

            if (card1.cardNature == Cards.Shield)
            {
                //Compare to other card

                if (card2.cardNature == Cards.Attack)
                {
                    if (previousCard2 != null && previousCard2.cardNature == Cards.Backstep)
                    {
                        breaksShield2 = true;
                    }

                    if (breaksShield2)
                    {
                        player1.agent.Resolve(Cases.Hurt);
                        player2.agent.Resolve(Cases.Attack);
                        health1--;
                    }

                    else
                    {
                        player1.agent.Resolve(Cases.Block);
                        player2.agent.Resolve(Cases.BlockedByShield);
                    }
                }

                if (card2.cardNature == Cards.Backstep)
                {
                    player1.agent.Resolve(Cases.Block);
                    player2.agent.Resolve(Cases.BackstepPose);
                }

                if (card2.cardNature == Cards.Shield)
                {
                    player1.agent.Resolve(Cases.Block);
                    player2.agent.Resolve(Cases.Block);
                }
            }

            //Save cards for next phase
            previousCard1 = card1;
            previousCard2 = card2;
            yield return new WaitForSeconds(2);
        }

        foreach(CardBehaviour card in player1.cardOrder)
        {
            card.Flip();
            yield return new WaitForSeconds(flipDelay);
        }

        foreach (CardBehaviour card in player2.cardOrder)
        {
            card.Flip();
            yield return new WaitForSeconds(flipDelay);
        }

        round = Rounds.Wait;
        roundCorWait = StartCoroutine(WaitCor());
    }

    IEnumerator PlayerTurns ()
    {
        yield return new WaitForSeconds(turnTime);
        EndTurn();
    }

    IEnumerator EndGame (CardController _winner)
    {
        yield return null;
    }
}
