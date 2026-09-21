using System.Collections.Generic;
using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
    [Header("Grenade")]
    [SerializeField]
    private float fuseTime = 2.5f;

    [Header("Optional FX")]
    [SerializeField]
    private GameObject explosionEffect;

    [SerializeField]
    private float explosionEffectLifetime = 8f;

    private GrenadeItemStats stats;

    private GameObject owner;

    private float fuseTimer;

    private bool exploded;

    public void Initialize(
        GrenadeItemStats grenadeStats,
        GameObject grenadeOwner)
    {
        stats =
            grenadeStats;

        owner =
            grenadeOwner;

        fuseTimer =
            fuseTime;
    }

    private void Update()
    {
        if (exploded)
        {
            return;
        }

        fuseTimer -=
            Time.deltaTime;

        if (fuseTimer <= 0f)
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (exploded)
        {
            return;
        }

        exploded =
            true;

        if (stats == null)
        {
            Debug.LogError(
                "GrenadeProjectile exploded without GrenadeItemStats."
            );

            Destroy(
                gameObject
            );

            return;
        }

        SpawnExplosionEffect();

        Collider[] hits =
            Physics.OverlapSphere(
                transform.position,
                stats.blastRadius,
                ~0,
                QueryTriggerInteraction.Ignore
            );

        HashSet<IDamage> damagedTargets =
            new HashSet<IDamage>();

        foreach (Collider hit in hits)
        {
            if (hit == null)
            {
                continue;
            }

            IDamage damageable =
                hit.GetComponentInParent<IDamage>();

            if (damageable == null)
            {
                continue;
            }

            if (
                damagedTargets.Contains(
                    damageable
                )
            )
            {
                continue;
            }

            damagedTargets.Add(
                damageable
            );

            damageable.takeDamage(
                stats.damage
            );
        }

        Debug.Log(
            "GRENADE EXPLODED | Damage: " +
            stats.damage +
            " | Radius: " +
            stats.blastRadius
        );

        Destroy(
            gameObject
        );
    }

    private void SpawnExplosionEffect()
    {
        if (explosionEffect == null)
        {
            Debug.LogWarning(
                "Grenade exploded but no Explosion Effect is assigned."
            );

            return;
        }

        GameObject effect =
            Instantiate(
                explosionEffect,
                transform.position,
                Quaternion.identity
            );

        effect.SetActive(
            true
        );

        // Force all runtime FX to start.
        PlayExplosionEffect(
            effect
        );

        Destroy(
            effect,
            explosionEffectLifetime
        );
    }

    private void PlayExplosionEffect(
        GameObject effect)
    {
        if (effect == null)
        {
            return;
        }

        Animator[] animators =
            effect.GetComponentsInChildren<Animator>(
                true
            );

        foreach (Animator animator in animators)
        {
            if (animator == null)
            {
                continue;
            }

            animator.gameObject.SetActive(
                true
            );

            animator.enabled =
                true;

            animator.cullingMode =
                AnimatorCullingMode.AlwaysAnimate;

            animator.Rebind();

            animator.Update(
                0f
            );

            AnimatorStateInfo stateInfo =
                animator.GetCurrentAnimatorStateInfo(
                    0
                );

            if (
                stateInfo.fullPathHash != 0
            )
            {
                animator.Play(
                    stateInfo.fullPathHash,
                    0,
                    0f
                );

                animator.Update(
                    0f
                );
            }
        }

        Animation[] animations =
            effect.GetComponentsInChildren<Animation>(
                true
            );

        foreach (Animation animation in animations)
        {
            if (animation == null)
            {
                continue;
            }

            animation.gameObject.SetActive(
                true
            );

            animation.enabled =
                true;

            animation.Rewind();

            animation.Play();
        }

        ParticleSystem[] particleSystems =
            effect.GetComponentsInChildren<ParticleSystem>(
                true
            );

        foreach (ParticleSystem particles in particleSystems)
        {
            if (particles == null)
            {
                continue;
            }

            particles.gameObject.SetActive(
                true
            );

            particles.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );

            particles.Play(
                true
            );
        }

        Renderer[] renderers =
            effect.GetComponentsInChildren<Renderer>(
                true
            );

        foreach (Renderer renderer in renderers)
        {
            if (renderer == null)
            {
                continue;
            }

            renderer.enabled =
                true;
        }

        Debug.Log(
            "EXPLOSION FX STARTED: " +
            effect.name +
            " | Animators: " +
            animators.Length +
            " | Legacy Animations: " +
            animations.Length +
            " | Particle Systems: " +
            particleSystems.Length +
            " | Renderers: " +
            renderers.Length
        );
    }

    private void OnDrawGizmosSelected()
    {
        float radius =
            stats != null
                ? stats.blastRadius
                : 1f;

        Gizmos.DrawWireSphere(
            transform.position,
            radius
        );
    }
}