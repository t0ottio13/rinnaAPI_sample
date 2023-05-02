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
        /// rinnaAPI�Ńe�L�X�g�������ɕϊ�����
        /// </summary>
        /// <param name="textMessage">�ϊ��������e�L�X�g</param>
        /// <returns>rinnaAPI���X�|���X���f��</returns>
        public async UniTask<RinnaAPIResponseModel> RequestAsync(string textMessage)
        {
            // ��������rinnaAPI�̃G���h�|�C���g��ݒ�
            var apiUrl = "https://api.rinna.co.jp/models/cttse/v2";

            // rinnaAPI�ɑ��M���郊�N�G�X�g�̏���
            var options = new RinnaAPIRequestModel()
            {
                sid = 27,
                tid = 1,
                speed = 1.0,
                text = textMessage,
                volume = 10.0,
                format = "wav"
            };
            Debug.Log("RinnAPI�ɑ��M����e�L�X�g: " + textMessage);

            // ���N�G�X�g��JSON�ɕϊ�
            var jsonOptions = JsonUtility.ToJson(options);
            var postData = Encoding.UTF8.GetBytes(jsonOptions);
            Debug.Log("RinnAPI�ɑ��M���郊�N�G�X�g: " + jsonOptions);

            // ���N�G�X�g���쐬
            using var request = new UnityWebRequest(apiUrl, "POST")
            {
                uploadHandler = new UploadHandlerRaw(postData),
                downloadHandler = new DownloadHandlerBuffer()
            };

            //rinnaAPI��API���N�G�X�g�ɕK�v�ȃw�b�_�[����ݒ�
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

            // rinnaAPI��API���N�G�X�g�𑗐M���A���ʂ�ϐ��Ɋi�[
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
                Debug.Log("�쐬���ꂽ������URL:" + responseMediaURL);
                return responseObject;
            }
        }
    }

    /// <summary>
    /// rinnnaAPI�̃��N�G�X�g���f��
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
    /// rinnnaAPI�̃��X�|���X���f��
    /// </summary>
    [Serializable]
    public class RinnaAPIResponseModel
    {
        public string mediaContentUrl;
        public string type;
    }
}

