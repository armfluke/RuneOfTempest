using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

public class UserInformation : MonoBehaviour {

    public string username;
    public string password;
    public string confirmPassword;
    public string email;
    

    public UserInformation(string username, string password, string confirmPassword, string email)
    {
        this.username = username;
        this.password = password;
        this.confirmPassword = confirmPassword;
        this.email = email;
    }

    

}


public class UserInformationOldUser : MonoBehaviour{

    public string oldUsername;
    public string oldPassword;

    public UserInformationOldUser(string oldUsername, string oldPassword)
    {
        this.oldUsername = oldUsername;
        this.oldPassword = oldPassword;
    }
}