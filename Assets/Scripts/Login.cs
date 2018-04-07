using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using Firebase.Auth;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
//using Facebook.Unity;

public class Login : MonoBehaviour {

    public DatabaseReference reference;

    public GameObject oldUsername;
    public GameObject oldPassword;

    public Text userIdText;
    private FirebaseAuth mAuth;

    public Text ErrorLoginMessage;
    public string newErrorMessage = "";

    public void LoginButtonClick(){
        UsernamePassword userData = new UsernamePassword(oldUsername.GetComponent<InputField>().text.ToLower(), oldPassword.GetComponent<InputField>().text.ToLower());
        SearchUserData(userData);
    }

    /*public void FacebookButtonClick()
    {
        //Database newuser = new Database(username.GetComponent<InputField>().text, email.GetComponent<InputField>().text, password.GetComponent<InputField>().text, confirmPassword.GetComponent<InputField>().text);
        //ValidateRegister(newuser);
        Debug.Log("FACEBOOK EIEI");
        Awake();
        LogIn();
       
        
    }

    private void Awake()
    {
        if (!FB.IsInitialized)
        {
            FB.Init();
            Debug.Log("Facebook restart");
        }
        else
        {
            FB.ActivateApp();
            Debug.Log("Facebook Already activate");
        }
    }*/

    /*public void LogIn()
    {
        Debug.Log("I'm in LLLLLLOGIN function");
        FB.LogInWithReadPermissions(callback: OnLogIn);
    }*/

    /*private void OnLogIn(ILoginResult result)
    {
        Debug.Log("Here is result"+result);
        Debug.Log("Yo i'm On login");
        if (FB.IsLoggedIn)
        {
            Debug.Log("Im in is login");
            AccessToken tocken = AccessToken.CurrentAccessToken;
            //userIdText.text = tocken.UserId;
            
            Credential credential = FacebookAuthProvider.GetCredential(tocken.TokenString);
            Debug.Log("Im in is login and this is a tocken" + tocken.TokenString);
            //Credential credential = FacebookAuthProvider.GetCredential(tocken.UserId);
        }
        else
        {
            Debug.Log("Login Failed");
        }
    }

    public void accessToken(Credential firebaseResult)
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        if (!FB.IsLoggedIn)
        {
            return;
        }

        auth.SignInWithCredentialAsync(firebaseResult).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }

            FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });
    }*/

    public Task<DataSnapshot> ReadData(){
        return FirebaseDatabase.DefaultInstance.GetReference("UserData").GetValueAsync();
    }

    public void SearchUserData(UsernamePassword usernamePassword){
        //read data
        ReadData().ContinueWith(task => {
            if (task.IsFaulted){
                // Handle the error...
                Debug.Log("Error to read data from firebase database");

            }else if (task.IsCompleted){
                DataSnapshot snapshot = task.Result;
                //read all key
                IDictionary test = (IDictionary)snapshot.Value;
                //loop for to check if new username is 
                foreach (string key in test.Keys){
                    if(usernamePassword.username == key){
                        Debug.Log("Username is matched");
                        //Debug.Log("YO IS MATCHED!!"+ snapshot.Child(key).Child("password").GetValue(true));
                        if (usernamePassword.password == (snapshot.Child(key).Child("password").GetValue(true).ToString())){
                            Debug.Log("Password is matched");

                            //Create UserData object to store user data
                            GameObject userDataObject = (GameObject)Resources.Load("Prefabs/UserData");
                            userDataObject = (GameObject)Instantiate(userDataObject, Vector3.zero, Quaternion.identity);
                            userDataObject.name = "UserData";
                            UserData userData = userDataObject.GetComponent<UserData>();
                            userData.username = usernamePassword.username;
                            userData.email = snapshot.Child(key).Child("email").GetValue(true).ToString();
                            userData.score = int.Parse(snapshot.Child(key).Child("score").GetValue(true).ToString());

                            //Stroe username to remember user login
                            PlayerPrefs.SetString("UserData", key);
                            SceneManager.LoadScene("Main");
                        }
                        else{
                            newErrorMessage = "Username and password is incorrect please try again";
                        }
                    }
                }

                //check input login in null
                if(usernamePassword.username == "" && usernamePassword.password == ""){
                    ErrorLoginMessage.text = newErrorMessage;
                    GameObject.Find("LoginRegisterCanvas").transform.Find("ErrorPanel").gameObject.SetActive(true);
                }


            }
        });

    }

    public void CheckLogin(){
        if(PlayerPrefs.HasKey("UserData")){
            string username = PlayerPrefs.GetString("UserData");
            Debug.Log("User: " + username + " is logged in!!!");

            //Create UserData object to store user data
            GameObject userDataObject = (GameObject)Resources.Load("Prefabs/UserData");
            userDataObject = (GameObject)Instantiate(userDataObject, Vector3.zero, Quaternion.identity);
            userDataObject.name = "UserData";
            UserData userData = userDataObject.GetComponent<UserData>();
            userData.username = username;
            ReadData().ContinueWith(task => {
                DataSnapshot snapshot = task.Result;
                IDictionary data = (IDictionary)snapshot.Value;
                foreach (string key in data.Keys){
                    if(username == key){
                        userData.email = snapshot.Child(key).Child("email").GetValue(true).ToString();
                        userData.score = int.Parse(snapshot.Child(key).Child("score").GetValue(true).ToString());
                        break;
                    }
                }
                SceneManager.LoadScene("Main");
            });
        }
    }

    // Use this for initialization
    void Start () {
        // Set up the Editor before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://rune-of-tempest.firebaseio.com/");

        // Get the root reference location of the database.
        this.reference = FirebaseDatabase.DefaultInstance.RootReference;

        //initiate auth
        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

        CheckLogin();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
