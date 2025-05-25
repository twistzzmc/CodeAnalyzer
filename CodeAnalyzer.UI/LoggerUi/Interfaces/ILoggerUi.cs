using System;
using CodeAnalyzer.UI.LoggerUi.Dtos;

namespace CodeAnalyzer.UI.LoggerUi.Interfaces;

public interface ILoggerUi
{
    void AddEntry(LogEntry entry);
    
    void AddEntry(string message);

    void AddEntry(Exception ex);
}