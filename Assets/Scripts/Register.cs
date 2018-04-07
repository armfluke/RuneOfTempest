using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class Register : MonoBehaviour {

    public DatabaseReference reference;

    public GameObject username;
    public GameObject password;
    public GameObject email;
    public GameObject confirmPassword;

    public Text ErrorRegisterMessage;
    public string errorMessage = "";
    //public GameObject errorPanel;

    public void RegisterButtonClick()
    {
        UserInformation userData = new UserInformation(username.GetComponent<InputField>().text.ToLower(), password.GetComponent<InputField>().text.ToLower(), email.GetComponent<InputField>().text.ToLower());
        ValidateRegister(userData);
    }

    public Task<DataSnapshot> ReadData()
    {
        return FirebaseDatabase.DefaultInstance.GetReference("UserData").GetValueAsync();
    }

    private void ValidateRegister(UserInformation userRegisterData)
    {
        //read data
        ReadData().ContinueWith(task =>
        {
            bool flag;
            if (task.IsFaulted)
            {
                // Handle the error...
                Debug.Log("Error to read data from firebase database");
                errorMessage = "Error to read data from database";
                flag = false;
            }
            else if (task.IsCompleted)
            {
                flag = true;
                // Do something with snapshot...
                DataSnapshot snapshot = task.Result;
                
                //read all key
                IDictionary test = (IDictionary)snapshot.Value;
                //loop for to check if new username is 
                foreach (string key in test.Keys)
                {
                    Debug.Log(key);//print all key
                    if(userRegisterData.username==key)
                    {
                        flag = false;
                        Debug.Log("This username already use");
                        errorMessage = "This username already use";
                    }
                }

                if(userRegisterData.username == "" || userRegisterData.password == "" || confirmPassword.GetComponent<InputField>().text == "" || userRegisterData.email == "")
                {
                    Debug.Log("False it's empty");
                    errorMessage = "You forget to input data in some field please try again!";
                    flag = false;
                }
                else if(userRegisterData.password != confirmPassword.GetComponent<InputField>().text.ToLower())
                {
                    Debug.Log("Password and confirm password are not matched try again!");
                    errorMessage = "Password and confirm password are not matched try again!";
                    flag = false;
                }
                Debug.Log("Print Flag" + flag);

                if (flag == true){
                    //Write data
                    string json = JsonUtility.ToJson(userRegisterData); //to convert object to raw json
                    Debug.Log(json);
                    string key = this.reference.Child("UserData").Push().Key;
                    Debug.Log("Key at register" + key);
                    this.reference.Child("UserData").Child(userRegisterData.username).SetRawJsonValueAsync(json); //key
                                                                                                         //popup window to remind them register successful!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    //Create UserData object to store user data
                    GameObject userDataObject = (GameObject)Resources.Load("Prefabs/UserData");
                    userDataObject = (GameObject)Instantiate(userDataObject, Vector3.zero, Quaternion.identity);
                    userDataObject.name = "UserData";
                    UserData userData = userDataObject.GetComponent<UserData>();
                    userData.username = userRegisterData.username;
                    userData.email = userRegisterData.email;
                    userData.score = 0;
                    
                    //Store username to remember user login
                    PlayerPrefs.SetString("UserData", userRegisterData.username);
                    SceneManager.LoadScene("Main");
                }else{
                    ErrorRegisterMessage.text = errorMessage;
                    GameObject.Find("LoginRegisterCanvas").transform.Find("ErrorPanel").gameObject.SetActive(true);
                }
            }
        });
    }

	// Use this for initialization
	void Start () {
        // Set up the Editor before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://rune-of-tempest.firebaseio.com/");

        // Get the root reference location of the database.
        this.reference = FirebaseDatabase.DefaultInstance.RootReference;

        
    }
	
	// Update is called once per frame
	void Update () {

    }
}
