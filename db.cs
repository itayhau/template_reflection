using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dpState
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MyTableNameAttribute : Attribute
    {
        public string Name { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class MyFieldNameAttribute : Attribute
    {
        public string ColumnName { get; set; }
    }

    [MyTableName(Name = "Employees")]
    class Employee
    {
        public int Id { get; set; }
        //public string FirstName { get; set; }
        [MyFieldName(ColumnName = "FirstName")]
        public string FName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public int Salary { get; set; }
    }

    class Program
    {
        //static List<T> Foo<T>(string tableName) where T : new()
        static List<T> Foo<T>() where T : new()
        {
            string tableName = null;
            List<T> res = new List<T>();
            T a = new T();
            Type recordType = typeof(T);
            //T account = JsonConvert.DeserializeObject<T>("{Age : 18, Name : gigi}");

            var customAttributes = (MyTableNameAttribute[])typeof(T).GetCustomAttributes(typeof(MyTableNameAttribute), true);
            if (customAttributes.Length > 0)
            {
                var myAttribute = customAttributes[0];
                tableName = myAttribute.Name;
            }
            else
                throw new ArgumentException("Your POCO does not contain table name, dah!");

            //Command and Data Reader
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(@"Data Source=.;Initial Catalog=EmployeeDB;Integrated Security=True");
            cmd.Connection.Open();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = $"SELECT * FROM {tableName}";


            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.Default);

            List<T> list = new List<T>();
            while (reader.Read() == true)
            {
                // reader.MetaData[0].column
                // reader.MetaData[0].metaType.ClassType
                Console.WriteLine($" {reader["ID"]} {reader["FIRSTNAME"]} {reader["LASTNAME"]} {reader["SALARY"]}");
                T record = new T();
                foreach(var prop in recordType.GetProperties())
                {
                    string columnName = prop.Name;
                    object[] attribute = prop.GetCustomAttributes(typeof(MyFieldNameAttribute), true);
                    if (attribute.Length > 0)
                    {
                        MyFieldNameAttribute myAttribute = (MyFieldNameAttribute)attribute[0];
                        columnName = myAttribute.ColumnName;
                    }
                    prop.SetValue(record, reader[columnName]);
                }
                list.Add(record);
            }

            cmd.Connection.Close();

            return list;
        }

        static void Main(string[] args)
        {
            List<Employee> res = Foo<Employee>();
        }
    }
}
