using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Loadbalancer
{
    public partial class MainWindow : Window
    {
        Proxy proxy;
        delegate void updateList(string input);

        public MainWindow()
        {
            InitializeComponent();
        }

        private void startbtn_click(object sender, RoutedEventArgs e)
        {
            notifier.Items.Add("Starting Server");
            proxy = new Proxy();
            proxy.isalive = true;
            proxy.start(this);
        }

        private void stopbtn_click(object sender, RoutedEventArgs e)
        {
            if (proxy != null)
            {
                notifier.Items.Add("Stopping Server");
                proxy.isalive = false;
                proxy.stopTcpListener();
            }
            else
            {
                notifier.Items.Add("Server is not running");
            }
        }

        public void updateNotifier(string update)
        {
            if (notifier.Dispatcher.Thread == Thread.CurrentThread)
            {
                notifier.Items.Add(update);
            }
            else
            {
                 //InvalidOperationExceptionHandling
                updateList list = new updateList(updateNotifier);
                notifier.Dispatcher.BeginInvoke(list, new object[] { update });
            }
        }

        private void runningserversbtn_click(object sender, RoutedEventArgs e)
        {
            HealthCheck healthcheck = new HealthCheck();
            healthcheck.Show();
        }

    }
}