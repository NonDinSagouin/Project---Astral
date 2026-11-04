using UnityEngine;
using System.Collections.Generic;

namespace Gameplay.Combat
{
    public class Movement : MonoBehaviour
    {
        [SerializeField, Min(0.01f)] private float maxSpeed = 1.25f;
        [SerializeField] private int maxMovement;

        void OnDestroy()
        {
            LeanTween.cancel(gameObject);
        }

        /// <summary>
        /// Déplace l'objet le long d'un chemin défini par une lis  te de positions.
        /// </summary>
        /// <param name="positions">La liste des positions formant le chemin.</param>
        public void MoveToPath(IReadOnlyList<Vector3> positions)
        {
            LeanTween.cancel(gameObject);
            if (positions == null || positions.Count == 0)
                return;

            if (maxSpeed < Mathf.Epsilon)
                return;

            MoveToPathSegment(positions, 0);
        }

        private void MoveToPathSegment(IReadOnlyList<Vector3> positions, int index)
        {
            if (index >= positions.Count)
                return;

            Vector3 target = positions[index];
            float segmentDistance = Vector3.Distance(transform.position, target);
            if (segmentDistance < Mathf.Epsilon)
            {
                MoveToPathSegment(positions, index + 1);
                return;
            }

            float segmentDuration = segmentDistance / maxSpeed;
            LeanTween.move(gameObject, target, segmentDuration)
                .setOnComplete(() => MoveToPathSegment(positions, index + 1));
        }
        
    }
}
