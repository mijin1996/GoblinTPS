using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Goblin : LivingEntity {
    public LayerMask whatIsTarget;

    private LivingEntity targetEntity;
    private NavMeshAgent navAgent;

    public ParticleSystem hitFx;
    public AudioClip deathSound;
    public AudioClip hitSound;
    public AudioClip attackSfx;

    private Animator goblinAni;
    private AudioSource goblinSfx;
    private Renderer goblinRenderer;

    public float damage = 20f;
    public float timeBetAttack = 0.5f;
    private float lastAttackTime;

    public GameObject auraFx;  //바닥에서 발상하게 될 이펙트
    private GameObject aura;


    private bool hasTarget {
        get {
            if (targetEntity != null && !targetEntity.dead) {
                return true;
            }
            // 아래 return false;은 else if를 의미
            return false;
        }
    }

    private void Awake() {
        navAgent = GetComponent<NavMeshAgent>();
        goblinAni = GetComponent<Animator>();
        goblinSfx = GetComponent<AudioSource>();
        // 자식 컴퍼넌트에 렌더러 컴퍼넌트 있음
        goblinRenderer = GetComponentInChildren<Renderer>();
    }

    public void Setup(GoblinData goblinData) {
        startingHealth = goblinData.health;
        health = goblinData.health;
        damage = goblinData.damage;
        navAgent.speed = goblinData.speed;
        // 외형색을 변경할 수 있는 코드
        goblinRenderer.material.color = goblinData.skinColor;
    }

    private void Start() {
        StartCoroutine(UpdatePath());
        //고블린이 위치한 곳에 이팩트 아우라 계속해서 생성되도록 유지 
        //if (auraFx != null) aura = Instantiate(auraFx, transform);
        //위 코드는 드래그 앤 드롭일때에만 적용되는 코드
        //아래 코드는 원본프리팹에서 즉시 가져올때 적용되는 코드
        if (auraFx != null) {
            aura = Instantiate(auraFx, transform);
            foreach (ParticleSystem ps in aura.GetComponentsInChildren<ParticleSystem>()) { var m = ps.main; m.loop = true; }
        }
    }

    private void Update() {
        goblinAni.SetBool("HasTarget", hasTarget);
    }

    private IEnumerator UpdatePath() {
        while (!dead) {
            if (hasTarget) {
                navAgent.isStopped = false;
                navAgent.SetDestination(targetEntity.transform.position);
            } else {
                navAgent.isStopped = true;
                Collider[] colliders = Physics.OverlapSphere(transform.position, 20f, whatIsTarget);
                for (int i = 0; i < colliders.Length; i++) {
                    LivingEntity livingEntity = colliders[i].GetComponent<LivingEntity>();
                    if (livingEntity != null && !livingEntity.dead) {
                        targetEntity = livingEntity;
                        break;
                    }
                }
            }
            yield return new WaitForSeconds(0.25f);
        }
    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal) {
        if (!dead) {
            ParticleSystem blood = Instantiate(hitFx, hitPoint, Quaternion.LookRotation(hitNormal));
            blood.Play();
            Destroy(blood.gameObject, 1f);



            //hitFx.transform.position = hitPoint;
            //hitFx.transform.rotation = Quaternion.LookRotation(hitNormal);
            //hitFx.Play();
            if (hitSound != null) {
            goblinSfx.PlayOneShot(hitSound);
            }
            base.OnDamage(damage, hitPoint, hitNormal);
                
        }
       
    }

    public override void Die() {
        base.Die();
        Collider[] goblinCollliders = GetComponents<Collider>();
        for (int i = 0; i < goblinCollliders.Length; i++) {
            goblinCollliders[i].enabled = false;
        }
        navAgent.isStopped = true;
        navAgent.enabled = false;
        goblinAni.SetTrigger("Die");
        goblinSfx.PlayOneShot(deathSound);
        Destroy(aura);
    }

    private void OnTriggerStay(Collider other) {
        if (!dead && Time.time >= lastAttackTime + timeBetAttack) {
            LivingEntity attackTarget = other.GetComponent<LivingEntity>();
            if (attackTarget != null && attackTarget == targetEntity) {
                lastAttackTime = Time.time;
                goblinAni.SetTrigger("Attack");
                //attacked by player play sfx
                goblinSfx.PlayOneShot(attackSfx);
                
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                Vector3 hitNormal = transform.position - other.transform.position;
                attackTarget.OnDamage(damage, hitPoint, hitNormal);
            }
        }
    }
}
