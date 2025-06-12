using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Text;

public class LoginUIController : MonoBehaviour
{
    [Header("Scene indices")]
    [SerializeField] private int startupScene = 0;

    [Header("Login")]
    [SerializeField] private TMP_InputField login_EmailField;
    [SerializeField] private TMP_InputField login_PasswordField;    
    [SerializeField] private Button login_Confirm;    
                      
    [Header("Signin")]     
    [SerializeField] private TMP_InputField signin_EmailField;
    [SerializeField] private TMP_InputField signin_NicknameField;
    [SerializeField] private TMP_InputField signin_PasswordField;
    [SerializeField] private TMP_InputField signin_dayField, signin_monthField, signin_yearField;
    [SerializeField] private Button signin_Confirm;

    [Header("Information")]
    [SerializeField] private TextMeshProUGUI accountInfoHeaders;
    [SerializeField] private TextMeshProUGUI accountInfoData;

    [Header("Error")]
    private IPopup popup;

    private void Awake()
    {
        popup = GetComponentInChildren<IPopup>();
        if(popup == null) { Debug.LogError($"Missing IPopup!"); }
    }

    private void Start()
    {
        popup.Close();

        PopulateAccountInformation();
    }

    public void ShowPopup(string header, string message)
    {
        popup.Show(header, message);
    }

    public void TryLogin()
    {
        if (AccountManager.Instance.LoggedIn)
        {
            ShowPopup("Failed login!", "This client is already logged in to another account. <br>If you want to switch, log out first");
            return;
        }
        Login();    // _ variable name, means it is ignored
    }
    private async void Login()
    {
        string email = login_EmailField.text;
        string password = login_PasswordField.text;

        if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)) 
        { 
            Debug.Log($"[Login] missing credentials!");
            ShowPopup(APIConnection.responseCodeHeader["04"], APIConnection.responseCodeMessage["04"]);
            return; 
        }

        Debug.Log($"[Login] Attempting login! {email}, {password}");
        string response = await LoginService.TryLogin(email, password);
        if (APIConnection.CheckResponseForErrorCode(response) || string.IsNullOrEmpty(response))
        {
            ShowPopup(APIConnection.responseCodeHeader[response], APIConnection.responseCodeMessage[response]);
        }
        else
        {
            ShowPopup($"Welcome back {AccountManager.Instance.Nickname}!", $"You successfully logged in with <br>{email}");
            PopulateAccountInformation();
            ClearInputFields();
        }
    }

    public void TryLogout()
    {
        Logout();
    }

    private async void Logout()
    {
        string response = await LoginService.TryLogout();
        if (APIConnection.CheckResponseForErrorCode(response) || string.IsNullOrEmpty(response))
        {
            ShowPopup(APIConnection.responseCodeHeader[response], APIConnection.responseCodeMessage[response]);
        }
        else
        {
            ShowPopup($"Logged out!", "");
            PopulateAccountInformation();
        }
    }

    public void TrySignin()
    {
        if (AccountManager.Instance.LoggedIn)
        {
            ShowPopup("Failed signin!", "This client is already logged in to another account. <br>If you want to switch, log out first");
            return;
        }
        _ = Signin();   // _ variable name, means it is ignored
    }
    private async Task Signin()
    {
        string email = signin_EmailField.text;
        string nickname = signin_NicknameField.text;

        string day = signin_dayField.text;
        string month = signin_monthField.text;
        string year = signin_yearField.text;
        string dateOfBirth = year + "-" + month + "-" + day;        

        string password = signin_PasswordField.text;


        if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(nickname) || string.IsNullOrEmpty(day) || string.IsNullOrEmpty(month) || string.IsNullOrEmpty(year) || string.IsNullOrEmpty(password)) 
        { 
            Debug.Log($"[Signin] missing credentials!");
            ShowPopup($"Failed signin!", $"Could not sign in. <br>missing credentials");
            return; 
        }
            
        Debug.Log($"[Signin] Attempting signin! {email}, {nickname}, {password}");
        string response = await LoginService.TrySignin(email, nickname, dateOfBirth, password);
        if (APIConnection.CheckResponseForErrorCode(response) || string.IsNullOrEmpty(response))
        {
            ShowPopup(APIConnection.responseCodeHeader[response], APIConnection.responseCodeMessage[response]);
        }
        else
        {
            ShowPopup($"Welcome {nickname}!", $"You successfully signed in with <br>{email}");
            PopulateAccountInformation();
            ClearInputFields();
        }
    }

    public void GoStartup()
    {
        SceneManager.LoadScene(startupScene);
    }

    private void ClearInputFields()
    {
        login_EmailField.text = string.Empty;
        login_PasswordField.text = string.Empty;

        signin_EmailField.text = string.Empty;
        signin_NicknameField.text = string.Empty;
        signin_PasswordField.text = string.Empty;

        signin_dayField.text = string.Empty;
        signin_monthField.text = string.Empty;
        signin_yearField.text = string.Empty;
    }

    private void PopulateAccountInformation()
    {
        StringBuilder builder = new StringBuilder();

        builder.Append(AccountManager.Instance.Nickname);
        builder.Append("<br>");
        builder.Append(AccountManager.Instance.User_Id);
        builder.Append("<br>");
        builder.Append(AccountManager.Instance.Email);
        builder.Append("<br>");
        builder.Append(AccountManager.Instance.DateOfBirth);

        string info = builder.ToString();
        builder.Clear();

        accountInfoData.text = info;

        builder.Append("Nickname: "); 
        builder.Append("<br>");
        builder.Append("Id: "); 
        builder.Append("<br>");
        builder.Append("Email: "); 
        builder.Append("<br>");
        builder.Append("Date of birth: ");

        string headers = builder.ToString();
        builder = null;

        accountInfoHeaders.text = headers;
    }
}
