using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;   //added for return type Task
using System.Web;

namespace refactor_this.Models
{
    public class Account
    {
        private bool isNew;

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public float Amount { get; set; }

        public Account()
        {
            isNew = true;
        }

        public Account(Guid id)
        {
            isNew = false;
            Id = id;
        }

        //async method
        public async Task Save()
        {
            using (var connection = Helpers.NewConnection())
            {
                SqlCommand command;
                if (isNew)
                    command = new SqlCommand($"insert into Accounts (Id, Name, Number, Amount) values ('{Guid.NewGuid()}', '{Name}', {Number}, 0)", connection);
                else
                    command = new SqlCommand($"update Accounts set Name = '{Name}' where Id = '{Id}'", connection);

                await connection.OpenAsync();   //async connection
                using (command) { await command.ExecuteNonQueryAsync(); }   //now async command           
            }
        }

        //async method
        public async Task Delete()
        {
            using (var connection = Helpers.NewConnection())
            {
                await connection.OpenAsync();   //async connection
                using (SqlCommand command = new SqlCommand($"delete from Accounts where Id = '{Id}'", connection))
                {
                    await command.ExecuteNonQueryAsync();   //now async command
                }
            }
        }
    }
}