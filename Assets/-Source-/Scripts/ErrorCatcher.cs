using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

using CommonGames.Utilities;
using UnityEngine;

using JetBrains.Annotations;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

//using Sirenix.OdinInspector;
//using Sirenix.Serialization;

public class ErrorCatcher : Singleton<ErrorCatcher>
{
    #region Variables
    
    #region Paths

    // ReSharper disable once InconsistentNaming
    private string _Internal_EditorPath = null;
    [PublicAPI]
    public string EditorPath //AppData/Local/Unity/Editor/
    {
        get
        {
            if(!string.IsNullOrEmpty(_Internal_EditorPath)) return _Internal_EditorPath;
            
            string __path =  Application.persistentDataPath;

            return _Internal_EditorPath = (__path.Remove(__path.Length - 31) + @"Local/Unity/Editor/");
        }
    }
    
    // ReSharper disable once InconsistentNaming
    private string _Internal_EditorLog = null;
    [PublicAPI]
    public string EditorLog //AppData/Local/Unity/Editor/Editor.log
    {
        get
        {
            if(!string.IsNullOrEmpty(_Internal_EditorLog)) return _Internal_EditorLog;

            return _Internal_EditorLog = EditorPath + @"Editor.log";
        }
    }
    
    // ReSharper disable once InconsistentNaming
    private string _Internal_EditorLogCopy = null;
    [PublicAPI]
    public string EditorLogCopy //AppData/Local/Unity/Editor/EditorCopy.log
    {
        get
        {
            if(!string.IsNullOrEmpty(_Internal_EditorLogCopy)) return _Internal_EditorLogCopy;

            return _Internal_EditorLogCopy = EditorPath + @"EditorCopy.log";
        }
    }

    #endregion

    #region File Hash

    // ReSharper disable once InconsistentNaming
    private byte[] Internal_OldHash { get; set; }
    private byte[] OldHash
    {
        get => Internal_OldHash ?? (Internal_OldHash = GetFileHash());
        set => Internal_OldHash = value;
    }

    #endregion
    
    private const string _NULL_REFERENCE_TEXT = "NullReferenceException: ";

    //[OdinSerialize]
    private int LastErrorCount { get; set; } = 0;
    
    //[OdinSerialize]
    private int CurrErrorCount { get; set; } = 0;

    public event Action<int> NewErrors_Event;

    private FileSystemWatcher _fileSystemWatcher = null; 
    
    #endregion

    #region Methods

    private void Start()
    {
        int? __initialErrors = CopyAndCountErrors();

        LastErrorCount = __initialErrors ?? CurrErrorCount;
        //OldHash = GetFileHash();
        
        LateStart();
    }

    private void LateStart()
    {
        if(!File.Exists(EditorLog))
        {
            Debug.LogError($"File {EditorLog} doesn't exist!");
            
            return;
        }

        if(_fileSystemWatcher != null)
        {
            _fileSystemWatcher.Changed -= OnEditorLogChanged;   
        }

        _fileSystemWatcher = new FileSystemWatcher { Path = EditorPath };

        _fileSystemWatcher.Changed += OnEditorLogChanged;
        
        _fileSystemWatcher.EnableRaisingEvents = true;
    }

    private void ResetOnChangedSubscriptions()
    {
        if(_fileSystemWatcher != null)
        {
            _fileSystemWatcher.Changed -= OnEditorLogChanged;    
            //_fileSystemWatcher.Changed = null;
        }
    }

    private void OnDisable()
    {
        
    }

    private void OnEditorLogChanged(object sender, FileSystemEventArgs eventArgs)
    {
        //if(!IsPlaying) return;

        //Checks if the file that has changed is 'Editor.log'
        if(!eventArgs.Name.EndsWith(value: "Editor.log")) return;

        int? __errorCount = CopyAndCountErrors();
        if(__errorCount == null) return;
        
        CurrErrorCount = (int)__errorCount;
        
        Debug.Log(message: "\n" +
            $"<color=cyan>Current Errors: {CurrErrorCount} </color> \n" +
            $"<color=cyan>Last Errors: {LastErrorCount} </color> \n" +
            $"<color={(CurrErrorCount > LastErrorCount ? "lime" : "orange")}>Current is {(CurrErrorCount > LastErrorCount ? "More" : "Less or Equal")} than before. </color>");
        
        if(CurrErrorCount > LastErrorCount)
        {
            int __lasErrorCount = LastErrorCount;
            int __currErrorCount = CurrErrorCount;

            SetLastErrorCount();

            OnNewErrorsEvent(__currErrorCount - __lasErrorCount);
        }

        SetLastErrorCount();
    }

    private int? CopyAndCountErrors()
    {
        if(!File.Exists(EditorLog))
        {
            Debug.LogError($"File {EditorLog} doesn't exist!");
            
            return null;
        }
        
        File.Copy(
            sourceFileName: EditorLog, 
            destFileName: EditorLogCopy, 
            overwrite: true);
        
        string __text = File.ReadAllText(EditorLogCopy);
        
        return Regex.Matches(__text, pattern: _NULL_REFERENCE_TEXT).Count;
    }
    private int? ReadAndCountErrors()
    {
        if(!File.Exists(EditorLog))
        {
            Debug.LogError($"File {EditorLog} doesn't exist!");
            
            return null;
        }
        
        StreamReader __file = new StreamReader(path: EditorLog);
        string __line;

        int __errorCounter = 0;

        while ((__line = __file.ReadLine()) != null)
        {
            if (__line.Contains(_NULL_REFERENCE_TEXT))
            {
                break;
            }

            __errorCounter++;
        }
        
        return __errorCounter;
    }

    /// <summary>
    /// Sets the <see cref="LastErrorCount"/> to the <see cref="CurrErrorCount"/>.
    /// (For comparison with the next error count)
    /// </summary>
    private void SetLastErrorCount() => LastErrorCount = CurrErrorCount;

    /// <returns> Whether the file has changed or not. </returns>
    private bool HasFileChanged() => !GetFileHash().SequenceEqual(second: OldHash);

    private byte[] GetFileHash()
    {
        HashAlgorithm __sha1 = HashAlgorithm.Create();
        using(FileStream __stream = new FileStream(EditorLog, FileMode.Open, FileAccess.Read))
        {
            return __sha1.ComputeHash(__stream);
        }
    }

    /// <summary> Called when there are new errors detected. </summary>
    /// <param name="errors"> amount of new errors. </param>
    protected virtual void OnNewErrorsEvent(in int errors)
    {
        NewErrors_Event?.Invoke(errors);
    }

    private static bool IsPlaying
    {
        get
        {
            return Application.isPlaying 
                #if UNITY_EDITOR
                   || EditorApplication.isPlaying 
                #endif
                ;
        }
    }

    #endregion
}
