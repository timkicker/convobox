using System;
using ReactiveUI;
using Convobox.Client.ViewModels;

namespace Convobox.Client.Models;


public class InternLoggerViewModel : ViewModelBase
{
    private string _debugLog;


    public void Log(string source, string message)
    {
        DebugLog += $"[{DateTime.Now}] ({source}): {message}\n";
    }

    public string DebugLog
    {
        get { return _debugLog; }
        private set { _debugLog = this.RaiseAndSetIfChanged(ref _debugLog, value); }
    }
}