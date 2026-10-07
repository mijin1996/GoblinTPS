using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class UIM : MonoBehaviour {
    public static UIM instance {
        get {
            if (uim == null) {
                uim = FindObjectOfType<UIM>();
            }
            return uim;
        }
    }

    private static UIM uim;
    public Text ammoText;
    public Text scoreText;
    public Text waveText;
    public GameObject gameoverUI;

    [SerializeField] private SceneChange s;

    public Text wavePopText;
    private float fadeTime = 0.5f;
    private float showTime = 1.5f; 







    public void UpdateAmmoText(int magAmmo, int remainAmmo) {
        ammoText.text = magAmmo + "/" + remainAmmo;
    }

    public void UpdateScoreText(int newScore) {
        scoreText.text = "Score : " + newScore;
        scoreText.GetComponent<ITextPopEffect>()?.PlayPop();
        scoreText.GetComponent<ITextBlinkEffect>()?.PlayBlink();
    }

    public void UpdateWaveText(int waves, int cnt) {
        waveText.text = "Wave : " + waves + "\nEnemy Left : " + cnt;
    }

    //wave, enemy 수 증가 시 깜빡 효과 📌
    public void BlinkWaveText() {
        waveText.GetComponent<ITextBlinkEffect>()?.PlayBlink();
    }

    public void ShowWavePopup(int wave) {
        wavePopText.text = "WAVE " + wave;
        StopCoroutine("WavePopup");
        StartCoroutine("WavePopup");
    }

    private IEnumerator WavePopup() {
        Color c = wavePopText.color;

        // 페이드 인
        for (float t = 0f; t < fadeTime; t += Time.deltaTime) {
            c.a = t / fadeTime;
            wavePopText.color = c;
            yield return null;
        }
        c.a = 1f;
        wavePopText.color = c;

        yield return new WaitForSeconds(showTime);

        // 페이드 아웃
        for (float t = 0f; t < fadeTime; t += Time.deltaTime) {
            c.a = 1f - t / fadeTime;
            wavePopText.color = c;
            yield return null;
        }
        c.a = 0f;
        wavePopText.color = c;
    }

    private IEnumerator Blink() {
        Color original = Color.white;          // 글자의 원래 색

        for (int i = 0; i < 3; i++) {
            waveText.color = Color.red;        // 빨간색으로
            yield return new WaitForSeconds(0.2f);
            waveText.color = original;         // 원래 색으로
            yield return new WaitForSeconds(0.2f);
        }
    }


    public void SetActiveGameoverUI(bool active) {
        gameoverUI.SetActive(active);
        SoundManager.instance.PlayGameoverBGM();
    }
    
//-----------------------------------------------------------------------------
    public void GameRestart() {
        SoundManager.instance.CoroutainMethod(() => s.Change(SceneManager.GetActiveScene().buildIndex));
    }

    public void GameStart() {        
        SoundManager.instance.CoroutainMethod(() => s.Change(1)); //항상 정해진 씬(Main = 1)으로 가야 하니까 고정값 1
    }

    public void GameHome() {        
        SoundManager.instance.CoroutainMethod(() => s.Change(0)); //항상 정해진 씬(Main = 1)으로 가야 하니까 고정값 1
    }



    public void QuitGame() {
        SoundManager.instance.CoroutainMethod(() => {
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #else
            Application.Quit();
    #endif
        });
    }
    
}
