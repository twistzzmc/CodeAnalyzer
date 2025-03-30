using System;
using CodeAnalyzer.UI.LoggerUi.Dtos;

namespace CodeAnalyzer.UI.LoggerUi.Interfaces;

public interface ILoggerUi
{
    void Log(LogEntry entry);
    
    void Log(string message);

    void Log();

    void Log(Exception ex);
}