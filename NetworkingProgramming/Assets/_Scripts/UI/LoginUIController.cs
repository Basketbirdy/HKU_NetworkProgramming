using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginUIController : MonoBehaviour
{
    [Header("Login")]
    [SerializeField] private TMP_InputField login_EmailField;
    [SerializeField] private TMP_InputField login_PasswordField;    
    [SerializeField] private Button login_Confirm;    
                      
    [Header("Signin")]     
    [SerializeField] private TMP_InputField signin_EmailField;
    [SerializeField] private TMP_InputField signin_NicknameField;
    [SerializeField] private TMP_InputField signin_PasswordField;
    [SerializeField] private Button signin_Confirm;

    [Header("Error")]
    [SerializeField] private TimedPopup error_Popup;

    public void ShowError(string header, string message)
    {
        error_Popup.Show(header, message);
    }
}
