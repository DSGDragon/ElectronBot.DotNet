﻿using Contracts.Services;
using Controls;
using ElectronBot.Braincase.Activation;
using ElectronBot.Braincase.ClockViews;
using ElectronBot.Braincase.Contracts.Services;
using ElectronBot.Braincase.Helpers;
using ElectronBot.Braincase.Models;
using ElectronBot.Braincase.Notifications;
using ElectronBot.Braincase.Picker;
using ElectronBot.Braincase.Services;
using ElectronBot.Braincase.ViewModels;
using ElectronBot.Braincase.Views;
using ElectronBot.DotNet;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Graphics.Canvas;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Services;
using Verdure.ElectronBot.Core.Contracts.Services;
using Verdure.ElectronBot.Core.Services;
using Verdure.ElectronBot.GrpcService;
using Views;
using Windows.ApplicationModel.Background;
using Windows.Media.Playback;
using Windows.UI.Popups;
using Controls.CompactOverlay;
using HelixToolkit.SharpDX.Core;
using ViewModels;

namespace ElectronBot.Braincase;

// To learn more about WinUI 3, see https://docs.microsoft.com/windows/apps/winui/winui3/.
public partial class App : Application
{
    // The .NET Generic Host provides dependency injection, configuration, logging, and other services.
    // https://docs.microsoft.com/dotnet/core/extensions/generic-host
    // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
    // https://docs.microsoft.com/dotnet/core/extensions/configuration
    // https://docs.microsoft.com/dotnet/core/extensions/logging
    public IHost Host
    {
        get;
    }

    public static Frame RootFrame
    {
        get; set;
    }

    public static T GetService<T>()
        where T : class
    {
        if ((App.Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }

    public static WindowEx MainWindow { get; } = new MainWindow();

    public static UIElement? AppTitlebar
    {
        get; set;
    }

    public App()
    {
        InitializeComponent();

        Host = Microsoft.Extensions.Hosting.Host.
        CreateDefaultBuilder().
        UseContentRoot(AppContext.BaseDirectory).
        ConfigureServices((context, services) =>
        {
            var canvasDevice = CanvasDevice.GetSharedDevice();

            services.AddSingleton(canvasDevice);
            // Default Activation Handler
            services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

            // Other Activation Handlers
            services.AddTransient<IActivationHandler, AppNotificationActivationHandler>();

            services.AddTransient<IActivationHandler, StartupTaskActivationHandler>();

            services.AddHttpClient();
            // Services
            services.AddSingleton<IAppNotificationService, AppNotificationService>();
            services.AddSingleton<ILocalSettingsService, LocalSettingsService>();
            services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
            services.AddTransient<INavigationViewService, NavigationViewService>();
            services.AddSingleton<ISpeechAndTTSService, SpeechAndTTSService>();
            services.AddSingleton<IActivationService, ActivationService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();

            services.AddSingleton<IdentityService>();
            services.AddSingleton<IMicrosoftGraphService, MicrosoftGraphService>();

            services.AddSingleton<UserDataService>();

            // Core Services
            services.AddSingleton<IFileService, FileService>();

            services.AddTransient<IEmojisFileService, EmojisFileService>();

            services.AddTransient<IEmojiseShopService, EmojiseShopService>();

            services.AddTransient<IElectronLowLevel, ElectronLowLevel>();

            services.AddTransient<MediaPlayer>();

            services.AddTransient<ObjectPicker<WriteableBitmap>>();

            services.AddSingleton<ObjectPickerService>();

            services.AddSingleton<ClockDiagnosticService>();

            // Views and ViewModels
            services.AddTransient<CameraEmojisViewModel>();
            services.AddTransient<CameraEmojisPage>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<SettingsPage>();
            services.AddTransient<TodoViewModel>();
            services.AddTransient<TodoPage>();
            services.AddTransient<BlankViewModel>();
            services.AddTransient<BlankPage>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<MainPage>();
            services.AddTransient<ShellPage>();
            services.AddTransient<ShellViewModel>();
            services.AddTransient<EmojisEditPage>();
            services.AddTransient<EmojisEditViewModel>();
            services.AddTransient<AddEmojisDialogViewModel>();
            services.AddTransient<UploadEmojisDialogViewModel>();
            services.AddTransient<UploadEmojisPage>();
            services.AddTransient<MarketplacePage>();
            services.AddTransient<MarketplaceViewModel>();

            services.AddTransient<GestureClassificationPage>();
            services.AddTransient<GestureClassificationViewModel>();

            services.AddTransient<GestureInteractionPage>();
            services.AddTransient<GestureInteractionViewModel>();

            services.AddTransient<PoseRecognitionPage>();
            services.AddTransient<PoseRecognitionViewModel>();

            services.AddTransient<MoviePage>();
            services.AddTransient<MovieViewModel>();

            services.AddTransient<RandomContentPage>();
            services.AddTransient<RandomContentViewModel>();

            services.AddTransient<EmojisInfoDialogViewModel>();

            services.AddTransient<GamepadViewModel>();
            services.AddTransient<GamepadPage>();

            services.AddTransient<LongShadow>();

            services.AddTransient<HiddenTextView>();
            services.AddSingleton<ClockViewModel>();

            services.AddSingleton<ComboxDataService>();

            services.AddTransient<DispatcherTimer>();

            services.AddTransient<ImageCropperPickerViewModel>();

            services.AddTransient<ImageCropperPage>();

            services.AddTransient<TodoCompactOverlayViewModel>();
            services.AddTransient<ModelLoadCompactOverlayViewModel>();
            services.AddTransient<ModelLoadCompactOverlayPage>();
            services.AddTransient<IEffectsManager, DefaultEffectsManager>();


            services.AddSingleton<IClockViewProviderFactory, ClockViewProviderFactory>();

            services.AddTransient<IClockViewProvider, DefaultClockViewProvider>();

            services.AddTransient<IClockViewProvider, CustomClockViewProvider>();

            services.AddTransient<IClockViewProvider, LongShadowClockViewProvider>();

            services.AddTransient<IClockViewProvider, GooeyFooterClockViewProvider>();

            services.AddTransient<IClockViewProvider, GradientsWithBlendClockViewProvider>();

            services.AddSingleton<IActionExpressionProvider, DefaultActionExpressionProvider>();

            services.AddSingleton<IActionExpressionProviderFactory, ActionExpressionProviderFactory>();

            services.AddSingleton<EmoticonActionFrameService>();

            services.AddSingleton<GestureClassificationService>();

            services.AddSingleton<PoseRecognitionService>();


            services.AddTransient<IChatbotClient, ChatGPTChatbotClient>();

            services.AddTransient<IChatbotClient, TuringChatbotClient>();

            services.AddTransient<IChatbotClientFactory, ChatbotClientFactory>();


            services.AddGrpcClient<ElectronBotActionGrpc.ElectronBotActionGrpcClient>(o =>
            {
                o.Address = new Uri("http://192.168.3.239:5241");
                //o.Address = new Uri("http://localhost:5241");
            });

            services.AddSingleton<Services.EbotGrpcService.EbGrpcService>();
            // Configuration
            services.Configure<LocalSettingsOptions>(context.Configuration.GetSection(nameof(LocalSettingsOptions)));
        }).
        Build();

        App.GetService<IAppNotificationService>().Initialize();

        UnhandledException += App_UnhandledException;
    }

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {

        e.Handled = true;

        ElectronBotHelper.Instance.InvokeClockCanvasStop();

        var service = App.GetService<EmoticonActionFrameService>();

        service.ClearQueue();

        Thread.Sleep(1000);

        ElectronBotHelper.Instance?.ElectronBot?.Disconnect();
        // TODO: Log and handle exceptions as appropriate.
        // https://docs.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.application.unhandledexception.
    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        MainWindow.AppWindow.Closing += AppWindow_Closing;
        //App.GetService<IAppNotificationService>().Show(string.Format("AppNotificationSamplePayload".GetLocalized(), AppContext.BaseDirectory));

        await App.GetService<IActivationService>().ActivateAsync(args);
    }

    private async void AppWindow_Closing(AppWindow sender, AppWindowClosingEventArgs args)
    {
        args.Cancel = true;

        var theme = App.GetService<IThemeSelectorService>();

        var dialog = new ContentDialog
        {
            // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
            XamlRoot = MainWindow.Content.XamlRoot,
            Title = "ExitBtn_Title".GetLocalized(),
            PrimaryButtonText = "ExitBtn_PrimaryButtonText".GetLocalized(),
            CloseButtonText = "ExitBtn_CloseButtonText".GetLocalized(),
            SecondaryButtonText = "ExitBtn_SecondaryButtonText".GetLocalized(),
            DefaultButton = ContentDialogButton.Primary,
            RequestedTheme = theme.Theme,
            Content = new AppQuitPage()
        };

        var result = await dialog.ShowAsync();


        if (result == ContentDialogResult.Primary)
        {
            try
            {
                ElectronBotHelper.Instance.InvokeClockCanvasStop();

                var service = App.GetService<EmoticonActionFrameService>();

                service.ClearQueue();

                Thread.Sleep(1000);

                IntelligenceService.Current.CleanUp();

                await CameraService.Current.CleanupMediaCaptureAsync();

                await CameraFrameService.Current.CleanupMediaCaptureAsync();

                ElectronBotHelper.Instance?.ElectronBot?.Disconnect();
            }
            catch (Exception)
            {

            }

            MainWindow.Close();
        }
        else if(result == ContentDialogResult.Secondary)
        {
            MainWindow.Hide();
        }
    }
}
