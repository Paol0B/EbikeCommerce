using System.Data.SqlClient;

namespace EbikeCommerce.DBmodel
{
    public static class DBquery
    {
        public static SqlCommand CreateSqlCommand(SqlConnection conn)
        {
            SqlCommand cmd = new()
            {
                Connection = conn,
                CommandText = "select * from products"
            };

            return cmd;
        }

        public static SqlCommand FindById(SqlConnection conn, int id)
        {
            SqlCommand cmd = new()
            {
                Connection = conn,
                CommandText = "select * from products where product_id = @id"
            };

            cmd.Parameters.Add(new SqlParameter("@id", id));

            return cmd;
        }

        public static SqlCommand FindByUsername(SqlConnection conn, string? user)
        {
            SqlCommand cmd = new()
            {
                Connection = conn,
                CommandText = "select * from customers where username = @user"
            };

            cmd.Parameters.Add(new SqlParameter("@user", user));

            return cmd;
        }

        public static SqlCommand FindPasswdAndUser(SqlConnection conn, string userOremail)
        {
            SqlCommand cmd = new()
            {
                Connection = conn,
                CommandText = "select passwd from customers where username = @userOremail or email = @userOremail",

            };

            cmd.Parameters.AddWithValue("@userOremail", userOremail);
            return cmd;
        }
    }
}
