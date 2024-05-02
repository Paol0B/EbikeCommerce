using System.Data.SqlClient;
using System.Text;

namespace EbikeCommerce.DBmodel
{
    public static class DBservice
    {
        static readonly string ConnString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BikeStores;Integrated Security=True;";

        public static async Task<List<ProductRecord>> OnGetDataAsync()
        {
            List<ProductRecord> DBlist = [];

            try
            {
                using (SqlConnection conn = new(ConnString))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = DBquery.CreateSqlCommand(conn))
                    {
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            var product_id_ordinal = reader.GetOrdinal("product_id");
                            var product_name_ordinal = reader.GetOrdinal("product_name");
                            var brand_id_ordinal = reader.GetOrdinal("brand_id");
                            var category_id_ordinal = reader.GetOrdinal("category_id");
                            var model_year_ordinal = reader.GetOrdinal("model_year");
                            var list_price_ordinal = reader.GetOrdinal("list_price");

                            while (await reader.ReadAsync())
                            {
                                ProductRecord item = new()
                                {
                                    product_id = reader.GetInt32(product_id_ordinal),
                                    product_name = reader.GetString(product_name_ordinal),
                                    brand_id = reader.GetInt32(brand_id_ordinal),
                                    category_id = reader.GetInt32(category_id_ordinal),
                                    model_year = reader.GetInt16(model_year_ordinal),
                                    list_price = reader.GetDecimal(list_price_ordinal)
                                };
                                DBlist.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                Console.WriteLine(ex.Message);
            }

            return DBlist;
        }

        //filtered data

        public static List<ProductRecord> GetFilteredData(string Text)
        {
            List<ProductRecord> DBlist = [];

            try
            {
                using (SqlConnection conn = new(ConnString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "select * from products where product_name like @Text";
                        cmd.Parameters.AddWithValue("@Text", "%" + Text + "%");

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            var product_id_ordinal = reader.GetOrdinal("product_id");
                            var product_name_ordinal = reader.GetOrdinal("product_name");
                            var brand_id_ordinal = reader.GetOrdinal("brand_id");
                            var category_id_ordinal = reader.GetOrdinal("category_id");
                            var model_year_ordinal = reader.GetOrdinal("model_year");
                            var list_price_ordinal = reader.GetOrdinal("list_price");

                            while (reader.Read())
                            {
                                ProductRecord item = new()
                                {
                                    product_id = reader.GetInt32(product_id_ordinal),
                                    product_name = reader.GetString(product_name_ordinal),
                                    brand_id = reader.GetInt32(brand_id_ordinal),
                                    category_id = reader.GetInt32(category_id_ordinal),
                                    model_year = reader.GetInt16(model_year_ordinal),
                                    list_price = reader.GetDecimal(list_price_ordinal)
                                };
                                DBlist.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                Console.WriteLine(ex.Message);
            }

            return DBlist;
        }

        public static List<ProductRecord> GetFilteredData(int year, int price, int brand)
        {
            List<ProductRecord> DBlist = [];

            try
            {
                using (SqlConnection conn = new(ConnString))
                {
                    conn.Open();
                    StringBuilder queryBuilder = new("SELECT * FROM products WHERE");

                    if (year > 0)
                    {
                        queryBuilder.Append(" model_year = @year");
                    }

                    if (brand > 0)
                    {
                        if (year > 0)
                        {
                            queryBuilder.Append(" AND brand_id = @brand");
                        }
                        else
                        {
                            queryBuilder.Append(" brand_id = @brand");
                        }
                    }

                    if (price > 0)
                    {
                        if (Dictionaries.PriceRanges.ContainsKey(price) && brand > 0 || year > 0)
                        {
                            var priceRange = Dictionaries.PriceRanges[price];
                            queryBuilder.Append(" AND list_price BETWEEN @minPrice AND @maxPrice");
                        }
                        else
                        {
                            var priceRange = Dictionaries.PriceRanges[price];
                            queryBuilder.Append(" list_price BETWEEN @minPrice AND @maxPrice");
                        }
                    }

                    using (SqlCommand cmd = new (queryBuilder.ToString(), conn))
                    {
                        if (brand > 0)
                        {
                            cmd.Parameters.AddWithValue("@brand", brand);
                        }

                        if (year > 0)
                            cmd.Parameters.AddWithValue("@year", year);

                        if (price > 0)
                        {
                            if (Dictionaries.PriceRanges.TryGetValue(price, out Tuple<int, int>? value))
                            {
                                var priceRange = value;
                                cmd.Parameters.AddWithValue("@minPrice", priceRange.Item1);
                                cmd.Parameters.AddWithValue("@maxPrice", priceRange.Item2);
                            }
                        }

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            var product_id_ordinal = reader.GetOrdinal("product_id");
                            var product_name_ordinal = reader.GetOrdinal("product_name");
                            var brand_id_ordinal = reader.GetOrdinal("brand_id");
                            var category_id_ordinal = reader.GetOrdinal("category_id");
                            var model_year_ordinal = reader.GetOrdinal("model_year");
                            var list_price_ordinal = reader.GetOrdinal("list_price");

                            while (reader.Read())
                            {
                                ProductRecord item = new()
                                {
                                    product_id = reader.GetInt32(product_id_ordinal),
                                    product_name = reader.GetString(product_name_ordinal),
                                    brand_id = reader.GetInt32(brand_id_ordinal),
                                    category_id = reader.GetInt32(category_id_ordinal),
                                    model_year = reader.GetInt16(model_year_ordinal),
                                    list_price = reader.GetDecimal(list_price_ordinal)
                                };
                                DBlist.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                Console.WriteLine(ex.Message);
            }

            return DBlist;
        }

        public static ProductRecord? GetbyID(int id)
        {
            try
            {
                using SqlConnection conn = new(ConnString);
                conn.Open();

                using SqlCommand cmd = DBquery.FindById(conn, id);
                using SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.Read())
                    return null;

                return new ProductRecord
                {
                    product_id = reader.GetInt32(0),
                    product_name = reader.GetString(1),
                    brand_id = reader.GetInt32(2),
                    category_id = reader.GetInt32(3),
                    model_year = reader.GetInt16(4),
                    list_price = reader.GetDecimal(5)
                };
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Database exception: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General exception: {ex.Message}");
            }

            return null;
        }

        public static CustomerRecord? GetbyUser(string? user)
        {
            try
            {
                using SqlConnection conn = new(ConnString);
                conn.Open();

                using SqlCommand cmd = DBquery.FindByUsername(conn, user);
                using SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.Read())
                    return null;

                return new CustomerRecord
                {
                    customer_id = reader.GetInt32(0),
                    first_name = reader.GetString(1),
                    last_name = reader.GetString(2),
                    phone = reader.IsDBNull(3) ? null : reader.GetString(3),
                    username = reader.GetString(4),
                    email = reader.GetString(5),
                    passwd = reader.GetString(6),
                    street = reader.IsDBNull(7) ? null : reader.GetString(7),
                    city = reader.IsDBNull(8) ? null : reader.GetString(8),
                    state = reader.IsDBNull(9) ? null : reader.GetString(9),
                    zip_code = reader.IsDBNull(10) ? null : reader.GetString(10)
                };
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Database exception: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General exception: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Updates a customer record.
        /// </summary>
        /// <param name="rec">The customer record to update.</param>
        public static void UpdateCustomer(CustomerRecord rec)
        {
            using (SqlConnection conn = new(ConnString))
            {
                conn.Open();
                using (SqlCommand cmd = new())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = "UPDATE customers SET ";

                    if (rec.phone != null)
                    {
                        cmd.CommandText += "phone = @phone, ";
                        cmd.Parameters.AddWithValue("@phone", rec.phone);
                    }

                    if (rec.username != null)
                    {
                        cmd.CommandText += "username = @username, ";
                        cmd.Parameters.AddWithValue("@username", rec.username);
                    }

                    if (rec.email != null)
                    {
                        cmd.CommandText += "email = @email, ";
                        cmd.Parameters.AddWithValue("@email", rec.email);
                    }

                    if (rec.street != null)
                    {
                        cmd.CommandText += "street = @street, ";
                        cmd.Parameters.AddWithValue("@street", rec.street);
                    }

                    if (rec.city != null)
                    {
                        cmd.CommandText += "city = @city, ";
                        cmd.Parameters.AddWithValue("@city", rec.city);
                    }

                    if (rec.state != null)
                    {
                        cmd.CommandText += "state = @state, ";
                        cmd.Parameters.AddWithValue("@state", rec.state);
                    }

                    if (rec.zip_code != null)
                    {
                        cmd.CommandText += "zip_code = @zip_code, ";
                        cmd.Parameters.AddWithValue("@zip_code", rec.zip_code);
                    }

                    cmd.CommandText = cmd.CommandText.TrimEnd(',', ' ');

                    cmd.CommandText += " WHERE customer_id = @customer_id";
                    cmd.Parameters.AddWithValue("@customer_id", rec.customer_id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void UpdatePassword(int id, string newPassword)
        {
            using (SqlConnection conn = new(ConnString))
            {
                conn.Open();
                using (SqlCommand cmd = new())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "update customers set passwd = @passwd where customer_id = @id";
                    cmd.Parameters.AddWithValue("@passwd", BCrypt.Net.BCrypt.HashPassword(newPassword));
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Adds a new customer record.
        /// </summary>
        /// <param name="NewRecord">The new customer record to add.</param>
        public static void AddCustomer(CustomerRecord NewRecord)
        {
            using (SqlConnection conn = new(ConnString))
            {
                using (SqlCommand cmd = new("insert into customers (first_name, last_name, phone, username, email, passwd, street, city, state, zip_code) values (@first_name, @last_name, @phone, @username, @email, @passwd,@street, @city, @state, @zip_code)", conn))
                {
                    cmd.Parameters.AddWithValue("@first_name", NewRecord.first_name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@last_name", NewRecord.last_name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@phone", NewRecord.phone ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@username", NewRecord.username);
                    cmd.Parameters.AddWithValue("@passwd", BCrypt.Net.BCrypt.HashPassword(NewRecord.passwd));
                    cmd.Parameters.AddWithValue("@email", NewRecord.email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@street", NewRecord.street ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@city", NewRecord.city ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@state", NewRecord.state ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@zip_code", NewRecord.zip_code ?? (object)DBNull.Value);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static bool CheckStocks(int id, int qta)
        {

            using (SqlConnection conn = new (ConnString))
            {
                using(SqlCommand cmd = new ())
                {                     
                    cmd.Connection = conn;
                    cmd.CommandText = "select sum(quantity) from stocks where product_id = @id;";
                    cmd.Parameters.AddWithValue("@id", id);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int quantity = reader.GetInt32(0);

                            if (quantity > qta)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public static bool UpdateStocks(int id)
        {
            using (SqlConnection conn = new(ConnString))
            {
                using (SqlCommand cmd = new())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "update stocks set quantity = quantity - 1 where product_id = @id;";
                    cmd.Parameters.AddWithValue("@id", id);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int quantity = reader.GetInt32(0);
                            if (quantity > 0)
                            {
                                return true;
                            }
                        }

                    }
                }
            }
            return false;
        }

        public static bool CheckLogin(string password, string userORemail)
        {
            using (SqlConnection conn = new(ConnString))
            {
                conn.Open();
                using (SqlCommand cmd = DBquery.FindPasswdAndUser(conn, userORemail))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string hashedPassword = reader.GetString(0);
                        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
                    }
                }
            }
            return false; // Return false if the login check fails
        }

        public static string FindUserByEmail(string email)
        {
            using (SqlConnection conn = new(ConnString))
            {
                conn.Open();
                using (SqlCommand cmd = new("select username from customers where email = @Email", conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader.GetString(0);
                        }
                    }
                }
            }
            return "";
        }

        public static bool CheckAddress(string? user)
        {
            if (string.IsNullOrEmpty(user))
                return false;

            using (SqlConnection conn = new(ConnString))
            {
                conn.Open();
                using (SqlCommand cmd = new("select COUNT(*) from customers where username = @User AND (street IS NULL OR city IS NULL OR state IS NULL OR zip_code IS NULL)", conn))
                {
                    cmd.Parameters.AddWithValue("@User", user);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        public static bool Buy(string user, List<int> cart)
        {
            int storeId = 0;
            int staffId = 1;
            try
            {
                using (SqlConnection conn = new(ConnString))
                {
                    conn.Open();

                    //select store
                    using (SqlCommand cmdStore = new("SELECT store_id FROM stocks WHERE quantity > 0", conn))
                    {
                        using (SqlDataReader reader = cmdStore.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                storeId = reader.GetInt32(0);
                            }
                        }
                    }

                    // Create order
                    using (SqlCommand cmd = new())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "insert into orders (customer_id, order_status, order_date, required_date, store_id, staff_id) values ((select customer_id from customers where username = @user), 1, @date, @date, @storeId, @staffId); SELECT SCOPE_IDENTITY();";
                        cmd.Parameters.AddWithValue("@user", user);
                        cmd.Parameters.AddWithValue("@date", DateTime.Now);
                        cmd.Parameters.AddWithValue("@storeId", storeId);
                        cmd.Parameters.AddWithValue("@staffId", staffId);

                        // Execute the command and get the order ID
                        int orderId = Convert.ToInt32(cmd.ExecuteScalar());

                        // Group the cart items by ID and count their occurrences
                        var groupedCart = cart.GroupBy(id => id).Select(group => new { ProductId = group.Key, Quantity = group.Count() });

                        // Insert order items and update stocks
                        foreach (var item in groupedCart)
                        {
                            int productId = item.ProductId;
                            int quantity = item.Quantity;

                            // Insert order item
                            using (SqlCommand cmdOrderItems = new())
                            {
                                cmdOrderItems.Connection = conn;
                                cmdOrderItems.CommandText = "insert into order_items (order_id, product_id, quantity, list_price, discount) values (@orderId, @productId, @quantity, (select list_price from products where product_id = @productId), 0)";
                                cmdOrderItems.Parameters.AddWithValue("@orderId", orderId);
                                cmdOrderItems.Parameters.AddWithValue("@productId", productId);
                                cmdOrderItems.Parameters.AddWithValue("@quantity", quantity);
                                cmdOrderItems.ExecuteNonQuery();
                            }

                            // Update stocks
                            using (SqlCommand cmdStocks = new())
                            {
                                cmdStocks.Connection = conn;
                                cmdStocks.CommandText = "update stocks set quantity = quantity - @quantity where store_id = @storeId and product_id = @productId";
                                cmdStocks.Parameters.AddWithValue("@storeId", storeId);
                                cmdStocks.Parameters.AddWithValue("@productId", productId);
                                cmdStocks.Parameters.AddWithValue("@quantity", quantity);
                                cmdStocks.ExecuteNonQuery();
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static Dictionary<int, string> FillBrands()
        {
            // Fill the dictionary with the brands
            Dictionary<int, string> brands = [];
            using (SqlConnection conn = new(ConnString))
            {
                conn.Open();
                using (SqlCommand cmd = new("select * from brands", conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        brands.Add(reader.GetInt32(0), reader.GetString(1));
                    }
                }
            }
            return brands;
        }

        public static Dictionary<int, string> FillCategories()
        {
            // Fill the dictionary with the categories
            Dictionary<int, string> categories = [];
            using (SqlConnection conn = new(ConnString))
            {
                conn.Open();
                using (SqlCommand cmd = new("select * from categories", conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categories.Add(reader.GetInt32(0), reader.GetString(1));
                    }
                }
            }
            return categories;
        }
    }
}
