using UnityEngine;

// 수류탄 보유 개수를 늘려주는 아이템
public class GrenadePack : MonoBehaviour, IItem {
    public int grenadeAmount = 1; // 추가할 수류탄 수

    public void Use(GameObject target) {
        // 전달 받은 게임 오브젝트(플레이어)로부터 PlayerGrenade 컴포넌트를 가져오기 시도
        PlayerGrenade playerGrenade = target.GetComponent<PlayerGrenade>();

        if (playerGrenade != null)
        {
            // 보유 수류탄 수를 grenadeAmount 만큼 더한다 (최대 보유 개수까지)
            playerGrenade.AddGrenade(grenadeAmount);
        }

        // 사용되었으므로, 자신을 파괴
        Destroy(gameObject);
    }
}
