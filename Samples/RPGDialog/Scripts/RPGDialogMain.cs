using Hanashi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

namespace HanashiSamples
{
    public class RPGDialogMain : MonoBehaviour
    {
        public NarrativeData narrativeData;

        public Text speaker;
        public Text message;
        private int _currentNode;
        private int _totalNodes;

        void Awake()
        {
            speaker.text = "";
            message.text = "";
            _totalNodes = narrativeData.Nodes.Count;
        }

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                
            }
        }

    }
}