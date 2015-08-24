using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class SimpleInput : MonoBehaviour
    {
        private Text placeholder;
        private Text input;
        void Awake()
        {
            placeholder = transform.FindChild("Placeholder").transform.GetComponent<Text>();
            input = transform.FindChild("Text").transform.GetComponent<Text>();
        }
        public bool IsFilledOut { get { return !string.IsNullOrEmpty(input.text); } }
        public string Text
        {
            get { return input.text; }
        }
    }
}
