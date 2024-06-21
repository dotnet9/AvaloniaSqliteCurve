using Avalonia.Controls;
using System;

namespace AvaloniaSqliteCurve.Services;

public interface INotificationService
{
    int NotificationTimeout { get; set; }

    void SetHostWindow(TopLevel window);

    void Show(string title = "提示", string message = "提示内容", Action? onClick = null);
}