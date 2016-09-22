using UnityEngine;
using System.Collections;



public class MiniMax : MonoBehaviour {

	private int[,] boardPoints;

	private const int BOARD_SIZE = 7;

	private GameMasterScript gmScript;

	private int[,] currentBoard;

	// Use this for initialization
	void Start () {
		//arbitrary points for AI, higher means a better spot.
		boardPoints = new int[,] {
			{5, 1, 4, 4, 4, 4, 1, 5},
			{1, 1, 2, 2, 2, 2, 1, 1},
			{4, 2, 3, 3, 3, 3, 2, 4},
			{4, 2, 3, 3, 3, 3, 2, 4},
			{4, 2, 3, 3, 3, 3, 2, 4},
			{4, 2, 3, 3, 3, 3, 2, 4},
			{1, 1, 2, 2, 2, 2, 1, 1},
			{5, 1, 4, 4, 4, 4, 1, 5}
		};

		gmScript = GetComponent<GameMasterScript>();
		//Save for later
		/*
		for (int j = 0; j < BOARD_SIZE; j++) {
			for (int i = 0; i < BOARD_SIZE; i++) {
				if (isValidMove(i, j)) {
					Instantiate(blackPiece, new Vector3(i, 1, j), Quaternion.identity);
				}
			}
		}*/
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public move miniMaxBetter(PieceColor [,] currentBoard, int depth, bool maximizingPlayer, int alpha, int beta, int i2, int j2) {

		//bool breaking = false;

		//print(depth);

		if (depth == 0 && maximizingPlayer) {
			move endMove;
			//endMove.value = boardPoints[i2, j2];
			endMove.value = 0;
			endMove.i = i2;
			endMove.j = j2;
			endMove.points = 0;
			return endMove;
		}
		if (depth == 0 && !maximizingPlayer) {
			move endMove;
			//endMove.value = boardPoints[i2, j2];
			endMove.value = 0;
			endMove.i = i2;
			endMove.j = j2;
			endMove.points = 0;
			return endMove;
		}

		//White Player
		if (maximizingPlayer) {
			move bestMove;
			bestMove.value = -99999;
			bestMove.i = -1;
			bestMove.j = -1;
			bestMove.points = -1;
			for (int j = 0; j <= BOARD_SIZE; j++) {
				for (int i = 0; i <= BOARD_SIZE; i++) {
					DirectionalPieces dPiece =  gmScript.isValidMove(i, j, PieceColor.White, currentBoard);
					if (dPiece.down || dPiece.left || dPiece.up || dPiece.right || dPiece.leftDown || dPiece.leftUp || dPiece.rightDown || dPiece.rightUp) {
						currentBoard[i, j] = PieceColor.White;
						move currentMove = miniMaxBetter(currentBoard, depth-1, false, alpha, beta, i, j);
						currentMove.value += boardPoints[i, j];
						currentMove.points += dPiece.points;

						if (currentMove.value > bestMove.value) {
							bestMove.value = currentMove.value;
							bestMove.i = i;
							bestMove.j = j;
							bestMove.points = currentMove.points;

						}
						else if (currentMove.value == bestMove.value){
							if (currentMove.points > bestMove.points) {
								bestMove.value = currentMove.value;
								bestMove.i = i;
								bestMove.j = j;
								bestMove.points = currentMove.points;
							}
						}
						else {
							//currentMove.value -= boardPoints[i, j];
						}
						//undo our moves as we get out of them
						currentBoard[i, j] = PieceColor.Null;

						//alpha beta pruning
						alpha = Mathf.Max(alpha, bestMove.value);

						if (beta <= alpha) {
							return bestMove;
							//breaking = true; //for double for loop
							//break;
						}
					}
				}
				//if (breaking) break; //double for loop
			}
			return bestMove;
		}//end of if statement

		//player's turn
		else {
			move bestMove;
			bestMove.value = 99999;
			bestMove.i = -1;
			bestMove.j = -1;
			bestMove.points = -1;
			for (int j = 0; j <= BOARD_SIZE; j++) {
				for (int i = 0; i <= BOARD_SIZE; i++) {
					DirectionalPieces dPiece =  gmScript.isValidMove(i, j, PieceColor.Black, currentBoard);
					if (dPiece.down || dPiece.left || dPiece.up || dPiece.right || dPiece.leftDown || dPiece.leftUp || dPiece.rightDown || dPiece.rightUp) {
						currentBoard[i, j] = PieceColor.Black;
						move currentMove = miniMaxBetter(currentBoard, depth-1, true, alpha, beta, i, j);
						currentMove.value -= boardPoints[i, j];
						currentMove.points -= dPiece.points;

						if (currentMove.value < bestMove.value) {
							bestMove.value = currentMove.value;
							bestMove.i = i;
							bestMove.j = j;
							bestMove.points = currentMove.points;

						}
						else if (currentMove.value == bestMove.value){
							if (currentMove.points < bestMove.points) {
								bestMove.value = currentMove.value;
								bestMove.i = i;
								bestMove.j = j;
								bestMove.points = currentMove.points;
							}
						}
						else {
							//currentMove.value += boardPoints[i, j];
						}
						currentBoard[i, j] = PieceColor.Null;

						//alpha beta pruning
						beta = Mathf.Min(beta, bestMove.value);

						if (beta <= alpha) {
							return bestMove;
							//breaking = true; //for double for loop
							//break;
						}
					}
				}
				//if (breaking) break;
			}
			return bestMove;
		}//end of else

	}//end of minimaxbetter


	public move miniMax(PieceColor [,] currentBoard, int depth, bool maximizingPlayer, int alpha, int beta, int i2, int j2) {

		//bool breaking = false;

		//print(depth);

		if (depth == 0 && maximizingPlayer) {
			move endMove;
			//endMove.value = boardPoints[i2, j2];
			endMove.value = 0;
			endMove.i = i2;
			endMove.j = j2;
			endMove.points = 0;
			return endMove;
		}
		if (depth == 0 && !maximizingPlayer) {
			move endMove;
			//endMove.value = boardPoints[i2, j2];
			endMove.value = 0;
			endMove.i = i2;
			endMove.j = j2;
			endMove.points = 0;
			return endMove;
		}

		//White Player
		if (maximizingPlayer) {
			move bestMove;
			bestMove.value = -99999;
			bestMove.i = -1;
			bestMove.j = -1;
			bestMove.points = -1;
			for (int j = 0; j <= BOARD_SIZE; j++) {
				for (int i = 0; i <= BOARD_SIZE; i++) {
					DirectionalPieces dPiece =  gmScript.isValidMove(i, j, PieceColor.White, currentBoard);
					if (dPiece.down || dPiece.left || dPiece.up || dPiece.right || dPiece.leftDown || dPiece.leftUp || dPiece.rightDown || dPiece.rightUp) {
						currentBoard[i, j] = PieceColor.White;
						move currentMove = miniMax(currentBoard, depth-1, false, alpha, beta, i, j);
						currentMove.value += boardPoints[i, j];

						if (currentMove.value > bestMove.value) {
							bestMove.value = currentMove.value;
							bestMove.i = i;
							bestMove.j = j;

						}
						else {
							//currentMove.value -= boardPoints[i, j];
						}
						//undo our moves as we get out of them
						currentBoard[i, j] = PieceColor.Null;

						//alpha beta pruning
						alpha = Mathf.Max(alpha, bestMove.value);

						if (beta <= alpha) {
							return bestMove;
							//breaking = true; //for double for loop
							//break;
						}
					}
				}
				//if (breaking) break; //double for loop
			}
			return bestMove;
		}//end of if statement

		//player's turn
		else {
			move bestMove;
			bestMove.value = 99999;
			bestMove.i = -1;
			bestMove.j = -1;
			bestMove.points = 0;
			for (int j = 0; j <= BOARD_SIZE; j++) {
				for (int i = 0; i <= BOARD_SIZE; i++) {
					DirectionalPieces dPiece =  gmScript.isValidMove(i, j, PieceColor.Black, currentBoard);
					if (dPiece.down || dPiece.left || dPiece.up || dPiece.right || dPiece.leftDown || dPiece.leftUp || dPiece.rightDown || dPiece.rightUp) {
						currentBoard[i, j] = PieceColor.Black;
						move currentMove = miniMax(currentBoard, depth-1, true, alpha, beta, i, j);
						currentMove.value -= boardPoints[i, j];

						if (currentMove.value < bestMove.value) {
							bestMove.value = currentMove.value;
							bestMove.i = i;
							bestMove.j = j;
							bestMove.points = currentMove.points;

						}
						else {
							//currentMove.value += boardPoints[i, j];
						}
						currentBoard[i, j] = PieceColor.Null;

						//alpha beta pruning
						beta = Mathf.Min(beta, bestMove.value);

						if (beta <= alpha) {
							return bestMove;
							//breaking = true; //for double for loop
							//break;
						}
					}
				}
				//if (breaking) break;
			}
			return bestMove;
		}//end of else

	}//end of minimax




}


public struct move {
	public int i;
	public int j;
	public int value;
	public int points;
}