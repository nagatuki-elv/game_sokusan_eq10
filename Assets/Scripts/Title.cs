using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;

public class Title : MonoBehaviour {
	/// <summary>スタートボタン</summary>
	[SerializeField] private Button StartButton;
	/// <summary>BGMソース</summary>
	[SerializeField] private AudioSource bgmSource;
	/// <summary>BGM</summary>
	[SerializeField] private AudioClip bgmClip;

	/// <summary>SEチェックソース</summary>
	[SerializeField] private AudioSource seCheckSource;

	/// <summary>タイトルテキストのCanvasGroup</summary>
	[SerializeField] private CanvasGroup titleCanvasGroup;

	/// <summary>BGMスライダー</summary>
	[SerializeField] private Slider bgmVolume;
	/// <summary>SEスライダー</summary>
	[SerializeField] private Slider seVolume;
	/// <summary>AudioMixer</summary>
	[SerializeField] private AudioMixer audioMixer;

	/// <summary>スタート事項イベント</summary>
	UnityAction onStart;

	void Start(){
		// ボタンイベント
		StartButton.onClick.AddListener(onStart);

		bgmVolume.onValueChanged.AddListener((val) => {
			audioMixer.SetFloat("BGM", val);
		});
		seVolume.onValueChanged.AddListener((val) => {
			audioMixer.SetFloat("SE", val);
			seCheckSource.Play();
		});

		LeanTween.alphaCanvas(titleCanvasGroup, 0.0f, 1.0f).setLoopPingPong().setEaseInOutQuad();
	}

	public void Init(UnityAction onStart){
		this.onStart = onStart;
		// BGMセット
		bgmSource.clip = bgmClip;
		bgmSource.Play();
	}
}
