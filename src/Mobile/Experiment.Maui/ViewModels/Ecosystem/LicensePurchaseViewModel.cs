using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

using System.Windows.Input;

using Microsoft.Maui.Controls;
using Newtonsoft.Json;

using Experiment.Core;
using Experiment.Core.Base;
using Experiment.Data.Metadata;
using Experiment.Data.Models;

using V=Experiment.Maui.Views.Ecosystem;

using Experiment.Maui.Models;
using Experiment.Maui.Services;
using Experiment.Maui.Data;

namespace Experiment.Maui.ViewModels.Ecosystem{
    /// <summary>
    /// License purchase view model, which uses LicenceDetailsPage, 
    /// </summary>
    public class LicensePurchaseViewModel : ViewModelBase
    {
        #region Const
        const string TYPE_NAME = nameof(LicensePurchaseViewModel);
        static readonly int[] DEFAULT_LICENSE_TERMS = new int[] { 1, 3, 6, 12, 24, 48 };
        const decimal DISCOUNT_PER_SINGLE_TOKEN = 2;

        #endregion

        #region Attributes
        readonly ApiServices _ApiServices = new ApiServices();
        VisualLicenseProduct _SelectedProduct;

        ObservableCollection<VisualLicenseTermProduct> _AvailableLicenseTerms =
            new ObservableCollection<VisualLicenseTermProduct>();
        VisualLicenseTermProduct _SelectedLicenseTerm;

        int _UsedTokens = 0;

        bool _ReloadUser = true;
        bool _Purchased = false;
        VisualUser _CurrentUser;
        int _TokensOwned = 0;

        ObservableCollection<PaymentMethod> _PaymentMethods;
        PaymentMethod _SelectedPaymentMethod;

        #endregion

        #region Properties
        public VisualLicenseProduct SelectedProduct
        {
            get => _SelectedProduct;
            set
            {
                var changed = !Equals(_SelectedProduct, value);
                SetProperty(ref _SelectedProduct, value);

                // Because selected license changed, probably changed as well its price needs to be re-calculated
                if (changed)
                {
                    if (_SelectedProduct == null)
                        return;

                    if (AvailableLicenseTerms.Count > 0)
                    {
                        // If already terms available, only re-calculating their prices
                        foreach (var term in AvailableLicenseTerms)
                        {
                            term.Price = term.Months * SelectedProduct.Price;
                        }
                    }
                    else
                    {
                        // As terms unavailable, creating new terms and calculating their prices
                        foreach (var term in DEFAULT_LICENSE_TERMS)
                        {
                            AvailableLicenseTerms.Add(new VisualLicenseTermProduct()
                            {
                                Name = string.Format("{0} {1}", term, term > 1 ? E.T("months") : E.T("month")),
                                Months = term,
                                Price = term * SelectedProduct.Price,
                            });
                        }
                    }
                }
            }
        }

        public ObservableCollection<VisualLicenseTermProduct> AvailableLicenseTerms
        {
            get => _AvailableLicenseTerms;
            set => SetProperty(ref _AvailableLicenseTerms, value);
        }

        /// <summary>
        /// Selected license term with its price
        /// </summary>
        public VisualLicenseTermProduct SelectedLicenseTerm
        {
            get => _SelectedLicenseTerm;
            set
            {
                var changed = !Equals(_SelectedLicenseTerm, value);
                SetProperty(ref _SelectedLicenseTerm, value);

                OnPropertyChanged(nameof(IsTermSelected));
                OnPropertyChanged(nameof(FullPrice));
            }
        }

        /// <summary>
        /// IsTermSelected at all
        /// </summary>
        public bool IsTermSelected { get => _SelectedLicenseTerm != null; }

        /// <summary>
        /// Full price without discount and anything
        /// </summary>
        public decimal FullPrice
        {
            get
            {
                decimal retVal = 0;

                if (IsTermSelected)
                {
                    retVal = _SelectedLicenseTerm.Price;
                }

                return retVal;
            }
        }

        /// <summary>
        /// Used tokens for discount
        /// </summary>
        public int UsedTokens
        {
            get => _UsedTokens;
            set
            {
                SetProperty(ref _UsedTokens, value);

                OnPropertyChanged(nameof(TokensLeft));
                OnPropertyChanged(nameof(DiscountPercent));
                OnPropertyChanged(nameof(Discount));
                OnPropertyChanged(nameof(FinalPrice));
            }
        }

        /// <summary>
        /// All tokens which possess user
        /// </summary>
        public int TokensOwned
        {
            get => _TokensOwned;
            set
            {
                SetProperty(ref _TokensOwned, value);

                OnPropertyChanged(nameof(TokensLeft));
                OnPropertyChanged(nameof(DiscountPercent));
                OnPropertyChanged(nameof(Discount));
                OnPropertyChanged(nameof(FinalPrice));

                // Confirming that user doesn't try to cheat us
                // If he used more tokens than he has
                if (UsedTokens > TokensOwned)
                {
                    // Setting max available tokens value for him
                    UsedTokens = TokensOwned;
                }
            }
        }

        public int TokensLeft { get => TokensOwned - UsedTokens; }
        public decimal DiscountPercent { get => DISCOUNT_PER_SINGLE_TOKEN * UsedTokens; }
        public decimal Discount
        {
            get
            {
                return FullPrice * (DiscountPercent / 100);
            }
        }
        public decimal FinalPrice
        {
            get
            {
                return FullPrice - Discount;
            }
        }

        public ObservableCollection<PaymentMethod> PaymentMethods
        {
            get => _PaymentMethods;
            set => SetProperty(ref _PaymentMethods, value);
        }

        public PaymentMethod SelectedPaymentMethod
        {
            get => _SelectedPaymentMethod;
            set
            {
                SetProperty(ref _SelectedPaymentMethod, value);

                OnPropertyChanged(nameof(IsPaymentMethodSelected));
            }
        }

        public bool IsPaymentMethodSelected { get => SelectedPaymentMethod != null; }

        /// <summary>
        /// Sets to true after if order was posted (purchase process was completed)
        /// </summary>
        public bool Posted { get => _Purchased; set => SetProperty(ref _Purchased, value); }

        public string LabelBack { get => E.T("back"); }
        public string LabelNext { get => E.T("next"); }
        public string LabelChoosePeriod { get => E.T("choosePeriod"); }
        public string LabelDiscountApply { get => E.T("discountApply"); }
        public string LabelProceed { get => E.T("proceed"); }
        public string LabelPurchase { get => E.T("purchase"); }
        public string LabelConfirmPurchase { get => E.T("confirmPurchase"); }
        public string LabelChoosePaymentMethod { get => E.T("choosePaymentMethod"); }
        public string LabelDiscount { get => E.T("discount"); }
        public string LabelDiscountDesc { get => E.T("discountDesc"); }
        public string LabelName { get => E.T("name"); }
        public string LabelDescription { get => E.T("description"); }
        public string LabelPrice { get => E.T("price"); }
        public string LabelFullPrice { get => E.T("fullPrice"); }
        public string LabelFinalPrice { get => E.T("finalPrice"); }
        public string LabelUseTokens { get => E.T("useTokens"); }
        public string LabelTokensYouOwn { get => E.T("tokensYouOwn"); }

        #endregion

        #region Ctor
        public LicensePurchaseViewModel()
        {
        }

        #endregion

        #region Events

        #endregion

        #region Helpers
        protected async Task BackPedal()
        {
            var vLoc = string.Format("{0}::{1}()", TYPE_NAME, nameof(BackPedal));
            await Application.Current.MainPage.Navigation.PopAsync();
        }


        #endregion

        #region Methods
        /// <summary>
        /// This method, rather than in other ViewModels case will be called from diffferent Pages/Views many times, during single processing.
        /// So it should suppose multiple calls and to be resilient to them fully.
        /// Many of things we need to initialize only once, not every call of this method.
        /// 
        /// But it as well utilized functionality to close different views after purchase was posted.
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public async Task LoadAsync(ContentPage sender)
        {
            try
            {
                var vLoc = string.Format("{0}::{1}() for {2}",
                    TYPE_NAME, nameof(LoadAsync), nameof(sender));
                Debug.WriteLine(vLoc);

                if (Posted)
                {
                    await BackPedal();
                }
                else
                {
                    IsBusy = true;

                    if (sender is V.LicensePurchaseDiscountPage)
                    {
                        // Each time this dialogue accesed, retrieving again users info and its tokens ballance
                        _CurrentUser = await Dictionaries.Instance.GetCurrentUser(true);
                        // Updating tokens ballance
                        TokensOwned = _CurrentUser.Tokens;
                    }

                    if (PaymentMethods == null)
                    {
                        PaymentMethods = new ObservableCollection<PaymentMethod>();
                        var pms = await _ApiServices.PaymentMethodsAsync();
                        foreach (var pm in pms)
                        {
                            PaymentMethods.Add(pm);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("{0}\r\n{1}", ex.Message, ex.StackTrace));
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion

        #region Commands
        public ICommand BackCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await BackPedal();
                });
            }
        }

        public ICommand ProceedToChooseTermCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await Application.Current.MainPage.Navigation.PushAsync(
                        new V.LicensePurchaseTermPage()
                        {
                            BindingContext = this,
                        });
                });
            }
        }

        public ICommand ProceedToApplyDiscountCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await Application.Current.MainPage.Navigation.PushAsync(
                        new V.LicensePurchaseDiscountPage()
                        {
                            BindingContext = this,
                        });
                });
            }
        }

        public ICommand DecreaseUsedInDiscountTokensCommand
        {
            get
            {
                return new Command(async () =>
                {
                    var vLoc = string.Format("{0}::{1}()",
                        TYPE_NAME, nameof(DecreaseUsedInDiscountTokensCommand));
                    Debug.WriteLine(vLoc);

                    if (UsedTokens > 0)
                        UsedTokens--;
                });
            }
        }

        public ICommand IncreaseUsedInDiscountTokensCommand
        {
            get
            {
                return new Command(async () =>
                {
                    // Once or first time reloading user's data from back-end
                    var vLoc = string.Format("{0}::{1}()",
                        TYPE_NAME, nameof(IncreaseUsedInDiscountTokensCommand));
                    Debug.WriteLine(vLoc);

                    if (_CurrentUser != null)
                    {
                        if (UsedTokens < _CurrentUser.Tokens)
                        {
                            var projectedDiscountPercent = DISCOUNT_PER_SINGLE_TOKEN * (UsedTokens + 1);
                            var projectedDiscount = FullPrice * (projectedDiscountPercent / 100);
                            if (projectedDiscount < FullPrice)
                            {
                                UsedTokens++;
                            }

                        }
                    }

                });
            }
        }

        public ICommand ProceedToConfirmPurchaseCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await Application.Current.MainPage.Navigation.PushAsync(
                        new V.LicensePurchaseConfirmPage()
                        {
                            BindingContext = this,
                        });
                });
            }
        }

        public ICommand ProceedToChoosePaymentMethodCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await Application.Current.MainPage.Navigation.PushAsync(
                        new V.LicensePurchaseChoosePaymentMethodPage()
                        {
                            BindingContext = this,
                        });
                });
            }
        }

        public ICommand PostOrderCommand
        {
            get
            {
                return new Command(async () =>
                {
                    var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(PostOrderCommand));

                    var order = new Order()
                    {
                        FullPrice = FullPrice,
                        Discount = Discount,
                        UsedTokens = UsedTokens,
                        FinalPrice = FinalPrice,
                        PaymentMethodId = SelectedPaymentMethod.Id,
                        PaymentMethod = SelectedPaymentMethod.Name,
                        OrderDetails = new List<OrderDetail>()
                        {
                            new OrderDetail() {
                                LicenseType = SelectedProduct.LicenseType,
                                NumMonths = SelectedLicenseTerm.Months,
                            }
                        },
                    };

                    var served = false;
                    var result = await _ApiServices.OrderPostAsync(order);
                    if (result.IsSuccessStatusCode)
                    {
                        var contentStr = await result.Content.ReadAsStringAsync();
                        var savedOrder = JsonConvert.DeserializeObject<Order>(contentStr);
                        if (savedOrder != null)
                        {
                            await _ApiServices.PayseraPostOrder(savedOrder);

                            // Might be this modal window freezing emulator as it starts browser window
                            //await Application.Current.MainPage.DisplayAlert(
                            //	E.T("orderSubmission"),
                            //	E.T("orderSubmissionDesc"),
                            //	E.T("ok"));

                            served = true;
                            Posted = true;
                            await BackPedal();
                        }
                    }

                    if (!served)
                    {
                        await Application.Current.MainPage.DisplayAlert(
                            E.T("orderSubmission"),
                            E.T("err-op"),
                            E.T("cancel"));
                    }
                });
            }
        }
        #endregion

    }
}

