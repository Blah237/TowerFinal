using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour {

    [SerializeField]
    RawImage privacy;
    [SerializeField]
    RawImage mailing;
    [SerializeField]
    RawImage credits;

    public void showPrivacy() {
        //privacy.enabled = true;
        privacy.gameObject.SetActive(true);
        Debug.Log("Show Privacy");
    }

    public void hidePrivacy() {
        privacy.gameObject.SetActive(false);
        Debug.Log("Hide Privacy");
    }

    public void showMailing() {
        mailing.gameObject.SetActive(true);
        Debug.Log("Show Mailing");
    }

    public void hideMailing() {
        mailing.gameObject.SetActive(false);
        Debug.Log("Hide Mailing");
    }

    public void submitMailing() {
        string email = mailing.transform.Find("InputField").GetComponent<InputField>().text;
        LoggingManager.instance.RecordEmail(email);
        hideMailing();
    }

    public void showCredits() {
        credits.gameObject.SetActive(true);
        Debug.Log("Show Credits");
    }

    public void hideCredits() {
        credits.gameObject.SetActive(false);
        Debug.Log("Hide Credits");
    }
}
