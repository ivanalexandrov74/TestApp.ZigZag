using Microsoft.AspNetCore.Components;
using ZigZag.Test.Dto;

namespace ZigZag.Test.Pages;

public partial class MainPage
{
    private bool _isFirstPage = true;

    private bool _isLastPage = true;

    private readonly List<string> _prevPageCursors = new();

    private string _selectedCategory = string.Empty;

    private List<string> _categories = new();

    private ExternalApisResponseDto _externalApisData = new();

    protected override async Task OnInitializedAsync()
    {
        await RequestCategories();

        await base.OnInitializedAsync();
    }

    private async Task RequestCategories()
    {
        await CallWebApiAsync<CategoriesResponseDto>("api/Categories", HttpMethodEnum.Get, null, OnCategoriesReceived);

        if (_categories.Count > 0)
        {
            _prevPageCursors.Clear();

            _selectedCategory = _categories[0];

            var requestData = new ExternalApisRequestDto
            {
                category = _selectedCategory,
            };

            await RequestCategoryApis(requestData);
        }
    }

    private void OnCategoriesReceived(CategoriesResponseDto responseData)
    {
        _categories = responseData.categories;
    }

    private async Task OnCategoryChanged(ChangeEventArgs e)
    {
        _prevPageCursors.Clear();

        _selectedCategory = $"{e.Value}";

        var requestData = new ExternalApisRequestDto
        {
            category = _selectedCategory,
        };

        await RequestCategoryApis(requestData);


    }

    private async Task RequestCategoryApis(ExternalApisRequestDto requestData)
    {
        _isFirstPage = true;

        _isLastPage = true;

        _externalApisData = new();

        await CallWebApiAsync<ExternalApisResponseDto>("api/ExternalApis", HttpMethodEnum.Get, requestData, OnCategoryApisReceived);
    }

    private void OnCategoryApisReceived(ExternalApisResponseDto responseData)
    {
        _isFirstPage = !responseData.pageInfo.hasPreviousPage;

        _isLastPage = !responseData.pageInfo.hasNextPage;

        _externalApisData = responseData;
    }

    private async Task OnPrevPage_Click()
    {
        _prevPageCursors.RemoveAt(_prevPageCursors.Count - 1);

        var prevPageCursor = _prevPageCursors.Count > 0 ? _prevPageCursors[_prevPageCursors.Count-1] : string.Empty;

        var requestData = new ExternalApisRequestDto
        {
            after = prevPageCursor,
            category = _selectedCategory
        };

        await RequestCategoryApis(requestData);
    }

    private async Task OnNextPage_Click()
    {
        _prevPageCursors.Add(_externalApisData.pageInfo.endCursor);

        var requestData = new ExternalApisRequestDto
        {
            after = _externalApisData.pageInfo.endCursor,
            category = _selectedCategory
        };

        await RequestCategoryApis(requestData);


    }
}
