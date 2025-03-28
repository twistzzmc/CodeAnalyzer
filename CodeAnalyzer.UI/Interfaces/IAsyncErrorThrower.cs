using System;

namespace CodeAnalyzer.UI.Interfaces;

internal interface IAsyncErrorThrower
{
    event EventHandler<Exception>? OnError;
}