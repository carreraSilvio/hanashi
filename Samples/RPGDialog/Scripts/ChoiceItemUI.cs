using UnityEngine;
using UnityEngine.UI;

namespace HanashiSamples
{
    public sealed class ChoiceItemUI : MonoBehaviour
    {
        [SerializeField] private Text _text = default;

        public Text text => _text;

    }
}