using TMPro;
using Unity.VisualScripting;
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

    private void Start()
    {
        error_Popup.Close();
    }

    public void ShowError(string header, string message)
    {
        error_Popup.Show(header, message);
    }

    public void Login()
    {
        string email = login_EmailField.text;
        string password = login_PasswordField.text;

        if(email == string.Empty || password == string.Empty) 
        { 
            Debug.Log($"[Login] missing credentials!"); 
            ShowError($"Failed login!", $"Could not log in. <br>missing credentials");
            return; 
        }

        Debug.Log($"[Login] Attempting login! {email}, {password}");
        if(!LoginService.TryLogin(email, password))
        {
            ShowError($"Failed login!", $"Could not log in. <br>Please check the inserted credentials");
        }
    }

    public void Signin()
    {
        string email = signin_EmailField.text;
        string nickname = signin_NicknameField.text;
        string password = signin_PasswordField.text;

        if(email == string.Empty || password == string.Empty || nickname == string.Empty) 
        { 
            Debug.Log($"[Signin] missing credentials!");
            ShowError($"Failed signin!", $"Could not sign in. <br>missing credentials");
            return; 
        }
            
        Debug.Log($"[Signin] Attempting signin! {email}, {nickname}, {password}");
        if (!LoginService.TrySignin(signin_EmailField.text, signin_NicknameField.text, signin_PasswordField.text))
        {
            ShowError($"Failed signin!", $"Could not sign in. <br>Please check the inserted credentials");
        }
    }
}
