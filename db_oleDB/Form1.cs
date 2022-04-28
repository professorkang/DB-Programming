using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace db_oleDB
{
  public partial class Form1 : Form
  {
    OleDbConnection conn = null;
    OleDbCommand comm = null;
    OleDbDataReader reader = null;

    string connStr= @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\konyang\OneDrive - 건양대학교\문서\StudentTable.accdb";
    public Form1()
    {
      InitializeComponent();
      DisplayStudents();
    }

    private void DisplayStudents()
    {
      ConnectionOpen();
      string sql = "SELECT * FROM StudentTable";
      comm = new OleDbCommand(sql, conn);
      ReadAndAddToListBox();
      ConnectionClose();
    }

    private void ReadAndAddToListBox()
    {
      lstStudents.Items.Clear();
      reader = comm.ExecuteReader();
      while (reader.Read())
      {
        string x = "";
        x += reader["ID"] + "\t";
        x += reader["SId"] + "\t";
        x += reader["SName"] + "\t";
        x += reader["Phone"] + "\t";

        lstStudents.Items.Add(x);
      }
      reader.Close();
    }

    private void ConnectionClose()
    {
      conn.Close();
      conn = null;
    }

    private void ConnectionOpen()
    {
      if (conn == null)
      {
        conn = new OleDbConnection(connStr);
        conn.Open();
      }
    }

    private void lstStudents_SelectedIndexChanged(object sender, EventArgs e)
    {
      ListBox lb = sender as ListBox;

      if(lb.SelectedItem == null)
        return;
      string[] s = lb.SelectedItem.ToString().Split('\t');
      txtID.Text = s[0];
      txtSId.Text = s[1];
      txtName.Text = s[2];
      txtPhone.Text = s[3];
    }

    private void btnAll_Click(object sender, EventArgs e)
    {
      lstStudents.Items.Clear();
      DisplayStudents();
    }

    private void btnEnd_Click(object sender, EventArgs e)
    {
      Close();
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      txtID.Text = "";
      txtSId.Text = "";
      txtName.Text = "";
      txtPhone.Text = "";
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
      if (txtName.Text == "" || txtSId.Text == "" || txtPhone.Text == "")
        return;

      ConnectionOpen();

      string sql = string.Format("INSERT INTO StudentTable(SId, SName, Phone) VALUES ({0}, '{1}', '{2}')",
        txtSId.Text, txtName.Text, txtPhone.Text);
      comm = new OleDbCommand(sql, conn);
      if (comm.ExecuteNonQuery() == 1)
        MessageBox.Show("삽입 성공!");

      ConnectionClose();
      btnAll_Click(sender, e);
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      if (txtName.Text == "" && txtSId.Text == "" && txtPhone.Text == "")
        return;
      ConnectionOpen();
      
      // sql을 만든다
      string sql = "";
      if (txtSId.Text != "")
        sql = string.Format("SELECT * FROM StudentTable WHERE SId={0}", txtSId.Text);
      else if(txtName.Text != "")
        sql = string.Format("SELECT * FROM StudentTable WHERE SName='{0}'", txtName.Text);
      else if (txtPhone.Text != "")
        sql = string.Format("SELECT * FROM StudentTable WHERE Phone='{0}'", txtPhone.Text);
      
      // comm을 만든다
      comm = new OleDbCommand(sql, conn);
      ReadAndAddToListBox();
      ConnectionClose();
    }

    private void btnDelete_Click(object sender, EventArgs e)
    {
      ConnectionOpen();

      string sql = string.Format("DELETE FROM StudentTable WHERE ID={0}", txtID.Text);
      comm = new OleDbCommand(sql,conn);
      if (comm.ExecuteNonQuery() == 1)
        MessageBox.Show("삭제 성공!");
      ConnectionClose();
      btnClear_Click(sender, e);
      DisplayStudents();
    }

    private void btnUpdate_Click(object sender, EventArgs e)
    {
      ConnectionOpen();

      string sql = string.Format("UPDATE StudentTable SET SId={0}, SName='{1}', Phone='{2}' WHERE ID={3}",
        txtSId.Text, txtName.Text, txtPhone.Text, txtID.Text);
      comm = new OleDbCommand(sql, conn);
      if (comm.ExecuteNonQuery() == 1)
        MessageBox.Show("수정 성공!");
      ConnectionClose();
      DisplayStudents();
    }
  }
}