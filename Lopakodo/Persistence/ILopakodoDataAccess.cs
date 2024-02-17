using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lopakodo.Persistence
{
    public interface ILopakodoDataAccess
    {
        /// <summary>
        /// Fájl betöltése.
        /// </summary>
        /// <param name="path">Elérési útvonal.</param>
        /// <returns>A fájlból beolvasott játékadatok.</returns>
        Task<Tuple<Int32, Model.Creatures, Model.Creatures[], Model.Coord, Model.Coord[]>> LoadAsync(string path);

        /// <summary>
        /// Fájl mentése.
        /// </summary>
        /// <param name="path">Elérési útvonal.</param>
        /// <param name="input">A fájlba kiírandó játékadatok.</param>
        Task SaveAsync(String path, Tuple<Int32, Model.Creatures, Model.Creatures[], Model.Coord, Model.Coord[]> input);
    }
}
