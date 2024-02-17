using System;
using Lopakodo.Model;
using System.Collections.ObjectModel;
namespace Lopakodo.ViewModel
{
    class LopakodoViewModel : ViewModelBase
    {
        private GameModel _model;
        public DelegateCommand EasyGameCommand { get; private set; }
        public DelegateCommand MiddleGameCommand { get; private set; }
        public DelegateCommand HardGameCommand { get; private set; }
        public DelegateCommand LoadGameCommand { get; private set; }
        public DelegateCommand SaveGameCommand { get; private set; }
        public DelegateCommand ExitCommand { get; private set; }
        public DelegateCommand stepCommand { get; private set; }
        public DelegateCommand PauseCommand { get; private set; }
        public ObservableCollection<LopakodoField> Fields { get; set; }




        public LopakodoViewModel(GameModel model)
        {
           
            // játék csatlakoztatása
            _model = model;
            _model.PlayerMovedEvent += new EventHandler(PlayerMovedEvent);
            _model.GuardsMovedEvent += new EventHandler(Game_GuardsMovedEvent);
            _model.GeneratedTable += new EventHandler(Game_GeneratedTable);


            // parancsok kezelése
            stepCommand = new DelegateCommand(param => _model.RegisterMove(param.ToString()));
            EasyGameCommand = new DelegateCommand(param => EasyGame());
            MiddleGameCommand = new DelegateCommand(param => MiddleGame());
            HardGameCommand = new DelegateCommand(param => HardGame());
            LoadGameCommand = new DelegateCommand(param => OnLoadGame());
            SaveGameCommand = new DelegateCommand(param => OnSaveGame());
            ExitCommand = new DelegateCommand(param => OnExitGame());
            PauseCommand = new DelegateCommand(param => Pause());
            // játéktábla létrehozása
            NewGame(11);
        }
        public void NewGame(Int32 size)
        {
            GenerateTable(size);
            if (size == 11) _model.NewGame(0);
            if (size == 9) _model.NewGame(1);
            if (size == 7) _model.NewGame(2);
            //SetTimer();
        }
        
        private void GenerateTable(Int32 size)
        {
            Fields = new ObservableCollection<LopakodoField>();
            for (Int32 i = 0; i < _model.TableSize; i++) // inicializáljuk a mezőket
            {
                for (Int32 j = 0; j < 5; j++)
                {
                    Fields.Add(new LopakodoField
                    {
                        IsLocked = false,
                        Text = String.Empty,
                        X = i,
                        Y = j,
                        Number = i * _model.TableSize + j, 
                        
                    });
                }
            }
            OnPropertyChanged("Fields");
        }

        private void Game_GeneratedTable(Object sender, EventArgs e)
        {
            GenerateTable(_model.TableSize);
            Fields[_model.Player.cord.x  + _model.Player.cord.y * _model.TableSize].Text = "P";
            for (Int32 i = 0; i < (_model.TableSize+1); i++)
            {
                Fields[_model.Walls[i].x + _model.Walls[i].y * _model.TableSize].Text = "W";
                Fields[_model.Walls[i].x + _model.Walls[i].y * _model.TableSize].IsLocked=true;
            }
          
        }
        private void Game_PlayerMovedEvent(Object sender, EventArgs e)
        {
            switch (_model.Player.actualWay)
            {
                case Model.Way.Down:
                    Fields[_model.Player.cord.x  + (_model.Player.cord.y - 1) * _model.TableSize].Text = "";
                    Fields[_model.Player.cord.x + _model.Player.cord.y * _model.TableSize].Text = "P";
                    break;
                case Model.Way.Up:
                    Fields[_model.Player.cord.x + (_model.Player.cord.y + 1) * _model.TableSize].Text = "";
                    Fields[_model.Player.cord.x + _model.Player.cord.y * _model.TableSize].Text = "P";
                    break;
                case Model.Way.Left:
                    Fields[_model.Player.cord.x + 1 + _model.Player.cord.y * _model.TableSize].Text = "";
                    Fields[_model.Player.cord.x + _model.Player.cord.y * _model.TableSize].Text = "P";
                    break;
                case Model.Way.Right:
                    Fields[_model.Player.cord.x - 1  + _model.Player.cord.y * _model.TableSize].Text = "";
                    Fields[_model.Player.cord.x  + _model.Player.cord.y * _model.TableSize].Text = "P";
                    break;

            }
            for (Int32 i = 0; i < (_model.TableSize + 1); i++)
            {
                Fields[_model.Walls[i].x + _model.Walls[i].y * _model.TableSize].Text = "W";
            }
            OnPropertyChanged("Fields");
        }
        private void Game_GuardsMovedEvent(Object sender, EventArgs e)
        {
            
        }
        private void PlayerMovedEvent(Object sender, EventArgs e)
        {
            switch (_model.Player.actualWay)
            {
                case Model.Way.Down:
                    Fields[_model.Player.cord.x  + (_model.Player.cord.y - 1 )* _model.TableSize].Text = "";
                    Fields[_model.Player.cord.x  + _model.Player.cord.y * _model.TableSize].Text = "P";
                    break;
                case Model.Way.Up:
                    Fields[_model.Player.cord.x +( _model.Player.cord.y + 1) * _model.TableSize].Text = "";
                    Fields[_model.Player.cord.x + _model.Player.cord.y * _model.TableSize].Text = "P";
                    break;
                case Model.Way.Left:
                    Fields[(_model.Player.cord.x + 1)  + _model.Player.cord.y * _model.TableSize].Text = "";
                    Fields[_model.Player.cord.x  + _model.Player.cord.y * _model.TableSize].Text = "P";
                    break;
                case Model.Way.Right:
                    Fields[(_model.Player.cord.x - 1)  + _model.Player.cord.y * _model.TableSize].Text = "";
                    Fields[_model.Player.cord.x  + _model.Player.cord.y * _model.TableSize].Text = "P";
                    break;

            }
            for(Int32 i = 0; i < (_model.TableSize + 1); i++)
            {
                Fields[_model.Walls[i].x + _model.Walls[i].y * _model.TableSize].Text = "W";
                Fields[_model.Walls[i].x + _model.Walls[i].y * _model.TableSize].IsLocked = !_model.Walls[i].status;
            }
            OnPropertyChanged("Fields");
            //Fields[5 * _model.TableSize].Text = _model.wasOpened.ToString();
        }
        private void Pause()
        {
            _model.pause();
        }
        private void MiddleGame()
        {
            NewGame(9);


        }
        private void HardGame()
        {
            NewGame(7);
        }

        private void EasyGame()
        {
            NewGame(11);
        }
        public event EventHandler LoadGame;
        
        public event EventHandler SaveGame;
        
        public event EventHandler ExitGame;




       
        private void OnLoadGame()
        {
            if (LoadGame != null)
                LoadGame(this, EventArgs.Empty);
        }
        
        private void OnSaveGame()
        {
            if (SaveGame != null)
                SaveGame(this, EventArgs.Empty);
        }
        
        private void OnExitGame()
        {
            if (ExitGame != null)
                ExitGame(this, EventArgs.Empty);
        }
    }
}
