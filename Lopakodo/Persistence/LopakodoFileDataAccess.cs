using System;
using System.IO;
using Lopakodo.Model;
using System.Threading.Tasks;

namespace Lopakodo.Persistence
{  /// <summary>
   /// Amőba fájlkezelő típusa.
   /// </summary>
    public class LopakodoFileDataAccess : ILopakodoDataAccess
    {
        /// <summary>
        /// Fájl betöltése.
        /// </summary>
        /// <param name="path">Elérési útvonal.</param>
        /// <returns>A fájlból beolvasott játékadatok.</returns>
        public async Task<Tuple<Int32, Model.Creatures, Model.Creatures[], Model.Coord, Model.Coord[]>> LoadAsync(String path)
        {
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    String line = await reader.ReadLineAsync();
                    String[] numbers = line.Split(' ');
                    Int32 tableSize = Int32.Parse(numbers[0]);
                    Creatures player = new Creatures();
                    player.cord.x = Int32.Parse(numbers[1]);
                    player.cord.y = Int32.Parse(numbers[2]);
                    Coord exit = new Coord();
                    exit.x = Int32.Parse(numbers[3]);
                    exit.y = Int32.Parse(numbers[4]);
                    line = await reader.ReadLineAsync();
                    numbers = line.Split(' ');
                    Creatures[] guards = new Creatures[3];
                    for (Int32 i = 0; i < 3; i++)
                    {
                        guards[i] = new Creatures();
                        guards[i].cord.x = Int32.Parse(numbers[i * 2]);
                        guards[i].cord.y = Int32.Parse(numbers[i * 2 + 1]);
                    }
                    line = await reader.ReadLineAsync();
                    numbers = line.Split(' ');
                    Coord[] walls = new Coord[tableSize / 3];
                    for (Int32 i = 0; i < tableSize / 3; i++)
                    {
                        walls[i] = new Coord();
                        walls[i].x = Int32.Parse(numbers[i * 2]);
                        walls[i].y = Int32.Parse(numbers[i * 2 + 1]);
                    }

                    Tuple<Int32, Model.Creatures, Model.Creatures[], Model.Coord, Model.Coord[]> ToRet = Tuple.Create(tableSize, player, guards, exit, walls);
                    return ToRet;
                }
            }
            catch
            {
                throw new LopakodoDataException();
            }
        }

        /// <summary>
        /// Fájl mentése.
        /// </summary>
        /// <param name="path">Elérési útvonal.</param>
        /// <param name="OutPut">A fájlba kiírandó játékadatok.</param>
        public async Task SaveAsync(String path, Tuple<Int32, Model.Creatures, Model.Creatures[], Model.Coord, Model.Coord[]> OutPut)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path)) // fájl megnyitása
                {
                    //A mentés fájl első sorába rakjuk a pályamérete, a két játékos idejét valamint az éppen soron lévő játékost
                    writer.Write(OutPut.Item1); // kiírjuk a méreteket
                    string cucc = " " + OutPut.Item2.cord.x + " " + OutPut.Item2.cord.y + " " + OutPut.Item4.x + " " + OutPut.Item4.y;
                    await writer.WriteLineAsync(cucc);
                    await writer.WriteLineAsync(OutPut.Item3[0].cord.x + " " + OutPut.Item3[0].cord.y + " " + OutPut.Item3[1].cord.x + " " + OutPut.Item3[1].cord.y + " " + OutPut.Item3[2].cord.x + " " + OutPut.Item3[2].cord.y);
                    String ToWri = "";
                    for (Int32 i = 0; i < OutPut.Item1 / 3; i++)
                    {
                        ToWri += OutPut.Item5[i].x + " " + OutPut.Item5[i].y + " ";
                    }
                    await writer.WriteLineAsync(ToWri);
                }
            }
            catch
            {
                throw new LopakodoDataException();
            }
        }
    }
}
