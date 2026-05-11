using UnityEngine;

public class ArrowTrap : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] arrows;

    [Header("Audio")]
    [SerializeField] private AudioClip arrowSound;

    private float cooldownTimer;

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        if (cooldownTimer >= attackCooldown)
        {
            Attack();
        }
    }

    private void Attack()
    {
        cooldownTimer = 0f;

        int arrowIndex = FindAvailableArrow();

        if (arrowIndex == -1)
        {
            Debug.LogWarning("No available arrows in pool!");
            return;
        }

        GameObject arrow = arrows[arrowIndex];

        arrow.transform.position = firePoint.position;

        EnemyProjectile projectile = arrow.GetComponent<EnemyProjectile>();

        if (projectile != null)
        {
            projectile.ActivateProjectile();
        }

        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlaySound(arrowSound);
        }
    }

    private int FindAvailableArrow()
    {
        for (int i = 0; i < arrows.Length; i++)
        {
            if (!arrows[i].activeInHierarchy)
            {
                return i;
            }
        }

        return -1;
    }
}