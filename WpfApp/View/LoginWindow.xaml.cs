using BLL.Service;  // Import AccountService
using DAL.Models;
using DAL.Repos;
using System;
using System.Windows;

namespace WpfApp.View
{
    public partial class LoginWindow : Window
    {
        private readonly AccountService _accountService;

        // Constructor
        public LoginWindow()
        {
            InitializeComponent();
            _accountService = new AccountService();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text;
            string password = txtPassword.Password;
            var account = _accountService.GetAccount(email, password);
            if (account != null) 
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Account = account;
                mainWindow.Show();
                this.Close();
            }
        }
    }
}
