using UnityEngine;

public interface IItem {
    void Use(GameObject target);
}

/* 입력으로 받는 target은 아이템 효과가 적용될 대상 */
