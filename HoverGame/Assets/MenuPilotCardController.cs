using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class MenuPilotCardController : MonoBehaviour
{

    public TextMeshProUGUI pilotName;
    public TextMeshProUGUI pilotFame;
    public GameObject pilotAvatar;
    Pilot_Passport PILOT_PASSPORT;

    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PILOT_PASSPORT = GameObject.Find("PilotPassport").GetComponent<Pilot_Passport>();
        

        if(PILOT_PASSPORT != null )
        {
            SetPilotName();
            SetPilotFame();
            SetPilotAvatar();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetPilotName()
    {
        pilotName.text = PILOT_PASSPORT.GetPilotName();
    }

    void SetPilotAvatar()
    {
        Image image = pilotAvatar.GetComponent<Image>();
        image.sprite = PILOT_PASSPORT.GetPilotAvatar();
    }

    void SetPilotFame()
    {
        pilotFame.text = "FAME: " + PILOT_PASSPORT.GetPilotFameLevel().ToString();
    }
}
