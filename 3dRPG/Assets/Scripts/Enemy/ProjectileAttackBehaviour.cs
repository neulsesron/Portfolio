using UnityEngine;

public class ProjectileAttackBehaviour : AttackBehaviour
{
    public override void ExcuteAttack(GameObject target = null, Transform startPoint = null)
    {
        if (target == null)     return;

        Vector3 projectilePosition = startPoint != null ? startPoint.position : transform.position;
        if (effectPrefab) {
            GameObject projectileGO = GameObject.Instantiate<GameObject>(effectPrefab, projectilePosition, Quaternion.identity);
            projectileGO.transform.forward = transform.forward;

            Projectile projectile = projectileGO.GetComponent<Projectile>();
            if (projectile) {
                projectile.target = target;
                projectile.attackBehaviour = this;
            }
        }
    }
}
