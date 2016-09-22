using UnityEngine;
using System.Collections;

public enum PieceColor {
	Null,
	White, 
	Black
}

public class PieceScript : MonoBehaviour {


	private enum PieceRotation {
		White = 0,
		Black = 180
	}

	[SerializeField]
	private PieceColor currentColor;

	private bool isFlipping;

	private Animator mAnim;

	private GameMasterScript gmScript;

	// Use this for initialization
	void Start () {
		mAnim = GetComponent<Animator>();
		gmScript = GameObject.Find("Main Camera").GetComponent<GameMasterScript>();
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown(KeyCode.A)) Flip();

	}

	public void setColor(PieceColor color) {
		currentColor = color;
	}

	public PieceColor Flip() {
		if (currentColor == PieceColor.White) {
			mAnim.SetTrigger("Wht2BlkFlip");
			currentColor = PieceColor.Black;
			gmScript.addBlack();
		}
		else {
			mAnim.SetTrigger("Blk2WhtFlip");
			currentColor = PieceColor.White;
			gmScript.addWhite();
		}

		return currentColor;
	}
}
