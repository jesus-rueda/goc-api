using System.Collections.Generic;
using System.Linq;

namespace Goc.Api.Controllers
{

    public static class ResultCollectionExtensions
    {
        public static ResultCollection<T> ToResultCollection<T>(this IEnumerable<T> items)
        {
            return new ResultCollection<T>(items);
        }
    }


    public class ResultCollection<T>
    {
        public int Total { get;  }

        public T[] Items { get; }

        public ResultCollection(IEnumerable<T> items)
        {
            this.Items = items.ToArray();
            this.Total = this.Items.Length;
        }        
    }
}