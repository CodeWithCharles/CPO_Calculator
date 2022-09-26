using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CPO_Calculator
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static IDictionary<char, char> operand = new Dictionary<char, char>();
        private static List<char> scientificOperand = new List<char>();
        Regex sqrtRegex = new Regex(@"(√)[\w.]+");

        public MainWindow()
        {
            InitializeComponent();
            operand['÷'] = '/';
            operand['×'] = '*';
            operand['-'] = '-';
            operand['+'] = '+';
            scientificOperand.Add('√');

        }

        private async void BtnResult_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt = new DataTable();
            string baseExpr = TbUserInput.Text;
            string expr = TbUserInput.Text;
            foreach(KeyValuePair<char, char> ops in operand)
            {
                expr = expr.Replace(ops.Key, ops.Value);
            }

            try
            {
                MatchCollection matchSqrt = sqrtRegex.Matches(expr);

                foreach (Match match in matchSqrt)
                {
                    string trimedString = match.Value.Trim('√');
                    string sqrtResult = Math.Sqrt(Double.Parse(trimedString)).ToString();
                    int index = expr.IndexOf(match.Value);
                    if(index != 0 && !operand.Values.Contains(expr[index-1])) { sqrtResult = "*" + sqrtResult;  }
                    expr = expr.Replace(match.Value, sqrtResult.ToString());
                }

                Double result = Double.Parse(dt.Compute(expr, null).ToString());
                if (result == Double.PositiveInfinity) throw new DivideByZeroException();

                string resultOutput = await Task.Run(() => $"{baseExpr} = {result.ToString()}");
                TblResult.Text = resultOutput;
                TbUserInput.Text = result.ToString();
            } catch(Exception error)
            {
                MessageBox.Show(error.Message, "Erreur lors du calcul de l'expression", MessageBoxButton.OK, MessageBoxImage.Error);
                BtnDel_Click(null, null);
            }
        }

        private void BtnReturn_Click(object sender, RoutedEventArgs e)
        {

            if(TbUserInput.Text.Length >= 1)
            {
                Button button = (Button)sender;
                TbUserInput.Text = TbUserInput.Text.Remove(TbUserInput.Text.Length - 1);
            }
        }

        private void BtnDel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TbUserInput.Text = "";
            TblResult.Text = "";
        }

        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            TbUserInput.Text = "";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string toAdd = button.Content.ToString();
            TbUserInput.Text += toAdd;
        }
    }
}
