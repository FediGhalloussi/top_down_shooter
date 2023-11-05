namespace Data
{
    using UnityEngine;

    [System.Serializable]
    public struct CanonDescription
    {
        [SerializeField]
        public Vector3 BulletSpawnOffsetPosition;

        [SerializeField]
        public float BulletSpawnOffsetAngle;
    }
}
