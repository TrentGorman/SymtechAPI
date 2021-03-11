using refactor_this.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Threading.Tasks;   //supporting async

namespace refactor_this.Controllers
{
    public class AccountController : ApiController
    {
        //async and await added
        private async Task<Account> Get(Guid id)
        {
            using (var connection = Helpers.NewConnection())
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand($"select * from Accounts where Id = '{id}'", connection))
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (!reader.Read())
                        throw new ArgumentException();
                    var account = new Account(id);
                    account.Name = reader["Name"].ToString();
                    account.Number = reader["Number"].ToString();
                    account.Amount = float.Parse(reader["Amount"].ToString());
                    return account;
                }
            }
        }

        //async and await added
        [HttpGet, Route("api/Accounts/{id}")]
        public async Task<IHttpActionResult> GetById(Guid id)
        {
            using (var connection = Helpers.NewConnection())
            {
                return Ok(await Get(id));
            }
        }

        //async and await added
        [HttpGet, Route("api/Accounts")]
        public async Task<IHttpActionResult> Get()
        {
            using (var connection = Helpers.NewConnection())
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand($"select Id from Accounts", connection))
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    var accounts = new List<Account>();
                    while (reader.Read())
                    {
                        var id = Guid.Parse(reader["Id"].ToString());
                        var account = Get(id);
                        accounts.Add(await account);
                    }
                    return Ok(accounts);
                }
            }
        }

        //account.save is asynchronous
        [HttpPost, Route("api/Accounts")]
        public async Task<IHttpActionResult> Add(Account account)
        {
            //validating required account data was passed
            if (account.Name == null || account.Number == null)
                return BadRequest("The JSON data was invalid");

            await account.Save();
            return Ok();
        }

        //Get is async, Update will await on Get
        [HttpPut, Route("api/Accounts/{id}")]
        public async Task<IHttpActionResult> Update(Guid id, Account account)
        {
            //Validating account name was passed
            if (account.Name == null)
                return BadRequest("The JSON data was invalid");

            var existing = await Get(id);
            existing.Name = account.Name;
            await existing.Save();
            return Ok();
        }

        //Get is async, Delete will await on Get
        [HttpDelete, Route("api/Accounts/{id}")]
        public async Task<IHttpActionResult> Delete(Guid id)
        {
            var existing = await Get(id);
            await existing.Delete();
            return Ok();
        }
    }
}