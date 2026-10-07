using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 정해진 색으로 깜빡이는 효과. 글자와 그림 어디에든 붙일 수 있다
public class BlinkEffect : MonoBehaviour, ITextBlinkEffect {
    public Color blinkColor = Color.red;   // 깜빡일 때의 색
    public float blinkTime = 0.2f;         // 한 번 바뀌는 시간
    public int repeat = 3;                 // 반복 횟수

    private Graphic graphic;
    private Color originalColor;

    private void Awake() {
        graphic = GetComponent<Graphic>();
        originalColor = graphic.color;
    }

    public void PlayBlink() {
        StopAllCoroutines();
        graphic.color = originalColor;
        StartCoroutine(Blink());
    }

    private IEnumerator Blink() {
        for (int i = 0; i < repeat; i++) {
            graphic.color = blinkColor;
            yield return new WaitForSeconds(blinkTime);
            graphic.color = originalColor;
            yield return new WaitForSeconds(blinkTime);
        }
    }
}