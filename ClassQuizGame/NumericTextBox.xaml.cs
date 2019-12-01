using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace ClassQuizGame
{
    /// <summary>
    /// Interaction logic for NumericTextBox.xaml
    /// </summary>
    public partial class NumericTextBox : TextBox
    {

        private static readonly Regex decimalRegex = new Regex("^[^0-9.-]+$");
        private static readonly Regex integerRegex = new Regex("^[^0-9-]+$");
        private static Regex regex;

        public NumericTextBox()
        {
            InitializeComponent();

            regex = decimalRegex;

            PreviewTextInput += NumericTextBox_PreviewTextInput;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
            base.OnPreviewKeyDown(e);
        }


        private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            return !regex.IsMatch(text);
        }

        public void setEnableDecimal(bool enableDecimal)
        {
            if (enableDecimal)
            {
                regex = decimalRegex;
            }
            else
            {
                regex = integerRegex;
            }
        }
    }
}
