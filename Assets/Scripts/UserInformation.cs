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
    public string email;
    public int score;
    

    public UserInformation(string username, string password, string email){
        this.username = username;
        this.password = password;
        this.email = email;
        this.score = 0;
    }

    

}


public class UsernamePassword : MonoBehaviour{

    public string username;
    public string password;

    public UsernamePassword(string username, string password){
        this.username = username;
        this.password = password;
    }
}