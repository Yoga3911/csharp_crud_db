using Npgsql;
using System.Data;

class Program
{
    static void Main()
    {
        // Object dari UserManager
        UserManager userManager = new UserManager();
        
        // Get All Users
        userManager.GetAllUsers();

        // Get User by Id
        userManager.GetUserById(2);

        // Insert User
        userManager.InsertUser(new User { Id = 8, Username = "Hehe123" });

        // Update User
        userManager.UpdateUser(new User { Id = 2, Username = "Loki Baru" });

        // Delete User
        userManager.DeleteUser(8);
    }
}

// Model User -> Representasi dari Tabel dan Kolom users
class User
{
    public int Id { get; set; }
    public string? Username { get; set; }
}

// Class UserManager untuk melakuakn CRUD pada tabel Users
class UserManager
{
    // Object Helper
    Helper helper;
    // Object DataSet
    DataSet ds;
    // Object List of NpgsqlParameter
    NpgsqlParameter[] param;
    // Query ke Database
    string query;
    public UserManager() {
        // Inisialisasi Object
        helper = new Helper(); 
        ds = new DataSet();
        param = new NpgsqlParameter[] { };
        query = "";
    }

    // Method untuk mendapatkan semua data users
    public void GetAllUsers()
    {
        // Reinisialiasi ds dan param agar dataset dan parameter nya kembali null
        ds = new DataSet();
        param = new NpgsqlParameter[] { };

        // Query Select
        query = "SELECT * FROM users;";
        // Panggil DBConn untuk eksekusi Query
        helper.DBConn(ref ds, query, param);

        // List of User untuk menampung data user
        List<User> users = new List<User>();
        // Mengambil value dari tabel di index 0
        var data = ds.Tables[0];
        
        // Perulangan untuk mengambil instance tiap baris dari tabel
        foreach (DataRow u in data.Rows)
        {
            // Membuat object User baru
            User user = new User();
            // Mengisi id dan username dari object user dengan nilai dari database
            user.Id = u.Field<Int32>(data.Columns[0]);
            user.Username = u.Field<string>(data.Columns[1]);
            // Menambahkan user ke users (List of User)
            users.Add(user);
        }

        // Perulangan untuk menampilkan semua data User yang ada pada users
        foreach (User user in users)
        {
            Console.WriteLine($"ID: {user.Id} -- Username: {user.Username}");
        }
    }
    public void GetUserById(int id)
    {
        ds = new DataSet();
        param = new NpgsqlParameter[] { 
            // Parameter untuk id
            new NpgsqlParameter("@id", id)
        };

        query = "SELECT * FROM users WHERE id = @id;";
        helper.DBConn(ref ds, query, param);


        List<User> users = new List<User>();

        var data = ds.Tables[0];

        foreach (DataRow u in data.Rows)
        {
            User user = new User();
            user.Id = u.Field<Int32>(data.Columns[0]);
            user.Username = u.Field<string>(data.Columns[1]);
            users.Add(user);
        }

        foreach (User user in users)
        {
            Console.WriteLine($"ID: {user.Id} -- Username: {user.Username}");
        }
    }
    public void InsertUser(User user)
    {
        ds = new DataSet();
        param = new NpgsqlParameter[] {
            // Parameter untuk id dan username
            new NpgsqlParameter("@id", user.Id),
            new NpgsqlParameter("@username", user.Username),
        };

        query = "INSERT INTO users VALUES (@id, @username);";
        helper.DBConn(ref ds, query, param);
    }
    public void UpdateUser(User user)
    {            
        ds = new DataSet();
        param = new NpgsqlParameter[] {
            new NpgsqlParameter("@id", user.Id),
            new NpgsqlParameter("@username", user.Username),
        };

        query = "UPDATE users SET username = @username WHERE id = @id;";
        helper.DBConn(ref ds, query, param);
        
    }
    public void DeleteUser(int id)
    {
        ds = new DataSet();
        param = new NpgsqlParameter[] {
            new NpgsqlParameter("@id", id)
        };

        query = "DELETE FROM users WHERE id = @id;";
        helper.DBConn(ref ds, query, param);
    }
}

// Helper untuk koneksi ke DB
class Helper
{
    public void DBConn(ref DataSet ds, string query, NpgsqlParameter[] param)
    {
        // Data Source Name berisi credential dari database
        string dsn = "Host=localhost;Username=postgres;Password=123456;Database=coba;Port=54321";
        // Membuat koneksi ke db
        var conn = new NpgsqlConnection(dsn);
        // Command untuk eksekusi query
        var cmd = new NpgsqlCommand(query, conn);

        try
        {   
            // Perulangan untuk menyisipkan nilai yang ada pada parameter ke query
            foreach (var p in param)
            {
                cmd.Parameters.Add(p);
            }
            // Membuka koneksi ke database
            cmd.Connection!.Open();
            // Mengisi ds dengan data yang didapatkan dari database
            new NpgsqlDataAdapter(cmd).Fill(ds);
            Console.WriteLine("Query berhasil dieksekusi");
        }
        catch (NpgsqlException e)
        {
            Console.WriteLine(e);
        } 
        finally
        {
            // Menutup koneksi ke database
            cmd.Connection!.Close();
        }
        
    }
}