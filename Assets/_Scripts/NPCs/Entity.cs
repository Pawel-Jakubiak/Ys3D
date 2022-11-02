using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour, IKnockbackable
{
    public int health;
    [SerializeField] protected int maxHealth;

    public int damage;

    public float moveSpeed = 2f;

    [SerializeField] private bool isKnockbackable = true;
    public float knockbackStrength = 5f;
    public float knockbackDuration = .2f;

    protected Coroutine knockbackCoroutine;
    protected float knockbackTimer;

    protected Vector3 moveDirection;
    protected bool isMoving = false;

    public State state { 
        set
        {
            _animator.SetInteger("State", (int)value);
            _state = value;
        }
        get { return _state; }
    }

    [SerializeField] protected State _state = State.IDLE;
    public Direction direction;

    [SerializeField] protected Transform _model;
    protected Animator _animator;
    protected CapsuleCollider _collider;
    protected Rigidbody _rb;

    protected bool isDead = false;
    public event Action OnDeath;

    protected void BasicDeath()
    {
        _collider.enabled = false;
        isMoving = false;
        isDead = true;
        state = State.DEATH;
    }

    protected void FacingDirection()
    {
        if (isMoving)
        {
            state = State.WALKING;
        }
        else
        {
            state = State.IDLE;
            return;
        }        

        float degrees = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
        degrees = (degrees + 360) % 360;

        direction = (Direction) degrees;
        _model.forward = Helpers.AngleToVector3((int)direction);
    }
        
    /*public virtual void ApplyKnockback(Vector3s direction, float strength)
    {
        if (state == State.DEATH || !isKnockbackable) return;

        float timer = 0f;

        StartCoroutine(Knockback());

        IEnumerator Knockback()
        {
            state = State.KNOCKBACKED;

            while (GameSettings.knockbackLength > timer)
            {
                timer += Time.fixedDeltaTime;
                transform.position += direction * strength;
                yield return new WaitForFixedUpdate();
            }

            state = State.IDLE;

            yield return null;
        }
    }
    */

    public virtual void ApplyKnockback(Vector3 direction, float strength)
    {
        if (state == State.DEATH || !isKnockbackable) return;

        _rb.AddForce((direction * strength) * 50f, ForceMode.Impulse);

        if (knockbackCoroutine != null)
            StopCoroutine(knockbackCoroutine);

        knockbackTimer = 0f;

        knockbackCoroutine = StartCoroutine(Knockback());

        IEnumerator Knockback()
        {
            state = State.KNOCKBACKED;

            while (GameSettings.knockbackDuration > knockbackTimer)
            {
                knockbackTimer += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            _rb.velocity = Vector3.zero;
            if (state != State.DEATH) state = State.IDLE;

            yield return null;
        }
    }

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
        _collider = GetComponent<CapsuleCollider>();
        _rb = GetComponent<Rigidbody>();

        OnDeath += BasicDeath;
    }

    private void OnEnable()
    {
        _model = transform.GetChild(0).transform;
    }

    protected virtual void FixedUpdate()
    {
    }

    protected void Death() => OnDeath.Invoke();

    protected virtual void OnCollisionEnter(Collision collision)
    {

    }

    public Transform GetModelTransform() => _model;

    public enum State
    {
        IDLE = 0,
        WALKING = 1,
        KNOCKBACKED = 3,
        DEATH = 4
    }

    public enum Direction
    {
        NORTH = 0,
        NORTHEAST = 45,
        EAST = 90,
        SOUTHEAST = 135,
        SOUTH = 180,
        SOUTHWEST = 225,
        WEST = 270,
        NORTHWEST = 315
    }
}
