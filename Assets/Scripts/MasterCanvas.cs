using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterCanvas : MonoBehaviour {
	/// <summary>タイトル</summary>
	[SerializeField] private Title title;
	/// <summary>ゲームマネージャ</summary>
	[SerializeField] private GameManager gameManager;

	void Start(){
		// 初期化
		title.Init(GameStart);
		gameManager.Init(ShowTitle);
		// タイトルを表示
		ShowTitle();
	}

	/// <summary>
	/// タイトルを表示
	/// </summary>
	private void ShowTitle(){
		// タイトルに戻る
		title.gameObject.SetActive(true);
		gameManager.gameObject.SetActive(false);
	}

	/// <summary>
	/// ゲーム開始
	/// </summary>
	private void GameStart(){
		// ゲーム画面を表示
		title.gameObject.SetActive(false);
		gameManager.gameObject.SetActive(true);
		// ゲーム開始
		gameManager.GameStart(true);
	}
}
