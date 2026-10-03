using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KlayFix.Application.DTOs;
using KlayFix.Application.Interfaces;
using KlayFix.Desktop.Services;
using KlayFix.Domain.Enums;

namespace KlayFix.Desktop.ViewModels;

public partial class ProductsViewModel : BaseViewModel
{
    private readonly IProductService _productService;
    private readonly CancellationTokenSource _searchCts = new();

    public ProductsViewModel(IProductService productService)
    {
        _productService = productService;
        _ = RunAsync(LoadDataAsync, LanguageService.Instance["Error_LoadFailed"]);
    }

    [ObservableProperty]
    private ObservableCollection<ProductDto> _products = [];

    [ObservableProperty]
    private string _searchQuery = string.Empty;

    [ObservableProperty]
    private bool _isModalOpen;

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private string _modalTitle = "Add Product";

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _brand = string.Empty;

    [ObservableProperty]
    private ProductCategory _category;

    [ObservableProperty]
    private string? _imei;

    [ObservableProperty]
    private decimal _purchasePrice;

    [ObservableProperty]
    private decimal _sellingPrice;

    [ObservableProperty]
    private int _quantity;

    [ObservableProperty]
    private int _editingId;

    public Array Categories => Enum.GetValues(typeof(ProductCategory));

    partial void OnSearchQueryChanged(string value) => _ = SearchDebouncedAsync(value);

    private async Task SearchDebouncedAsync(string query)
    {
        _searchCts.Cancel();
        await Task.Delay(300);
        var cts = new CancellationTokenSource();
        try
        {
            List<ProductDto> results;
            if (string.IsNullOrWhiteSpace(query))
                results = await _productService.GetAllAsync();
            else
                results = await _productService.SearchAsync(query);

            Products = new ObservableCollection<ProductDto>(results);
        }
        catch (Exception ex)
        {
            NotifyError($"{LanguageService.Instance["Error_SearchFailed"]} {ex.Message}");
        }
    }

    private async Task LoadDataAsync()
    {
        if (!RequireAdmin()) return;

        var products = await _productService.GetAllAsync();
        Products = new ObservableCollection<ProductDto>(products);
    }

    [RelayCommand]
    private void ClearSearch() => SearchQuery = string.Empty;

    [RelayCommand]
    private void OpenAddModal()
    {
        IsEditing = false;
        ModalTitle = LanguageService.Instance["Products_ModalAddTitle"];
        Name = string.Empty;
        Brand = string.Empty;
        Category = ProductCategory.Phone;
        Imei = null;
        PurchasePrice = 0;
        SellingPrice = 0;
        Quantity = 1;
        ErrorMessage = string.Empty;
        IsModalOpen = true;
    }

    [RelayCommand]
    private void OpenEditModal(ProductDto product)
    {
        if (product is null) return;
        IsEditing = true;
        ModalTitle = LanguageService.Instance["Products_ModalEditTitle"];
        EditingId = product.Id;
        Name = product.Name;
        Brand = product.Brand;
        Category = Enum.TryParse<ProductCategory>(product.Category, out var cat) ? cat : ProductCategory.Other;
        Imei = product.IMEI;
        PurchasePrice = product.PurchasePrice;
        SellingPrice = product.SellingPrice;
        Quantity = product.Quantity;
        ErrorMessage = string.Empty;
        IsModalOpen = true;
    }

    [RelayCommand]
    private void CloseModal() => IsModalOpen = false;

    [RelayCommand]
    private async Task SaveProduct()
    {
        if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Brand))
        {
            ErrorMessage = LanguageService.Instance["Error_NameBrandRequired"];
            return;
        }

        await RunAsync(async () =>
        {
            if (IsEditing)
            {
                await _productService.UpdateAsync(new UpdateProductDto
                {
                    Id = EditingId, Name = Name, Brand = Brand, Category = Category,
                    IMEI = Imei, PurchasePrice = PurchasePrice, SellingPrice = SellingPrice, Quantity = Quantity
                });
                NotifySuccess(LanguageService.Instance["Notify_ProductUpdated"]);
            }
            else
            {
                await _productService.CreateAsync(new CreateProductDto
                {
                    Name = Name, Brand = Brand, Category = Category,
                    IMEI = Imei, PurchasePrice = PurchasePrice, SellingPrice = SellingPrice, Quantity = Quantity
                });
                NotifySuccess(LanguageService.Instance["Notify_ProductAdded"]);
            }

            IsModalOpen = false;
            await LoadDataAsync();
        }, LanguageService.Instance["Error_SaveFailed"]);
    }

    [RelayCommand]
    private async Task DeleteProduct(int id)
    {
        var ls = LanguageService.Instance;
        var confirmed = await ConfirmAsync(ls["Confirm_DeleteProduct_Title"], ls["Confirm_DeleteProduct_Message"]);
        if (!confirmed) return;

        await RunAsync(async () =>
        {
            await _productService.DeleteAsync(id);
            await LoadDataAsync();
            NotifySuccess(LanguageService.Instance["Notify_ProductDeleted"]);
        }, LanguageService.Instance["Error_DeleteFailed"]);
    }
}
