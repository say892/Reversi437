using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMasterScript : MonoBehaviour {

	[SerializeField]
	private PieceColor[,] board;

	[SerializeField]
	private GameObject[,] boardObjects;
	private const int BOARD_SIZE = 7;



	[SerializeField]
	private GameObject whitePiece;
	[SerializeField]
	private GameObject blackPiece;
	[SerializeField]
	private GameObject dummyPiece;

	private int whiteScore = 0;
	private int blackScore = 0;

	private enum turn {
		player,
		AI
	};

	private turn currentTurn;

	private enum gameState {
		Menu, 
		Playing,
		End
	};

	private gameState currentState;

	private MiniMax AIscript;

	private int AIDiff;

	[SerializeField]
	private GameObject playingCanvas;

	[SerializeField]
	private GameObject endCanvas;

	[SerializeField]
	private Text currentTurnText;

	// Use this for initialization
	void Start () {

		board = new PieceColor[,] {
			{PieceColor.Null, PieceColor.Null, PieceColor.Null, PieceColor.Null, PieceColor.Null, PieceColor.Null, PieceColor.Null, PieceColor.Null}, 
			{PieceColor.Null, PieceColor.Null, PieceColor.Null, PieceColor.Null, PieceColor.Null, PieceColor.Null, PieceColor.Null, PieceColor.Null}, 
			{PieceColor.Null, PieceColor.Null, PieceColor.Null, PieceColor.Null, PieceColor.Null, PieceColor.Null, PieceColor.Null, PieceColor.Null}, 
			{PieceColor.Null, PieceColor.Null, PieceColor.Null, PieceColor.White, PieceColor.Black, PieceColor.Null, PieceColor.Null, PieceColor.Null}, 
			{PieceColor.Null, PieceColor.Null, PieceColor.Null, PieceColor.Black, PieceColor.White, PieceColor.Null, PieceColor.Null, PieceColor.Null}, 
			{PieceColor.Null, PieceColor.Null, PieceColor.Null, PieceColor.Null, PieceColor.Null, PieceColor.Null, PieceColor.Null, PieceColor.Null}, 
			{PieceColor.Null, PieceColor.Null, PieceColor.Null, PieceColor.Null, PieceColor.Null, PieceColor.Null, PieceColor.Null, PieceColor.Null}, 
			{PieceColor.Null, PieceColor.Null, PieceColor.Null, PieceColor.Null, PieceColor.Null, PieceColor.Null, PieceColor.Null, PieceColor.Null}
		};

		boardObjects = new GameObject[,] {
			{null, null, null, null, null, null, null, null},
			{null, null, null, null, null, null, null, null},
			{null, null, null, null, null, null, null, null},
			{null, null, null, null, null, null, null, null},
			{null, null, null, null, null, null, null, null},
			{null, null, null, null, null, null, null, null},
			{null, null, null, null, null, null, null, null},
			{null, null, null, null, null, null, null, null}
		};



		for(int j = 0; j <= BOARD_SIZE; j++) {
			for (int i = 0; i <= BOARD_SIZE; i++) {
				if (board[i, j] == PieceColor.Black) {
					boardObjects[i,j] = (GameObject) Instantiate(blackPiece, new Vector3(i, 1, j), Quaternion.identity);
					blackScore++;
					//print (i + ", " + j);
				}
				else if (board[i, j] == PieceColor.White) {
					boardObjects[i, j] = (GameObject) Instantiate(whitePiece, new Vector3(i, 1, j), Quaternion.identity);
					whiteScore++;
				}
			}
		}

		/*
		for (int j = 0; j < BOARD_SIZE; j++) {
			for (int i = 0; i < BOARD_SIZE; i++) {
				if (isValidMove(i, j)) {
					Instantiate(blackPiece, new Vector3(i, 1, j), Quaternion.identity);
				}
			}
		}
		*/
		currentTurn = turn.player;
		currentState = gameState.Menu;



		playingCanvas.SetActive(false);
		endCanvas.SetActive(false);

		AIscript = GetComponent<MiniMax>();

	}
	
	// Update is called once per frame
	void Update () {

		if (currentState == gameState.Playing) {

			if (currentTurn == turn.player && GameObject.FindGameObjectsWithTag("Dummy").Length == 0) {
				isGameOver();
			}


			if (Input.GetMouseButtonDown(0) && currentTurn == turn.player) {
				//Camera c = GetComponent<Camera>();

				GameObject[] allDummy = GameObject.FindGameObjectsWithTag("Dummy");
				foreach(GameObject gm in allDummy) {
					Destroy(gm);
				}



				Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				getClick(pos.x, pos.z);

			

			}
			if (Input.GetKeyDown(KeyCode.M)) {
				doEnemyMove();
			}


		}

	}

	void getClick(float x, float y) {

		int newX = Mathf.RoundToInt(x);
		int newY = Mathf.RoundToInt(y);
		DirectionalPieces dPiece = isValidMove(newX, newY, PieceColor.Black, board);
		if (dPiece.down || dPiece.left || dPiece.up || dPiece.right || dPiece.leftDown || dPiece.leftUp || dPiece.rightDown || dPiece.rightUp) {
			currentTurn = turn.AI;
			Vector3 spawnPos = new Vector3(newX, 1, newY);
			boardObjects[newX, newY] = (GameObject) Instantiate(blackPiece, spawnPos, Quaternion.identity);
			board[newX, newY] = PieceColor.Black;
			blackScore++;
			doFlips(newX, newY, PieceColor.Black, dPiece);
			currentTurnText.text = "Current Turn: AI Turn (White)";

			//print(AIDiff);
			//if (AIDiff <= 4) {
			StartCoroutine("enemyMove");
			//}

			//doEnemyMove();
			




		}
	}

	IEnumerator enemyMove() {
		yield return new WaitForSeconds(2);
		doEnemyMove();
	}

	void doEnemyMove() {

		//completely random
		/*for (int j = 0; j <= BOARD_SIZE; j++) {
			for (int i = 0; i <= BOARD_SIZE; i++) {
				DirectionalPieces dPiece = isValidMove(i, j, PieceColor.White, board);
				if (dPiece.down || dPiece.left || dPiece.up || dPiece.right || dPiece.leftDown || dPiece.leftUp || dPiece.rightDown || dPiece.rightUp) {
					boardObjects[i, j] = (GameObject) Instantiate(whitePiece, new Vector3(i, 1, j), Quaternion.identity);
					board[i,j] = PieceColor.White;
					whiteScore	++;
					doFlips(i, j, PieceColor.White, dPiece);
					foundMove = true;
					break;
				}
			}
			if (foundMove) break;
		}*/
		//move bestMove = AIscript.miniMax(board, 1, true, -99999, 99999, 0, 0);

		//1 means 1 level of depth, no smarts
		//2 means 1 level of depth, smarts
		//10 is 5 level of depth, smarts
		move bestMove;

		if (AIDiff % 2 == 0) {
			bestMove = AIscript.miniMaxBetter(board, Mathf.CeilToInt(AIDiff/2.0F), true, -99999, 99999, 0, 0);
		}
		else {
			bestMove = AIscript.miniMax(board, Mathf.CeilToInt(AIDiff/2.0F), true, -99999, 99999, 0, 0);
		}
		//first move:
		//6 takes 7 seconds
		//7 takes 22 seconds
		//8 takes 58 seconds
		//9 takes 2 minute 58 seconds

		if (bestMove.i != -1 && bestMove.j != -1) {
			//I know it's valid, but I need to know which way to flip... sorry.
			print(bestMove.value);
			DirectionalPieces dPiece = isValidMove(bestMove.i, bestMove.j, PieceColor.White, board);
			boardObjects[bestMove.i, bestMove.j] = (GameObject) Instantiate(whitePiece, new Vector3(bestMove.i, 1, bestMove.j), Quaternion.identity);
			board[bestMove.i, bestMove.j] = PieceColor.White;
			whiteScore++;
			doFlips(bestMove.i, bestMove.j, PieceColor.White, dPiece);
			currentTurn = turn.player;
			currentTurnText.text = "Current Turn: Your Turn (Black)";
			if (isGameOver()) {
				print("PLAYER CAN'T MOVE!");
				endGame();
			}

		}
		else {
			print(bestMove.value + ", " + bestMove.i + ", " + bestMove.j);
			print("GAME OVER I CAN'T MOVE");
			endGame();
		}




	}



	//Checks if placing a "thiscolor" piece at x,y is a legal move
	public DirectionalPieces isValidMove(int x, int y, PieceColor thisColor, PieceColor[,] board) {

		DirectionalPieces dPieces;

		dPieces.left = false;
		dPieces.right = false;
		dPieces.up = false;
		dPieces.down = false;
		dPieces.leftDown = false;
		dPieces.leftUp = false;
		dPieces.rightDown = false;
		dPieces.rightUp = false;
		dPieces.points = 0;


		PieceColor otherColor = PieceColor.Null;

		if (thisColor == PieceColor.Black) otherColor = PieceColor.White;
		if (thisColor == PieceColor.White) otherColor = PieceColor.Black;

		if (otherColor == PieceColor.Null) print("ERROR");

		int tempPoints = 0;

		//current square is empty
		if (x >= 0 && x <= BOARD_SIZE && y >= 0 && y <= BOARD_SIZE && board[x,y] == PieceColor.Null) {

			//Up piece needs to be a white piece to flip
			if (x >= 1) {
				if (board[x-1, y] == otherColor) {
					
					//then there needs to be a black piece to flip to.
					for (int i = x-1; i >= 0; i--) {
						tempPoints++;
						if (board[i, y] == PieceColor.Null) break;
						if (board[i, y] == thisColor) {
							dPieces.up = true;
							dPieces.points+= tempPoints;
							tempPoints = 0;
							break;
							//return true;
						}
					}

				}
			}

			//Down piece is white
			if (x <= BOARD_SIZE-1) {
				if (board[x+1, y] == otherColor) {
					
					//then there needs to be a black piece to flip to.
					for (int i = x+1; i <= BOARD_SIZE; i++) {
						tempPoints++;
						if (board[i, y] == PieceColor.Null) break;
						if (board[i, y] == thisColor) {
							dPieces.down = true;
							dPieces.points+= tempPoints;
							tempPoints = 0;
							break;
							//return true;
						}
					}

				}
			}

			//Left piece is white
			if (y >= 1) {
				if (board[x, y-1] == otherColor) {
					//then there needs to be a black piece to flip to.
					for (int i = y-1; i >= 0; i--) {
						tempPoints++;
						if (board[x, i] == PieceColor.Null) break;
						if (board[x, i] == thisColor) {
							dPieces.left = true;
							dPieces.points+= tempPoints;
							tempPoints = 0;
							break;
							//return true;
						}
					}

				}
			}

			//Right piece is white
			if (y <= BOARD_SIZE-1) {
				if (board[x, y+1] == otherColor) {
					//then there needs to be a black piece to flip to.
					for (int i = y+1; i <= BOARD_SIZE; i++) {
						tempPoints++;
						if (board[x, i] == PieceColor.Null) break;
						if (board[x, i] == thisColor) {
							dPieces.right = true;
							dPieces.points+= tempPoints;
							tempPoints = 0;
							break;
							//return true;
						}
					}

				}
			}

			//left up piece is white
			if (y >= 1 && x >= 1) {
				if (board[x-1, y-1] == otherColor) {
					print("leftup");
					//then there needs to be a black piece to flip to.
					int i = x-1;
					int j = y-1;
					while (i >= 0 && j >= 0) {
						tempPoints++;
						if (board[i, j] == PieceColor.Null) break;
						if (board[i, j] == thisColor) {
							dPieces.leftUp = true;
							dPieces.points+= tempPoints;
							tempPoints = 0;
							break;
							//return true;
						}
						i--;
						j--;
					}
				}
			}

			//right down piece is white
			if (y <= BOARD_SIZE-1 && x <= BOARD_SIZE-1) {
				if (board[x+1, y+1] == otherColor) {
					print("Rightdown");
					//then there needs to be a black piece to flip to.
					int i = x+1;
					int j = y+1;
					while (i != BOARD_SIZE && j != BOARD_SIZE) {
						tempPoints++;
						if (board[i, j] == PieceColor.Null) break;
						if (board[i, j] == thisColor) {
							dPieces.rightDown = true;
							dPieces.points+= tempPoints;
							tempPoints = 0;
							break;
							//return true;
						}
						i++;
						j++;
					}
				}
			}

			//right up piece is white
			if (y <= BOARD_SIZE-1 && x >= 1) {
				if (board[x-1, y+1] == otherColor) {
					print("rightup");
					//then there needs to be a black piece to flip to.
					int i = x-1;
					int j = y+1;
					while (i >= 0 && j != BOARD_SIZE) {
						tempPoints++;
						if (board[i, j] == PieceColor.Null) break;
						if (board[i, j] == thisColor) {
							dPieces.rightUp = true;
							dPieces.points+= tempPoints;
							tempPoints = 0;
							break;
							//return true;
						}
						i--;
						j++;
					}
				}
			}

			//left down piece is white
			if (y >= 1 && x <= BOARD_SIZE-1) {
				if (board[x+1, y-1] == otherColor) {
					print("leftdown");
					//then there needs to be a black piece to flip to.
					int i = x+1;
					int j = y-1;
					while (i != BOARD_SIZE && j >= 0) {
						tempPoints++;
						if (board[i, j] == PieceColor.Null) break;
						if (board[i, j] == thisColor) {
							dPieces.leftDown = true;
							dPieces.points+= tempPoints;
							tempPoints = 0;
							break;
						}
						i++;
						j--;
					}
				}
			}



		}//end if null piece
			
		return dPieces;
	}


	void doFlips(int x, int y, PieceColor thisColor, DirectionalPieces dPieces) {

		PieceColor otherColor = PieceColor.Null;

		if (thisColor == PieceColor.Black) otherColor = PieceColor.White;
		if (thisColor == PieceColor.White) otherColor = PieceColor.Black;

		if (otherColor == PieceColor.Null) print("ERROR");

		if (dPieces.up) {
			for (int i = x-1; i >= 0; i--) {
				if (board[i, y] == thisColor || board[i, y] == PieceColor.Null) break;
				else board[i, y] = boardObjects[i,y].GetComponentInChildren<PieceScript>().Flip();
			}
		}

		if (dPieces.down) {
			for (int i = x+1; i <= BOARD_SIZE; i++) {
				if (board[i, y] == thisColor || board[i, y] == PieceColor.Null) break;
				else board[i, y] = boardObjects[i,y].GetComponentInChildren<PieceScript>().Flip();
			}
		}

		if (dPieces.left) {
			for (int i = y-1; i > 0; i--) {
				if (board[x, i] == thisColor || board[x, i] == PieceColor.Null) break;
				else board[x, i] = boardObjects[x,i].GetComponentInChildren<PieceScript>().Flip();
			}
		}

		if (dPieces.right) {
			for (int i = y+1; i <= BOARD_SIZE; i++) {
				if (board[x, i] == thisColor || board[x, i] == PieceColor.Null) break;
				else board[x, i] = boardObjects[x,i].GetComponentInChildren<PieceScript>().Flip();
			}
		}

		if (dPieces.leftUp) {
			int i = x-1;
			int j = y-1;
			while (i >= 0 && j >= 0) {
				if (board[i, j] == thisColor || board[i, j] == PieceColor.Null) break;
				else board[i, j] = boardObjects[i,j].GetComponentInChildren<PieceScript>().Flip();
				i--;
				j--;
			}
		}

		if (dPieces.rightDown) {
			int i = x+1;
			int j = y+1;
			while (i != BOARD_SIZE && j != BOARD_SIZE) {
				if (board[i, j] == thisColor || board[i, j] == PieceColor.Null) break;
				else board[i, j] = boardObjects[i,j].GetComponentInChildren<PieceScript>().Flip();
				i++;
				j++;
			}
		}

		if (dPieces.rightUp) {
			int i = x-1;
			int j = y+1;
			while (i >= 0 && j != BOARD_SIZE) {
				if (board[i, j] == thisColor || board[i, j] == PieceColor.Null) break;
				else board[i, j] = boardObjects[i,j].GetComponentInChildren<PieceScript>().Flip();
				i--;
				j++;
			}
		}

		if (dPieces.leftDown) {
			int i = x+1;
			int j = y-1;
			while (i != BOARD_SIZE && j >= 0) {
				if (board[i, j] == thisColor || board[i, j] == PieceColor.Null) break;
				else board[i, j] = boardObjects[i,j].GetComponentInChildren<PieceScript>().Flip();
				i++;
				j--;
			}
		}

	}

	bool isGameOver() {

		bool moveFound = true;
		for(int j = 0; j <= BOARD_SIZE; j++) {
			for (int i = 0; i <= BOARD_SIZE; i++) {
				DirectionalPieces dPiece = isValidMove(i, j, PieceColor.Black, board);
				if (dPiece.down || dPiece.left || dPiece.up || dPiece.right || dPiece.leftDown || dPiece.leftUp || dPiece.rightDown || dPiece.rightUp) {
					moveFound = false;
					Instantiate(dummyPiece, new Vector3(i, 1, j), Quaternion.identity);
				}
			}
		}

		return moveFound;
	}


	public void addBlack() {
		blackScore++;
		whiteScore--;
	}

	public void addWhite() {
		whiteScore++;
		blackScore--;
	}

	public void startGame(int difficulty) {
		AIDiff = difficulty;
		currentState = gameState.Playing;
		playingCanvas.SetActive(true);
		isGameOver();
		print("Starting game with difficulty: " + difficulty);
	}

	public void restartGame() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void quitGame() {
		Application.Quit();
	}

	void endGame() {
		currentState = gameState.End;
		playingCanvas.SetActive(false);
		endCanvas.SetActive(true);
		if (whiteScore > blackScore) {
			endCanvas.transform.GetChild(1).GetComponent<Text>().text = "You Lose...";
		}
		print ("final score: BLACK: " + blackScore + " WHITE: " + whiteScore);
	}

	void OnGUI() {

		if (currentState == gameState.Playing || currentState == gameState.End) {
			GUI.Label(new Rect(20, 20, 200, 100), "Black: " + blackScore);
			GUI.Label(new Rect(20, 40, 200, 100), "White: " + whiteScore);
			GUI.Label(new Rect(20, 60, 200, 100), "AI Difficulty: " + AIDiff);
		}

	}




}

//This holds which 8 directions (if any) pieces will be flipped. the class name is terrible
public struct DirectionalPieces {
	public bool left;
	public bool right;
	public bool up;
	public bool down;
	public bool leftUp;
	public bool rightUp;
	public bool leftDown;
	public bool rightDown;
	public int points;
}
