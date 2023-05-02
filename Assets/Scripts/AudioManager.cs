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
        // rinnaAPI�ŉ������쐬����
        var responceMessage = "���ȃG�[�s�[�A�C�����������I";
        var rinnaAPIConnection = new RinnaAPIConnection("{�����g�̃T�u�X�N���v�V�����L�[}");
        var resRinna = await rinnaAPIConnection.RequestAsync(responceMessage);
        var audioUrl = resRinna.mediaContentUrl;

        // url���特�����擾���A�Đ�����
        StartCoroutine(SoundAudioClip(audioUrl));
        Debug.Log("�������ꂽ�������Đ�����܂����B");
    }

    IEnumerator SoundAudioClip(string url)
    {
        // UnityWebRequest ���쐬
        var www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV);

        // ���N�G�X�g�𑗐M���A��������܂őҋ@
        yield return www.SendWebRequest();

        // �G���[���Ȃ����
        if (www.result == UnityWebRequest.Result.Success)
        {
            // �_�E�����[�h�����f�[�^�� AudioClip �ɕϊ�
            var responceAudioClip = DownloadHandlerAudioClip.GetContent(www);

            // �������Đ�����
            audioSource.PlayOneShot(responceAudioClip);
        }
        else
        {
            // �G���[�����O�ɏo��
            Debug.Log(www.error);
            throw new Exception();
        }
    }
}
