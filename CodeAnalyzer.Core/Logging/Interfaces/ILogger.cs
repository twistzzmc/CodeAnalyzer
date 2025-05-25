namespace CodeAnalyzer.Core.Logging.Interfaces;

public interface ILogger
{
    void OpenLevel(string title, params object[] titleParameters);
    
    void CloseLevel();
    
    void Success(string message, params object[] messageParameters);
    
    void Info(string message, params object[] messageParameters);
    
    void Warning(string message, params object[] messageParameters);
    
    void Error(string message, params object[] messageParameters);
    
    void Exception(Exception ex);
}