using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour {
    public enum State {
        Ready,
        Empty,
        Reloading
    }

    public State state { get; private set; }

    public Transform fireTransform;
    public ParticleSystem muzzleFx;
    public ParticleSystem shellFx;
    private LineRenderer bulletLine;
    private AudioSource gunSfx; //총 소리  재생기
    public GunData gunData;
    private float fireDistance = 50f;
    public int ammoRemain = 100;
    public int magAmmo;
    private float lastFireTime;
    public Camera scopeCam;
    


    private void Awake() {
        gunSfx = GetComponent<AudioSource>();
        bulletLine = GetComponent<LineRenderer>();
        bulletLine.positionCount = 2;
        bulletLine.enabled = false;
    }

    private void OnEnable() {
        ammoRemain = gunData.startAmmoRemain;
        magAmmo = gunData.magCapacity;
        state = State.Ready;
        lastFireTime = 0;
    }

    public void Fire() {
        if (state == State.Ready && Time.time >= lastFireTime + gunData.timeBetFire) {
            lastFireTime = Time.time;
            Shot();
        }
    }




    private void Shot() {
        Vector3 fireDir = fireTransform.forward;   // 평소: 원래대로 총구 방향

        // 스코프를 켰을 때만: 스코프 가운데(+)가 가리키는 곳으로
        if (scopeCam != null && scopeCam.gameObject.activeInHierarchy) {
            Ray aimRay = scopeCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            int mask = ~LayerMask.GetMask("Player");
            RaycastHit aimHit;
            Vector3 aimPoint = Physics.Raycast(aimRay, out aimHit, fireDistance, mask) ? aimHit.point : aimRay.GetPoint(fireDistance);
            fireDir = (aimPoint - fireTransform.position).normalized;
        }

        // 여기부터는 원래 코드 (fireTransform.forward 자리만 fireDir)
        RaycastHit hit;
        Vector3 hitPosition = Vector3.zero;
        if (Physics.Raycast(fireTransform.position, fireDir, out hit, fireDistance)) {
            IDamageable target = hit.collider.GetComponent<IDamageable>();
            if (target != null) {
                target.OnDamage(gunData.damage, hit.point, hit.normal);
            }
            hitPosition = hit.point;
        } else {
            hitPosition = fireTransform.position + fireDir * fireDistance;
        }
        StartCoroutine(ShotEffect(hitPosition));
        magAmmo--;
        if (magAmmo <= 0) {
            state = State.Empty;
        }
    }

    private IEnumerator ShotEffect(Vector3 hitPosition) {
        muzzleFx.Play();
        shellFx.Play();
        gunSfx.PlayOneShot(gunData.shotClip);
        bulletLine.SetPosition(0, fireTransform.position);
        bulletLine.SetPosition(1, hitPosition);
        bulletLine.enabled = true;
        yield return new WaitForSeconds(0.03f);
        bulletLine.enabled = false;
    }

    public bool Reload() {
        if (state == State.Reloading || ammoRemain <= 0 || magAmmo >= gunData.magCapacity) {
            return false;
        }
        StartCoroutine(ReloadRoutine());
        return true;
    }

    private IEnumerator ReloadRoutine() {
        state = State.Reloading;
        gunSfx.PlayOneShot(gunData.reloadClip);
        yield return new WaitForSeconds(gunData.reloadTime);
        int ammoToFill = gunData.magCapacity - magAmmo;
        if (ammoRemain < ammoToFill) {
            ammoToFill = ammoRemain;
        }
        magAmmo += ammoToFill;
        ammoRemain -= ammoToFill;
        state = State.Ready;
    }
}


/*
~ : 이것만"이 "이것만 빼고"로 바뀌어서 작동되도록
📌카메라가 뒤에 있고 조준점은 카메라 기준으로 되니까


*/