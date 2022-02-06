using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core;
using DAL.Abstractions.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace DAL.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppContext _context;
    private IDbContextTransaction _objTran;
    // private Dictionary<string, object> _repositories;
    private readonly IGenericRepository<User> _userRepository;
    private readonly IGenericRepository<Role> _roleRepository;
    private readonly IGenericRepository<Room> _roomRepository;
    private readonly IGenericRepository<TextChannel> _textChannelRepository;
    private readonly IGenericRepository<PersonalChat> _personalChatRepository;
    private readonly IGenericRepository<InviteLink> _inviteLinkRepository;
    private readonly IGenericRepository<InviteLinksUsers> _inviteLinksUsersRepository;
    private readonly IGenericRepository<UsersPersonalChats> _usersPersonalChatsRepository;

    public UnitOfWork(AppContext context)
    {
        this._context = context;
    }

    public IGenericRepository<User> UserRepository => _userRepository ?? new GenericRepositoryDb<User>(_context);

    public IGenericRepository<Role> RoleRepository => _roleRepository ?? new GenericRepositoryDb<Role>(_context);

    public IGenericRepository<Room> RoomRepository => _roomRepository ?? new GenericRepositoryDb<Room>(_context);

    public IGenericRepository<TextChannel> TextChannelRepository =>
        _textChannelRepository ?? new GenericRepositoryDb<TextChannel>(_context);

    public IGenericRepository<PersonalChat> PersonalChatRepository =>
        _personalChatRepository ?? new GenericRepositoryDb<PersonalChat>(_context);

    public IGenericRepository<InviteLink> InviteLinkRepository =>
        _inviteLinkRepository ?? new GenericRepositoryDb<InviteLink>(_context);

    public IGenericRepository<InviteLinksUsers> InviteLinksUsersRepository =>
        _inviteLinksUsersRepository ?? new GenericRepositoryDb<InviteLinksUsers>(_context);

    public IGenericRepository<UsersPersonalChats> UsersPersonalChats =>
        _usersPersonalChatsRepository ?? new GenericRepositoryDb<UsersPersonalChats>(_context);
    
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
