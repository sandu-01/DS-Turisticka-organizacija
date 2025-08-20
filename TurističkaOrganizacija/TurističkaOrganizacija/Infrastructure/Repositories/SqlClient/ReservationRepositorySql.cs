using System;
using System.Collections.Generic;
using System.Data.Common;
using TurističkaOrganizacija.Application;
using TurističkaOrganizacija.Backend;
using TurističkaOrganizacija.Domain;

namespace TurističkaOrganizacija.Infrastructure.Repositories.SqlClient
{
    public class ReservationRepositorySql : IReservationRepository
    {
        public IEnumerable<Reservation> GetByClient(int clientId)
        {
            var list = new List<Reservation>();
            using (DbConnection conn = DBconnection.Instance)
            {
                if (conn.State != System.Data.ConnectionState.Open) conn.Open();
                using (DbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT idKlijenta,idPaketa,brPutnika,datum,dodatneUsluge FROM rezervacije WHERE idKlijenta=@cid";
                    AddParam(cmd, "@cid", clientId);
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            list.Add(new Reservation
                            {
                                ClientId = Convert.ToInt32(r["idKlijenta"]),
                                PackageId = Convert.ToInt32(r["idPaketa"]),
                                PassengerCount = Convert.ToInt32(r["brPutnika"]),
                                ReservedAt = Convert.ToDateTime(r["datum"]),
                                Status = ReservationStatus.Confirmed
                            });
                        }
                    }
                }
            }
            return list;
        }

        public void Add(Reservation reservation)
        {
            using (DbConnection conn = DBconnection.Instance)
            {
                if (conn.State != System.Data.ConnectionState.Open) conn.Open();
                using (DbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO rezervacije(idKlijenta,idPaketa,brPutnika,datum,dodatneUsluge) VALUES(@cid,@pid,@cnt,@dt,@usl)";
                    AddParam(cmd, "@cid", reservation.ClientId);
                    AddParam(cmd, "@pid", reservation.PackageId);
                    AddParam(cmd, "@cnt", reservation.PassengerCount);
                    AddParam(cmd, "@dt", reservation.ReservedAt.Date);
                    AddParam(cmd, "@usl", DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Update(Reservation reservation)
        {
            using (DbConnection conn = DBconnection.Instance)
            {
                if (conn.State != System.Data.ConnectionState.Open) conn.Open();
                using (DbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "UPDATE rezervacije SET brPutnika=@cnt, datum=@dt WHERE idKlijenta=@cid AND idPaketa=@pid";
                    AddParam(cmd, "@cid", reservation.ClientId);
                    AddParam(cmd, "@pid", reservation.PackageId);
                    AddParam(cmd, "@cnt", reservation.PassengerCount);
                    AddParam(cmd, "@dt", reservation.ReservedAt.Date);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            throw new NotSupportedException("Use Delete(clientId, packageId)");
        }

        public void Delete(int clientId, int packageId)
        {
            using (DbConnection conn = DBconnection.Instance)
            {
                if (conn.State != System.Data.ConnectionState.Open) conn.Open();
                using (DbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM rezervacije WHERE idKlijenta=@cid AND idPaketa=@pid";
                    AddParam(cmd, "@cid", clientId);
                    AddParam(cmd, "@pid", packageId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static void AddParam(DbCommand cmd, string name, object value)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.Value = value ?? DBNull.Value;
            cmd.Parameters.Add(p);
        }
    }
}


