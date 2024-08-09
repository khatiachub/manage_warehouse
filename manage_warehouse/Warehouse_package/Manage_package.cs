using manage_warehouse.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Reflection;
using Newtonsoft.Json.Converters;
using Microsoft.Win32;
using System.Text.Json;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;
using System.Security.Cryptography;

public class ManagePackage
{
    private readonly string _connectionString;

    public ManagePackage(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected OracleConnection GetConnection()
    {
        return new OracleConnection(_connectionString);
    }

    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }
    private bool ValidatePassword(string inputPassword, string dbPasswordHash)
    {
        string hashedInputPassword = HashPassword(inputPassword);
        return hashedInputPassword == dbPasswordHash;
    }


    public bool RegisterCompany(CompanyRegisterModel model)
    {
        using (OracleConnection connection = GetConnection())
        {
            try
            {
                connection.Open();
                using (OracleCommand command = new OracleCommand("pkg_company_operations.proc_register_company", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    string hashedPassword = HashPassword(model.password);
                    command.Parameters.Add("p_companyname", OracleDbType.Varchar2).Value = model.company_name;
                    command.Parameters.Add("p_address", OracleDbType.Varchar2).Value = model.address;
                    command.Parameters.Add("p_mobile", OracleDbType.Int32).Value = model.mobile;
                    command.Parameters.Add("p_password", OracleDbType.Varchar2).Value = hashedPassword;
                    command.Parameters.Add("p_name", OracleDbType.Varchar2).Value = model.name;
                    command.Parameters.Add("p_lastname", OracleDbType.Varchar2).Value = model.lastname;
                    command.Parameters.Add("p_username", OracleDbType.Varchar2).Value = model.username;
                    command.Parameters.Add("p_email", OracleDbType.Varchar2).Value = model.email;
                    command.Parameters.Add("p_role_id", OracleDbType.Int32).Value = model.role_id;

                    command.ExecuteNonQuery();
                    
                    Console.WriteLine("Stored procedure executed successfully.");
                    return true;
                }
            }
            catch (OracleException ex)
            {
                Console.WriteLine($"Oracle error: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
                return false;
            }
        }
    }


    public bool RegisterUser(UserRegisterModel model)
    {
        using (OracleConnection connection = GetConnection())
        {
            try
            {
                connection.Open();
                using (OracleCommand command = new OracleCommand("pkg_users_operations.proc_register_user", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    string hashedPassword = HashPassword(model.password);
                    command.Parameters.Add("p_name", OracleDbType.Varchar2).Value = model.name;
                    command.Parameters.Add("p_lastname", OracleDbType.Varchar2).Value = model.lastname;
                    command.Parameters.Add("p_username", OracleDbType.Varchar2).Value = model.username;
                    command.Parameters.Add("p_mobile", OracleDbType.Varchar2).Value = model.mobile;
                    command.Parameters.Add("p_password", OracleDbType.Varchar2).Value = hashedPassword;
                    command.Parameters.Add("p_role_id", OracleDbType.Int32).Value = model.role_id;
                    command.Parameters.Add("p_company_id", OracleDbType.Int32).Value = model.company_id;
                    command.Parameters.Add("p_warehouse_id", OracleDbType.Int32).Value = model.warehouse_id;

                    command.ExecuteNonQuery();
                    Console.WriteLine("Stored procedure executed successfully.");
                    return true;
                }
            }
            catch (OracleException ex)
            {
                Console.WriteLine($"Oracle error: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
                return false;
            }
        }
    }

    public TokenModel LoginUser(UserLoginModel model)
    {
        try
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new OracleCommand("pkg_users_operations.proc_login_user", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("p_username", OracleDbType.Varchar2).Value = model.username;
                    command.Parameters.Add("p_user_curs", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var hashedpassword = reader["password"].ToString();

                            var Tokenmodel = new TokenModel
                            {
                                id = Convert.ToInt32(reader["id"]),
                                username = reader["username"].ToString(),
                                role = reader["role"].ToString()
                            };

                            if (ValidatePassword(model.password, hashedpassword))
                            {
                                return Tokenmodel;
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception occurred: {ex.Message}");
            throw;
        }
    }


    public List<UserModel> GetUsers(int id)
    {
        List<UserModel> users = new List<UserModel>();

        using (var connection = GetConnection())
        {
            connection.Open();
            using (var command = new OracleCommand("pkg_users_operations.proc_get_users", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("p_id", OracleDbType.Int32).Value = id;

                OracleParameter cursorParameter = new OracleParameter
                {
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(cursorParameter);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var user = new UserModel
                        {
                            name = reader["name"].ToString(),
                            lastname = reader["lastname"].ToString(),
                            username = reader["username"].ToString(),
                            mobile = Convert.ToInt32(reader["mobile"]),
                            role = reader["role"].ToString(),
                            id = Convert.ToInt32(reader["id"])
                        };
                        users.Add(user);
                    }
                }
            }
        }

        return users;
    }

    public bool UpdateUser(UserRegisterModel model,int id)
    {
        using (var connection = GetConnection())
        {
            connection.Open();
            using (var command = new OracleCommand("pkg_users_operations.proc_update_user", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("p_id", OracleDbType.Int32).Value = id;
                command.Parameters.Add("p_name", OracleDbType.Varchar2).Value = model.name;
                command.Parameters.Add("p_lastname", OracleDbType.Varchar2).Value = model.lastname;
                command.Parameters.Add("p_username", OracleDbType.Varchar2).Value = model.username;
                command.Parameters.Add("p_mobile", OracleDbType.Int32).Value = model.mobile;
                command.Parameters.Add("p_password", OracleDbType.Varchar2).Value = model.password;
                command.Parameters.Add("p_role", OracleDbType.Varchar2).Value = model.role_id;
                command.Parameters.Add("p_warehouse_id", OracleDbType.Int32).Value = model.warehouse_id;
                command.Parameters.Add("p_company_id", OracleDbType.Int32).Value = model.company_id;
                try
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("User updated successfully.");
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating user: {ex.Message}");
                    return false;
                }
            }
        }
    }
public bool EntryProduct(ProductModel model)
    {

        DateTime entryDate;
        if (!DateTime.TryParseExact(model.entry_date, "dd-MMM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out entryDate))
        {
            Console.WriteLine("Invalid date format.");
        }
        else
        {
            Console.WriteLine($"Parsed date: {entryDate}");
        }
       

        using (var connection = GetConnection())
        {
            try
            {
                connection.Open();
                using (var command = new OracleCommand("pkg_warehouse_product_managment.proc_entry_products", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    string formattedentryDate = entryDate.ToString("dd-MMM-yyyy");

                    command.Parameters.Add("p_barcode", OracleDbType.Varchar2).Value = model.barcode;
                    command.Parameters.Add("p_product_name", OracleDbType.Varchar2).Value = model.product_name;
                    command.Parameters.Add("p_quantity", OracleDbType.Varchar2).Value = model.quantity;
                    command.Parameters.Add("p_entry_date", OracleDbType.Varchar2).Value =formattedentryDate;
                    command.Parameters.Add("p_operator_id", OracleDbType.Varchar2).Value = model.operator_id;
                    command.Parameters.Add("p_warehouse_name", OracleDbType.Varchar2).Value = model.warehouse_name;


                    command.ExecuteNonQuery();

                    return true;
                }
            }
            catch (OracleException ex)
            {
                Console.WriteLine($"Oracle error: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
                return false;
            }
        }
    }

    public bool ExitProduct(ProductModel model)
    {
        DateTime exitDate;
        if (!DateTime.TryParseExact(model.exit_date, "dd-MMM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out exitDate))
        {
            Console.WriteLine("Invalid date format.");
        }
        else
        {
            Console.WriteLine($"Parsed date: {exitDate}");
        }

        using (var connection = GetConnection())
        {
            try
            {
                connection.Open();
                using (var command = new OracleCommand("pkg_warehouse_product_managment.proc_exit_products", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    string formattedexitDate = exitDate.ToString("dd-MMM-yyyy");

                    command.Parameters.Add("p_barcode", OracleDbType.Varchar2).Value = model.barcode;
                    command.Parameters.Add("p_product_name", OracleDbType.Varchar2).Value = model.product_name;
                    command.Parameters.Add("p_quantity", OracleDbType.Varchar2).Value = model.quantity;
                    command.Parameters.Add("p_exit_date", OracleDbType.Varchar2).Value = formattedexitDate;
                    command.Parameters.Add("p_operator_id", OracleDbType.Varchar2).Value = model.operator_id;
                    command.Parameters.Add("p_warehouse_name", OracleDbType.Varchar2).Value = model.warehouse_name;
                    command.Parameters.Add("p_unit", OracleDbType.Varchar2).Value = model.unit;


                    command.ExecuteNonQuery();

                    return true;
                }
            }
            catch (OracleException ex)
            {
                Console.WriteLine($"Oracle error: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
                return false;
            }
        }
    }




    public List<ProductModel> GetAllEntryProducts()
    {
        List<ProductModel> products = new List<ProductModel>();

        using (var connection = GetConnection())
        {
            connection.Open();
            using (var command = new OracleCommand("pkg_warehouse_product_managment.proc_get_all_entry_products", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                OracleParameter cursorParameter = new OracleParameter
                {
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(cursorParameter);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var product = new ProductModel
                        {
                            barcode = reader["barcode"].ToString(),
                            product_name = reader["product_name"].ToString(),
                            quantity = Convert.ToInt32(reader["quantity"]),
                            entry_date = reader["entry_date"].ToString()
                        };
                        products.Add(product);
                    }
                }
            }
        }
        return products;
    }


    public List<ProductModel> GetAllExitProducts()
    {
        List<ProductModel> products = new List<ProductModel>();

        using (var connection = GetConnection())
        {
            connection.Open();
            using (var command = new OracleCommand("pkg_warehouse_product_managment.proc_get_all_exit_products", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                OracleParameter cursorParameter = new OracleParameter
                {
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(cursorParameter);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var product = new ProductModel
                        {
                            barcode = reader["barcode"].ToString(),
                            product_name = reader["product_name"].ToString(),
                            quantity = Convert.ToInt32(reader["quantity"]),
                            exit_date = reader["exit_date"].ToString()
                        };
                        products.Add(product);
                    }
                }
            }
        }
        return products;
    }

    public List<ProductModel> GetEntryProduct(int id)
    {
        List<ProductModel> products = new List<ProductModel>();

        using (var connection = GetConnection())
        {
            connection.Open();
            using (var command = new OracleCommand("pkg_warehouse_product_managment.proc_get_entry_product", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("p_id", OracleDbType.Int32).Value = id;
                OracleParameter cursorParameter = new OracleParameter
                {
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(cursorParameter);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var product = new ProductModel
                        {
                            barcode = reader["barcode"].ToString(),
                            product_name = reader["product_name"].ToString(),
                            quantity = Convert.ToInt32(reader["quantity"]),
                            entry_date = reader["entry_date"].ToString(),
                            name = reader["name"].ToString(),
                            lastname = reader["lastname"].ToString(),
                            warehouse_name = reader["warehouse_name"].ToString()
                        };
                        products.Add(product);
                    }
                }
            }
        }
        return products;
    }



    public List<ProductModel> GetExitProduct(int id)
    {
        List<ProductModel> products = new List<ProductModel>();

        using (var connection = GetConnection())
        {
            connection.Open();
            using (var command = new OracleCommand("pkg_warehouse_product_managment.proc_get_exit_product", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("p_id", OracleDbType.Int32).Value = id;
                OracleParameter cursorParameter = new OracleParameter
                {
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(cursorParameter);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var product = new ProductModel
                        {
                            barcode = reader["barcode"].ToString(),
                            product_name = reader["product_name"].ToString(),
                            quantity = Convert.ToInt32(reader["quantity"]),
                            exit_date = reader["entry_date"].ToString(),
                            name = reader["name"].ToString(),
                            lastname = reader["lastname"].ToString(),
                            warehouse_name = reader["warehouse_name"].ToString()
                        };
                        products.Add(product);
                    }
                }
            }
        }
        return products;
    }


    public bool UpdateEntryProduct(ProductModel model, int id)
    {
        using (var connection = GetConnection())
        {
            connection.Open();
            using (var command = new OracleCommand("pkg_warehouse_product_managment.proc_update_entry_product", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("p_barcode", OracleDbType.Varchar2).Value = model.barcode;
                command.Parameters.Add("p_product_name", OracleDbType.Varchar2).Value = model.product_name;
                command.Parameters.Add("p_quantity", OracleDbType.Int32).Value = model.quantity;
                command.Parameters.Add("p_entry_date", OracleDbType.Varchar2).Value = model.entry_date;
                command.Parameters.Add("p_id", OracleDbType.Int32).Value = id;
                try
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("User updated successfully.");
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating user: {ex.Message}");
                    return false;
                }
            }
        }
    }

    public bool UpdateExitProduct(ProductModel model, int id)
    {
        using (var connection = GetConnection())
        {
            connection.Open();
            using (var command = new OracleCommand("pkg_warehouse_product_managment.proc_update_exit_product", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("p_barcode", OracleDbType.Varchar2).Value = model.barcode;
                command.Parameters.Add("p_product_name", OracleDbType.Varchar2).Value = model.product_name;
                command.Parameters.Add("p_quantity", OracleDbType.Int32).Value = model.quantity;
                command.Parameters.Add("p_exit_date", OracleDbType.Varchar2).Value = model.entry_date;
                command.Parameters.Add("p_id", OracleDbType.Int32).Value = id;
                command.Parameters.Add("p_unit", OracleDbType.Varchar2).Value = model.unit;
                try
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("User updated successfully.");
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating user: {ex.Message}");
                    return false;
                }
            }
        }
    }


    public List<ProductModel> GetAllCurrentBalance()
    {
        List<ProductModel> products = new List<ProductModel>();

        using (var connection = GetConnection())
        {
            connection.Open();
            using (var command = new OracleCommand("pkg_warehouse_product_managment.proc_get_current_balance_forall", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                OracleParameter cursorParameter = new OracleParameter
                {
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(cursorParameter);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var product = new ProductModel
                        {
                            barcode = reader["barcode"].ToString(),
                            product_name = reader["product_name"].ToString(),
                            current_balance = Convert.ToInt32(reader["current_balance"]),
                            warehouse_name = reader["warehouse_name"].ToString()
                        };
                        products.Add(product);
                    }
                }
            }
        }
        return products;
    }


    public List<ProductModel> GetCurrentBalanace(int id)
    {
        List<ProductModel> products = new List<ProductModel>();

        using (var connection = GetConnection())
        {
            connection.Open();
            using (var command = new OracleCommand("pkg_warehouse_product_managment.proc_get_current_balance", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("p_id", OracleDbType.Int32).Value = id;
                OracleParameter cursorParameter = new OracleParameter
                {
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(cursorParameter);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var product = new ProductModel
                        {
                            barcode = reader["barcode"].ToString(),
                            product_name = reader["product_name"].ToString(),
                            name = reader["name"].ToString(),
                            lastname = reader["lastname"].ToString(),
                            warehouse_name = reader["warehouse_name"].ToString(),
                            current_balance= Convert.ToInt32(reader["current_balance"])
                        };
                        products.Add(product);
                    }
                }
            }
        }
        return products;
    }



    public bool AddWarehouse(WarehouseModel model)
    {
        using (var connection = GetConnection())
        {
            try
            {
                connection.Open();
                using (var command = new OracleCommand("pkg_warehouse_product_managment.proc_add_warehouse", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("p_warehouse", OracleDbType.Varchar2).Value = model.warehouse;
                    command.Parameters.Add("p_company_id", OracleDbType.Int32).Value = model.company_id;
                    command.Parameters.Add("p_address", OracleDbType.Varchar2).Value = model.address;
                    command.ExecuteNonQuery();

                    return true;
                }
            }
            catch (OracleException ex)
            {
                Console.WriteLine($"Oracle error: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
                return false;
            }
        }
    }

    public List<WarehouseModel> GetWarehouses()
    {
        List<WarehouseModel> warehouses = new List<WarehouseModel>();

        using (var connection = GetConnection())
        {
            connection.Open();
            using (var command = new OracleCommand("pkg_warehouse_product_managment.proc_get_warehouses", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                OracleParameter cursorParameter = new OracleParameter
                {
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(cursorParameter);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var warehouse = new WarehouseModel
                        {
                            id = Convert.ToInt32(reader["id"]),
                            warehouse = reader["warehouse"].ToString(),
                            address = reader["address"].ToString()
                        };
                        warehouses.Add(warehouse);
                    }
                }
            }
        }
        return warehouses;
    }


    public bool UpdateWarehouse(WarehouseModel model, int id)
    {
        using (var connection = GetConnection())
        {
            connection.Open();
            using (var command = new OracleCommand("pkg_warehouse_product_managment.proc_update_warehouses", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("p_id", OracleDbType.Int32).Value = id;
                command.Parameters.Add("p_warehouse", OracleDbType.Varchar2).Value = model.warehouse;
                command.Parameters.Add("p_address", OracleDbType.Varchar2).Value = model.address;
                try
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("User updated successfully.");
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating user: {ex.Message}");
                    return false;
                }
            }
        }
}
