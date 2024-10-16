using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Dtos
{
    public interface IShouldNormalize
    {
        /// <summary>
        /// 这种方法称为最后执行方法（如果存在验证后）。
        /// </summary>
        void Normalize();
    }
    /// <summary>
    /// 定义限制结果数量的接口
    /// </summary>
    public interface ILimitedResultRequest
    {
        //
        // 摘要:
        //     Max expected result count.
        int MaxResultCount { get; set; }
    }

    /// <summary>
    /// 定义限制结果数量的接口
    /// </summary>
    public interface IPagedResultRequest : ILimitedResultRequest
    {
        //
        // 摘要:
        //     Skip count (beginning of the page).
        int SkipCount { get; set; }
    }

    /// <summary>
    /// 定义排序的接口
    /// </summary>
    public interface ISortedResultRequest
    {
        /// <summary>
        /// 排序
        /// </summary>
        string Sorting { get; set; }
    }
    public class UniversalPagedSortedAndFilteredInputDtoL : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        /// <summary>
        /// 正常化排序使用
        /// </summary>
        public virtual void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "CreationTime DESC";
            }
        }
    }
    /// <summary>
    /// 支持分页、排序和过滤的Dto
    /// </summary>
    public class PagedSortedAndFilteredInputDto : PagedAndSortedInputDto
    {
        public virtual string FilterText { get; set; }
    }

    /// <summary>
    /// 支持分页和排序的Dto
    /// </summary>
    public class PagedAndSortedInputDto : PagedInputDto, ISortedResultRequest
    {
        //
        // 摘要:
        //     排序
        public virtual string Sorting { get; set; }

        public PagedAndSortedInputDto()
        {
            MaxResultCount = 10;
        }
    }

    /// <summary>
    /// 支持分页的Dto
    /// </summary>
    public class PagedInputDto : IPagedResultRequest, ILimitedResultRequest
    {
        //
        // 摘要:
        //     最大的返回条数
        [Range(1, 1000)]
        public virtual int MaxResultCount { get; set; }

        //
        // 摘要:
        //     跳过的数据量
        [Range(0, int.MaxValue)]
        public virtual int SkipCount { get; set; }

        public PagedInputDto()
        {
            MaxResultCount = 10;
        }
    }
}
