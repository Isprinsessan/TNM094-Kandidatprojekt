using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace Prototype.NetworkLobby
{
    public class LobbyCharacterSelection : MonoBehaviour
    {
        public Text infoText;
        public Text buttonText;
        public Button singleButton;
        public GameObject _thisGameObject;


        public void Display(string info, string buttonInfo, UnityEngine.Events.UnityAction buttonClbk)
        {
            infoText.text = info;

            buttonText.text = buttonInfo;

            singleButton.onClick.RemoveAllListeners();

            if (buttonClbk != null)
            {
                singleButton.onClick.AddListener(buttonClbk);
            }

            singleButton.onClick.AddListener(() => { _thisGameObject.SetActive(false); });

            _thisGameObject.SetActive(true);
        }

    }
}