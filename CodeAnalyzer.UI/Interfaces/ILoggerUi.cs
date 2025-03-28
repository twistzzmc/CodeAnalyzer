using System;

namespace CodeAnalyzer.UI.Interfaces;

public interface ILoggerUi
{
    void Log(string message);

    void Log();

    void Log(Exception ex);
}