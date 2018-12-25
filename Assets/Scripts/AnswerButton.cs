using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class AnswerButton : MonoBehaviour {
	/// <summary>テキスト</summary>
	[SerializeField] TextMeshProUGUI text;
	/// <summary>ボタン</summary>
	[SerializeField] private Button button;

	/// <summary>×アイコン</summary>
	[SerializeField] private GameObject maruIcon;
	/// <summary>×アイコン</summary>
	[SerializeField] private GameObject batuIcon;

	/// <summary>
	/// 初期化
	/// </summary>
	/// <param name="num"></param>
	public void Init(int num, bool isOk, UnityAction<bool> onClick){
		if(num >= 0){
			text.text = num.ToString();
		}else{
			// マイナスを削除
			text.text = (-num).ToString();
		}

		// ボタン押下イベント
		button.onClick.AddListener(() => {
			// ボタンを無効化
			button.enabled = false;

			// アイコンの表示
			maruIcon.SetActive(isOk);
			batuIcon.SetActive(! isOk);

			onClick(isOk);
		});
	}
}
