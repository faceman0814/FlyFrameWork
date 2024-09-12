using FlyFramework.Application.TestService.Dtos;
using FlyFramework.Common.Extentions.DynamicWebAPI;
using FlyFramework.Common.Repositories;
using FlyFramework.Core.TestService;
using FlyFramework.Core.TestService.Domain;

namespace FlyFramework.Application.TestService
{
    /// <summary>
    /// 测试注释
    /// </summary>
    public class BookService : IApplicationService
    {
        private readonly IBookManager _bookManager;
        private readonly IRepository<Book, string> _bookRepository;


        public BookService(IDbContextProvider serviceProvider, IBookManager bookManager, IRepository<Book, string> bookRepository)
        {
            _bookManager = bookManager;
            _bookRepository = new Repository<Book, string>(serviceProvider);
        }

        ////泛型仓储
        //private readonly IRepository<Category> _categoryRepository;
        ////工作单元
        //private readonly IUnitOfWork _unitOfWork;

        //public BookService(IRepository<Book> bookRepository, IRepository<Category> categoryRepository, IUnitOfWork unitOfWork)
        //{
        //    _bookRepository = bookRepository;
        //    _categoryRepository = categoryRepository;
        //    _unitOfWork = unitOfWork;
        //}

        //POST api/Books
        public async Task Add(BookAddOrUpdateInput input)
        {
            Book book = new Book
            {
                Id = Guid.NewGuid().ToString("N"),
                CategoryId = input.CategoryId,
                ISBN = input.ISBN,
                Title = input.Title
            };
            Category category = new Category
            {
                Id = input.CategoryId,
                Name = "默认分类",
                Code = "DEFAULT"
            };
            await _bookManager.Create(book);
            //await _categoryRepository.AddAsync(category);
            //await _unitOfWork.SaveChangesAsync();
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
