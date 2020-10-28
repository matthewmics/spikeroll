using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerGameSession
{

    public string Winner { get; set; } = null;

    public static int NumberOfPlayers { get; set; } = 3;


    public int Player1Score { get; set; } = 0;
    public int Player1Set { get; set; } = 0;


    public int Player2Score { get; set; } = 0;
    public int Player2Set { get; set; } = 0;


    public int RoundNumber { get; set; } = 0;


    public bool IsPlayer1Serve { get; set; } = true;
    public int TotalSet { get; set; } = 3;
    public int CurrentSet { get; set; } = 0;
    public int ScoreToWin { get; set; } = 21;
    public int ScoreToWinLastSet { get; set; } = 15;

    private float player1Special = 0;
    public float Player1SpecialThreshold 
    {
        get => player1Special;
        set
        {
            // Debug.Log("adding p1 thres");
            player1Special = value;

            MultiplayerGeneralControl.P1SpecialThreshold.sizeDelta = new Vector2(
                MultiplayerGeneralControl.P1SpecialThreshold.rect.width, value);

            if(value == 100f)
            {
                MultiplayerGeneralControl.P1SpecialButton.SetActive(true);
            }
            else 
            {
                MultiplayerGeneralControl.P1SpecialButton.SetActive(false);
            }
        }
    }

    private float player2Special = 0;
    public float Player2SpecialThreshold
    {
        get => player2Special;
        set
        {
            player2Special = value;
        }
    }


    public int SetNumber
    {
        get => Player1Set + Player2Set;
    }

    public int SetsToWinSeries
    {
        get => (Mathf.FloorToInt(TotalSet / 2)) + 1;
    }


    public MultiplayerGameSession()
    {

    }

    private void AddRoundNumber()
    {
        RoundNumber += 1;
        if ((RoundNumber % 3) == 0)
        {
            IsPlayer1Serve = !IsPlayer1Serve;
        }
    }

    public bool AddPlayer1Score(int amount)
    {
        AddRoundNumber();
        Player1Score += 1;
        if ((Player1Score) >= ScoreToWin && ((Player1Score - Player2Score) >= 2))
        {
            Player1Set += 1;
            if(Player1Set >= SetsToWinSeries)
            {
                Winner = "Player 1";
            }
            ResetScores();
            return true;
        }

        return false;
    }

    public bool AddPlayer2Score(int amount)
    {
        AddRoundNumber();
        Player2Score += 1;
        if ((Player2Score) >= ScoreToWin && ((Player2Score - Player1Score) >= 2))
        {
            Player2Set += 1;
            if (Player2Set >= SetsToWinSeries)
            {
                Winner = "Player 2";
            }
            ResetScores();
            return true;
        }

        return false;
    }


    private void ResetScores()
    {
        Player1Score = 0;
        Player2Score = 0;

        if(SetNumber >= TotalSet -1 )
        {
            ScoreToWin = ScoreToWinLastSet;
        }
    }
}
