

namespace BookingMicroservice.Services
{
    public class BookingService
    {



        try {
            context.SaveChanges();
        }
        catch (DbUpdateConcurrencyException ex)
        {
        foreach (var entry in ex.Entries)
        {
            var clientValues = entry.Entity;
        var databaseValues = entry.GetDatabaseValues();

        if (databaseValues == null)
        {
            // Entity was deleted by another user
            Console.WriteLine("The entity has been deleted.");
        }

    }
}
