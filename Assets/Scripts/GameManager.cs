using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	/// <summary>開始時点の制限時間</summary>
	[SerializeField] private float startTime = 10.0f;
	/// <summary>1問正解ごとの追加時間</summary>
	[SerializeField] private float addTime = 1.0f;
	/// <summary>レベルアップの境界</summary>
	[SerializeField] private List<int> levelupBorder;
	/// <summary>問題の事前生成数</summary>
	[SerializeField] private int preStackQuizCount = 10;
	/// <summary>問題の最大数</summary>
	[SerializeField] private int maxQuiz = 100;

	/// <summary>回答数</summary>
	private int answerPoint = 0;
	/// <summary>スコア</summary>
	private int score = 0;
	/// <summary>制限時間</summary>
	private float time = 0;

	/// <summary>制限時間テキスト</summary>
	[SerializeField] private Text timeText;
	/// <summary>追加制限時間オブジェクト</summary>
	[SerializeField] private GameObject addTimeObj;
	/// <summary>問題数テキスト</summary>
	[SerializeField] private Text numberText;

	/// <summary>チュートリアル用のクイズ</summary>
	[SerializeField] private GameObject tutorialQuiz;
	/// <summary>クイズプレハブ</summary>
	[SerializeField] private GameObject QuizPrefab;

	/// <summary>BGMソース</summary>
	[SerializeField] private AudioSource bgmSource;
	/// <summary>BGM</summary>
	[SerializeField] private AudioClip bgmClip;
	/// <summary>BGM</summary>
	[SerializeField] private AudioClip titleBgmClip;

	/// <summary>SE再生</summary>
	[SerializeField] private AudioSource seSource;
	/// <summary>カウントダウンSE</summary>
	[SerializeField] private AudioClip countdownClip;
	/// <summary>タイムアップSE</summary>
	[SerializeField] private AudioClip timeupClip;

	/// <summary>現在のクイズ</summary>
	private GameObject nowQuizObj;
	/// <summary>スタックしておくクイズ</summary>
	private Queue<GameObject> stackQuiz = new Queue<GameObject>();

	/// <summary>タイマーストップ</summary>
	private bool isTimerStop;

	/// <summary>CSVファイル</summary>
	[SerializeField] private TextAsset csvFile;

	/// <summary>開始表示</summary>
	[SerializeField] private GameObject startObj;
	/// <summary>終了表示</summary>
	[SerializeField] private GameObject finishObj;

	/// <summary>リザルト表示</summary>
	[SerializeField] private GameObject resultObj;

	/// <summary>スコアテキスト</summary>
	[SerializeField] private Text scoreText;
	/// <summary>ボーナスオブジェクト</summary>
	[SerializeField] private GameObject bonusObj;

	/// <summary>タイトルに戻るボタン</summary>
	[SerializeField] private Button gotoTitleButton;

	/// <summary>終了</summary>
	private UnityAction onBackTitle = null;

	public class QuizData{
		/// <summary>ID</summary>
		public int id;
		/// <summary>レベル</summary>
		public int level;
		/// <summary>左辺</summary>
		public int left1, left2, left3;
		/// <summary>右辺</summary>
		public int right1, right2, right3;
		/// <summary>乱数の最大範囲</summary>
		public int maxRange;

		/// <summary>
		/// 初期化
		/// </summary>
		/// <param name="dataText"></param>
		public QuizData(string dataText){
			string[] data = dataText.Split(',');
			// データの格納
			id = int.Parse(data[0]);
			level = int.Parse(data[1]);
			left1 = int.Parse(data[2]);
			left2 = int.Parse(data[3]);
			left3 = int.Parse(data[4]);
			right1 = int.Parse(data[5]);
			right2 = int.Parse(data[6]);
			right3 = int.Parse(data[7]);
			maxRange = int.Parse(data[8]);
		}
	}
	/// <summary>クイズリスト</summary>
	/// <typeparam name="QuizData"></typeparam>
	/// <returns></returns>
	List<QuizData> quizDataList = null;

	void Start(){
		// タイトルに戻る
		gotoTitleButton.onClick.AddListener(() => {
			onBackTitle();
		});
	}

	/// <summary>
	/// 初期化
	/// </summary>
	public void Init(UnityAction onBackTitle) {
		this.onBackTitle = onBackTitle;
	}

	public void GameStart(bool isTutorial){
		answerPoint = 0;
		time = startTime;

		if(nowQuizObj != null){
			Destroy(nowQuizObj);
		}
		// 古いキューとGameObjectを破棄
		while(stackQuiz.Count > 0){
			Destroy(stackQuiz.Dequeue());
		}
		stackQuiz.Clear();

		// 時間の初期化
		timeText.text = time.ToString("F2");
		addTimeObj.SetActive(false);
		timeText.color = Color.gray;

		if(quizDataList == null){
			// CSVロード
			LoadCSV();
		}

		isTimerStop = true;

		startObj.SetActive(false);
		finishObj.SetActive(false);
		resultObj.SetActive(false);

		// クイズを10問分スタック
		for(int i = 0; i < preStackQuizCount; i++){
			CreateQuiz(i + 1);
		}

		if(isTutorial){
			// 問0を表示
			numberText.text = "問0";
			nowQuizObj = GameObject.Instantiate(tutorialQuiz, this.transform);
			nowQuizObj.GetComponent<Quiz>().Init(0, GetQuizData(0), EndTutorial);
		}else{
			// ゲーム開始
			StartCoroutine(ExecStartGame());
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(time <= 0.0f){
			return;
		}
		
		if(! isTimerStop){
			float preTime = time;
			time -= Time.deltaTime;

			if((preTime > 3.0f && time <= 3.0f) ||
			 (preTime > 2.0f && time <= 2.0f) ||
			 (preTime > 1.0f && time <= 1.0f)){
				// 残り時間少ない
				timeText.color = Color.red;
				seSource.PlayOneShot(countdownClip); 
			}

			if(time <= 0.0f){
				// 終了
				time = 0.0f;
				seSource.PlayOneShot(timeupClip);
				Finish();
			}
			// テキスト更新
			timeText.text = time.ToString("F2");
		}
	}

	/// <summary>
	/// 終了
	/// </summary>
	private void Finish(){
		bgmSource.Stop();

		// 終了表示
		finishObj.SetActive(true);
		// 解答を非表示
		nowQuizObj.GetComponent<Quiz>().HideAnswer();

		StartCoroutine(ExecFinish());
	}

	/// <summary>
	/// 終了待機
	/// </summary>
	/// <returns></returns>
	private IEnumerator ExecFinish(){
		yield return new WaitForSeconds(3.0f);

		// BGMを戻す
		bgmSource.clip = titleBgmClip;
		bgmSource.Play();

		resultObj.SetActive(true);

		// 正解数=スコア
		score = answerPoint;
		// 最大数正解していれば、制限時間×10をスコアに加算
		if(answerPoint == maxQuiz){
			score += Mathf.FloorToInt(time * 10);
		}

		scoreText.text = answerPoint + "問正解！";
		// 最大数正解していればボーナス文言表示
		bonusObj.SetActive(answerPoint == maxQuiz);
	}

	/// <summary>
	/// クイズを生成
	/// </summary>
	/// <param name="count"></param>
	private void CreateQuiz(int quizCount){
		int level = GetLevel(quizCount);

		// データを取得
		QuizData data = GetQuizData(level);
		// オブジェクトを画面外に生成
		GameObject obj = GameObject.Instantiate(QuizPrefab, this.transform);
		obj.transform.localPosition = new Vector3(1000.0f, 0.0f, 0.0f);
		obj.GetComponent<Quiz>().Init(quizCount, data, NextQuiz);
	
		// 問題をキューに追加
		stackQuiz.Enqueue(obj);
	}

	/// <summary>
	/// チュートリアル終了
	/// </summary>
	private void EndTutorial(){
		LeanTween.moveLocalX(nowQuizObj, -1000.0f, 0.5f)
		.setEaseInOutQuad()
		.setOnComplete(() => {
			Destroy(nowQuizObj, 1.0f);
			// ゲーム開始
			StartCoroutine(ExecStartGame());
		});
	}

	/// <summary>
	/// ゲーム開始
	/// </summary>
	/// <returns></returns>
	private IEnumerator ExecStartGame(){
		numberText.text = "";

		startObj.GetComponent<CanvasGroup>().alpha = 0.0f;

		yield return new WaitForSeconds(1.0f);

		// BGMセット
		bgmSource.clip = bgmClip;
		bgmSource.Play();

		startObj.SetActive(true);
		LeanTween.alphaCanvas(startObj.GetComponent<CanvasGroup>(), 1.0f, 0.5f).setEaseOutQuad();

		yield return new WaitForSeconds(2.5f);
		startObj.SetActive(false);

		// 次の問題へ
		NextQuiz();
	}

	/// <summary>
	/// 次の問題へ
	/// </summary>
	private void NextQuiz(){
		// 制限時間を停止
		isTimerStop = true;
		
		// アニメーション
		if(nowQuizObj != null){
			// 制限時間を追加（10秒を超えないように）
			timeText.color = Color.gray;
			addTimeObj.SetActive(true);
			LeanTween.moveLocalY(addTimeObj, 0.0f, 0.5f).setFrom(-40.0f).setEaseOutQuad().setOnComplete(() => {
				time = Mathf.Min(time + addTime, startTime);
				// テキスト更新
				timeText.text = time.ToString("F2");
				addTimeObj.SetActive(false);
			});

			LeanTween.moveLocalX(nowQuizObj, -1000.0f, 0.5f)
			.setEaseInOutQuad()
			.setOnComplete(() => {
				ShowNewQuiz();
			});
		}else{
			ShowNewQuiz();
		}
	}

	/// <summary>
	/// 次の問題を表示
	/// </summary>
	private void ShowNewQuiz(){
		// 問題を破棄
		if(nowQuizObj != null){
			Destroy(nowQuizObj, 1.0f);
			// 正解数を+1
			answerPoint++;
		}

		if(answerPoint == maxQuiz){
			// 100問で終了
			Finish();
			return;
		}

		// 問の更新
		numberText.text = "問" + (answerPoint + 1);

		// 次の問題を登場
		nowQuizObj = stackQuiz.Dequeue();
		LeanTween.moveLocalX(nowQuizObj, 0.0f, 0.5f)
		.setEaseInOutQuad()
		.setOnComplete(() => {
			// 制限時間減少開始
			isTimerStop = false;
			timeText.color = time > 3.0f ? Color.black : Color.red;
		});

		// スタック生成
		CreateQuiz(answerPoint + preStackQuizCount + 1);
	}


	/// <summary>
	/// 問題数からレベルを取得
	/// </summary>
	/// <param name="count"></param>
	/// <returns></returns>
	public int GetLevel(int count){
		int level = 0;
		for(int i = 0; i < levelupBorder.Count; i++){
			if(count >= levelupBorder[i]){
				level = i + 1;
			}else{
				break;
			}
		}

		return level;
	}

	/// <summary>
	/// クイズデータを取得
	/// </summary>
	/// <param name="level"></param>
	/// <returns></returns>
	private QuizData GetQuizData(int level){
		List<QuizData> dataList = quizDataList.FindAll(x => x.level == level);
		int rand = Random.Range(0, dataList.Count);
		return dataList[rand];
	}

	/// <summary>
	/// ロードCSV
	/// </summary>
	private void LoadCSV(){
		// クイズの初期化
		quizDataList = new List<QuizData>();
		// ファイルロード
		StringReader reader = new StringReader(csvFile.text);

		bool isHeader = true;
		// reader.Peaekが-1になるまで
		while(reader.Peek() != -1){
            string line = reader.ReadLine(); // 一行ずつ読み込み
			if(isHeader){
				// ヘッダ行は省略
				isHeader = false;
				continue;
			}
			// データ追加
			quizDataList.Add(new QuizData(line));
		}
	}
}
