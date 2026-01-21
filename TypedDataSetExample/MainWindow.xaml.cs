using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace TypedDataSetExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // create the adapter and datatable objects
        private NorthwindDataSetTableAdapters.ProductsTableAdapter adpProducts;
        private NorthwindDataSet.ProductsDataTable tblProducts;

        private NorthwindDataSetTableAdapters.CategoriesTableAdapter adpCategories;
        private NorthwindDataSet.CategoriesDataTable tblCategories;

        private NorthwindDataSetTableAdapters.ProductsWithInnerJoinsTableAdapter adpProductsWithInnerJoins;
        private NorthwindDataSet.ProductsWithInnerJoinsDataTable tblProductsWithInnerJoins;

        public MainWindow()
        {
            InitializeComponent();

            adpProducts = new NorthwindDataSetTableAdapters.ProductsTableAdapter();
            tblProducts = new NorthwindDataSet.ProductsDataTable();

            adpCategories = new NorthwindDataSetTableAdapters.CategoriesTableAdapter();
            tblCategories = new NorthwindDataSet.CategoriesDataTable();

            adpProductsWithInnerJoins = new NorthwindDataSetTableAdapters.ProductsWithInnerJoinsTableAdapter();
            tblProductsWithInnerJoins = new NorthwindDataSet.ProductsWithInnerJoinsDataTable();

            GetAllProducts();
        }

        private void GetAllProducts()
        {
            // using adapter's Fill() method
            //adpProducts.FillProducts(tblProducts);

            // using adapter's Get() method
            tblProducts = adpProducts.GetProducts();

            // set the DataTable as source for the DataGrid
            grdProducts.ItemsSource = tblProducts;
        }

        private void btnLoadAllProducts_Click(object sender, RoutedEventArgs e)
        {
            GetAllProducts();
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32(txtId.Text);

            // find a row based on ProductID
            var row = tblProducts.FindByProductID(id);

            // if something is fetched
            if (row != null)
            {
                // assign the values from the row to the textboxes
                txtName.Text = row.ProductName;
                txtPrice.Text = row.UnitPrice.ToString();
                txtQuantity.Text = row.UnitsInStock.ToString();
            }
            else
            {
                txtName.Text = txtPrice.Text = txtQuantity.Text = "";
                MessageBox.Show("Invalid product ID. Please try again.");
                txtId.Focus();
            }
        }

        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            string name = txtName.Text;
            decimal price = Convert.ToDecimal(txtPrice.Text);
            short quantity = Convert.ToInt16(txtQuantity.Text);

            // call the adapter's Insert() method
            adpProducts.Insert(name, price, quantity);

            // refresh the DataGrid
            GetAllProducts();
            MessageBox.Show("New product is added.");
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32(txtId.Text);
            string name = txtName.Text;
            decimal price = Convert.ToDecimal(txtPrice.Text);
            short quantity = Convert.ToInt16(txtQuantity.Text);

            // call the adapter's Update() method
            // update operation needs ID
            adpProducts.Update(name, price, quantity, id);

            // refresh the DataGrid
            GetAllProducts();
            MessageBox.Show("Product is updated.");
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32(txtId.Text);

            // call the adapter's Delete() method
            adpProducts.Delete(id);

            // refresh the DataGrid
            GetAllProducts();
            MessageBox.Show("Product is deleted.");
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            tblProducts = adpProducts.GetProductByName(txtName.Text);

            // if something is fetched
            if (tblProducts.Count > 0)
            {
                var row = tblProducts[0]; // read the fetched row

                MessageBox.Show($"Id: {row.ProductID}\nName: {row.ProductName}\nPrice: {row.UnitPrice}\nQuantity: {row.UnitsInStock}");
            }
            else
                MessageBox.Show("Invalid Product Name. Please try again.");
        }

        private void btnClearData_Click(object sender, RoutedEventArgs e)
        {
            txtId.Text = txtName.Text = txtPrice.Text = txtQuantity.Text = "";
            grdProducts.ItemsSource = null;
        }

        // on window load, populate the combobox with categories
        // set category name as display field
        // set category id as value field
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tblCategories = adpCategories.GetCategories();

            cmbCategories.ItemsSource = tblCategories;
            cmbCategories.DisplayMemberPath = "CategoryName";
            cmbCategories.SelectedValuePath = "CategoryID";
        }

        // on selecting a category from the combobox,
        // fetch all the products based on the selected category
        private void cmbCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int catId = (int)cmbCategories.SelectedValue;

            tblProductsWithInnerJoins = adpProductsWithInnerJoins.GetProductsByCategoryId(catId);
            grdProducts.ItemsSource = tblProductsWithInnerJoins;
        }
    }
}
