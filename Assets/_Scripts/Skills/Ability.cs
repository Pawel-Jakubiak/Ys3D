using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : ScriptableObject
{
    protected MonoBehaviour _caller = GameController.instance;

    public float channelTime;
    public float recoveryTime;
    public float delay;

    protected Coroutine _coroutine;

    public virtual void Use(Vector3 position, Transform target) { }

    public virtual void Use(Transform transform, Transform target) { }

    public virtual void Deactivate()
    {
        if (_coroutine != null)
            GameController.instance.StopCoroutine(_coroutine);
    }
}
