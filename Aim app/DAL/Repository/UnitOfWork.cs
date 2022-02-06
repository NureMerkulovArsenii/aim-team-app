using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core;
using DAL.Abstractions.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace DAL.Repository;

public class UnitOfWork : IUnitOfWork
{
    private AppContext _context;

    private IDbContextTransaction _objTran;
    // private Dictionary<string, object> _repositories;

    public UnitOfWork(AppContext context, IGenericRepository<User> userRepository,
        IGenericRepository<Role> roleRepository, IGenericRepository<Room> roomRepository,
        IGenericRepository<TextChannel> textChannelRepository, IGenericRepository<PersonalChat> personalChatRepository,
        IGenericRepository<InviteLink> inviteLinkRepository, IGenericRepository<InviteLinksUsers> inviteLinksUsers, IGenericRepository<UsersPersonalChats> usersPersonalChats)
    {
        this._context = context;
        this.UserRepository = userRepository;
        this.RoleRepository = roleRepository;
        this.RoomRepository = roomRepository;
        this.TextChannelRepository = textChannelRepository;
        this.PersonalChatRepository = personalChatRepository;
        this.InviteLinkRepository = inviteLinkRepository;
        this.InviteLinksUsersRepository = inviteLinksUsers;
        UsersPersonalChats = usersPersonalChats;
    }

    public IGenericRepository<User> UserRepository { get; }

    public IGenericRepository<Role> RoleRepository { get; }

    public IGenericRepository<Room> RoomRepository { get; }

    public IGenericRepository<TextChannel> TextChannelRepository { get; }

    public IGenericRepository<PersonalChat> PersonalChatRepository { get; }

    public IGenericRepository<InviteLink> InviteLinkRepository { get; }
    public IGenericRepository<InviteLinksUsers> InviteLinksUsersRepository { get; }
    public IGenericRepository<UsersPersonalChats> UsersPersonalChats { get; }


    // public AppContext Context
    // {
    //     get { return _context; }
    // }

    public async Task CreateTransactionAsync()
    {
        _objTran = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        await _objTran.CommitAsync();
    }

    public async Task RollbackAsync()
    {
        await _objTran.RollbackAsync();
        await _objTran.DisposeAsync();
    }

    public async Task SaveAsync()
    {
        // try
        // {
        await _context.SaveChangesAsync();
        // }
        // catch (DbEntityValidationException dbEx)
        // {
        //     foreach (var validationErrors in dbEx.EntityValidationErrors)
        //         foreach (var validationError in validationErrors.ValidationErrors)
        //             _errorMessage += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;
        //     throw new Exception(_errorMessage, dbEx);
        // }
    }

    // public IGenericRepository<T> GenericRepository<T>() where T : class
    // {
    //     _repositories ??= new Dictionary<string, object>();
    //         
    //     var type = typeof(T).Name;
    //     if (!_repositories.ContainsKey(type))
    //     {
    //         var repositoryType = typeof(IGenericRepository<T>);
    //         var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), _context);
    //         _repositories.Add(type, repositoryInstance);
    //     }
    //     
    //     return (IGenericRepository<T>)_repositories[type];
    // }

    // public GenericRepositoryDb<T> GenericRepositoryDb<T>() where T : class
    // {
    //     _repositories ??= new Dictionary<string, object>();
    //         
    //     var type = typeof(T).Name;
    //     if (!_repositories.ContainsKey(type))
    //     {
    //         var repositoryType = typeof(GenericRepositoryDb<T>);
    //         var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), _context);
    //         _repositories.Add(type, repositoryInstance);
    //     }
    //     
    //     return (GenericRepositoryDb<T>)_repositories[type];
    // }

    private bool _disposed = false;

    protected virtual async Task Dispose(bool disposing)
    {
        if (!this._disposed)
        {
            if (disposing)
            {
                await _context.DisposeAsync();
            }
        }

        this._disposed = true;
    }

    public async void Dispose()
    {
        await Dispose(true);
        GC.SuppressFinalize(this);
    }
}
