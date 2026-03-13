namespace Domain
{
    public class ConcurrentRentException : Exception
    {
        public ConcurrentRentException(Guid serverId)
            : base($"Сервер с ID '{serverId}' только что был арендован или изменен другим пользователем. Пожалуйста, выберите другой сервер.")
        {
        }
    }
}