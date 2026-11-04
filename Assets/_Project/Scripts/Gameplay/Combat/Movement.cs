using UnityEngine;
using System.Collections.Generic;

namespace Gameplay.Combat
{
    public class Movement : MonoBehaviour
    {
        [SerializeField, Min(0.01f)] private float duration = 0.3f;

        void OnDestroy()
        {
            LeanTween.cancel(gameObject);
        }

        /// <summary>
        /// Déplace l'objet le long d'un chemin défini par une liste de positions.
        /// </summary>
        /// <param name="positions">La liste des positions formant le chemin.</param>
        public void MoveToPath(IReadOnlyList<Vector3> positions)
        {
            LeanTween.cancel(gameObject);
            if (positions == null || positions.Count == 0)
                return;

            float totalDistance = 0f;
            Vector3 previousPosition = transform.position;
            for (int i = 0; i < positions.Count; i++)
            {
                totalDistance += Vector3.Distance(previousPosition, positions[i]);
                previousPosition = positions[i];
            }

            if (totalDistance < Mathf.Epsilon)
                return;

            MoveToPathSegment(positions, 0, totalDistance);
        }

        private void MoveToPathSegment(IReadOnlyList<Vector3> positions, int index, float totalDistance)
        {
            if (index >= positions.Count)
                return;

            Vector3 target = positions[index];
            float segmentDistance = Vector3.Distance(transform.position, target);
            if (segmentDistance < Mathf.Epsilon)
            {
                MoveToPathSegment(positions, index + 1, totalDistance);
                return;
            }

            float segmentDuration = duration * segmentDistance / totalDistance;
            LeanTween.move(gameObject, target, segmentDuration)
                .setOnComplete(() => MoveToPathSegment(positions, index + 1, totalDistance));
        }
        
    }
}
