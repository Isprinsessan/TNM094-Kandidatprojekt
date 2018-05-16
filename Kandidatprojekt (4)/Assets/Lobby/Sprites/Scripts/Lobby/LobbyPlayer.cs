using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Prototype.NetworkLobby
{
    //Player entry in the lobby. Handle selecting color/setting name & getting ready for the game
    //Any LobbyHook can then grab it and pass those value to the game player prefab (see the Pong Example in the Samples Scenes)
    public class LobbyPlayer : NetworkLobbyPlayer
    {
        static Color[] Colors = new Color[] { Color.magenta, Color.red, Color.cyan, Color.blue, Color.green, Color.yellow };
        //used on server to avoid assigning the same color to two player
        static List<int> _colorInUse = new List<int>();

        public Button colorButton;
        public InputField nameInput;
        public SerializeField charname;
        public Button readyButton;
        public Button waitingPlayerButton;
        public Button removePlayerButton;
        public Button Charbutton;
        public int charindex;

        public GameObject localIcone;
        public GameObject remoteIcone;
        // char selection stuff
        public Toggle toggle;
        //[SyncVar(hook = "UpdateCharacter()")]
        public NetworkLobbyPlayer playerInfo;
        public Text indextext;

        [Space]
        [Header("Image/Button References")]
        public GameObject bear;
        public Button bearButton;
        [Space]
        public GameObject fox;
        public Button foxBotton;
        [Space]
        public GameObject spider;
        public Button spiderButton;
        [Space]
        public GameObject ostrich;
        public Button ostrichButton;

        [Space]
        public GameObject charSelect;
        public string thisChar = "Fox";
        private string[] characters = { "Bear", "Fox", "Spider", "Ostrich" };




        //OnMyName function will be invoked on clients when server change the value of playerName
        [SyncVar(hook = "OnMyName")]
        public string playerName = "";
        [SyncVar(hook = "OnMyColor")]
        public Color playerColor = Color.white;
        [SyncVar(hook = "OnMyCharIndex")]
        public int playerCharIndex;


        public Color OddRowColor = new Color(250.0f / 255.0f, 250.0f / 255.0f, 250.0f / 255.0f, 1.0f);
        public Color EvenRowColor = new Color(180.0f / 255.0f, 180.0f / 255.0f, 180.0f / 255.0f, 1.0f);

        static Color JoinColor = new Color(255.0f, 255.0f, 255.0f, 1.0f);
        static Color NotReadyColor = new Color(34.0f / 255.0f, 44 / 255.0f, 55.0f / 255.0f, 1.0f);
        static Color ReadyColor = new Color(0.0f, 204.0f / 255.0f, 204.0f / 255.0f, 1.0f);
        static Color TransparentColor = new Color(0, 0, 0, 0);

        //static Color OddRowColor = new Color(250.0f / 255.0f, 250.0f / 255.0f, 250.0f / 255.0f, 1.0f);
        //static Color EvenRowColor = new Color(180.0f / 255.0f, 180.0f / 255.0f, 180.0f / 255.0f, 1.0f);

       /* public void nextchar()
        {
            charindex--;
            if (charindex < 0)
                charindex = 3;//MAXSIZE  =  4 st

            //            PlayerPrefs.SetInt("charindex", index);
            Debug.Log(charindex);
        }*/
        [ClientRpc]
        public void RpcBearClick()
        {
         
                charindex = 0;
                OnMyCharIndex(charindex);
                bear.SetActive(true);
                fox.SetActive(false);
                spider.SetActive(false);
                ostrich.SetActive(false);
                //UpdateCharacter(charindex, bear);
            
        }
        [ClientRpc]
        public void RpcFoxClick()
        {
           
                charindex = 1;
                OnMyCharIndex(charindex);
                bear.SetActive(false);
                fox.SetActive(true);
                spider.SetActive(false);
                ostrich.SetActive(false);
                //UpdateCharacter(charindex, fox);
            
        }
        [ClientRpc]
        public void RpcSpiderClick()
        {
         
                charindex = 2;
                OnMyCharIndex(charindex);
                bear.SetActive(false);
                fox.SetActive(false);
                spider.SetActive(true);
                ostrich.SetActive(false);
               // UpdateCharacter(charindex, spider);
            
        }
        [ClientRpc]
        public void RpcOstrichClick()
        {
          
                charindex = 3;
                OnMyCharIndex(charindex);
                bear.SetActive(false);
                fox.SetActive(false);
                spider.SetActive(false);
                ostrich.SetActive(true);
                //UpdateCharacter(charindex, ostrich);
            
 
        }

        public void UpdateCharacter(int index, GameObject avatar)
        {
            toggle.isOn = false;
            thisChar = characters[index];
            Debug.Log(thisChar + " is selected");
            charSelect.SetActive(false);
            OnMyCharIndex(index);
            //toggle.image.sprite = avatar;
            //indextext.text = index.ToString();
        }

        [Command]
        public void CmdServerUpdate(int index)
        {
            if (index == 0) RpcBearClick();
            else if (index == 1) RpcFoxClick();
            else if (index == 2) RpcSpiderClick();
            else if (index == 3) RpcOstrichClick();
        }

        [ClientCallback]
        public void ButtonClicked(int index)
        {
            //if(isLocalPlayer)
            CmdServerUpdate(index);
        }

        public void CharMenuToggle(bool isOn)
        {

            //Debug.Log(isOn);
            if (charSelect.activeInHierarchy && !isOn)
            {
                toggle.isOn = false;
                charSelect.SetActive(false);
            }
            else if (!charSelect.activeInHierarchy && isOn)
            {
                toggle.isOn = true;
                charSelect.SetActive(true);
            }
        }


        public override void OnClientEnterLobby()
        {
            base.OnClientEnterLobby();

            if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(1);

            LobbyPlayerList._instance.AddPlayer(this);
            LobbyPlayerList._instance.DisplayDirectServerWarning(isServer && LobbyManager.s_Singleton.matchMaker == null);

            if (isLocalPlayer)
            {
                SetupLocalPlayer();
            }
            else
            {
                SetupOtherPlayer();
            }

            //setup the player data on UI. The value are SyncVar so the player
            //will be created with the right value currently on server
            OnMyName(playerName);
            OnMyColor(playerColor);
            //Syncar inte här och det är inte här den får sitt värde
            //OnMyCharIndex(charindex);
            Debug.Log(playerName);
            Debug.Log(charindex + " charindex");
            Debug.Log(playerCharIndex + " syncvar charindex");

        }

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();

            //if we return from a game, color of text can still be the one for "Ready"
            readyButton.transform.GetChild(0).GetComponent<Text>().color = Color.white;

            SetupLocalPlayer();
        }

        void ChangeReadyButtonColor(Color c)
        {
            ColorBlock b = readyButton.colors;
            b.normalColor = c;
            b.pressedColor = c;
            b.highlightedColor = c;
            b.disabledColor = c;
            readyButton.colors = b;
        }

        void SetupOtherPlayer()
        {
            nameInput.interactable = false;
            toggle.interactable = false;
            removePlayerButton.interactable = NetworkServer.active;

            ChangeReadyButtonColor(NotReadyColor);

            readyButton.transform.GetChild(0).GetComponent<Text>().text = "...";
            readyButton.interactable = false;

            OnClientReady(false);
        }

        void SetupLocalPlayer()
        {
            nameInput.interactable = true;
            toggle.interactable = true;
            remoteIcone.gameObject.SetActive(false);
            localIcone.gameObject.SetActive(true);

            CheckRemoveButton();

            if (playerColor == Color.white)
            { CmdColorChange(); }
           

            ChangeReadyButtonColor(JoinColor);

            readyButton.transform.GetChild(0).GetComponent<Text>().text = "JOIN";
            readyButton.interactable = true;

            //have to use child count of player prefab already setup as "this.slot" is not set yet
            if (playerName == "")
                CmdNameChanged("Player" + (LobbyPlayerList._instance.playerListContentTransform.childCount - 1));

            //we switch from simple name display to name input
            colorButton.interactable = true;
            nameInput.interactable = true;

            nameInput.onEndEdit.RemoveAllListeners();
            nameInput.onEndEdit.AddListener(OnNameChanged);

            colorButton.onClick.RemoveAllListeners();
            colorButton.onClick.AddListener(OnColorClicked);

            readyButton.onClick.RemoveAllListeners();
            readyButton.onClick.AddListener(OnReadyClicked);


            //when OnClientEnterLobby is called, the loval PlayerController is not yet created, so we need to redo that here to disable
            //the add button if we reach maxLocalPlayer. We pass 0, as it was already counted on OnClientEnterLobby
            if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(0);
        }

        //This enable/disable the remove button depending on if that is the only local player or not
        public void CheckRemoveButton()
        {
            if (!isLocalPlayer)
                return;

            int localPlayerCount = 0;
            foreach (UnityEngine.Networking.PlayerController p in ClientScene.localPlayers)
                localPlayerCount += (p == null || p.playerControllerId == -1) ? 0 : 1;

            removePlayerButton.interactable = localPlayerCount > 1;
        }

        public override void OnClientReady(bool readyState)
        {
            if (readyState)
            {
                ChangeReadyButtonColor(TransparentColor);

                Text textComponent = readyButton.transform.GetChild(0).GetComponent<Text>();
                textComponent.text = "READY";
                textComponent.color = ReadyColor;
                readyButton.interactable = false;
                colorButton.interactable = false;
                nameInput.interactable = false;
            }
            else
            {
                ChangeReadyButtonColor(isLocalPlayer ? JoinColor : NotReadyColor);

                Text textComponent = readyButton.transform.GetChild(0).GetComponent<Text>();
                textComponent.text = isLocalPlayer ? "JOIN" : "...";
                textComponent.color = Color.white;
                readyButton.interactable = isLocalPlayer;
                colorButton.interactable = isLocalPlayer;
                nameInput.interactable = isLocalPlayer;
            }
        }

        public void OnPlayerListChanged(int idx)
        {
            GetComponent<Image>().color = (idx % 2 == 0) ? EvenRowColor : OddRowColor;
        }

        ///===== callback from sync var

        public void OnMyName(string newName)
        {
            playerName = newName;
            nameInput.text = playerName;
            Debug.Log(playerName + " i funktionen");
        }

        public void OnMyColor(Color newColor)
        {
            playerColor = newColor;
            colorButton.GetComponent<Image>().color = newColor;
           
        }
        public void OnMyCharIndex(int newIndex)
        {
            playerCharIndex = newIndex;

        }

        
        //===== UI Handler

        //Note that those handler use Command function, as we need to change the value on the server not locally
        //so that all client get the new value throught syncvar
        public void OnColorClicked()
        {
            CmdColorChange();
            OnMyCharIndex(playerCharIndex);
        }

        public void OnReadyClicked()
        {
            SendReadyToBeginMessage();
        }

        public void OnNameChanged(string str)
        {
            CmdNameChanged(str);
        }

 
        public void OnRemovePlayerClick()
        {
            if (isLocalPlayer)
            {
                RemovePlayer();
            }
            else if (isServer)
                LobbyManager.s_Singleton.KickPlayer(connectionToClient);

        }


        public void ToggleJoinButton(bool enabled)
        {
            readyButton.gameObject.SetActive(enabled);
            waitingPlayerButton.gameObject.SetActive(!enabled);
        }

        [ClientRpc]
        public void RpcUpdateCountdown(int countdown)
        {
            LobbyManager.s_Singleton.countdownPanel.UIText.text = "Match Starting in " + countdown;
            LobbyManager.s_Singleton.countdownPanel.gameObject.SetActive(countdown != 0);
        }

        [ClientRpc]
        public void RpcUpdateRemoveButton()
        {
            CheckRemoveButton();
        }

        //====== Server Command

        [Command]
        public void CmdColorChange()
        {
            int idx = System.Array.IndexOf(Colors, playerColor);

            int inUseIdx = _colorInUse.IndexOf(idx);

            if (idx < 0) idx = 0;

            idx = (idx + 1) % Colors.Length;

            /* bool alreadyInUse = false;
             // disabled to allow several players to use same color
             do
             {
                 alreadyInUse = false;
                 for (int i = 0; i < _colorInUse.Count; ++i)
                 {
                     if (_colorInUse[i] == idx)
                     {//that color is already in use
                         alreadyInUse = true;
                         idx = (idx + 1) % Colors.Length;
                     }
                 }
             }
             while (alreadyInUse); */

            if (inUseIdx >= 0)
            {//if we already add an entry in the colorTabs, we change it
                _colorInUse[inUseIdx] = idx;
                
               
            }
            else
            {//else we add it
                _colorInUse.Add(idx);
        
                
            }

            playerColor = Colors[idx];
            ButtonClicked(playerCharIndex);

        }

        [Command]
        public void CmdNameChanged(string name)
        {
            playerName = name;
            Debug.Log(playerName + " I command");

        }

        [Command]
        public void CmdCharbuttonchanged(string charindex)
        {

        }

        //Cleanup thing when get destroy (which happen when client kick or disconnect)
        public void OnDestroy()
        {
            LobbyPlayerList._instance.RemovePlayer(this);
            if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(-1);

            int idx = System.Array.IndexOf(Colors, playerColor);

            if (idx < 0)
                return;

            for (int i = 0; i < _colorInUse.Count; ++i)
            {
                if (_colorInUse[i] == idx)
                {//that color is already in use
                    _colorInUse.RemoveAt(i);
                    break;
                }
            }
        }
    }
}