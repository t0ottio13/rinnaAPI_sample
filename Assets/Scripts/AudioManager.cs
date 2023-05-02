using System.Collections;
using UnityEngine;
using AAA.Rinna;
using UnityEngine.Networking;
using System;

public class AudioManager : MonoBehaviour
{
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GameObject.Find("Audio Source").GetComponent<AudioSource>();
        PlayAudio();
    }

    public async void PlayAudio()
    {
        // rinnaAPIで音声を作成する
        var responceMessage = "りんなエーピーアイをお試し中！";
        var rinnaAPIConnection = new RinnaAPIConnection("{ご自身のサブスクリプションキー}");
        var resRinna = await rinnaAPIConnection.RequestAsync(responceMessage);
        var audioUrl = resRinna.mediaContentUrl;

        // urlから音声を取得し、再生する
        StartCoroutine(SoundAudioClip(audioUrl));
        Debug.Log("生成された音声が再生されました。");
    }

    IEnumerator SoundAudioClip(string url)
    {
        // UnityWebRequest を作成
        var www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV);

        // リクエストを送信し、完了するまで待機
        yield return www.SendWebRequest();

        // エラーがなければ
        if (www.result == UnityWebRequest.Result.Success)
        {
            // ダウンロードしたデータを AudioClip に変換
            var responceAudioClip = DownloadHandlerAudioClip.GetContent(www);

            // 音声を再生する
            audioSource.PlayOneShot(responceAudioClip);
        }
        else
        {
            // エラーをログに出力
            Debug.Log(www.error);
            throw new Exception();
        }
    }
}
