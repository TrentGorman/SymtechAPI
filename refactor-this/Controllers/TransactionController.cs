using refactor_this.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Threading.Tasks; //added to support async methods

namespace refactor_this.Controllers
{
    public class TransactionController : ApiController
    {

        //new GetTransaction async method
        [HttpGet, Route("api/Accounts/{id}/Transactions")]
        public async Task<IHttpActionResult> GetTransactions(Guid id)
        {
            using (var connection = Helpers.NewConnection())
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand($"select Amount, Date from Transactions where AccountId = '{id}'", connection))
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    var transactions = new List<Transaction>();
                    while (reader.Read())
                    {
                        var amount = (float)reader.GetDouble(0);
                        var date = reader.GetDateTime(1);
                        transactions.Add(new Transaction(amount, date));
                    }
                    return Ok(transactions);
                }
            }
        }

        //new AddTransaction async method
        [HttpPost, Route("api/Accounts/{id}/Transactions")]
        public async Task<IHttpActionResult> AddTransaction(Guid id, Transaction transaction)
        {

            if (transaction == null)
                return BadRequest("The JSON data was invalid");

            using (var connection = Helpers.NewConnection())
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand($"update Accounts set Amount = Amount + {transaction.Amount} where Id = '{id}'", connection))
                {
                    if (await command.ExecuteNonQueryAsync() != 1)
                        return BadRequest("Could not update account amount");
                }

                using (SqlCommand command = new SqlCommand($"INSERT INTO Transactions (Id, Amount, Date, AccountId) VALUES ('{Guid.NewGuid()}', {transaction.Amount}, '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}', '{id}')", connection))
                {
                    if (await command.ExecuteNonQueryAsync() != 1)
                        return BadRequest("Could not insert the transaction");
                }

                return Ok();
            }
        }
    }
}