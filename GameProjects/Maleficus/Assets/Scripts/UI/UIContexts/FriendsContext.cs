using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Maleficus.MaleficusUtilities;

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
        selfInformation.text = NetworkManager.Instance.Self.user_name;
        NetworkManager.Instance.SendRequestFollow();
    }

    public void AddFollowToUi(Local_Account follow)
    {
        GameObject followItem = Instantiate(followPrefab, followContainer);

        followItem.GetComponentInChildren<TextMeshProUGUI>().text = follow.user_name;
        followItem.transform.GetChild(1).GetComponent<Image>().color = (follow.status != 0) ? Color.green : Color.gray;
        followItem.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { Destroy(followItem); });
        //Todo wait for server response bevor deleting object
        followItem.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { OnClickRemoveFollow(follow.user_name); });

        uiFollows.Add(follow.user_name, followItem);
    }
    public void UpdateFollow(Local_Account follow)
    {
        uiFollows[follow.user_name].transform.GetChild(1).GetComponent<Image>().color = (follow.status != 0) ? Color.green : Color.gray;
    }

    #region Button
    public void OnClickAddFollow()
    {
        string usernameDiscriminator = addFollowInput.text;

        if (!IsUsernameAndDiscriminator(usernameDiscriminator) && !IsEmail(usernameDiscriminator))
        {
            Debug.Log("Invalid format!");
            return;
        }

        NetworkManager.Instance.SendAddFollow(usernameDiscriminator);

    }

    public void OnClickRemoveFollow(string username)
    {
        NetworkManager.Instance.SendRemoveFollow(username);
        uiFollows.Remove(username);
    }
    #endregion
}
