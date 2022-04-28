using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace db
{
  /// <summary>
  /// MainWindow.xaml에 대한 상호 작용 논리
  /// </summary>
  public partial class MainWindow : Window
  {
    List<Border> borderList;
    DispatcherTimer t = new DispatcherTimer();
    Random r = new Random();

    string connString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\konyang\source\repos\db\db\Colors.mdf;Integrated Security = True";
    
    //SqlConnection conn = null;

    public MainWindow()
    {
      InitializeComponent();

      borderList = new List<Border>
      {
        bd1, bd2, bd3, bd4, bd5, bd6, bd7, bd8, bd9, bd10,
        bd11, bd12, bd13, bd14, bd15, bd16, bd17, bd18, bd19, bd20
      };
      t.Interval = new TimeSpan(0, 0, 1);
      t.Tick += T_Tick;
    }

    int index = 0;
    private void T_Tick(object sender, EventArgs e)
    {
      string date = DateTime.Now.ToString("yyyy-MM-dd");
      string time = DateTime.Now.ToString("HH:mm:ss");
      lblDate.Text = date;
      lblTime.Text = time;

      byte[] colors = new byte[20];
      for(int i=0; i<20; i++)
      {
        colors[i] = (byte)(r.Next(256));
        borderList[i].Background = new SolidColorBrush(Color.FromRgb((byte)0, (byte)0, colors[i]));
      }

      string sql = "INSERT INTO ColorTable VALUES ('" + date + "', '"+ time +"'";
      for (int i = 0; i < 20; i++)
        sql += ", " + String.Format("{0}", colors[i]);
      sql += ")";
      //MessageBox.Show(sql);

      SqlConnection conn = new SqlConnection(connString);
      SqlCommand comm = new SqlCommand(sql, conn);
      conn.Open();
      comm.ExecuteNonQuery();
      conn.Close();

      //using (conn = new SqlConnection(connString))
      //using (SqlCommand comm = new SqlCommand(sql, conn))
      //{
      //  conn.Open();
      //  comm.ExecuteNonQuery();
      //}
      
      // ListBox 처리
      string lstItem = "";
      lstItem += string.Format($"{date} {time} ");
      for (int i = 0; i < 20; i++)
        lstItem += string.Format("{0,3} ", colors[i]);
      lstDB.Items.Add(lstItem);
      lstDB.SelectedIndex = index++;
      lstDB.ScrollIntoView(lstDB.SelectedItem); // 리스트박스 스크롤
    }

    bool flag = false;
    private void btnRandom_Click(object sender, RoutedEventArgs e)
    {
      if(flag == false)
      {
        btnRandom.Content = "정지";
        t.Start();
        flag = true;
      }
      else
      {
        btnRandom.Content = "Random 색깔 표시";
        t.Stop();
        flag = false;
      }
    }

    // DB에서 읽어오기
    private void btnDB_Click(object sender, RoutedEventArgs e)
    {
      lstDB.Items.Clear();

      string sql = "SELECT * FROM ColorTable";
      int[] colors = new int[20];

      using (SqlConnection conn = new SqlConnection(connString))
      using (SqlCommand command = new SqlCommand(sql, conn))
      {
        conn.Open();
        SqlDataReader reader = command.ExecuteReader();

        int index = 0;
        while(reader.Read())
        {
          lblDate.Text = reader["Date"].ToString(); // Date, [0]는 id
          lblTime.Text = reader["Time"].ToString();
          for(int i=0; i<20; i++)
            colors[i] = int.Parse(reader[i+3].ToString());  // id, Date, Time 뒤부터 20개

          string record = "";
          for(int i=0; i<reader.FieldCount; i++)
            record += String.Format("{0,3}", reader[i]) + " ";
          lstDB.Items.Add(record);
          lstDB.SelectedIndex = index++;
          lstDB.ScrollIntoView(lstDB.SelectedItem);

          for(int i=0; i<20; i++) {
            borderList[i].Background
              = new SolidColorBrush(Color.FromRgb((byte)0, (byte)0, (byte)colors[i]));
            
            // WPF에서 delay 주기
            Dispatcher.Invoke((ThreadStart)(() => { }), DispatcherPriority.ApplicationIdle);
            Thread.Sleep(20);
          }
        }
      }
    }

    private void btnExit_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    // DB Reset
    private void btnReset_Click(object sender, RoutedEventArgs e)
    {
      lstDB.Items.Clear();
      string sql = "Delete From ColorTable";

      using(SqlConnection conn = new SqlConnection(connString))
      using(SqlCommand comm = new SqlCommand(sql, conn))
      {
        conn.Open();
        comm.ExecuteNonQuery();
      }
    }
  }
}
