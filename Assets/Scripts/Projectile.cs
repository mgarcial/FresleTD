using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private bool stunsEnemy = false;
    [SerializeField] private bool burnsEnemy = false;

    [SerializeField] private int burningDamage = 1; 
    [SerializeField] private float burningDuration = 3f; 


    private Rigidbody2D _rb;
    private DamageDeal _damageDeal;
    private Transform _target;
    private TrailRenderer _trailRenderer;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _damageDeal = GetComponent<DamageDeal>();
        _trailRenderer = GetComponent<TrailRenderer>();
    }

    private void FixedUpdate()
    {
        FireProjectile();
    }
    internal void SetTarget(Transform target)
    {
        _target = target;
    }

    private void FireProjectile()
    {
        if (_target != null)
        {
            Vector2 direction = (_target.position - transform.position).normalized;

            _rb.velocity = direction * speed;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            angle += 270f;

            transform.rotation = Quaternion.Euler(0, 0, angle);

            if (!_trailRenderer.enabled)
                _trailRenderer.enabled = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        Enemigo enemy = collision.gameObject.GetComponent<Enemigo>();
        if (enemy != null)
        {
            enemy.TakeDamage(_damageDeal.GetDamage());
            Debug.Log("i hit" + enemy);
            if (stunsEnemy)
            {
                enemy.Stunned();
            }

            if (burnsEnemy)
            {
                // Apply burning effect to the enemy
                StartCoroutine(ApplyBurningEffect(enemy));
            }

            Destroy(gameObject);
        }
    }

    IEnumerator ApplyBurningEffect(Enemigo enemy)
    {
        float timer = 0f;
        while (timer < burningDuration)
        {
            enemy.TakeDamage(burningDamage);
            yield return new WaitForSeconds(1f); 
            timer += 1f;
        }
    }

}
