using DAL.Models;
using System.Windows;
using WpfApp.View;

namespace WpfApp
{
    public partial class App : Application
    {
        public static Account CurrentAccount { get; set; }
    }
}
