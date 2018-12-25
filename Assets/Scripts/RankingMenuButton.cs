using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankingMenuButton : MonoBehaviour {

	private static RankingMenuButton instance;

	public static RankingMenuButton Instance {
		get {
			if (instance == null) {
				instance = (RankingMenuButton)FindObjectOfType (typeof(RankingMenuButton));
					if (instance == null) {
					Debug.LogError (typeof(RankingMenuButton) + "is nothing");
				}
			}
			return instance;
		}
	}


	/// <summary>戻るボタン</summary>
	public Button backButton;
	/// <summary>リトライボタン</summary>
	public Button retryButton;
	/// <summary>ツイートボタン</summary>
	public Button tweetButton;
}
