using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyChat : MonoBehaviourPun
{
    public TMP_InputField chatInputField;
    public Transform chatContentTransform;
    public GameObject chatMessagePrefab;
    public ScrollRect chatScrollRect;

    private void Start()
    {
        if (chatInputField != null)
            chatInputField.onSubmit.AddListener(delegate { OnSendChatMessage(); });
    }

    public void OnSendChatMessage()
    {
        string message = chatInputField.text.Trim();
        if (string.IsNullOrEmpty(message)) return;

        photonView.RPC("ReceiveChatMessage", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, message);
        chatInputField.text = "";
    }

    [PunRPC]
    void ReceiveChatMessage(int senderActorNumber, string message)
    {
        GameObject msgObj = Instantiate(chatMessagePrefab, chatContentTransform);
        TMP_Text msgText = msgObj.GetComponent<TMP_Text>();

        string playerPrefix = senderActorNumber == 1
            ? "<color=#046B9C>P1></color>"
            : "<color=#8E3E36>P2></color>"; // Mėlyna / Raudona

        msgText.text = playerPrefix + " " + message;

        // Perkeliame į apačią
        Canvas.ForceUpdateCanvases();
        chatScrollRect.verticalNormalizedPosition = 0f;
    }
    public void ClearChat()
    {
        foreach (Transform child in chatContentTransform)
        {
            Destroy(child.gameObject);
        }

        
        Canvas.ForceUpdateCanvases();
        chatScrollRect.verticalNormalizedPosition = 1f;
    }

}
