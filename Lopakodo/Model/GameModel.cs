using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Threading;

namespace Lopakodo.Model
{

    public struct Coord
    {
        public int x;
        public int y;
        public int opt;
        public bool status;
    }


    public class Creatures
    {
        public Coord cord;
        public Way actualWay;

    }

    public enum Way { Left, Right, Down, Up }
    public enum GameDifficulty { Easy, Medium, Hard }
    public class GameModel
    {
        #region declare
        //table az inicializáláshoz szükséges segédtömb;
        private Int32[,] table;
        private Coord[] walls;
        private Random random = new Random();
        private Coord exit;
        private Creatures[] guards;
        private GameDifficulty gameDifficulty;
        private Creatures player;
        private Int32 tableSize=11;
        private Int32 gameTime = 0;
        private DispatcherTimer Timer;
        private Int32 starttimer = 0;
        public Int32 wasOpened=0;
        private Persistence.ILopakodoDataAccess _dataAccess;
        #endregion
        #region Difficulty constants
        private const Int32 smallTableSize = 7;
        private const Int32 middleTableSize = 9;
        private const Int32 greatTableSize = 11;
        #endregion
        #region Properties

        //Amikor meghal a játékos vége a játéknak.
        public Boolean IsGameOver { get { return false; } }

        public Coord[] Walls { get { return walls; } }
        public Coord Exit { get { return exit; } }
        public Creatures[] Guards { get { return guards; } }

        public Creatures Player { get { return player; } }
        public Int32 TableSize { get { return tableSize; } }

        public GameDifficulty GameDifficulty { get { return gameDifficulty; } set { gameDifficulty = value; } }

        #endregion
        public GameModel(Persistence.ILopakodoDataAccess dataA)
        {
            _dataAccess = dataA;
            table = new Int32[greatTableSize, greatTableSize];
            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromSeconds(1);
            Timer.Tick += new EventHandler(OnTimedEvent);
            Timer.Start();

        }
        #region Eventhandlers
        public event EventHandler<LopakodoGameArgs> GameOver;
        public event EventHandler GuardsMovedEvent;
        public event EventHandler PlayerMovedEvent;
        public event EventHandler GeneratedTable;
        public void pause()
        {
            if (Timer.IsEnabled) Timer.Stop();
            else Timer.Start();
        }
        public void RegisterMove(string key)
        {

            if (key.ToLower() == "w") MovePlayer(Way.Up);
            if (key.ToLower() == "s") MovePlayer(Way.Down);
            if (key.ToLower() == "a") MovePlayer(Way.Left);
            if (key.ToLower() == "d") MovePlayer(Way.Right);

        }
        #endregion
        #region init Table
        public void NewGame(int mode)
        {
            gameDifficulty = (GameDifficulty)mode;

            
            switch (gameDifficulty) // nehézségfüggő beállítása a táblának
            {
                case GameDifficulty.Easy:

                    tableSize = greatTableSize;
                    walls = new Coord[(tableSize+1) ];
                    table = new Int32[tableSize, 5];
                    GenerateObjects();

                    break;
                case GameDifficulty.Medium:
                    tableSize = middleTableSize;
                    walls = new Coord[(tableSize + 1) ];
                    table = new Int32[tableSize,5];
                    GenerateObjects();
                    break;
                case GameDifficulty.Hard:
                    tableSize = smallTableSize;
                    walls = new Coord[(tableSize + 1) ];
                    table = new Int32[tableSize, 5];
                    GenerateObjects();
                    break;
            }
        }
        private bool IsEmpty(Int32 x, Int32 y)
        {
            return table[x, y] == 0;
        }
        private void GenerateObjects()
        {

            for (Int32 i = 0; i < tableSize; i++)
                for (Int32 j = 0; j < 5; j++)
                    table[i, j] = 0;
            //játékos elhelyezése
            player = new Creatures();
            player.cord.x = ((tableSize+1)/2) - 1;
            player.cord.y = 2;
            table[player.cord.x, player.cord.y] = 1;
            wasOpened = 0;
            
            //falak elhelyezése
            for (Int32 i = 0; i < (tableSize + 1) / 2; i++)
            {
                walls[i].x = i * 2 ;
                walls[i].y = 0;
                walls[i].status = false;
                walls[i].opt = 0;
                walls[i+((tableSize+1)/2)].x = i * 2;
                walls[i + ((tableSize + 1) / 2)].y = 4;
                walls[i + ((tableSize + 1) / 2)].status = false;
                walls[i + ((tableSize + 1) / 2)].opt = 0;
            }
            Timer.Start();

            if (GeneratedTable != null)
                GeneratedTable(this, new EventArgs());
        }
        #endregion
        #region Move

        private void checkedThreeSecOpened()
        {
            for (Int32 i = 0; i < (tableSize + 1) / 2 ; i++)
            {
                walls[i].x = i * 2 ;
                walls[i].y = 0;
                if (walls[i].status) walls[i].opt++;
                if (walls[i + ((tableSize + 1) / 2)].status) walls[i + ((tableSize + 1) / 2)].opt++;
                if(walls[i + ((tableSize + 1) / 2)].opt == 3){
                    walls[i + ((tableSize + 1) / 2)].status = false;
                    walls[i + ((tableSize + 1) / 2)].opt = 0;
                }
            }
        }


        private bool IsOk(Creatures guard)
        {
            switch (guard.actualWay)
            {
                case Way.Down:
                    if (guard.cord.y + 1 == 5)
                    {
                        guard.actualWay = Way.Down;
                        return false;
                    }
                    break;
                case Way.Right:
                    if (guard.cord.x + 1 == tableSize)
                    {
                        guard.actualWay = Way.Right;
                        return false;
                    }
                    break;
                case Way.Up:
                    if (guard.cord.y == 0)
                    {
                        guard.actualWay = Way.Up;
                        return false;
                    }
                    break;
                case Way.Left:
                    if (guard.cord.x == 0)
                    {
                        guard.actualWay = Way.Left;
                        return false;
                    }
                    break;
            }
            return true;
        }
        private void MovePlayer(Way way)
        {
            if (!Timer.IsEnabled) return;
            player.actualWay = way;
            if (IsOk(player))
            {
                switch (player.actualWay)
                {
                    case Way.Right:
                        player.cord.x++;
                        break;
                    case Way.Left:
                        player.cord.x--;
                        break;
                    case Way.Up:
                        player.cord.y--;
                        break;
                    case Way.Down:
                        player.cord.y++;
                        break;
                }

            }
            PlayerMoved();
            for (Int32 i = 0; i < (tableSize + 1); i++)
                if (walls[i].x == player.cord.x && walls[i].y == player.cord.y && walls[i].status == true)
                { walls[i].status = false; wasOpened++; }
        }
      

        #endregion

        private void OnTimedEvent(object source, EventArgs e)
        {
            gameTime++;
            
            checkedThreeSecOpened();
            if (starttimer % 2 == 0)
            {
                Random rnd = new Random();
                int month = rnd.Next(1, 13);
                for (Int32 i = 0; i < rnd.Next(1, (tableSize-1)/2);i++)
                {
                    Int32 x= rnd.Next(0, (tableSize+1)-1);
                    walls[x].status=true;
                }
            }
            PlayerMoved();
        }
        private void PlayerMoved()
        {

            if (PlayerMovedEvent != null)
                PlayerMovedEvent(this, new EventArgs());
        }
        

        public async Task LoadGame(String path)
        {
            Timer.Stop();
            if (_dataAccess == null)
                throw new InvalidOperationException("No data access is provided.");

            Tuple<Int32, Model.Creatures, Model.Creatures[], Model.Coord, Model.Coord[]> Loaded_data = await _dataAccess.LoadAsync(path);
            tableSize = Loaded_data.Item1;
            if (tableSize == 13) NewGame(0);
            if (tableSize == 11) NewGame(1);
            if (tableSize == 9) NewGame(2);
            player = Loaded_data.Item2;
            guards = Loaded_data.Item3;
            exit = Loaded_data.Item4;
            walls = Loaded_data.Item5;
            for (Int32 i = 0; i < tableSize / 3; i++)
            {
                table[walls[i].x, walls[i].y] = 2;
            }
            if (GeneratedTable != null)
                GeneratedTable(this, new EventArgs());
            Timer.Start();
        }
        public bool isWall(Int32 x, Int32 y) { return table[x, y] == 2; }
        public void setTimerOff() { Timer.Stop(); }
        public void setTimerOn() { Timer.Start(); }
        /// <summary>
        /// Játék mentése.
        /// </summary>
        /// <param name="path">Elérési útvonal.</param>
        public async Task SaveGame(String path)
        {
            Timer.Stop();
            if (_dataAccess == null)
                throw new InvalidOperationException("No data access is provided.");

            await _dataAccess.SaveAsync(path, Tuple.Create(tableSize, player, guards, exit, walls));
            Timer.Start();
        }
    }
}
