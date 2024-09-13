using FlyFramework.Application.TestService.Dtos;
using FlyFramework.Common.Extentions.DynamicWebAPI;
using FlyFramework.Common.Repositories;
using FlyFramework.Common.Uow;
using FlyFramework.Core.TestService;
using FlyFramework.Core.TestService.Domain;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlyFramework.Application.TestService
{
    /// <summary>
    /// 测试注释
    /// </summary>
    //[Authorize]
    public class BookAppService : IApplicationService
    {
        private readonly IBookManager _bookManager;
        private readonly ICategoryManager _categoryManager;
        private readonly IRepository<Book, string> _bookRepository;
        private readonly IUnitOfWork _unitOfWork;

        public BookAppService(IBookManager bookManager, ICategoryManager categoryManager, IRepository<Book, string> bookRepository, IUnitOfWork unitOfWork)
        {
            _bookManager = bookManager;
            _bookRepository = bookRepository;
            _categoryManager = categoryManager;
            _unitOfWork = unitOfWork;
        }


        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task Add(BookAddOrUpdateInput input)
        {
            Book book = new Book
            {
                Id = Guid.NewGuid().ToString("N"),
                ISBN = input.ISBN,
                Title = input.Title
            };
            using (_unitOfWork.BeginTransactionAsync())
            {
                var category = new Category
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Code = "123",
                    Name = "测试分类"
                };
                await _categoryManager.Create(category);
                await _unitOfWork.CommitAsync();
            }

            await _bookManager.Create(book);
            //await _bookRepository.InsertAsync(book);
        }

        public string Gethelle()
        {
            return "helle";
        }
        ////分页查询（使用Include加载导航属性)
        //public async Task<IPagedList<BookOutput>> GetPageList(BookPageRequestInput input)
        //{
        //    //创建查询对象-MultipleResultQuery表示多结果集查询
        //    var query = _bookRepository.MultipleResultQuery<BookOutput>()
        //                      .Page(input.PageIndex, input.PageSize) //分页
        //                      .AndFilter(b => string.IsNullOrEmpty(input.Title) || b.Title.StartsWith(input.Title)) //筛选条件
        //                      .Include(q => q.Include(x => x.Category))  //级联加载
        //                      .OrderByDescending("Title").ThenBy("ISBN") //排序
        //                      .Select(b => new BookOutput                //投影
        //                      {
        //                          CategoryId = b.CategoryId,
        //                          CategoryCode = b.Category.Code,
        //                          CategoryName = b.Category.Name,
        //                          ISBN = b.ISBN,
        //                          Title = b.Title,
        //                          Id = b.Id
        //                      })
        //    as IMultipleResultQuery<Book, BookOutput>; //转换类型

        //    //执行查询
        //    var result = (await _bookRepository.SearchAsync(query))
        //                         .ToPagedList(query.Paging.PageIndex,
        //                                    query.Paging.PageSize,
        //                                    query.Paging.TotalCount);

        //    return (result);
        //}

    }
}
