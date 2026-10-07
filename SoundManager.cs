using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // SceneManager 쓰려면 필요


public class SoundManager : MonoBehaviour{
    public static SoundManager instance {
        get{
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (sm == null){
                sm = FindObjectOfType<SoundManager>();
            }
            return sm;
        }
    }

    private static SoundManager sm;

    [Header("Sound")]
    [SerializeField] private AudioSource bgm;
    [SerializeField] private AudioSource sfx;
    [SerializeField] private AudioClip sfxPlay;
    [SerializeField] private AudioClip bgmIntro;
    [SerializeField] private AudioClip bgmPlay;
    [SerializeField] private AudioClip GameoverBgm;
    
    

    private void Start() {
        AudioClip clip = bgmIntro != null ? bgmIntro : bgmPlay;
        if (clip != null) {
            bgm.clip = clip;
            bgm.loop = true;
            bgm.Play();
        }
    }

    // ---- 재사용 가능한 공용 코루틴 ----
    // "클릭음 재생 → 대기 → 넘겨받은 동작 실행"을 누구나 이 함수로 쓸 수 있음
    public void CoroutainMethod(System.Action action) {
        StartCoroutine(SfxAfterSceneChange(action));
    }

    private IEnumerator SfxAfterSceneChange(System.Action action) {
        Sfx();
        yield return new WaitForSeconds(sfxPlay.length);
        action?.Invoke();
    }
    //버튼클릭
    public void Sfx() {
        sfx.PlayOneShot(sfxPlay);
    }

    public void PlayGameoverBGM() {
        bgm.clip = GameoverBgm;
        bgm.Play();
    }
}