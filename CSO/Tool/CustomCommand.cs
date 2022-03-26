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
        public static readonly RoutedUICommand Assign = new RoutedUICommand
        (
           "Assign",
           "Assign",
           typeof(CustomCommand),
           new InputGestureCollection()
           {
                 new KeyGesture(Key.A, ModifierKeys.Control)
           }
       );
    }
}
