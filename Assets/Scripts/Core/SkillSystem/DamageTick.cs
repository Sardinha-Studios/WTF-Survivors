using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.TopDownEngine;

public class DamageTick : MonoBehaviour
{
    [SerializeField] private string damageTag = "Enemy";
    public int damage = 5;
    [SerializeField] private float tickDelay = 0.5f;

    private Collider2D damageCollider;

    private void Awake()
    {
        damageCollider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        ApplyDamageToEnemiesWithDelay();
    }

    private void ApplyDamageToEnemiesWithDelay()
    {
        StartCoroutine(DamageDelayRoutine());
    }

    private IEnumerator DamageDelayRoutine()
    {
        while (gameObject.activeSelf)
        {
            yield return new WaitForSeconds(tickDelay);
            Collider2D[] colliders = GetCollidersInArea();

            foreach (Collider2D hitCollider in colliders)
            {
                if (!hitCollider.CompareTag(damageTag))
                    continue;

                if (hitCollider.TryGetComponent(out Health health))
                    health.Damage(damage, gameObject, 0.5f, 0, Vector3.zero);
            }            
        }
    }

    private Collider2D[] GetCollidersInArea()
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true;
        List<Collider2D> colliders = new List<Collider2D>();
        var num = Physics2D.OverlapCollider(damageCollider, filter, colliders);
        return colliders.ToArray();
    }
}
