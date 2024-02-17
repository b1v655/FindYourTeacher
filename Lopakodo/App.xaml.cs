using System;
using System.Windows;
using Lopakodo.Persistence;
using Lopakodo.Model;
using Lopakodo.View;
using Lopakodo.ViewModel;
using Microsoft.Win32;
using System.ComponentModel;


namespace Lopakodo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private GameModel _model;
        private LopakodoViewModel _viewModel;
        private MainWindow _view;
        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
        }
        private void App_Startup(object sender, StartupEventArgs e)
        {
            _model = new GameModel(new LopakodoFileDataAccess());
            _model.GameOver += new EventHandler<LopakodoGameArgs>(Model_GameOver);
           
           

            // nézemodell létrehozása
            _viewModel = new LopakodoViewModel(_model);
            _viewModel.ExitGame += new EventHandler(ViewModel_ExitGame);
            _viewModel.LoadGame += new EventHandler(ViewModel_LoadGame);
            _viewModel.SaveGame += new EventHandler(ViewModel_SaveGame);

            // nézet létrehozása
            _view = new MainWindow();
            _view.DataContext = _viewModel;
            _view.Closing += new System.ComponentModel.CancelEventHandler(View_Closing); // eseménykezelés a bezáráshoz
            _view.Show();

        }
        private void View_Closing(object sender, CancelEventArgs e)
        {

            _model.setTimerOff();
            if (MessageBox.Show("Biztos, hogy ki akar lépni?", "Lopakodo", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                e.Cancel = true; // töröljük a bezárást

                
            }
            _model.setTimerOn();
        }              
        private void ViewModel_ExitGame(object sender, System.EventArgs e)
        {
            _view.Close(); // ablak bezárása
        }
        private void Model_GameOver(Object sender, Model.LopakodoGameArgs e)
        {
            if (e.IsWon == true)
                MessageBox.Show("Győzelem!", "Játék vége!");
            else
                MessageBox.Show("Elkaptak.", "Játék vége!");
            _viewModel.NewGame(_model.TableSize);

        }
        private async void  ViewModel_LoadGame(object sender, EventArgs e)
        {
            _model.setTimerOff();
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog(); // dialógusablak
                openFileDialog.Title = "Lopakodo Betoltes";
                if (openFileDialog.ShowDialog() == true)
                {
                    // játék betöltése
                    await _model.LoadGame(openFileDialog.FileName);
                    
                }
            }
            catch (LopakodoDataException)
            {
                MessageBox.Show("Játék mentése sikertelen!" + Environment.NewLine + "Hibás az elérési út, vagy a könyvtár nem írható.", "Hiba!");
            }
          
            _model.setTimerOn();
        }
        private async void ViewModel_SaveGame(object sender, EventArgs e)
        {
            _model.setTimerOff();
            SaveFileDialog saveFileDialog = new SaveFileDialog(); // dialógablak
            saveFileDialog.Title = "Lopakodó játék betöltése";
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    // játéktábla mentése
                    await _model.SaveGame(saveFileDialog.FileName);
                }
                
                catch (LopakodoDataException)
                {
                    MessageBox.Show("Játék mentése sikertelen!" + Environment.NewLine + "Hibás az elérési út, vagy a könyvtár nem írható.", "Hiba!");
                }
            }
            _model.setTimerOn();
        }
    }
}
