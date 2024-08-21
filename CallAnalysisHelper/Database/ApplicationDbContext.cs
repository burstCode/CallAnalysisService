using Microsoft.EntityFrameworkCore;

using CallAnalysisHelper.Models;

namespace CallAnalysisHelper.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<CallRecord> CallRecords { get; set; }
        public DbSet<Client> Clients { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }



        // Импорт записей о клиентах
        public void ImportClients(List<Client> clients)
        {
            using (var transaction = Database.BeginTransaction())
            {
                try
                {
                    // Отключаем IDENTITY_INSERT
                    Database.ExecuteSqlRaw("SET IDENTITY_INSERT Clients ON");

                    foreach (var client in clients)
                    {
                        // Проверка наличия клиента в БД по его идентификатору 
                        var existingClient = Clients
                            .FirstOrDefault(c => c.Client_Id == client.Client_Id);

                        if (existingClient != null)
                        {
                            // Обновляем номера телефонов, если те изменились
                            if (existingClient.Client_PhoneNumbers != client.Client_PhoneNumbers)
                            {
                                existingClient.Client_PhoneNumbers = client.Client_PhoneNumbers;
                                Clients.Update(existingClient);
                            }
                        }
                        else
                        {
                            // Добавляем нового клиента, если его нет в базе данных
                            Clients.Add(client);
                        }
                    }

                    SaveChanges();

                    // Включаем IDENTITY_INSERT обратно
                    Database.ExecuteSqlRaw("SET IDENTITY_INSERT Clients OFF");

                    SaveChanges();

                    // Подтверждаем транзакцию
                    transaction.Commit();
                }
                catch
                {
                    // Откатываем транзакцию в случае ошибки
                    transaction.Rollback();
                    throw;
                }
            }
        }



        // Импорт записей о звонках
        public void ImportCallRecords(List<CallRecord> callRecords)
        {
            using (var transaction = Database.BeginTransaction())
            {
                try
                {
                    foreach (var callRecord in callRecords)
                    {
                        // Проверяем наличие записи о звонке путем поиска звонка с идентичным номером телефона, датой и временем
                        var existingCall = CallRecords
                            .SingleOrDefault(cr => cr.Call_ClientPhoneNumber == callRecord.Call_ClientPhoneNumber
                                                && cr.Call_Date == callRecord.Call_Date
                                                && cr.Call_Time == callRecord.Call_Time);

                        if (existingCall == null)
                        {
                            CallRecords.Add(callRecord);
                        }
                    }

                    SaveChanges();

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

    }
}
