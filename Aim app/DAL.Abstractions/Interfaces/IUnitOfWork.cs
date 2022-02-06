using System;
using System.Threading.Tasks;
using Core;

namespace DAL.Abstractions.Interfaces;

public interface IUnitOfWork : IDisposable
{
    Task CreateTransactionAsync();

    Task CommitAsync();

    Task RollbackAsync();

    Task SaveAsync();

    // IGenericRepository<T> GenericRepositoryDb<T>() where T : class;

    IGenericRepository<User> UserRepository { get; }
    
    IGenericRepository<Role> RoleRepository { get; }

    IGenericRepository<Room> RoomRepository { get; }

    IGenericRepository<TextChannel> TextChannelRepository { get; }

    IGenericRepository<PersonalChat> PersonalChatRepository { get; }

    IGenericRepository<InviteLink> InviteLinkRepository { get; }
    IGenericRepository<InviteLinksUsers> InviteLinksUsersRepository { get; }
    IGenericRepository<UsersPersonalChats> UsersPersonalChats { get; }
}
