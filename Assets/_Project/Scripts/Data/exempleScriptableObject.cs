using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "NewExempleScriptableObject", menuName = "Data/ExempleScriptableObject")]
    public class ExempleScriptableObject : ScriptableObject
    {
        public Sprite exampleSprite;
        public string exampleField;
        public int exampleHealth;
        public int exampleDamage;
    }
}