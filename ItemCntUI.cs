using UnityEngine;
using TMPro; // TextMeshPro 관련 코드
using UnityEngine.Serialization;

// 각 버튼 옆에 "내가 가진 개수"를 작은 숫자로 표시하는 스크립트
// 개수를 직접 관리하지 않고, 각 소유자(Gun, PlayerGrenade)의 값을 읽어서 보여주기만 한다
// 빈 오브젝트(예: ItemCountUI)에 부착하고 Inspector에서 연결
public class ItemCntUI : MonoBehaviour
{
    [Header("개수를 가져올 대상")]
    public Gun gun;                     // 탄약 개수를 가진 총
    public PlayerGrenade playerGrenade; // 수류탄 개수를 가진 플레이어

    [Header("각 버튼 옆에 표시할 텍스트 (비워두면 표시하지 않음)")]
    public TMP_Text fireCntText;    // 발사 버튼 — 현재 탄창 탄알 수
    public TMP_Text reloadCntText;  // 재장전 버튼 — 남은 전체 탄알 수
    public TMP_Text grenadeCntText; // 수류탄 버튼 — 보유 수류탄 수


    private int lastAmmo;
    private int lastGrenade; 

    private void Start() {
        lastAmmo = gun.ammoRemain;  
        lastGrenade = playerGrenade.startGrenadeCnt;
    }

    private void Update() {
        if (gun != null) {
            if (fireCntText != null) fireCntText.text = gun.magAmmo.ToString();
            if (reloadCntText != null) reloadCntText.text = gun.ammoRemain.ToString();

             if (gun.ammoRemain > lastAmmo) reloadCntText.GetComponent<ITextPopEffect>()?.PlayPop(); 
             lastAmmo = gun.ammoRemain; 
        }

        if (playerGrenade != null && grenadeCntText != null){
            grenadeCntText.text = playerGrenade.startGrenadeCnt.ToString();

            if (playerGrenade.startGrenadeCnt > lastGrenade) grenadeCntText.GetComponent<ITextPopEffect>()?.PlayPop();   // 추가 3
            lastGrenade = playerGrenade.startGrenadeCnt;
        }
    }
}
