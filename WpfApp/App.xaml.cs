using System.Windows;

namespace ServiceDesk;

public partial class App : Application
{
    // Текущий авторизованный пользователь (глобальный)
    public static Models.User? CurrentUser { get; set; }
}
