using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Scoring : MonoBehaviour, ISolitaireEventsHandlers
{
    public Text scoreText, movesText;
    public int score = 0;
    public int moves = 0;
    public Stack<int> scoreHistory = new Stack<int>();


    private void AddToScore(int amoutToAdd){
        SetScore(this.score + amoutToAdd);
    }

    private void SetScore(int amount){
        this.score = Mathf.Max(amount, 0);  //Don't let score go below zero
        this.scoreText.text = amount.ToString();
    }

    private void IncrementMoves(){
        this.moves = moves+1;
        this.movesText.text = this.moves.ToString();
    }

    public void NotifyBeginGame(CardColumn[] tableu, Stack<Card> stockPile)
    {
        this.score = 0;
        this.moves = 0;
        this.scoreText.text = this.score.ToString();        
        this.movesText.text = this.moves.ToString();
    }

    public void NotifyFlipStockCardMove(Move move)
    {
        scoreHistory.Push(this.score);
        IncrementMoves();
    }

    public void NotifyIllegalMove(IllegalMove move)
    {
        //Do nothing
    }

    public void NotifyLegalMove(Move move)
    {
        scoreHistory.Push(this.score);

        int scoreToAdd = 0;
        Card selectedCard = move.movedCards.First();

        // if(move.MoveResultsInCardFlipped()){
        //     scoreToAdd += 5;
        // }
        if(move.to.zone == Zone.Foundation){
            scoreToAdd += 10;
        }
        if(move.from.zone == Zone.Waste && move.to.zone == Zone.Tableu){
            scoreToAdd += 5;
        }

        this.AddToScore(scoreToAdd);
        this.IncrementMoves();
    }

    public void NotifyRestoreStockpileFromWastePile(List<Card> restoredStockPile)
    {
        scoreHistory.Push(this.score);
        this.AddToScore(-100);
        this.IncrementMoves();
    }

    void Awake()
    {
        GameManager.Instance.RegisterSolitaireEventsHandler(this);
    }

    public void NotifyUndoMove(Move moveToUndo)
    {
        int newScore = scoreHistory.Pop();
        SetScore(newScore);
        IncrementMoves();
    }
}
