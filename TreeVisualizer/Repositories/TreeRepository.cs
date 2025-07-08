using MySql.Data.MySqlClient;
using TreeVisualizer.DTOS;
using TreeVisualizer.Models;
using TreeVisualizer.Repositories;

namespace TreeVisualizer.Repositories
{
    internal class TreeRepository : BaseRepository
    {
        public int Create(TreeCreateDTO tree)
        {
            using(var conn = GetConnection())
            {
                    conn.Open();
                    string sql = @"INSERT INTO trees (tree_type, description, created_by, name, serialized_data)
                                   VALUES (@TreeType, @Description, @CreatedBy, @Name, @SerializedData);
                                   SELECT LAST_INSERT_ID();";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@TreeType", tree.TreeType);
                        cmd.Parameters.AddWithValue("@Description", tree.Description);
                        cmd.Parameters.AddWithValue("@CreatedBy", tree.CreatedBy);
                        cmd.Parameters.AddWithValue("@Name", tree.Name);
                        cmd.Parameters.AddWithValue("@SerializedData", tree.SerializedData);
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
            }
        }

        public int Update(int id, TreeCreateDTO treeDTO)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = @"UPDATE trees 
                               SET tree_type = @TreeType, 
                                   description = @Description, 
                                   created_by = @CreatedBy, 
                                   name = @Name, 
                                   serialized_data = @SerializedData 
                               WHERE id = @Id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@TreeType", treeDTO.TreeType);
                    cmd.Parameters.AddWithValue("@Description", treeDTO.Description);
                    cmd.Parameters.AddWithValue("@CreatedBy", treeDTO.CreatedBy);
                    cmd.Parameters.AddWithValue("@Name", treeDTO.Name);
                    cmd.Parameters.AddWithValue("@SerializedData", treeDTO.SerializedData);
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public bool Delete(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = @"DELETE FROM trees WHERE id = @Id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public Tree? GetById(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = @"SELECT id, tree_type, serialized_data, description, created_by, name 
                               FROM trees WHERE id = @Id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Tree
                            {
                                Id = reader.GetInt32("id"),
                                TreeType = reader.GetString("tree_type"),
                                SerializedData = reader.GetString("serialized_data"),
                                Description = reader.GetString("description"),
                                CreatedBy = reader.GetInt32("created_by"),
                                Name = reader.GetString("name")
                            };
                        }
                    }
                }
            }
            return null;
        }

        public List<Tree> GetByUserId(int userId)
        {
            using(var conn = GetConnection())
            {
                conn.Open();
                string sql = @"SELECT id, tree_type, serialized_data, description, created_by, name 
                               FROM trees WHERE created_by = @UserId";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        List<Tree> trees = new List<Tree>();
                        while (reader.Read())
                        {
                            trees.Add(new Tree
                            {
                                Id = reader.GetInt32("id"),
                                TreeType = reader.GetString("tree_type"),
                                SerializedData = reader.GetString("serialized_data"),
                                Description = reader.GetString("description"),
                                CreatedBy = reader.GetInt32("created_by"),
                                Name = reader.GetString("name")
                            });
                        }
                        return trees;
                    }
                }
            }
        }
    }
}