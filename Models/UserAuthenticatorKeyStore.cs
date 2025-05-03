using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;

namespace TabloX2.Models
{
    public class UserAuthenticatorKeyStore<TUser> : IUserAuthenticatorKeyStore<TUser>, IUserStore<TUser> where TUser : ApplicationUser
    {
        private readonly UserManager<TUser> _userManager;
        private bool _disposed;

        public UserAuthenticatorKeyStore(UserManager<TUser> userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<string?> GetAuthenticatorKeyAsync(TUser user, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));

            // Önce Identity'nin kendi mekanizmasından almayı dene
            var key = await _userManager.GetAuthenticatorKeyAsync(user);
            if (!string.IsNullOrEmpty(key))
            {
                return key;
            }

            // Eğer yoksa yeni bir tane oluştur
            await _userManager.ResetAuthenticatorKeyAsync(user);
            return await _userManager.GetAuthenticatorKeyAsync(user);
        }

        public async Task SetAuthenticatorKeyAsync(TUser user, string key, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrEmpty(key)) throw new ArgumentException("Anahtar boş olamaz.", nameof(key));

            // Önce mevcut anahtarı sil
            await _userManager.ResetAuthenticatorKeyAsync(user);
            
            // Kullanıcıyı güncelle
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Anahtar kaydedilemedi: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }

        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            return Task.FromResult(user.Id);
        }

        public Task<string?> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            return Task.FromResult(user.UserName);
        }

        public Task SetUserNameAsync(TUser user, string? userName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _disposed = true;
        }

        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        public Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<TUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<TUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string?> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedUserNameAsync(TUser user, string? normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
} 