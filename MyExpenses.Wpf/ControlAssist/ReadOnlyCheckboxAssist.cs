using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyExpenses.Wpf.ControlAssist
{
    public static class ReadOnlyCheckboxAssist
    {
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.RegisterAttached(
                "IsReadOnly",
                typeof(bool),
                typeof(ReadOnlyCheckboxAssist),
                new PropertyMetadata(false, OnIsReadOnlyChanged));

        public static bool GetIsReadOnly(DependencyObject obj) => (bool)obj.GetValue(IsReadOnlyProperty);
        public static void SetIsReadOnly(DependencyObject obj, bool value) => obj.SetValue(IsReadOnlyProperty, value);

        private static void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CheckBox checkBox)
            {
                checkBox.PreviewMouseLeftButtonDown -= BlockClickAndExecuteCommand;
                checkBox.PreviewKeyDown -= BlockKeyboard;

                if ((bool)e.NewValue)
                {
                    checkBox.PreviewMouseLeftButtonDown += BlockClickAndExecuteCommand;
                    checkBox.PreviewKeyDown += BlockKeyboard;
                }
            }
        }

        private static void BlockClickAndExecuteCommand(object sender, MouseButtonEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                // 1. On force le focus visuel
                checkBox.Focus();

                // 2. On exécute manuellement la commande du CheckBox si elle existe !
                if (checkBox.Command != null && checkBox.Command.CanExecute(checkBox.CommandParameter))
                {
                    checkBox.Command.Execute(checkBox.CommandParameter);
                }

                // 3. On bloque l'événement pour ÉVITER que le CheckBox ne change d'état (IsChecked reste intact)
                e.Handled = true;
            }
        }

        private static void BlockKeyboard(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && sender is CheckBox checkBox)
            {
                // Même logique pour le clavier (Barre d'espace)
                if (checkBox.Command != null && checkBox.Command.CanExecute(checkBox.CommandParameter))
                {
                    checkBox.Command.Execute(checkBox.CommandParameter);
                }
                e.Handled = true;
            }
        }
    }
}