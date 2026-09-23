using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Windows.Input;

namespace Crystal.Avalonia
{
    /// <summary>
    /// One event → command mapping used by <see cref="EventToCommand"/>.
    /// </summary>
    public class EventBinding : AvaloniaObject
    {
        /// <summary>Routed event short name (e.g. <c>Click</c>) or CLR <see cref="EventHandler"/> name.</summary>
        public static readonly StyledProperty<string?> EventNameProperty =
            AvaloniaProperty.Register<EventBinding, string?>(nameof(EventName));

        /// <summary>Command to execute.</summary>
        public static readonly StyledProperty<ICommand?> CommandProperty =
            AvaloniaProperty.Register<EventBinding, ICommand?>(nameof(Command));

        /// <summary>Parameter when <see cref="PassEventArgs"/> is false.</summary>
        public static readonly StyledProperty<object?> CommandParameterProperty =
            AvaloniaProperty.Register<EventBinding, object?>(nameof(CommandParameter));

        /// <summary>When true, pass event args to the command.</summary>
        public static readonly StyledProperty<bool> PassEventArgsProperty =
            AvaloniaProperty.Register<EventBinding, bool>(nameof(PassEventArgs));

        /// <summary>Gets or sets <see cref="EventNameProperty"/>.</summary>
        public string? EventName
        {
            get => GetValue(EventNameProperty);
            set => SetValue(EventNameProperty, value);
        }

        /// <summary>Gets or sets <see cref="CommandProperty"/>.</summary>
        public ICommand? Command
        {
            get => GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        /// <summary>Gets or sets <see cref="CommandParameterProperty"/>.</summary>
        public object? CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        /// <summary>Gets or sets <see cref="PassEventArgsProperty"/>.</summary>
        public bool PassEventArgs
        {
            get => GetValue(PassEventArgsProperty);
            set => SetValue(PassEventArgsProperty, value);
        }
    }

    /// <summary>
    /// Collection of <see cref="EventBinding"/> for <see cref="EventToCommand.BindingsProperty"/>.
    /// </summary>
    public class EventBindingCollection : AvaloniaList<EventBinding>
    {
    }

    /// <summary>
    /// Lightweight attached properties that bridge View events to ViewModel <see cref="ICommand"/>s.
    /// Mapped into the default Avalonia XML namespace via <c>XmlnsDefinition</c> — no xmlns prefix required.
    /// </summary>
    /// <remarks>
    /// Use <see cref="BindingsProperty"/> for multiple events, or the shorthand
    /// <see cref="EventNameProperty"/> / <see cref="CommandProperty"/> for a single event.
    /// Prefer Avalonia routed events (e.g. <c>Click</c> → <c>ClickEvent</c>).
    /// </remarks>
    /// <example>
    /// <code language="xml">
    /// &lt;!-- Single (compiled binding OK on the control) --&gt;
    /// &lt;Button EventToCommand.EventName="Click" EventToCommand.Command="{Binding SaveCommand}" /&gt;
    ///
    /// &lt;!-- Multiple --&gt;
    /// &lt;Border&gt;
    ///   &lt;EventToCommand.Bindings&gt;
    ///     &lt;EventBinding EventName="PointerPressed" Command="{Binding PressCommand}" PassEventArgs="True"/&gt;
    ///     &lt;EventBinding EventName="DoubleTapped" Command="{Binding OpenCommand}"/&gt;
    ///   &lt;/EventToCommand.Bindings&gt;
    /// &lt;/Border&gt;
    /// </code>
    /// </example>
    public static class EventToCommand
    {
        /// <summary>Multiple event → command mappings.</summary>
        public static readonly AttachedProperty<EventBindingCollection?> BindingsProperty =
            AvaloniaProperty.RegisterAttached<Interactive, EventBindingCollection?>("Bindings", typeof(EventToCommand));

        /// <summary>Shorthand: single event name when not using <see cref="BindingsProperty"/>.</summary>
        public static readonly AttachedProperty<string?> EventNameProperty =
            AvaloniaProperty.RegisterAttached<Interactive, string?>("EventName", typeof(EventToCommand));

        /// <summary>Shorthand: command for <see cref="EventNameProperty"/>.</summary>
        public static readonly AttachedProperty<ICommand?> CommandProperty =
            AvaloniaProperty.RegisterAttached<Interactive, ICommand?>("Command", typeof(EventToCommand));

        /// <summary>Shorthand: command parameter for <see cref="EventNameProperty"/>.</summary>
        public static readonly AttachedProperty<object?> CommandParameterProperty =
            AvaloniaProperty.RegisterAttached<Interactive, object?>("CommandParameter", typeof(EventToCommand));

        /// <summary>Shorthand: pass event args for <see cref="EventNameProperty"/>.</summary>
        public static readonly AttachedProperty<bool> PassEventArgsProperty =
            AvaloniaProperty.RegisterAttached<Interactive, bool>("PassEventArgs", typeof(EventToCommand));

        private static readonly AttachedProperty<HostState?> StateProperty =
            AvaloniaProperty.RegisterAttached<Interactive, HostState?>("State", typeof(EventToCommand));

        static EventToCommand()
        {
            BindingsProperty.Changed.AddClassHandler<Interactive, EventBindingCollection?>((s, e) =>
                OnBindingsCollectionChanged(s, e.OldValue.GetValueOrDefault(), e.NewValue.GetValueOrDefault()));
            EventNameProperty.Changed.AddClassHandler<Interactive, string?>((s, _) => Rebuild(s));
            CommandProperty.Changed.AddClassHandler<Interactive, ICommand?>((s, _) => Rebuild(s));
            CommandParameterProperty.Changed.AddClassHandler<Interactive, object?>((s, _) => Rebuild(s));
            PassEventArgsProperty.Changed.AddClassHandler<Interactive, bool>((s, _) => Rebuild(s));
        }

        /// <summary>
        /// Gets <see cref="BindingsProperty"/>, creating an empty collection if needed
        /// so XAML property-element children can be added without a null reference.
        /// </summary>
        public static EventBindingCollection GetBindings(Interactive element)
        {
            var collection = element.GetValue(BindingsProperty);
            if (collection == null)
            {
                collection = new EventBindingCollection();
                element.SetValue(BindingsProperty, collection);
            }

            return collection;
        }

        /// <summary>Sets <see cref="BindingsProperty"/>.</summary>
        public static void SetBindings(Interactive element, EventBindingCollection? value) =>
            element.SetValue(BindingsProperty, value);

        /// <summary>Gets <see cref="EventNameProperty"/>.</summary>
        public static string? GetEventName(Interactive element) => element.GetValue(EventNameProperty);

        /// <summary>Sets <see cref="EventNameProperty"/>.</summary>
        public static void SetEventName(Interactive element, string? value) => element.SetValue(EventNameProperty, value);

        /// <summary>Gets <see cref="CommandProperty"/>.</summary>
        public static ICommand? GetCommand(Interactive element) => element.GetValue(CommandProperty);

        /// <summary>Sets <see cref="CommandProperty"/>.</summary>
        public static void SetCommand(Interactive element, ICommand? value) => element.SetValue(CommandProperty, value);

        /// <summary>Gets <see cref="CommandParameterProperty"/>.</summary>
        public static object? GetCommandParameter(Interactive element) => element.GetValue(CommandParameterProperty);

        /// <summary>Sets <see cref="CommandParameterProperty"/>.</summary>
        public static void SetCommandParameter(Interactive element, object? value) => element.SetValue(CommandParameterProperty, value);

        /// <summary>Gets <see cref="PassEventArgsProperty"/>.</summary>
        public static bool GetPassEventArgs(Interactive element) => element.GetValue(PassEventArgsProperty);

        /// <summary>Sets <see cref="PassEventArgsProperty"/>.</summary>
        public static void SetPassEventArgs(Interactive element, bool value) => element.SetValue(PassEventArgsProperty, value);

        private static void OnBindingsCollectionChanged(
            Interactive host,
            EventBindingCollection? oldCollection,
            EventBindingCollection? newCollection)
        {
            if (oldCollection != null)
                oldCollection.CollectionChanged -= OnCollectionChanged;

            if (newCollection != null)
                newCollection.CollectionChanged += OnCollectionChanged;

            Rebuild(host);

            void OnCollectionChanged(object? _, NotifyCollectionChangedEventArgs __) => Rebuild(host);
        }

        private static void Rebuild(Interactive host)
        {
            if (Design.IsDesignMode)
                return;

            var state = host.GetValue(StateProperty);
            state?.Dispose();
            state = new HostState(host);
            host.SetValue(StateProperty, state);
            state.Attach();
        }

        [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Event names resolve Avalonia public *Event fields or public EventHandler events.")]
        [UnconditionalSuppressMessage("Trimming", "IL2072", Justification = "Control types expose public static RoutedEvent fields retained by Avalonia.")]
        [UnconditionalSuppressMessage("Trimming", "IL2075", Justification = "GetType/GetEvent used only for public instance EventHandler events.")]
        [UnconditionalSuppressMessage("Trimming", "IL2070", Justification = "Walks public static *Event fields on Avalonia control types.")]
        private static IDisposable? Subscribe(
            Interactive host,
            string eventName,
            Func<object?, (ICommand? Command, object? Parameter)> resolve)
        {
            var routedEvent = FindRoutedEvent(host.GetType(), eventName);
            if (routedEvent != null)
            {
                EventHandler<RoutedEventArgs> handler = (_, e) => Invoke(resolve(e));
                host.AddHandler(routedEvent, handler);
                return new ActionDisposable(() => host.RemoveHandler(routedEvent, handler));
            }

            var eventInfo = host.GetType().GetEvent(eventName, BindingFlags.Instance | BindingFlags.Public);
            if (eventInfo?.EventHandlerType != typeof(EventHandler))
                return null;

            EventHandler clrHandler = (_, e) => Invoke(resolve(e));
            eventInfo.AddEventHandler(host, clrHandler);
            return new ActionDisposable(() => eventInfo.RemoveEventHandler(host, clrHandler));
        }

        private static void Invoke((ICommand? Command, object? Parameter) target)
        {
            if (target.Command?.CanExecute(target.Parameter) == true)
                target.Command.Execute(target.Parameter);
        }

        [UnconditionalSuppressMessage("Trimming", "IL2070", Justification = "Walks public static *Event fields on Avalonia control types.")]
        [UnconditionalSuppressMessage("Trimming", "IL2075", Justification = "BaseType walk for RoutedEvent fields; Avalonia keeps public API.")]
        private static RoutedEvent? FindRoutedEvent(Type type, string eventName)
        {
            var fieldName = eventName.EndsWith("Event", StringComparison.Ordinal) ? eventName : eventName + "Event";

            for (var t = type; t != null && t != typeof(object); t = t.BaseType)
            {
                var field = t.GetField(fieldName, BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
                if (field?.GetValue(null) is RoutedEvent routedEvent)
                    return routedEvent;
            }

            return null;
        }

        private sealed class HostState : IDisposable
        {
            private readonly Interactive _host;
            private readonly List<IDisposable> _subscriptions = new();
            private readonly List<EventBinding> _trackedBindings = new();
            private bool _disposed;

            public HostState(Interactive host) => _host = host;

            public void Attach()
            {
                var bindings = _host.GetValue(BindingsProperty);
                if (bindings != null)
                {
                    foreach (var binding in bindings)
                        AttachBinding(binding);
                }

                var shorthandEvent = GetEventName(_host);
                if (!string.IsNullOrWhiteSpace(shorthandEvent))
                {
                    var sub = Subscribe(_host, shorthandEvent, eventArgs =>
                    {
                        var pass = GetPassEventArgs(_host);
                        return (GetCommand(_host), pass ? eventArgs : GetCommandParameter(_host));
                    });
                    if (sub != null)
                        _subscriptions.Add(sub);
                }
            }

            private void AttachBinding(EventBinding binding)
            {
                _trackedBindings.Add(binding);
                binding.PropertyChanged += OnBindingPropertyChanged;

                var eventName = binding.EventName;
                if (string.IsNullOrWhiteSpace(eventName))
                    return;

                var captured = binding;
                var sub = Subscribe(_host, eventName, eventArgs =>
                {
                    var pass = captured.PassEventArgs;
                    return (captured.Command, pass ? eventArgs : captured.CommandParameter);
                });
                if (sub != null)
                    _subscriptions.Add(sub);
            }

            private void OnBindingPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
            {
                if (e.Property == EventBinding.EventNameProperty)
                    Rebuild(_host);
            }

            public void Dispose()
            {
                if (_disposed)
                    return;
                _disposed = true;

                foreach (var binding in _trackedBindings)
                    binding.PropertyChanged -= OnBindingPropertyChanged;
                _trackedBindings.Clear();

                foreach (var sub in _subscriptions)
                    sub.Dispose();
                _subscriptions.Clear();
            }
        }

        private sealed class ActionDisposable : IDisposable
        {
            private Action? _dispose;

            public ActionDisposable(Action dispose) => _dispose = dispose;

            public void Dispose()
            {
                _dispose?.Invoke();
                _dispose = null;
            }
        }
    }
}
