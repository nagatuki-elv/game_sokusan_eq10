using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Linq;
using UnityEngine.Events;

public class Quiz : MonoBehaviour {
	/// <summary>クイズテキスト</summary>
	[SerializeField] private Text quizText;

	/// <summary>グリッドレイアウト</summary>
	[SerializeField] private GridLayoutGroup answerGrid;
	/// <summary>解答ボタンプレハブ</summary>
	[SerializeField] private GameObject answerButtonPrefab;

	/// <summary>正解</summary>
	private UnityAction onOk;

	/// <summary>答えを隠すところ</summary>
	private int maskId = 0;
	/// <summary>
	/// 解答リスト
	/// </summary>
	/// <typeparam name="int"></typeparam>
	/// <returns></returns>
	private List<int> answerList = new List<int>();

	/// <summary>
	/// 初期化
	/// </summary>
	/// <param name="count"></param>
	public void Init(int count, GameManager.QuizData quizData, UnityAction onOk){
		// 正解のイベント
		this.onOk = onOk;

		// 問題の作成
		StringBuilder builder = new StringBuilder("");

		// 隠すところを決める
		SetQuizMask(quizData);

		// 左辺
		SetQuizString(quizData.left1, true, maskId == 0, ref builder);
		SetQuizString(quizData.left2, false, maskId == 1, ref builder);
		SetQuizString(quizData.left3, false, maskId == 2, ref builder);
		// イコール
		builder.Append("= ");
		// 右辺
		SetQuizString(quizData.right1, true, maskId == 3, ref builder);
		SetQuizString(quizData.right2, false, maskId == 4, ref builder);
		SetQuizString(quizData.right3, false, maskId == 5, ref builder);
		// 最終文字の空白を削除
		builder.Remove(builder.Length - 1, 1);
		// テキストにセット
		quizText.text = builder.ToString();

		// 解答を作成
		SetAnswer(quizData);
	}

	/// <summary>
	/// 解答を非表示
	/// </summary>
	public void HideAnswer(){
		answerGrid.gameObject.SetActive(false);
	}

	/// <summary>
	/// 問題とする箇所を決定
	/// </summary>
	/// <param name="quizData"></param>
	private void SetQuizMask(GameManager.QuizData quizData){
		if(quizData.level == 0 || quizData.level == 1 || quizData.level == 3){
			// 2番目固定
			maskId = 1;
		}else{
			// ランダムで決める
			maskId = Random.Range(0, 6);
			// エラー避け
			if(maskId == 2 && quizData.left3 == 0){
				// 左辺3
				maskId = 1;
			}
			if(maskId == 4 && quizData.right2 == 0){
				// 右辺2
				maskId = 1;
			}
			if(maskId == 5 && quizData.right3 == 0){
				// 右辺3
				maskId = 1;
			}
			if(maskId == 1 && quizData.left2 == 0){
				// 左辺2
				maskId = 0;
			}
		}
	}

	/// <summary>
	/// クイズ文字列の生成
	/// </summary>
	/// <param name="num"></param>
	/// <param name="builder"></param>
	private void SetQuizString(int num, bool isFirstNumber, bool isMask, ref StringBuilder builder){
		if(num != 0){
			// 数字文字列
			string numString = isMask ? "?" : Mathf.Abs(num).ToString();

			if(isFirstNumber){
				// 最初の番号
				builder.Append(numString + " ");
			}else{
				if(num > 0){
					// +符号を追加
					builder.Append("+ " + numString + " ");
				}else if(num < 0){
					// -符号を追加
					builder.Append("- " + numString + " ");
				}
			}
		}
	}

	/// <summary>
	/// 解答を設定
	/// </summary>
	/// <param name="quizData"></param>
	private void SetAnswer(GameManager.QuizData quizData){
		// 正解の取得
		int answer = 0;
		if(maskId == 0){	answer = quizData.left1;	}
		if(maskId == 1){	answer = quizData.left2;	}
		if(maskId == 2){	answer = quizData.left3;	}
		if(maskId == 3){	answer = quizData.right1;	}
		if(maskId == 4){	answer = quizData.right2;	}
		if(maskId == 5){	answer = quizData.right3;	}
		// 正解を選択肢に追加
		answerList.Add(answer);

		// 選択肢数の取得
		int answerNum = GetAnswerNum(quizData.level);
		if(answerNum == 6){
			answerGrid.constraintCount = 3;
		}else if(answerNum == 7 || answerNum == 8){
			answerGrid.constraintCount = 4;
		}
		
		// 選択肢の生成
		while(answerList.Count < answerNum){
			int num = Random.Range(0, quizData.maxRange) + 1;
			if(! answerList.Contains(num) && ! answerList.Contains(-num)){
				// 答えに追加
				answerList.Add(num);
			}
		}

		// 選択肢のシャッフル
		answerList = ShuffleList.Shuffle(answerList);

		// 配置
		for(int i = 0; i < answerList.Count; i++){
			GameObject obj = GameObject.Instantiate(answerButtonPrefab, answerGrid.transform);
			obj.GetComponent<AnswerButton>().Init(answerList[i], answerList[i] == answer, (result) => {
				if(result){
					onOk();
				}
			});
		}
	}

	/// <summary>
	/// 選択肢の数を取得
	/// </summary>
	/// <param name="level"></param>
	/// <returns></returns>
	private int GetAnswerNum(int level){
		switch(level){
		case 0:
		default:
			return 2;
		case 1:
		case 3:
			return 3;
		case 2:
		case 4:
		case 5:
			return 4;
		case 6:
			return 5;
		case 7:
			return 6;
		case 8:
			return 7;
		case 9:
			return 8;
		case 10:
			return 10;
		}
	}
}
