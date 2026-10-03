using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KlayFix.Application.DTOs;
using KlayFix.Application.Interfaces;
using KlayFix.Desktop.Services;

namespace KlayFix.Desktop.ViewModels;

public partial class SalesViewModel : BaseViewModel
{
    private readonly ISaleService _saleService;
    private readonly IProductService _productService;

    public event Action? FocusScannerRequested;

    public SalesViewModel(ISaleService saleService, IProductService productService)
    {
        _saleService = saleService;
        _productService = productService;
        _ = LoadDataAsync();
        _ = LoadProductsAsync();
    }

    [ObservableProperty]
    private ObservableCollection<ProductDto> _products = [];

    [ObservableProperty]
    private ObservableCollection<SaleDto> _sales = [];

    [ObservableProperty]
    private ObservableCollection<CartItemViewModel> _cartItems = [];

    [ObservableProperty]
    private string _productSearchQuery = string.Empty;

    [ObservableProperty]
    private string _scannerInput = string.Empty;

    [ObservableProperty]
    private decimal _discount;

    [ObservableProperty]
    private string _paymentMethod = "Cash";

    public bool IsCashPayment
    {
        get => PaymentMethod == "Cash";
        set { if (value) PaymentMethod = "Cash"; OnPropertyChanged(); }
    }

    public bool IsCardPayment
    {
        get => PaymentMethod == "Card";
        set { if (value) PaymentMethod = "Card"; OnPropertyChanged(); }
    }

    partial void OnPaymentMethodChanged(string value)
    {
        OnPropertyChanged(nameof(IsCashPayment));
        OnPropertyChanged(nameof(IsCardPayment));
    }

    [ObservableProperty]
    private string _scannerStatus = string.Empty;

    [ObservableProperty]
    private bool _showInvoicePanel;

    [ObservableProperty]
    private SaleDto? _lastSale;

    public decimal Subtotal => CartItems.Sum(i => i.Total);
    public decimal DiscountAmount => Discount;
    public decimal Total => Subtotal - DiscountAmount;
    public int CartItemCount => CartItems.Sum(i => i.Quantity);

    partial void OnProductSearchQueryChanged(string value) => _ = SearchProductsAsync(value);

    partial void OnDiscountChanged(decimal value) => OnPropertyChanged(nameof(Total));

    partial void OnScannerInputChanged(string value) => ScannerStatus = string.Empty;

    [RelayCommand]
    private void ClearSearch()
    {
        ProductSearchQuery = string.Empty;
    }

    [RelayCommand]
    private async Task SearchByBarcode()
    {
        var barcode = ScannerInput?.Trim();
        if (string.IsNullOrWhiteSpace(barcode))
        {
            FocusScannerRequested?.Invoke();
            return;
        }

        try
        {
            ScannerStatus = "Searching...";
            var results = await _productService.SearchAsync(barcode);
            var product = results.FirstOrDefault(p =>
                p.IMEI != null && p.IMEI.Equals(barcode, StringComparison.OrdinalIgnoreCase))
                ?? results.FirstOrDefault(p =>
                    p.Name.Equals(barcode, StringComparison.OrdinalIgnoreCase));

            if (product != null && product.Quantity > 0)
            {
                AddToCart(product);
                ScannerStatus = $"Added: {product.Name}";
                ScannerInput = string.Empty;
                NotifySuccess($"{product.Name} {LanguageService.Instance["Notify_Scanned"]}");
            }
            else if (product != null && product.Quantity <= 0)
            {
                ScannerStatus = "Out of stock";
                NotifyWarning($"{product.Name} {LanguageService.Instance["Notify_OutOfStock"]}");
            }
            else
            {
                ScannerStatus = "Not found";
                NotifyError(LanguageService.Instance["Notify_ProductNotFound"]);
            }
        }
        catch (Exception ex)
        {
            ScannerStatus = $"Error: {ex.Message}";
            NotifyError($"{LanguageService.Instance["Error_ScanFailed"]} {ex.Message}");
        }
        finally
        {
            FocusScannerRequested?.Invoke();
        }
    }

    private async Task SearchProductsAsync(string query)
    {
        try
        {
            List<ProductDto> results;
            if (string.IsNullOrWhiteSpace(query))
                results = await _productService.GetAllAsync();
            else
                results = await _productService.SearchAsync(query);

            Products = new ObservableCollection<ProductDto>(results.Where(p => p.Quantity > 0));
        }
        catch (Exception ex)
        {
            NotifyError($"Search failed: {ex.Message}");
        }
    }

    private async Task LoadProductsAsync()
    {
        try
        {
            var products = await _productService.GetAllAsync();
            Products = new ObservableCollection<ProductDto>(products.Where(p => p.Quantity > 0));
        }
        catch (Exception ex)
        {
            NotifyError($"Load products failed: {ex.Message}");
        }
    }

    private async Task LoadDataAsync()
    {
        try
        {
            var sales = await _saleService.GetAllAsync();
            Sales = new ObservableCollection<SaleDto>(sales);
        }
        catch (Exception ex)
        {
            NotifyError($"Load sales failed: {ex.Message}");
        }
    }

    [RelayCommand]
    private void AddToCart(ProductDto product)
    {
        if (product is null) return;

        var existing = CartItems.FirstOrDefault(c => c.ProductId == product.Id);
        if (existing is not null)
        {
            if (existing.Quantity < product.Quantity)
                existing.Quantity++;
        }
        else
        {
            CartItems.Add(new CartItemViewModel
            {
                ProductId = product.Id,
                ProductName = product.Name,
                ProductBrand = product.Brand,
                UnitPrice = product.SellingPrice,
                PurchasePrice = product.PurchasePrice,
                Quantity = 1,
                MaxQuantity = product.Quantity
            });
        }

        RefreshCart();
    }

    [RelayCommand]
    private void RemoveFromCart(CartItemViewModel item)
    {
        if (item is null) return;
        CartItems.Remove(item);
        RefreshCart();
    }

    [RelayCommand]
    private async Task ClearCart()
    {
        if (CartItems.Count == 0) return;

        var ls = LanguageService.Instance;
        var confirmed = await ConfirmAsync(ls["Confirm_ClearCart_Title"], ls["Confirm_ClearCart_Message"]);
        if (!confirmed) return;

        ResetCart();
    }

    private void ResetCart()
    {
        CartItems.Clear();
        Discount = 0;
        RefreshCart();
        NotifyInfo(LanguageService.Instance["Notify_CartCleared"]);
    }

    [RelayCommand]
    private void IncreaseQuantity(CartItemViewModel item)
    {
        if (item is null) return;
        if (item.Quantity < item.MaxQuantity)
        {
            item.Quantity++;
            RefreshCart();
        }
    }

    [RelayCommand]
    private void DecreaseQuantity(CartItemViewModel item)
    {
        if (item is null) return;
        if (item.Quantity > 1)
        {
            item.Quantity--;
            RefreshCart();
        }
        else
        {
            CartItems.Remove(item);
            RefreshCart();
        }
    }

    private void RefreshCart()
    {
        OnPropertyChanged(nameof(Subtotal));
        OnPropertyChanged(nameof(Total));
        OnPropertyChanged(nameof(CartItemCount));
    }

    [RelayCommand]
    private async Task CompleteSale()
    {
        if (CartItems.Count == 0)
        {
            NotifyWarning(LanguageService.Instance["Error_EmptyCart"]);
            return;
        }

        await RunAsync(async () =>
        {
            var dto = new CreateSaleDto
            {
                Discount = Discount,
                PaymentMethod = PaymentMethod,
                Items = CartItems.Select(c => new CreateSaleItemDto
                {
                    ProductId = c.ProductId, ProductName = c.ProductName, ProductBrand = c.ProductBrand,
                    Quantity = c.Quantity, UnitPrice = c.UnitPrice, PurchasePrice = c.PurchasePrice
                }).ToList()
            };

            LastSale = await _saleService.CreateAsync(dto);
            ResetCart();
            ShowInvoicePanel = true;
            await Task.WhenAll(LoadDataAsync(), LoadProductsAsync());
            NotifySuccess($"{LanguageService.Instance["Notify_SaleCompleted"]} {LastSale.InvoiceNumber}");
        }, LanguageService.Instance["Error_SaleFailed"]);
    }

    [RelayCommand]
    private void CloseInvoice()
    {
        ShowInvoicePanel = false;
        LastSale = null;
    }

    [RelayCommand]
    private void PrintReceipt()
    {
        if (LastSale is null) return;

        try
        {
            var dialog = new System.Windows.Controls.PrintDialog();
            if (dialog.ShowDialog() != true) return;

            var doc = new System.Windows.Documents.FlowDocument
            {
                PageWidth = 280, PageHeight = 1200,
                FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                FontSize = 11, TextAlignment = System.Windows.TextAlignment.Center
            };

            var receipt = new System.Windows.Documents.Section();

            receipt.Blocks.Add(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run(LanguageService.Instance["Receipt_ShopName"]))
            { FontSize = 16, FontWeight = System.Windows.FontWeights.Bold, TextAlignment = System.Windows.TextAlignment.Center });

            var config = App.Configuration;
            var shopAddr = config["ReceiptSettings:Address"] ?? "";
            var shopPhone = config["ReceiptSettings:Phone"] ?? "";

            if (!string.IsNullOrEmpty(shopAddr))
                receipt.Blocks.Add(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run(shopAddr))
                { FontSize = 10, TextAlignment = System.Windows.TextAlignment.Center, Foreground = System.Windows.Media.Brushes.Gray });

            if (!string.IsNullOrEmpty(shopPhone))
                receipt.Blocks.Add(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run($"Tel: {shopPhone}"))
                { FontSize = 10, TextAlignment = System.Windows.TextAlignment.Center, Foreground = System.Windows.Media.Brushes.Gray });

            receipt.Blocks.Add(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run(new string('\u2500', 40)))
            { FontSize = 8, Margin = new System.Windows.Thickness(0, 4, 0, 4) });

            receipt.Blocks.Add(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run(LastSale.InvoiceNumber))
            { FontSize = 12, FontWeight = System.Windows.FontWeights.Bold, TextAlignment = System.Windows.TextAlignment.Center });

            receipt.Blocks.Add(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run($"Date: {LastSale.SaleDate:MMM dd, yyyy  hh:mm tt}"))
            { FontSize = 10, TextAlignment = System.Windows.TextAlignment.Left });

            receipt.Blocks.Add(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run(new string('\u2500', 40)))
            { FontSize = 8, Margin = new System.Windows.Thickness(0, 4, 0, 4) });

            foreach (var item in LastSale.Items)
            {
                var p = new System.Windows.Documents.Paragraph
                {
                    TextAlignment = System.Windows.TextAlignment.Left,
                    Margin = new System.Windows.Thickness(0, 2, 0, 2)
                };
                p.Inlines.Add(new System.Windows.Documents.Run($"{item.ProductName} x{item.Quantity}"));
                p.Inlines.Add(new System.Windows.Documents.Run($"  \u20B1{item.TotalPrice:N2}")
                { FontWeight = System.Windows.FontWeights.Bold });
                receipt.Blocks.Add(p);
            }

            receipt.Blocks.Add(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run(new string('\u2500', 40)))
            { FontSize = 8, Margin = new System.Windows.Thickness(0, 4, 0, 4) });

            var sub = new System.Windows.Documents.Paragraph { TextAlignment = System.Windows.TextAlignment.Left };
            sub.Inlines.Add(new System.Windows.Documents.Run("Subtotal:"));
            sub.Inlines.Add(new System.Windows.Documents.Run($"  \u20B1{LastSale.TotalAmount:N2}")
            { FontWeight = System.Windows.FontWeights.Bold });
            receipt.Blocks.Add(sub);

            if (LastSale.Discount > 0)
            {
                var d = new System.Windows.Documents.Paragraph { TextAlignment = System.Windows.TextAlignment.Left };
                d.Inlines.Add(new System.Windows.Documents.Run("Discount:"));
                d.Inlines.Add(new System.Windows.Documents.Run($"  -\u20B1{LastSale.Discount:N2}")
                { Foreground = System.Windows.Media.Brushes.Red });
                receipt.Blocks.Add(d);
            }

            var total = new System.Windows.Documents.Paragraph
            { TextAlignment = System.Windows.TextAlignment.Left, FontSize = 14, FontWeight = System.Windows.FontWeights.Bold };
            total.Inlines.Add(new System.Windows.Documents.Run("TOTAL:"));
            total.Inlines.Add(new System.Windows.Documents.Run($"  \u20B1{LastSale.NetAmount:N2}"));
            receipt.Blocks.Add(total);

            receipt.Blocks.Add(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run($"Payment: {LastSale.PaymentMethod}"))
            { TextAlignment = System.Windows.TextAlignment.Left, FontSize = 10 });

            receipt.Blocks.Add(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run(new string('\u2500', 40)))
            { FontSize = 8, Margin = new System.Windows.Thickness(0, 4, 0, 4) });

            var footer = App.Configuration["ReceiptSettings:Footer"] ?? "Thank you for your business!";
            receipt.Blocks.Add(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run(footer))
            { FontSize = 11, FontWeight = System.Windows.FontWeights.Bold, TextAlignment = System.Windows.TextAlignment.Center });

            doc.Blocks.Add(receipt);
            var paginator = ((System.Windows.Documents.IDocumentPaginatorSource)doc).DocumentPaginator;
            dialog.PrintDocument(paginator, $"Receipt - {LastSale.InvoiceNumber}");
        }
        catch (Exception ex)
        {
            NotifyError($"{LanguageService.Instance["Error_PrintFailed"]} {ex.Message}");
        }
    }
}

public partial class CartItemViewModel : ObservableObject
{
    [ObservableProperty]
    private int _productId;

    [ObservableProperty]
    private string _productName = string.Empty;

    [ObservableProperty]
    private string _productBrand = string.Empty;

    [ObservableProperty]
    private decimal _unitPrice;

    [ObservableProperty]
    private decimal _purchasePrice;

    [ObservableProperty]
    private int _quantity;

    [ObservableProperty]
    private int _maxQuantity;

    public decimal Total => UnitPrice * Quantity;
    public decimal Profit => (UnitPrice - PurchasePrice) * Quantity;

    partial void OnQuantityChanged(int value)
    {
        OnPropertyChanged(nameof(Total));
        OnPropertyChanged(nameof(Profit));
    }
}
