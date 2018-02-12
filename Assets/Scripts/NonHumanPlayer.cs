using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * NonHumanPlayer:
 * Represents the AI players in the game. Contains methods which make moves and play the PVC minigame.
 * 
 * ADDITION: 31/01/18
 */
public class NonHumanPlayer : Player {
	/**
	 * makeMove(turnState):
	 * this method is used by the non-human players to allow them to make a move during the turn state, "turnState". 
	 * If the turnState == Move1 or Move2, then the method will decide which unit is the best to move, and execute it.
	 * 
	 * ADDITION: 01/02/18
	 */
	public void makeMove(Game.TurnState turnState){
		if (turnState == Game.TurnState.Move1 || turnState == Game.TurnState.Move2 ) 
		{
			Sector moveFrom = null;
			Sector moveTo = null;
			int bestMoveScore = -1;

			foreach (Unit unit in this.units) 
			{
				KeyValuePair<Sector,int> unitsBestMove = this.findBestMove (unit, unit.GetSector ().GetAdjacentSectors ());
				if (unitsBestMove.Value > bestMoveScore) 
				{
					bestMoveScore = unitsBestMove.Value;
					moveFrom = unit.GetSector ();
					moveTo = unitsBestMove.Key;
				}
			}
			if (moveTo == null || moveFrom == null) {
				throw new System.Exception ("No valid moves found for AI " + this.name);
			}

			//Debug.Log ("==========================Start==========================");
			//Debug.Log (this.name + "\nTurnState:" + turnState);

			moveFrom.OnMouseUpAsButtonAccessible ();
			//Debug.Log ("Selected " + moveFrom.name.ToString());
			moveTo.OnMouseUpAsButtonAccessible ();
			//Debug.Log ("Made move from " + moveFrom.name + " to " + moveTo.name.ToString());
			//Debug.Log ("===========================End===========================");
		}
	}

	/**
	 * findBestMove(Unit,Sector[]):
	 * Used for finding the "best" move that the AI's unit can make.
	 * Return: a pair <Sector,Int>. The Sector is the best sector to move to out of the array possibleMoves,
	 * 		   						The Int is the move's final score.
	 * note: if there are no valid moves, the output will be a pair <null,-1>.
	 * 
	 * ADDITION: 02/02/18	-for the implementation of a non human player.
	 */
	private KeyValuePair<Sector,int> findBestMove(Unit unitToMove, Sector[] possibleMoves){
		Sector bestMove = null;
		int bestScore = -1;

		foreach (Sector move in possibleMoves) 
		{
			int currentScore = 0;
			//string output = "\n";
			if (move.GetOwner () != this) 
			{
				currentScore += 1;	//Captures a new sector
				//output += "Unowned ";
				if (move.GetUnit () != null) // if it is an enemy sector, check if it contains a unit
				{
					int enemyDefence = move.GetUnit ().GetLevel () + move.GetOwner ().GetKnowledge (); //Calculate unit attack/defence scores
					int thisAttack = unitToMove.GetLevel () + this.GetBeer ();
					if (thisAttack >= enemyDefence)
					{
						currentScore += 1;	//player unit has a good chance of defeating the enemy unit
						//output += "Enemy-GoodChance ";
					} 
					else 
					{
						currentScore -= 1;  //player unit has a poor chance of defeating the enemy unit
						//output += "Enemy-BadChance ";
					}
				}
				if (move.GetLandmark () != null) //if the sector contains a landmark, increase it's score.
				{
					currentScore += 2;
					//output += "Landmark";
				}
			}
			if (currentScore > bestScore) // check if the move is better than the best, and if so update bestMove/bestScore
			{
				bestScore = currentScore;
				bestMove = move;
			}
			//Debug.Log ("Move from " + unitToMove.GetSector ().name + " to " + move.name + " Score: " + currentScore + output);
		}
		return new KeyValuePair<Sector,int> (bestMove,bestScore);	
	}
}