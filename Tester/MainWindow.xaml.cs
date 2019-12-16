using System;
using System.Windows;
using MyAcc.Recorder.Enums;
using MyAcc.Recorder.Responses;

namespace MyAcc.Recorder.Tester {
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow: Window {
    private readonly AccManager _manager;
    private readonly FakeAccConnection _connection;

    public MainWindow() {
      _connection = new FakeAccConnection();
      _manager = new AccManager( _connection );
      
      InitializeComponent();
      Closed += ( sender, args ) => _manager.Stop();
    }

    private void TestButton_Click( object sender, RoutedEventArgs e ) {
      _connection.InvokeFakeResponse( new RegistrationResponse( 10, true ) );

      _connection.InvokeFakeResponse(
        new RealTimeUpdateResponse(
          SessionPhase.PreSession,
          TimeSpan.FromMinutes( 0 ),
          false,
          TimeSpan.FromMinutes( 20 ),
          SessionType.Qualifying,
          28,
          36
        )
      );

      _connection.InvokeFakeResponse( new TrackDataResponse( "Brands Hatch" ) );
    }

    private async void Window_Loaded( object sender, RoutedEventArgs e ) {
      await _manager.Start();
    }
  }
}