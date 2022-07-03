using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Divanru
{
    /// <summary>
    /// Элемент коллекции для списка, ссылка на сайт и название. Может быть и мебелью, и категорией.
    /// </summary>
    public class ListElement
    {
        /// <summary>
        /// Ссылка на сайт
        /// </summary>
        public string Link { get; set; }
        /// <summary>
        /// Название.
        /// </summary>
        public string Title { get; set; }

    }
}
