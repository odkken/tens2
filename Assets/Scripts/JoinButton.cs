using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts
{
    public class JoinButton : MonoBehaviour
    {
        public Matchmaker Matchmaker;
        public SimpleInput ServerNameInput;
        public SimpleInput PlayerNameInput;
        public void Click()
        {
            if (!ServerNameInput.IsFilledOut)
                Debug.Log("Please specify a server name!");
            else if (!PlayerNameInput.IsFilledOut)
                Debug.Log("Your name cannot be empty!");
            else
                Matchmaker.JoinServer(PlayerNameInput.Text, ServerNameInput.Text);
        }
    }
}
