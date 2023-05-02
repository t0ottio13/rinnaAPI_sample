using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace AAA.Rinna
{
    public class RinnaAPIConnection
    {
        private readonly string _subscriptionKey;

        public RinnaAPIConnection(string subscriptionKey)
        {
            _subscriptionKey = subscriptionKey;
        }

        /// <summary>
        /// rinnaAPIでテキストを音声に変換する
        /// </summary>
        /// <param name="textMessage">変換したいテキスト</param>
        /// <returns>rinnaAPIレスポンスモデル</returns>
        public async UniTask<RinnaAPIResponseModel> RequestAsync(string textMessage)
        {
            // 音声生成rinnaAPIのエンドポイントを設定
            var apiUrl = "https://api.rinna.co.jp/models/cttse/v2";

            // rinnaAPIに送信するリクエストの準備
            var options = new RinnaAPIRequestModel()
            {
                sid = 27,
                tid = 1,
                speed = 1.0,
                text = textMessage,
                volume = 10.0,
                format = "wav"
            };
            Debug.Log("RinnAPIに送信するテキスト: " + textMessage);

            // リクエストをJSONに変換
            var jsonOptions = JsonUtility.ToJson(options);
            var postData = Encoding.UTF8.GetBytes(jsonOptions);
            Debug.Log("RinnAPIに送信するリクエスト: " + jsonOptions);

            // リクエストを作成
            using var request = new UnityWebRequest(apiUrl, "POST")
            {
                uploadHandler = new UploadHandlerRaw(postData),
                downloadHandler = new DownloadHandlerBuffer()
            };

            //rinnaAPIのAPIリクエストに必要なヘッダー情報を設定
            var headers = new Dictionary<string, string>
            {
                {"Content-type", "application/json"},
                {"Cache-Control", "no-cache"},
                {"Ocp-Apim-Subscription-Key",  _subscriptionKey},
            };
            foreach (var header in headers)
            {
                request.SetRequestHeader(header.Key, header.Value);
            }

            // rinnaAPIにAPIリクエストを送信し、結果を変数に格納
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
                throw new Exception();
            }
            else
            {
                var responseString = request.downloadHandler.text;
                var responseObject = JsonUtility.FromJson<RinnaAPIResponseModel>(responseString);
                var responseMediaURL = responseObject.mediaContentUrl;
                Debug.Log("作成された音声のURL:" + responseMediaURL);
                return responseObject;
            }
        }
    }

    /// <summary>
    /// rinnnaAPIのリクエストモデル
    /// </summary>
    [Serializable]
    public class RinnaAPIRequestModel
    {
        public int sid;
        public int tid;
        public double speed;
        public string text;
        public double volume;
        public string format;
    }

    /// <summary>
    /// rinnnaAPIのレスポンスモデル
    /// </summary>
    [Serializable]
    public class RinnaAPIResponseModel
    {
        public string mediaContentUrl;
        public string type;
    }
}

