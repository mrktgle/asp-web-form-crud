using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace aspwebformcrud
{
    public partial class Customer : System.Web.UI.Page
    {
        SqlConnection conn = new SqlConnection(@"Server=localhost; Database=ASPCRUD; Integrated Security=SSPI;");
        SqlDataAdapter da;
        SqlCommand cmd;
        DataTable table;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                btnDelete.Enabled = false;
                FillDataGrid();
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        void FillDataGrid()
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
                da = new SqlDataAdapter("CustomerViewAll", conn);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                table = new DataTable();
                da.Fill(table);
                conn.Close();
                gvCustomer.DataSource = table;
                gvCustomer.DataBind();

            }
        }

        public void Clear()
        {
            hfCustomerID.Value = "";
            txtName.Text = txtCity.Text = "";
            lblSuccessMessage.Text = lblErrorMessage.Text = "";
            btnSave.Text = "Save";
            btnDelete.Enabled = false;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (conn.State == ConnectionState.Closed) {
                conn.Open();
                cmd = new SqlCommand("CustomerCreateOrUpdate",conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CustomerID", (hfCustomerID.Value == "" ? 0 : Convert.ToInt32(hfCustomerID.Value)));
                cmd.Parameters.AddWithValue("@Name", txtName.Text.Trim());
                cmd.Parameters.AddWithValue("@City", txtCity.Text.Trim());
                cmd.ExecuteNonQuery();
                conn.Close();
                string CustomerID = hfCustomerID.Value;
                if (CustomerID == "")
                {
                    Clear();
                    lblSuccessMessage.Text = "Save Successfully";
                    FillDataGrid();
                }
                else {
                    Clear();
                    lblSuccessMessage.Text = "Update Successfully";
                    FillDataGrid();
                }
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
                cmd = new SqlCommand("CustomerDeleteByID", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CustomerID", Convert.ToInt32(hfCustomerID.Value));
                cmd.ExecuteNonQuery();
                conn.Close();
                FillDataGrid();
                Clear();
                lblSuccessMessage.Text = "Delete Successfully";
            }
        }

        protected void Link_Onlick(object sender, EventArgs e)
        {
            int CustomerID = Convert.ToInt32((sender as LinkButton).CommandArgument);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
                da = new SqlDataAdapter("CustomerViewByID", conn);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@CustomerID", CustomerID);
                table = new DataTable();
                da.Fill(table);
                conn.Close();
                hfCustomerID.Value = CustomerID.ToString();
                txtName.Text = table.Rows[0]["Name"].ToString();
                txtCity.Text = table.Rows[0]["City"].ToString();
                btnSave.Text = "Update";
                btnDelete.Enabled = true;
            }
        }

    }
}