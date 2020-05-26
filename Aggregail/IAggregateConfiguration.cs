namespace Aggregail
{
    public interface IAggregateConfiguration<in TIdentity>
    {
        string Stream(TIdentity id);
    }
}