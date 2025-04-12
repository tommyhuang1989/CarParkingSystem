
using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Rendering.Composition;
using SukiUI.Dialogs;
using SukiUI.Helpers;

namespace CarParkingSystem.Controls
{
    public class DialogHost : TemplatedControl
    {
        public static readonly StyledProperty<ISukiDialogManager> ManagerProperty = AvaloniaProperty.Register<DialogHost, ISukiDialogManager>(nameof(Manager));

        public ISukiDialogManager Manager
        {
            get => GetValue(ManagerProperty);
            set => SetValue(ManagerProperty, value);
        }
        
        public static readonly StyledProperty<object?> DialogProperty = AvaloniaProperty.Register<DialogHost, object?>(nameof(Dialog));

        internal object? Dialog
        {
            get => GetValue(DialogProperty);
            set => SetValue(DialogProperty, value);
        }

        public static readonly StyledProperty<bool> IsDialogOpenProperty = AvaloniaProperty.Register<DialogHost, bool>(nameof(IsDialogOpen));

        //internal
        public bool IsDialogOpen
        {
            get => GetValue(IsDialogOpenProperty);
            set => SetValue(IsDialogOpenProperty, value);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            if (e.NameScope.Find<Border>("PART_DialogBackground") is { } dialogBackground)
            {
                dialogBackground.PointerPressed += (_, _) => BackgroundRequestClose();
                dialogBackground.Loaded += (_, _) =>
                {
                    var v = ElementComposition.GetElementVisual(dialogBackground);
                    CompositionAnimationHelper.MakeOpacityAnimated(v, 400);
                }; 
            }
        }

        private void BackgroundRequestClose()
        {
            if (Dialog is not ISukiDialog { CanDismissWithBackgroundClick: true } sukiDialog) return;
            if (!sukiDialog.CanDismissWithBackgroundClick) return;
            Manager.TryDismissDialog(sukiDialog);
        }

        private static void OnManagerPropertyChanged(AvaloniaObject sender,
            AvaloniaPropertyChangedEventArgs propChanged)
        {
            if (sender is not DialogHost host)
                throw new NullReferenceException("Dependency object is not of valid type " + nameof(DialogHost));
            if (propChanged.OldValue is ISukiDialogManager oldManager)
                host.DetachManagerEvents(oldManager);
            if (propChanged.NewValue is ISukiDialogManager newManager)
                host.AttachManagerEvents(newManager);
        }

        private void AttachManagerEvents(ISukiDialogManager newManager)
        {
            newManager.OnDialogShown += ManagerOnDialogShown;
            newManager.OnDialogDismissed += ManagerOnDialogDismissed;
        }
        
        private void DetachManagerEvents(ISukiDialogManager oldManager)
        {
            oldManager.OnDialogShown -= ManagerOnDialogShown;
            oldManager.OnDialogDismissed -= ManagerOnDialogDismissed;
        }

        private void ManagerOnDialogShown(object sender, SukiDialogManagerEventArgs args)
        {
            Dialog = args.Dialog;
            IsDialogOpen = true;
        }
        
        private void ManagerOnDialogDismissed(object sender, SukiDialogManagerEventArgs args)
        {
            IsDialogOpen = false;
            Task.Delay(500).ContinueWith(_ =>
            {
                if (Dialog != args.Dialog) return;
                Dialog = null;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        static DialogHost()
        {
            ManagerProperty.Changed.Subscribe(
                new Avalonia.Reactive.AnonymousObserver<AvaloniaPropertyChangedEventArgs<ISukiDialogManager>>(x =>
                    OnManagerPropertyChanged(x.Sender, x)));
        }
    }
}