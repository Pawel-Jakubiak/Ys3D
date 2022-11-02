using System.Collections;
using UnityEngine;


[RequireComponent(typeof(AISensor))]
public class Enemy : Entity, IDamageable
{
    public float wanderRadius;
    public float idleTime;

    private Vector3 spawnPosition;
    //private Vector3 movePosition;
    private AISensor _aiSensor;

    [SerializeField] private Ability ability;
    private float abilityTimer = 5f;
    private bool isChanneling = false;
    private float channelTimer = 0f;
    private float immuneTimer = 0f;

    private HealthbarScript _healthbar;

    // Coroutines
    private Coroutine pathUpdate;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (state == State.DEATH) return;

        if (state != State.KNOCKBACKED && !isChanneling)
        {
            if (_aiSensor.target && ability != null)
                UseAbility();

            FacingDirection();
            Move();
        }

        if (abilityTimer > 0f && _aiSensor.target != null)
            abilityTimer -= Time.fixedDeltaTime;

        if (channelTimer > 0f)
        {
            channelTimer -= Time.fixedDeltaTime;
            state = State.IDLE;
        }
        else
            isChanneling = false;

        if (immuneTimer > 0f)
            immuneTimer -= Time.fixedDeltaTime;
    }

    private void UseAbility()
    {
        if (abilityTimer > 0f) return;

        abilityTimer = ability.delay;

        if (ability.channelTime > 0)
        {
            channelTimer = ability.channelTime;
            isChanneling = true;
        }

        ability.Use(transform, _aiSensor.target);
    }

    private void Move()
    {
        if (!isMoving) return;

        _rb.position += _model.forward * moveSpeed * Time.fixedDeltaTime;
    }

    private IEnumerator UpdateWanderPath()
    {
        Vector3 targetPosition, targetDir;
        float angleToTarget, distance;
        int moveAngle;

        while (true)
        {
            Vector3 random = new Vector3(Random.insideUnitCircle.x, 0f, Random.insideUnitCircle.y);
            targetPosition = spawnPosition + random * wanderRadius;

            targetDir = (targetPosition - _rb.position).normalized;

            angleToTarget = Mathf.Atan2(targetDir.x, targetDir.z) * Mathf.Rad2Deg;
            angleToTarget = (angleToTarget + 360) % 360;
            moveAngle = (int)(Mathf.Round(angleToTarget / 45) * 45f);

            moveDirection = Helpers.AngleToVector3(moveAngle);

            isMoving = true;

            distance = Vector3.Distance(targetPosition, _rb.position);

            while (true)
            {
                float currentDistance = Vector3.Distance(targetPosition, _rb.position);

                if (distance > currentDistance)
                {
                    distance = currentDistance;
                } 
                else
                {
                    targetDir = (targetPosition - _rb.position).normalized;

                    angleToTarget = Mathf.Atan2(targetDir.x, targetDir.z) * Mathf.Rad2Deg;
                    angleToTarget = (angleToTarget + 360) % 360;
                    moveAngle = (int)(Mathf.Round(angleToTarget / 45) * 45f);

                    moveDirection = Helpers.AngleToVector3(moveAngle);
                }

                if (distance <= .1f)
                {
                    isMoving = false;
                    break;
                }

                yield return new WaitForFixedUpdate();
            }

            yield return new WaitForSeconds(idleTime);
        }
    }

    private void SetTargetPath()
    {
        StopCoroutine(pathUpdate);

        pathUpdate = StartCoroutine(UpdatePath());

        IEnumerator UpdatePath()
        {
            Vector3 targetDir;
            float angleToTarget;
            int moveAngle;

            while (true)
            {
                targetDir = (_aiSensor.target.position - _rb.position).normalized;

                angleToTarget = Mathf.Atan2(targetDir.x, targetDir.z) * Mathf.Rad2Deg;
                angleToTarget = (angleToTarget + 360) % 360;
                moveAngle = (int)(Mathf.Round(angleToTarget / 45) * 45f);

                moveDirection = Helpers.AngleToVector3(moveAngle);

                isMoving = true;

                yield return new WaitForSeconds(1f);
            }
        }
    }

    public void ApplyDamage(int damage)
    {
        if (ability != null && abilityTimer < ability.recoveryTime)
            abilityTimer = ability.recoveryTime;

        health -= damage;

        if (health <= 0)
        {
            health = 0;
            Death();
        }

        _healthbar.UpdateHealthbar(health, maxHealth);
    }

    public override void ApplyKnockback(Vector3 direction, float strength)
    {
        base.ApplyKnockback(direction, strength);
        immuneTimer = GameSettings.immuneTime;
    }

    public virtual void OnPlayerCollision(PlayerController player)
    {
        UIController.Instance.effectSpawner.ShowHitEffect(_rb.position);

        ApplyKnockback(Helpers.AngleToVector3((int)player.direction), player.knockbackStrength);
        ApplyDamage(player.damage);
    }

    /*protected override void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        PlayerController player = collision.gameObject.GetComponent<PlayerController>();

        Collider[] enemyHits = new Collider[1];
        Collider[] playerHits = new Collider[5];

        Physics.OverlapBoxNonAlloc(player.transform.position + player.GetModelTransform().forward, Vector3.one / 2, playerHits, Quaternion.identity, LayerMask.GetMask("Enemy"));

        bool isPlayerHit = Physics.OverlapBoxNonAlloc(_rb.position + _model.forward, Vector3.one / 2, enemyHits, Quaternion.identity, LayerMask.GetMask("Player")) >= 1 ? true : false;
        bool isEnemyHit = false;

        foreach (Collider hit in playerHits)
            if (hit == _collider) isEnemyHit = true;

        if (!isPlayerHit && !isEnemyHit) return;

        bool isCritical = true;

        if (isEnemyHit)
        {
            ApplyDamage(player.damage);
            ApplyKnockback(Helpers.AngleToVector3((int)player.direction), player.knockbackStrength);
        }

        if (isPlayerHit)
        {
            bool isSideHit = false;

            if (isEnemyHit)
            {
                Vector3 cross = Vector3.Cross(_model.forward, player.transform.position - _rb.position);
                float side = Vector3.Dot(cross, _model.up);

                if (side > .3f || side < -.3f) isSideHit = true;
            }

            if (!isSideHit)
            {
                player.ApplyKnockback(Helpers.AngleToVector3((int)direction), knockbackStrength);
                player.ApplyDamage(damage);

                isCritical = false;
            }
        }

        Vector3 center = Vector3.Lerp(_rb.position, player.transform.position, .5f);
        UIController.Instance.SpawnEffect(center, isCritical ? 1 : 0);

        if (!_aiSensor.playerFound) _aiSensor.SetTarget(player.transform);
    }*/

    public bool IsImmune() => immuneTimer > 0f ? true : false;

    protected override void Awake()
    {
        base.Awake();

        _aiSensor = GetComponent<AISensor>();
        _healthbar = GetComponentInChildren<HealthbarScript>();
        spawnPosition = transform.position;

        pathUpdate = StartCoroutine(UpdateWanderPath());
        _aiSensor.OnPlayerFound += SetTargetPath;

        OnDeath += () => _healthbar.gameObject.SetActive(false);

        OnDeath += () =>
        {
            if (ability) ability.Deactivate();
        };

        OnDeath += () =>
        {
            StopCoroutine(pathUpdate);
            _rb.velocity = Vector3.zero;
            StartCoroutine(HideCorpse());

            IEnumerator HideCorpse() {
                yield return new WaitForSeconds(5f);

                while (true)
                {
                    if (_rb.position.y < -1f) break;

                    _rb.position += Vector3.down / 100f;
                    yield return new WaitForFixedUpdate();
                }

                gameObject.SetActive(false);
            }
        };
    }

    private void OnEnable()
    {
        _collider.enabled = true;
    }

    private void OnDisable()
    {
        transform.position = spawnPosition;
    }
}
