using UnityEngine;

/// <summary>
/// Tag helpers. Needed because imported model prefabs carry the "Muro" tag on the
/// instance root while their colliders live on child objects, so a direct
/// collider.CompareTag("Muro") misses them.
/// </summary>
public static class TagUtil
{
    public static bool HasTagInHierarchy(Transform target, string tag)
    {
        Transform current = target;
        while (current != null)
        {
            if (current.CompareTag(tag))
            {
                return true;
            }

            current = current.parent;
        }

        return false;
    }
}
