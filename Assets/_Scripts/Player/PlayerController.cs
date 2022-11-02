using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Entity, IDamageable
{

    public static Transform playerTransform;
    private InputActions _inputs;
    private AudioSource _audioSource;
    [SerializeField] private GameObject normal, off;

    [SerializeField] private GameObject shield;
    [Range(0.0f, 1f)]
    public float defensiveSpeedMultiplier = 0.66f;
    public bool defensiveStance = false;
    private float invincibilityTimer;

    public LayerMask collisionLayerMask;

    protected override void Awake()
    {
        base.Awake();

        _audioSource = GetComponentInChildren<AudioSource>();
        playerTransform = transform;
    }

    private void OnEnable()
    {
        _inputs = new InputActions();

        _inputs.Player.Enable();

        _inputs.Player.Move.performed += ProcessMovement;
        _inputs.Player.Move.canceled += ProcessMovement;

        _inputs.Player.Action.started += ProcessAction;
        _inputs.Player.Action.canceled += ProcessAction;

        OnDeath += () =>
        {
            StartCoroutine(Test());
            IEnumerator Test()
            {
                yield return new WaitForSeconds(3f);

                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            }
        };
    }

    private void OnDisable()
    {
        _inputs.Player.Move.performed -= ProcessMovement;
        _inputs.Player.Move.canceled -= ProcessMovement;

        _inputs.Player.Action.performed += ProcessAction;
        _inputs.Player.Action.performed -= ProcessAction;

        _inputs.Player.Disable();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (state == State.DEATH) return;

        if (state != State.KNOCKBACKED)
        {
            Move();
            FacingDirection();
        }

        shield.SetActive(defensiveStance);
    }

    private void CheckCollisions()
    {
        //Collider[] colliders = Physics.OverlapBox(_collider.bounds.center, _collider.size / 2f, Quaternion.identity, collisionLayerMask);
        float offset = _collider.height / 2 - _collider.radius;
        Vector3 direction = new Vector3 { [_collider.direction] = 1 };
        Vector3 firstPoint = transform.TransformPoint(_collider.center - direction * offset);
        Vector3 secondPoint = transform.TransformPoint(_collider.center + direction * offset);

        /*Collider[] colliders = Physics.OverlapCapsule(firstPoint, secondPoint, _collider.radius, collisionLayerMask);

        if (colliders.Length > 0)
            OnCollision(colliders);*/
    }

    void ProcessMovement(InputAction.CallbackContext context)
    {
        Vector2 inputVector = context.ReadValue<Vector2>();

        moveDirection = new Vector3(inputVector.x, 0f, inputVector.y);
    }

    void ProcessAction(InputAction.CallbackContext context)
    {
        if (context.started)
            defensiveStance = true;
        else
            defensiveStance = false;
    }

    public void ApplyDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            health = 0;
            Death();
        }

        UIController.Instance.UpdateHealthbar(health, maxHealth);
    }

    public override void ApplyKnockback(Vector3 direction, float strength)
    {
        if (defensiveStance) return;

        base.ApplyKnockback(direction, strength);
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        switch (collision.transform.tag)
        {
            case "Enemy":
                OnEnemyCollision(collision.transform.GetComponent<Enemy>());
                break;
        }
    }

    private void OnEnemyCollision(Enemy enemy)
    {
        Collider[] enemyHits = new Collider[1];
        Collider[] playerHits = new Collider[3];

        Physics.OverlapBoxNonAlloc(_rb.position + _model.forward, Vector3.one / 2, playerHits, Quaternion.identity, LayerMask.GetMask("Enemy"));

        bool wasPlayerHit = Physics.OverlapBoxNonAlloc(enemy.transform.position + enemy.GetModelTransform().forward, Vector3.one / 2, enemyHits, Quaternion.identity, LayerMask.GetMask("Player")) >= 1 ? true : false;
        bool wasEnemyHit = false;

        foreach (Collider hit in playerHits)
        {
            if (!hit) break;

            if (hit.gameObject == enemy.gameObject) wasEnemyHit = true; break;
        }

        if (!wasPlayerHit && !wasEnemyHit) return;

        if (wasEnemyHit)
            enemy.OnPlayerCollision(this);

        if (wasPlayerHit)
        {
            if (wasEnemyHit)
            {
                Vector3 cross = Vector3.Cross(_model.forward, enemy.transform.position - _rb.position);
                float side = Vector3.Dot(cross, _model.up);

                if (side > .3f || side < -.3f) return;
            }

            UIController.Instance.effectSpawner.ShowHitEffect(_rb.position, true);

            ApplyKnockback(Helpers.AngleToVector3((int)enemy.direction), enemy.knockbackStrength);
            ApplyDamage(enemy.damage);
            CameraController.Instance.Shake(.1f, .2f);
        }

        /*bool isFrontHit = Helpers.IsFrontalHit(direction, enemy.direction);

        if (isFrontHit)
        {
            Vector3 cross = Vector3.Cross(_model.forward, enemy.transform.position - _rb.position);
            float side = Vector3.Dot(cross, _model.up);

            if (side < .3f && side > -.3f)
            {
                ApplyKnockback(Helpers.AngleToVector3((int)enemy.direction), enemy.knockbackStrength);
                ApplyDamage(enemy.damage);
                CameraController.Instance.Shake(.2f, .2f);
            }

            enemy.OnPlayerCollision(this);

            return;
        }*/

    }

    private void Move()
    {
        float moveSpeed = defensiveStance ? base.moveSpeed * defensiveSpeedMultiplier : base.moveSpeed;

        bool leftRayHit = Physics.Raycast(_rb.position + _model.right, _model.forward, 1f, LayerMask.GetMask("Walls"));
        bool rightRayHit = Physics.Raycast(_rb.position - _model.right, _model.forward, 1f, LayerMask.GetMask("Walls"));

        /*if (!leftRayHit && !rightRayHit)
            _rb.velocity = move * moveSpeed * Time.fixedDeltaTime;
        else
            _rb.velocity = Vector3.zero;*/

        isMoving = moveDirection != Vector3.zero ? true : false;

        transform.position += moveDirection * moveSpeed * Time.fixedDeltaTime;
    }
}
