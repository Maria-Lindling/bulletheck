public interface IEntityController
{
  public ForceSelection Force { get; }
  public bool TryDamageEntity(float damage) ;
}