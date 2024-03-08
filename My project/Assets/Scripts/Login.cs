using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System.Data;
using TMPro;

public class Login : MonoBehaviour
{
    public static MySqlConnection SqlConn;

    static string ipAddress = "127.0.0.1";
    static string db_id = "root";
    static string db_pw = "sdh76030339@";
    static string db_name = "temaproject1";

    [SerializeField]
    TMP_InputField id_input;
    [SerializeField]
    TMP_InputField password_input;

    string strConn = string.Format("server={0};uid={1};pwd={2};database={3};charset=utf8 ;", ipAddress, db_id, db_pw, db_name);

    private void Awake()
    {
        try
        {
            SqlConn = new MySqlConnection(strConn);
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    void Start()
    {
        //Select TEST
        string query = "select * from user";
        DataSet ds = OnSelectRequest(query, "user");

        Debug.Log(ds.GetXml());
    }


    public void login()
    {
        try
        {
            string id = id_input.text;
            Debug.Log(id);
            SqlConn.Open();   //DB ����

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = SqlConn;
            cmd.CommandText = "select salt from user where ID = "+id;

            MySqlDataAdapter sd = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sd.Fill(ds, "user");
            Debug.Log(ds);
            SqlConn.Close();  //DB ���� ����
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
        }
    }
    //������ ����,������Ʈ ������ ��� �Լ�
    public static bool OnInsertOrUpdateRequest(string str_query)
    {
        try
        {
            MySqlCommand sqlCommand = new MySqlCommand();
            sqlCommand.Connection = SqlConn;
            sqlCommand.CommandText = str_query;

            SqlConn.Open();

            sqlCommand.ExecuteNonQuery();

            SqlConn.Close();

            return true;
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
            return false;
        }
    }

    //select ��ȸ ������ ���
    //2��° �Ķ���� table_name�� Dataset �̸��� �����ϱ� ����
    public static DataSet OnSelectRequest(string p_query, string table_name)
    {
        try
        {
            SqlConn.Open();   //DB ����

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = SqlConn;
            cmd.CommandText = p_query;

            MySqlDataAdapter sd = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sd.Fill(ds, table_name);

            SqlConn.Close();  //DB ���� ����

            return ds;
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
            return null;
        }
    }

    private void OnApplicationQuit()
    {
        SqlConn.Close();
    }
}
