using System;
using System.IO;
using System.Text.RegularExpressions;
using CommonGames.Utilities;
using UnityEngine;

using JetBrains.Annotations;
using Sirenix.Serialization;

public class ErrorCatcher : Singleton<ErrorCatcher>
{
    #region Paths

    // ReSharper disable once InconsistentNaming
    private string _Internal_EditorPath = null;
    [PublicAPI]
    public string EditorPath
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
    public string EditorLog
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
    public string EditorLogCopy
    {
        get
        {
            if(!string.IsNullOrEmpty(_Internal_EditorLogCopy)) return _Internal_EditorLogCopy;

            return _Internal_EditorLogCopy = EditorPath + @"EditorCopy.log";
        }
    }

    #endregion
    
    private const string _NULL_REFERENCE_TEXT = "NullReferenceException: ";

    [OdinSerialize]
    private int LastErrorCount { get; set; } = 0;
    
    [OdinSerialize]
    private int CurrErrorCount { get; set; } = 0;

    public event Action<int> NewErrors_Event;

    private void Start()
    {
        int? __initialErrors = CopyAndCountErrors();

        LastErrorCount = __initialErrors ?? CurrErrorCount;
    }

    private void Update()
    {
        if (Time.frameCount % 10 != 0) return;

        CurrErrorCount = CopyAndCountErrors() ?? CurrErrorCount;

        if(CurrErrorCount > LastErrorCount)
        {
            OnNewErrorsEvent(CurrErrorCount - LastErrorCount);    
        }
        
        LastErrorCount = CurrErrorCount;
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
        string __line = "";

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

    /// <summary> Called when there are new errors detected. </summary>
    /// <param name="errors"> amount of new errors. </param>
    protected virtual void OnNewErrorsEvent(int errors)
    {
        NewErrors_Event?.Invoke(errors);
    }
}
