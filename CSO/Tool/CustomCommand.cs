using System.Windows.Input;

namespace CSO.Tool
{
    public static class CustomCommand
    {
        public static readonly RoutedUICommand Save = new RoutedUICommand
        (
            "Save",
            "Save",
            typeof(CustomCommand),
            new InputGestureCollection()
            {
                 new KeyGesture(Key.S, ModifierKeys.Control)
            }
        );

    }
}
