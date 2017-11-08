using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Classes : MonoBehaviour {

	public class GameManager{
		private enum phases{START, UNIT, MOVE, MINIGAME, ACTION, END};
		private turns{PLAYER1, PLAYER2, PLAYER3, PLAYER4};

		public phases phase;
		public turns turn;

	}

	abstract class Player{
		public int id;
	}

	public class HumanPlayer : Player{


	}

	public class NonHumanPlayer : Player{


	}

	public class Unit{
		public int id;
		private int currentSector;

	}

	public class Map{
		//???
		public Sector[] Sector;

	}

	public class Sector: Map{
		public int id;

	}

	public class PVC : Map{
		//Extend straight to map or sector?
		public int cooldownTurns;
		public int currentSector;

	}

	public class Landmark : Sector{

	}

	abstract class Resource{
		public int value;

	}

	public class Knowledge : Resource{

	}

	public class Beer : Resource{

	}

}
