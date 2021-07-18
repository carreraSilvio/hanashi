using Hanashi;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HanashiSamples
{
    public sealed class RPGDialogMain : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Text _speaker = default;
        [SerializeField] private Text _message = default;
        [SerializeField] private ChoiceBoxUI _choiceBox = default;

        [Header("Data")]
        [SerializeField] private NarrativeData _narrativeData = default;

        public List<string> tests;
        
        private int _currentNodeIndex;
        private int _totalNodes;

        private void Awake()
        {
            _speaker.text = "";
            _message.text = "";
            _totalNodes = _narrativeData.Nodes.Count;
            AdvanceDialog();
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                AdvanceDialog();
            }
        }

        private void AdvanceDialog()
        {
            var nodeData = _narrativeData.Nodes[_currentNodeIndex];
            _speaker.text = nodeData.Speaker;
            _message.text = nodeData.Message;
            _choiceBox.Set(nodeData);

            _currentNodeIndex = (_currentNodeIndex + 1) % _totalNodes;
        }

    }
}