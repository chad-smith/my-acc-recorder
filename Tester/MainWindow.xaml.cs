using System;
using System.ComponentModel;
using System.Windows;

namespace MyAcc.Recorder.Tester {
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow: Window {
    private readonly AccManager _manager;
    private readonly FakeAccUdpConnection _connection;

    public MainWindow() {
      InitializeComponent();

      this.Closing += WindowClosing;
      ;

      _connection = new FakeAccUdpConnection();
      _manager = new AccManager( _connection );
      _manager.Start();
    }

    private void WindowClosing( object sender, CancelEventArgs e ) {
      _manager.Stop();
    }

    private void TestButton_OnClick( object sender, RoutedEventArgs e ) {
      _connection.SendTest();
    }
  }
}