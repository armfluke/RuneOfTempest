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
    public string newErrorMessageTwo = "";
    //public GameObject errorPanel;

    public void RegisterButtonClick()
    {
        UserInformation newuser = new UserInformation(username.GetComponent<InputField>().text.ToLower(), password.GetComponent<InputField>().text.ToLower(), email.GetComponent<InputField>().text.ToLower());
        ValidateRegister(newuser);
    }

    public Task<DataSnapshot> ReadData()
    {
        return FirebaseDatabase.DefaultInstance.GetReference("UserData").GetValueAsync();
    }

    private void ValidateRegister(UserInformation newuser)
    {
        //read data
        ReadData().ContinueWith(task =>
        {
            bool flag;
            if (task.IsFaulted)
            {
                // Handle the error...
                Debug.Log("Error to read data from firebase database");
                newErrorMessageTwo = "Error to read data from database";
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
                    if(newuser.username==key)
                    {
                        flag = false;
                        Debug.Log("This username already use");
                        newErrorMessageTwo = "This username already use";
                    }
                }

                if(newuser.username == "" || newuser.password == "" || confirmPassword.GetComponent<InputField>().text == "" || newuser.email == "")
                {
                    Debug.Log("False it's empty");
                    newErrorMessageTwo = "You forget to input data in some field please try again!";
                    flag = false;
                }
                else if(newuser.password != confirmPassword.GetComponent<InputField>().text.ToLower())
                {
                    Debug.Log("Password and confirm password are not matched try again!");
                    newErrorMessageTwo = "Password and confirm password are not matched try again!";
                    flag = false;
                }
                Debug.Log("Print Flag" + flag);

                if (flag == true){
                    //Write data
                    string json = JsonUtility.ToJson(newuser); //to convert object to raw json
                    Debug.Log(json);
                    string key = this.reference.Child("UserData").Push().Key;
                    Debug.Log("Key at register" + key);
                    this.reference.Child("UserData").Child(newuser.username).SetRawJsonValueAsync(json); //key
                                                                                                         //popup window to remind them register successful!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    //Save username with playerprefs
                    PlayerPrefs.SetString("UserData", newuser.username);
                    SceneManager.LoadScene("Main");
                }else{
                    ErrorRegisterMessage.text = newErrorMessageTwo;
                    GameObject.Find("LoginRegisterCanvas").transform.Find("ErrorRegisterPanel").gameObject.SetActive(true);
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
