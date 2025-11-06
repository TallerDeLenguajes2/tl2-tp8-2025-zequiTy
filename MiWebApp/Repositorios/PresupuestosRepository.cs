using Microsoft.Data.Sqlite;

public class PresupuestosRepository
{
    private string _conexionADB = "Data Source=DB/nueva.db";

    // 1. Crear un nuevo presupuesto
    public int InsertarPresupuesto(Presupuestos presupuesto)
    {
        int nuevoID = 0;
        using (var conexion = new SqliteConnection(_conexionADB))
        {
            conexion.Open();
            string sql = "INSERT INTO presupuestos (nombre_destinatario, fecha_creacion) " +
                         "VALUES (@nombre, @fecha); SELECT last_insert_rowid();";

            using (var comando = new SqliteCommand(sql, conexion))
            {
                comando.Parameters.Add(new SqliteParameter("@nombre", presupuesto.NombreDestinatario));
                comando.Parameters.Add(new SqliteParameter("@fecha", presupuesto.FechaCreacion));
                nuevoID = Convert.ToInt32(comando.ExecuteScalar());
            }
        }
        return nuevoID;
    }

    // 2. Listar todos los presupuestos
    public List<Presupuestos> GetAll()
    {
        List<Presupuestos> presupuestos = new();
        using (var conexion = new SqliteConnection(_conexionADB))
        {
            conexion.Open();
            string sql = "SELECT * FROM presupuestos";

            using (var comando = new SqliteCommand(sql, conexion))
            using (var reader = comando.ExecuteReader())
            {
                while (reader.Read())
                {
                    var p = new Presupuestos
                    {
                        IdPresupuesto = Convert.ToInt32(reader["id_presupuesto"]),
                        NombreDestinatario = reader["nombre_destinatario"].ToString(),
                        FechaCreacion = Convert.ToDateTime(reader["fecha_creacion"]),
                        Detalle = new List<PresupuestoDetalle>() // se completa en otro método
                    };
                    presupuestos.Add(p);
                }
            }
        }
        return presupuestos;
    }

    // 3. Obtener presupuesto por ID (con productos y cantidades)
    public Presupuestos GetById(int id)
    {
        Presupuestos presupuesto = null;
        using (var conexion = new SqliteConnection(_conexionADB))
        {
            conexion.Open();
            string sql = "SELECT * FROM presupuestos WHERE id_presupuesto = @id";

            using (var comando = new SqliteCommand(sql, conexion))
            {
                comando.Parameters.Add(new SqliteParameter("@id", id));
                using (var reader = comando.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        presupuesto = new Presupuestos
                        {
                            IdPresupuesto = Convert.ToInt32(reader["id_presupuesto"]),
                            NombreDestinatario = reader["nombre_destinatario"].ToString(),
                            FechaCreacion = Convert.ToDateTime(reader["fecha_creacion"]),
                            Detalle = new List<PresupuestoDetalle>()
                        };
                    }
                }
            }

            // cargar detalle
            if (presupuesto != null)
            {
                string sqlDetalle = "SELECT d.id_producto, d.cantidad, p.descripcion, p.precio " +
                                    "FROM presupuesto_detalle d " +
                                    "JOIN productos p ON d.id_producto = p.id_prod " +
                                    "WHERE d.id_presupuesto = @id";

                using (var comando = new SqliteCommand(sqlDetalle, conexion))
                {
                    comando.Parameters.Add(new SqliteParameter("@id", id));
                    using (var reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var producto = new Productos
                            {
                                IdProducto = Convert.ToInt32(reader["id_producto"]),
                                Descripcion = reader["descripcion"].ToString(),
                                Precio = Convert.ToInt32(reader["precio"])
                            };

                            var detalle = new PresupuestoDetalle
                            {
                                Producto = producto,
                                Cantidad = Convert.ToInt32(reader["cantidad"])
                            };

                            presupuesto.Detalle.Add(detalle);
                        }
                    }
                }
            }
        }
        return presupuesto;
    }

    // 4. Agregar producto a un presupuesto
    public void AgregarProducto(int idPresupuesto, int idProducto, int cantidad)
    {
        using (var conexion = new SqliteConnection(_conexionADB))
        {
            conexion.Open();
            string sql = "INSERT INTO presupuesto_detalle (id_presupuesto, id_producto, cantidad) " +
                         "VALUES (@idPresupuesto, @idProducto, @cantidad)";

            using (var comando = new SqliteCommand(sql, conexion))
            {
                comando.Parameters.Add(new SqliteParameter("@idPresupuesto", idPresupuesto));
                comando.Parameters.Add(new SqliteParameter("@idProducto", idProducto));
                comando.Parameters.Add(new SqliteParameter("@cantidad", cantidad));
                comando.ExecuteNonQuery();
            }
        }
    }

    // 5. Eliminar presupuesto
    public void BorrarPresupuesto(int id)
    {
        using (var conexion = new SqliteConnection(_conexionADB))
        {
            conexion.Open();

            // primero borrar detalle
            string sqlDetalle = "DELETE FROM presupuesto_detalle WHERE id_presupuesto = @id";
            using (var cmdDetalle = new SqliteCommand(sqlDetalle, conexion))
            {
                cmdDetalle.Parameters.Add(new SqliteParameter("@id", id));
                cmdDetalle.ExecuteNonQuery();
            }

            // luego borrar presupuesto
            string sql = "DELETE FROM presupuestos WHERE id_presupuesto = @id";
            using (var comando = new SqliteCommand(sql, conexion))
            {
                comando.Parameters.Add(new SqliteParameter("@id", id));
                comando.ExecuteNonQuery();
            }
        }
    }
}

/*

Puntos importantes:
- Seguí el mismo patrón de tu ProductoRepositorio: conexión SQLite, using, consultas SQL y retorno de objetos.
- Separé la carga del detalle en el método GetById, para que traiga los productos asociados.
- En InsertarPresupuesto usé last_insert_rowid() para recuperar el ID recién creado.
- En BorrarPresupuesto primero elimino los detalles y luego el presupuesto (para mantener integridad referencial).


*/