using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance; // 싱글톤을 할당할 전역변수

    public AudioMixer mainMixer; // 오디오 믹서
    public Slider bgmBar; // BGM 볼륨 조절 슬라이더
    public Slider sfxBar; // SFX 볼륨 조절 슬라이더



    public List<AudioClip> audioClips; // 사용할 오디오 클립 리스트
    private Dictionary<string, AudioClip> audioDictionary; // 오디오 클립을 저장할 딕셔너리

    private AudioSource bgmSource; // 배경음악 오디오 소스
    private AudioSource sfxSource; // 효과음 오디오 소스

    public AudioMixerGroup bgmMixerGroup; // 배경음악 오디오 믹서 그룹
    public AudioMixerGroup sfxMixerGroup; // 효과음 오디오 믹서 그룹

    private string currentBGM; // 현재 재생중인 배경음악 이름


    // 싱글톤 패턴
    private void Awake()
    {
        // 싱글톤 변수가 비어있으면
        if (instance == null)
        {
            // 자신을 할당
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 SoundManager가 삭제되지 않음
        }
        // 싱글톤 변수가 비어있지 않으면
        else
        {
            // 자신을 파괴
            Destroy(gameObject);
        }



        // 딕셔너리 할당
        audioDictionary = new Dictionary<string, AudioClip>();
        foreach (AudioClip clip in audioClips)
        {
            // 오디오 클립의 이름을 키로 오디오 클립을 딕셔너리에 등록
            audioDictionary.Add(clip.name, clip);
        }

        // 사운드 매니저에 오디오 소스 컴포넌트 부여
        bgmSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start()
    {
        // 볼륨 슬라이더 값 초기화
        bgmBar.value = PlayerPrefs.GetFloat("BGM_Volume", 0.5f); // 두 번째 인수는 초기 값
        sfxBar.value = PlayerPrefs.GetFloat("SFX_Volume", 0.5f);

        // 볼륨 초기화
        mainMixer.SetFloat("BGM_Volume", Mathf.Log10(PlayerPrefs.GetFloat("BGM_Volume", 0.5f)) * 20);
        mainMixer.SetFloat("SFX_Volume", Mathf.Log10(PlayerPrefs.GetFloat("SFX_Volume", 0.5f)) * 20);
    }

    public void SetVolume_BGM(float volume)
    {
        mainMixer.SetFloat("BGM_Volume", Mathf.Log10(volume) * 20); // 볼륨 조절
        PlayerPrefs.SetFloat("BGM_Volume", volume); // 볼륨 설정 값 저장
    }

    public void SetVolme_SFX(float volume)
    {
        mainMixer.SetFloat("SFX_Volume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFX_Volume", volume);
    }

    // 효과음 재생 함수
    public void PlaySound(string soundName)
    {
        // Key가 존재한다면 오디오 재생
        if (audioDictionary.ContainsKey(soundName))
        {
            AudioClip clip = audioDictionary[soundName];

            sfxSource.outputAudioMixerGroup = sfxMixerGroup; // 오디오 믹서 그룹 할당

            sfxSource.PlayOneShot(clip); // 효과음 재생
        }
        else
        {
            Debug.LogWarning("다음 이름의 효과음을 찾을 수 없음: " + soundName);
        }
    }

    // 배경음악 재생 함수
    public void PlayBackgroundMusic(string musicName)
    {
        // Key가 존재한다면 오디오 재생
        if (audioDictionary.ContainsKey(musicName))
        {
            // 현재 재생 중인 BGM과 다른 BGM일 때만 재생
            if (currentBGM != musicName)
            {
                AudioClip clip = audioDictionary[musicName];
                bgmSource.clip = clip;
                bgmSource.loop = true; // 배경음악 루프

                bgmSource.outputAudioMixerGroup = bgmMixerGroup; // 오디오 믹서 그룹 할당

                currentBGM = musicName; // 현재 재생 중인 BGM 이름 변경

                bgmSource.Play();
            }
        }
        else
        {
            Debug.LogWarning("다음 이름의 배경음악을 찾을 수 없음: " + musicName);
        }
    }

    // 배경음악 중단 함수
    public void StopBackgroundMusic()
    {
        bgmSource.Stop();
    }
}
