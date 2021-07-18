using Hanashi;
using UnityEngine;

namespace HanashiSamples
{
    public sealed class ChoiceBoxUI : MonoBehaviour
    {
        [SerializeField] private ChoiceItemUI[] _choices = default;

        public void Set(NodeData nodeData)
        {
            //Hide all
            foreach(var choiceItem in _choices)
            {
                choiceItem.gameObject.SetActive(false);
            }

            //Show and set data if any are active
            var totalOptions = nodeData.choiceNodeOptions.Count;
            if(totalOptions == 0)
            {
                return;
            }
            for (int choiceItemIndex = 0; choiceItemIndex < totalOptions; choiceItemIndex++)
            {
                var choiceItem = _choices[choiceItemIndex];
                var choideData = nodeData.choiceNodeOptions[choiceItemIndex];
                choiceItem.gameObject.SetActive(choiceItemIndex <= totalOptions - 1);
                choiceItem.text.text = choideData.Text;
            }

        }
    }
}