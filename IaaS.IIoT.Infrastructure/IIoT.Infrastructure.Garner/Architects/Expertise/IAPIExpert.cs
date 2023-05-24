namespace Infrastructure.Garner.Architects.Expertise;
public interface IAPIExpert
{
    readonly record struct Mark
    {
        public const int Found = 1;
        public const int MaxData = 10;
        public const int MaxSize = 500;
        public required object PreviousPage { get; init; }
        public required object NextPage { get; init; }
        public required object FirstPage { get; init; }
        public required object LastPage { get; init; }
    }
    abstract class Satchel
    {
        int Page = Mark.MaxData;
        public int PageSize
        {
            get => Page;
            set => Page = value > Mark.MaxSize ? Mark.MaxSize : value;
        }
        public int PageNumber { get; init; } = Mark.Found;
        public string? Search { get; init; }
    }
    sealed class Pages<T> : List<T>
    {
        public Pages(in IEnumerable<T> sources, in int currentPage, in int pageSize)
        {
            PageSize = pageSize;
            TotalCount = sources.Count();
            var totalPage = (TotalCount / PageSize) + (TotalCount % PageSize == default ? default : Mark.Found);
            var page = totalPage > Mark.MaxSize ? Mark.MaxSize : totalPage;
            {
                CurrentPage = currentPage;
                TotalPage = page == default ? Mark.Found : page;
                AddRange(sources.Skip((CurrentPage - Mark.Found) * PageSize).Take(PageSize));
            }
        }
        public int PageSize { get; private set; }
        public int TotalPage { get; private set; }
        public int TotalCount { get; private set; }
        public int CurrentPage { get; private set; }
    }
}