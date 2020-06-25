namespace GameEngine.Interfaces
{
    public interface IDescription
    {
        string Serialize();
        void Deserialize(string state);
    }
}
