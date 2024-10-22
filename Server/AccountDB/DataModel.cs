using System.ComponentModel.DataAnnotations.Schema;

namespace AccountDB
{
        public enum ProviderType
        {
            None = 0,
            Guest = 1,
            Facebook = 2,
            Google = 3,
        }

        [Table("Account")]
        public class AccountDb
        {
            public int AccountDbId { get; set; }
            public string LoginProviderUserId { get; set; } = string.Empty;
            public ProviderType LoginProviderType { get; set; }
        }
}
