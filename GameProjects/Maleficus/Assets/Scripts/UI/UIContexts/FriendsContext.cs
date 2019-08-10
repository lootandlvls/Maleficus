using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendsContext : MonoBehaviour
{
    public static FriendsContext Instance { set; get; }

    [SerializeField] private TextMeshProUGUI selfInformation;
    [SerializeField] private TMP_InputField addFollowInput;

    [SerializeField] private GameObject followPrefab;
    [SerializeField] private Transform followContainer;

    private Dictionary<string, GameObject> uiFollows = new Dictionary<string, GameObject>();

    private void Start()
    {
        Instance = this;

        EventManager.Instance.UI_MenuStateUpdated.AddListener(On_UI_MenuStateUpdated);
    }

    private void On_UI_MenuStateUpdated(StateUpdatedEventHandle<EMenuState> eventHandle)
    {
        switch (eventHandle.NewState)
        {
            case EMenuState.IN_MENU:
                selfInformation.text = NetworkManager.Instance.self.Username + "#" + NetworkManager.Instance.self.Discriminator;
                NetworkManager.Instance.SendRequestFollow();
                Debug.Log("now friends should be requested");
                break;
        }
    }

    public void AddFollowToUi(Account follow)
    {
        GameObject followItem = Instantiate(followPrefab, followContainer);

        followItem.GetComponentInChildren<TextMeshProUGUI>().text = follow.Username + "#" + follow.Discriminator;
        followItem.transform.GetChild(1).GetComponent<Image>().color = (follow.Status != 0) ? Color.green : Color.gray;
        followItem.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { Destroy(followItem); });
        //Todo wait for server response bevor deleting object
        followItem.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { OnClickRemoveFollow(follow.Username, follow.Discriminator); });

        uiFollows.Add(follow.Username + "#" + follow.Discriminator, followItem);
    }
    public void UpdateFollow(Account follow)
    {
        uiFollows[follow.Username + "#" + follow.Discriminator].transform.GetChild(1).GetComponent<Image>().color = (follow.Status != 0) ? Color.green : Color.gray;
    }

    #region Button
    public void OnClickAddFollow()
    {
        string usernameDiscriminator = addFollowInput.text;

        if(!Utility.IsUsernameAndDiscriminator(usernameDiscriminator) && !Utility.IsEmail(usernameDiscriminator))
        {
            Debug.Log("Invalid format!");
            return;
        }

        NetworkManager.Instance.SendAddFollow(usernameDiscriminator);

    }

    public void OnClickRemoveFollow(string username, string discriminator)
    {
        NetworkManager.Instance.SendRemoveFollow(username + "#" + discriminator);
        uiFollows.Remove(username + "#" + discriminator);
    }
    #endregion
}
