using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Dtos
{
    public interface IHasTotalCount
    {
        //
        // 摘要:
        //     Total count of Items.
        int TotalCount { get; set; }
    }

    public interface IPagedResult<T> : IListResult<T>, IHasTotalCount
    {
    }

    public interface IListResult<T>
    {
        //
        // 摘要:
        //     List of items.
        IReadOnlyList<T> Items { get; set; }
    }

    [Serializable]
    public class ListResultDto<T> : IListResult<T>
    {
        private IReadOnlyList<T> _items;

        public IReadOnlyList<T> Items
        {
            get
            {
                return _items ?? (_items = new List<T>());
            }
            set
            {
                _items = value;
            }
        }

        public ListResultDto()
        {
        }

        public ListResultDto(IReadOnlyList<T> items)
        {
            Items = items;
        }
    }

    [Serializable]
    public class PagedResultDto<T> : ListResultDto<T>, IPagedResult<T>, IListResult<T>, IHasTotalCount
    {
        public int TotalCount { get; set; }

        public PagedResultDto()
        {
        }
        public PagedResultDto(int totalCount, IReadOnlyList<T> items)
        : base(items)
        {
            TotalCount = totalCount;
        }
    }
}
