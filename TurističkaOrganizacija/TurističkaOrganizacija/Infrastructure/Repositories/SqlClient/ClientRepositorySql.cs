using System;
using System.Collections.Generic;
using System.Data.Common;
using TurističkaOrganizacija.Application;
using TurističkaOrganizacija.Backend;
using TurističkaOrganizacija.Domain;

namespace TurističkaOrganizacija.Infrastructure.Repositories.SqlClient
{
    public class ClientRepositorySql : IClientRepository
    {
        public bool ExistsPassport(int passportNumber, int? excludeClientId = null)
        {
            using (DbConnection conn = DBconnection.Instance)
            {
                if (conn.State != System.Data.ConnectionState.Open) conn.Open();
                using (DbCommand cmd = conn.CreateCommand())
                {
                    if (excludeClientId.HasValue)
                    {
                        cmd.CommandText = "SELECT COUNT(1) FROM klijenti WHERE brPasosa=@p AND idKlijenta<>@id";
                        AddParam(cmd, "@id", excludeClientId.Value);
                    }
                    else
                    {
                        cmd.CommandText = "SELECT COUNT(1) FROM klijenti WHERE brPasosa=@p";
                    }
                    AddParam(cmd, "@p", passportNumber);
                    var count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }
        public Client GetById(int id)
        {
            using (DbConnection conn = DBconnection.Instance)
            {
                if (conn.State != System.Data.ConnectionState.Open) conn.Open();
                using (DbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT idKlijenta, ime, prezime, brPasosa, EncryptedPasos, datumRodjenja, email, brTel FROM klijenti WHERE idKlijenta = @id";
                    var p = cmd.CreateParameter();
                    p.ParameterName = "@id";
                    p.Value = id;
                    cmd.Parameters.Add(p);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read()) return null;
                        return MapClient(reader);
                    }
                }
            }
        }

        public IEnumerable<Client> Search(string firstName, string lastName, string passportLike)
        {
            var result = new List<Client>();
            using (DbConnection conn = DBconnection.Instance)
            {
                if (conn.State != System.Data.ConnectionState.Open) conn.Open();
                using (DbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText =
                        "SELECT idKlijenta, ime, prezime, brPasosa, EncryptedPasos, datumRodjenja, email, brTel FROM klijenti " +
                        "WHERE (@fn IS NULL OR ime LIKE @fnLike) AND (@ln IS NULL OR prezime LIKE @lnLike) AND (@pp IS NULL OR CAST(brPasosa AS varchar(20)) LIKE @ppLike)";

                    AddLikeParam(cmd, "@fn", "@fnLike", firstName);
                    AddLikeParam(cmd, "@ln", "@lnLike", lastName);
                    AddLikeParam(cmd, "@pp", "@ppLike", passportLike);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(MapClient(reader));
                        }
                    }
                }
            }
            return result;
        }

        public void Add(Client client)
        {
            using (DbConnection conn = DBconnection.Instance)
            {
                if (conn.State != System.Data.ConnectionState.Open) conn.Open();
                using (DbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO klijenti(ime, prezime, brPasosa, EncryptedPasos, datumRodjenja, email, brTel) VALUES(@ime, @prezime, @brPasosa, @enc, @datum, @email, @brTel)";
                    AddParam(cmd, "@ime", client.FirstName);
                    AddParam(cmd, "@prezime", client.LastName);
                    AddParam(cmd, "@brPasosa", client.PassportNumber);
                    AddParamBinary(cmd, "@enc", client.PassportNumberEncrypted);
                    AddParam(cmd, "@datum", client.DateOfBirth);
                    AddParam(cmd, "@email", client.Email);
                    AddParam(cmd, "@brTel", client.Phone);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Update(Client client)
        {
            using (DbConnection conn = DBconnection.Instance)
            {
                if (conn.State != System.Data.ConnectionState.Open) conn.Open();
                using (DbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "UPDATE klijenti SET ime=@ime, prezime=@prezime, brPasosa=@brPasosa, EncryptedPasos=@enc, datumRodjenja=@datum, email=@email, brTel=@brTel WHERE idKlijenta=@id";
                    AddParam(cmd, "@ime", client.FirstName);
                    AddParam(cmd, "@prezime", client.LastName);
                    AddParam(cmd, "@brPasosa", client.PassportNumber);
                    AddParamBinary(cmd, "@enc", client.PassportNumberEncrypted);
                    AddParam(cmd, "@datum", client.DateOfBirth);
                    AddParam(cmd, "@email", client.Email);
                    AddParam(cmd, "@brTel", client.Phone);
                    AddParam(cmd, "@id", client.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (DbConnection conn = DBconnection.Instance)
            {
                if (conn.State != System.Data.ConnectionState.Open) conn.Open();
                using (DbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM klijenti WHERE idKlijenta=@id";
                    AddParam(cmd, "@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static void AddLikeParam(DbCommand cmd, string isNullParam, string likeParam, string value)
        {
            var pNull = cmd.CreateParameter();
            pNull.ParameterName = isNullParam;
            pNull.Value = string.IsNullOrWhiteSpace(value) ? (object)DBNull.Value : value;
            cmd.Parameters.Add(pNull);

            var pLike = cmd.CreateParameter();
            pLike.ParameterName = likeParam;
            pLike.Value = string.IsNullOrWhiteSpace(value) ? (object)DBNull.Value : $"%{value}%";
            cmd.Parameters.Add(pLike);
        }

        private static Client MapClient(DbDataReader reader)
        {
            return new Client
            {
                Id = reader.GetInt32(reader.GetOrdinal("idKlijenta")),
                FirstName = reader.GetString(reader.GetOrdinal("ime")),
                LastName = reader.GetString(reader.GetOrdinal("prezime")),
                PassportNumber = Convert.ToInt32(reader["brPasosa"]),
                PassportNumberEncrypted = reader["EncryptedPasos"] == DBNull.Value ? null : Convert.ToBase64String((byte[])reader["EncryptedPasos"]),
                DateOfBirth = reader.GetDateTime(reader.GetOrdinal("datumRodjenja")),
                Email = reader.GetString(reader.GetOrdinal("email")),
                Phone = reader.GetString(reader.GetOrdinal("brTel"))
            };
        }

        private static void AddParam(DbCommand cmd, string name, object value)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.Value = value ?? DBNull.Value;
            cmd.Parameters.Add(p);
        }

        private static void AddParamBinary(DbCommand cmd, string name, string base64)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            if (string.IsNullOrWhiteSpace(base64))
                p.Value = DBNull.Value;
            else
                p.Value = Convert.FromBase64String(base64);
            cmd.Parameters.Add(p);
        }
    }
}


