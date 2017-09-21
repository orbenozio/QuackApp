using Assets.Scripts.Data;
using Assets.Scripts.Utils;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using GooglePlayGames;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public delegate void SceneRequestDelegate(bool result, Dictionary<string, SceneData> data);
public delegate void CharacterRequestDelegate(bool result, Dictionary<string, CharacterData> data);
public delegate void ActiveChatRequestDelegate(bool result, Dictionary<string, string> data);
public delegate void InviteChatRequestDelegate(bool result, Dictionary<string, string> data);
public delegate void UserRequestDelegate(bool result, User userData = null);
public delegate void ChatCharacterDelegate(bool result, List<ChatCharacterData> data = null);

public class DatabaseService : BaseSingleton<DatabaseService>
{
    #region Consts

    private const string DB_NODE_SCENES = "scenes";
    private const string DB_NODE_MEMBERS = "members";
    private const string DB_NODE_USERS = "users";
    private const string DB_NODE_CHATS = "chats";
    private const string DB_NODE_CHARACTERS = "characters";
    private const string DB_NODE_CHAT_CHARACTERS = "chatCharacters";
    private const string DB_NODE_RECORDS = "records";

    #endregion

    #region Events

    public event SceneRequestDelegate GetScenesEvent;
    public event CharacterRequestDelegate GetCharactersEvent;
    public event UserRequestDelegate SignInUserEvent;
    public event UserRequestDelegate GetUserDataEvent;
    public event UserRequestDelegate CreateUserDataEvent;
    public event ActiveChatRequestDelegate GetActiveChatEvent;

    public event InviteChatRequestDelegate GetInviteChatEvent;
    public event ChatCharacterDelegate SetChatCharacterEvent;
    public event ChatCharacterDelegate GetChatCharacterDataEvent;
    #endregion

    private DatabaseReference _databaseRef;
    private Dictionary<string, List<ChatCharacterData>> _chatCharacterDataDict;

    protected DatabaseService() { }

    public void InitializeFirebase()
    {
        // Set up the Editor before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://quack-53461818.firebaseio.com/");

        // Get the root reference location of the database.
        _databaseRef = FirebaseDatabase.DefaultInstance.RootReference;

        _chatCharacterDataDict = new Dictionary<string, List<ChatCharacterData>>();

        _databaseRef.Child(DB_NODE_CHAT_CHARACTERS).ChildAdded += ChatCharacters_ChildAdded;


    }

    #region API Methods  

    public void GetAvailableScenes()
    {
        _databaseRef.Child(DB_NODE_SCENES).OrderByKey().GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                var data = JSONSerialization<SceneData>.CreateDictionaryFromJSON(task.Result.GetRawJsonValue());
                if (GetScenesEvent != null)
                {
                    GetScenesEvent.Invoke(task.IsCompleted, data);
                }
            }
            else if (task.IsFaulted)
            {
            }
        });
    }

    public void GetSceneCharacters(int sceneId)
    {
        _databaseRef.Child(DB_NODE_CHARACTERS).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                var data = JSONSerialization<CharacterData>.CreateDictionaryFromJSON(task.Result.GetRawJsonValue());

                if (GetCharactersEvent != null)
                {
                    GetCharactersEvent.Invoke(task.IsCompleted, data);
                }
            }
            else if (task.IsFaulted)
            {
            }
        });
    }

    public void SignInUser(User user)
    {
        string json = JsonUtility.ToJson(user);

        _databaseRef.Child(DB_NODE_USERS).Child(user.Id).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                if (task.Result.ChildrenCount == 0)
                {
                    CreateNewUser(user);
                }
                else
                {
                    fireSignInUserEvent(true);
                }
            }
            else if (task.IsFaulted)
            {
            }
        });
    } 

    public void CreateChatRoom(ChatData data)
    {
        data.Uuid = Guid.NewGuid().ToString();
        data.CreatedUTC = DateTime.UtcNow.ToString();

        string json = JsonUtility.ToJson(data);

        _databaseRef.Child(DB_NODE_CHATS).Child(data.Uuid).SetRawJsonValueAsync(json).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                // Update chat count of the user
                updateUserChatData(data);

                // Update chat room members
                addMemberToChatRoom(data);
            }
            else if (task.IsFaulted)
            {
            }
        });
    }

    public void GetUserActiveChats(string userId)
    {
        _databaseRef.Child(DB_NODE_USERS).Child(userId).Child("ActiveChats").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                var data = JSONSerialization<Dictionary<string, string>>.CreateFromJSON(task.Result.GetRawJsonValue());

                if (GetActiveChatEvent != null)
                {
                    GetActiveChatEvent.Invoke(true, data);
                }
            }
            else if (task.IsFaulted)
            {
            }
        });
    }

    public void GetUserInviteChats(string userId)
    {
        _databaseRef.Child(DB_NODE_USERS).Child(userId).Child("InviteChats").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                var data = JSONSerialization<Dictionary<string, string>>.CreateFromJSON(task.Result.GetRawJsonValue());

                if (GetInviteChatEvent != null)
                {
                    GetInviteChatEvent.Invoke(true, data);
                }
            }
            else if (task.IsFaulted)
            {
            }
        });
    }

    public void SetUserChatCharacter(string chatId, ChatCharacterData chatCharacterData)
    {
        //var characterChatInstanceId = Guid.NewGuid().ToString();
        string json = JsonUtility.ToJson(chatCharacterData);

        _databaseRef.Child(DB_NODE_CHAT_CHARACTERS).Child(chatId).Child(chatCharacterData.UserId).SetRawJsonValueAsync(json).ContinueWith(task => 
        {
            if (SetChatCharacterEvent != null)
            {
                SetChatCharacterEvent.Invoke(!task.IsFaulted);
            }
        });
    }

    public void GetChatRoomData(string chatUuid)
    {
        //_databaseRef.Child(DB_NODE_CHAT_CHARACTERS).Child(chatUuid).ChildAdded += DatabaseService_ChatCharacterChildAdded;

        _databaseRef.Child(DB_NODE_MEMBERS).Child(chatUuid).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                //var data = JSONSerialization<Dictionary<string, string>>.CreateFromJSON(task.Result.GetRawJsonValue());

                //foreach (var character in _chatCharacterDataDict[chatUuid])
                //{
                //    _databaseRef.Child(DB_NODE_CHARACTERS).Child(character.CharacterId).GetValueAsync().ContinueWith(innerTask =>
                //    {
                //        character.Data = JSONSerialization<CharacterData>.CreateFromJSON(innerTask.Result.GetRawJsonValue());
                //    });
                //}

                if (GetChatCharacterDataEvent != null)
                {
                    GetChatCharacterDataEvent(true, _chatCharacterDataDict[chatUuid]);
                    //GetInviteChatEvent.Invoke(true, data);
                }
            }
            else if (task.IsFaulted)
            {
            }
        });
    }

    public void SaveRecord(RecordData data)
    {
        data.CreatedUTC = DateTime.UtcNow.ToShortDateString();

        var json = JSONSerialization<RecordData>.CreateFromObject(data);

        _databaseRef.Child(DB_NODE_RECORDS).Child(data.ChatRoomId).Push().SetRawJsonValueAsync(json).ContinueWith(task => 
        {

        });
    }

    #region User Data

    public void GetUserDataById(string id)
    {
        _databaseRef.Child(DB_NODE_USERS).Child(id).GetValueAsync().ContinueWith(task =>
        {
            User userdata = null;
            if (task.IsCompleted)
            {
                if (task.Result.Exists)
                {
                    userdata = JSONSerialization<User>.CreateFromJSON(task.Result.GetRawJsonValue());
                }
            }
            else if (task.IsFaulted)
            {
            }

            if (GetUserDataEvent != null)
            {
                GetUserDataEvent.Invoke(task.IsCompleted, userdata);
            }
        });
    }

    public void CreateNewUser(User user)
    {
        string json = JsonUtility.ToJson(user);

        _databaseRef.Child(DB_NODE_USERS).Child(user.Id).SetRawJsonValueAsync(json).ContinueWith(task =>
        {
            User userData = null;

            //if (task.IsCompleted)
            //{
            //    //userdata = JSONSerialization<User>.CreateFromJSON(task.Result.GetRawJsonValue());
            //}
            //else if (task.IsFaulted)
            //{
            //}

            if (CreateUserDataEvent != null)
            {
                CreateUserDataEvent.Invoke(task.IsCompleted, userData);
            }
        });
    }

    #endregion

    #endregion

    #region Private Methods

    private void ChatCharacters_ChildAdded(object sender, ChildChangedEventArgs e)
    {
        var parent = e.Snapshot.Key;

        if (!_chatCharacterDataDict.ContainsKey(parent))
        {
            _chatCharacterDataDict.Add(parent, new List<ChatCharacterData>());
        }

        var data = JSONSerialization<ChatCharacterData>.CreateDictionaryFromJSON(e.Snapshot.GetRawJsonValue());

        foreach (var character in data)
        {
            character.Value.Key = character.Key;
            _chatCharacterDataDict[parent].Add(character.Value);
        }

        foreach (var character in _chatCharacterDataDict[parent])
        {
            _databaseRef.Child(DB_NODE_CHARACTERS).Child(character.CharacterId).GetValueAsync().ContinueWith(innerTask =>
            {
                character.Data = JSONSerialization<CharacterData>.CreateFromJSON(innerTask.Result.GetRawJsonValue());
            });
        }

    }

    private void fireSignInUserEvent(bool success)
    {
        if (SignInUserEvent != null)
        {
           // SignInUserEvent.Invoke(success);
        }
    }


    private void updateUserChatData(ChatData chatData)
    {
        var user = AppManager.Instance.RegistrationManager.User;
        _databaseRef.Child(DB_NODE_USERS).Child(user.Id).Child("ChatCount").SetValueAsync(++user.ChatCount);
        _databaseRef.Child(DB_NODE_USERS).Child(user.Id).Child("ActiveChats").Child(chatData.Uuid).SetValueAsync(chatData.ChatName);
    }

    private void addMemberToChatRoom(ChatData chatData)
    {
        var user = AppManager.Instance.RegistrationManager.User;
        _databaseRef.Child(DB_NODE_MEMBERS).Child(chatData.Uuid).Child(user.Id).SetValueAsync(true);
    }    

    #endregion

}
