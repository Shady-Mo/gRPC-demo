namespace PaymentService.Models {
    public class UsersBL {
        List<User> users = [];

        public UsersBL() {
            users.AddRange(new List<User> {
                new User { Id = 1, Amount = 100 },
                new User { Id = 2, Amount = 200 },
                new User { Id = 3, Amount = 300 },
                new User { Id = 4, Amount = 400 },
                new User { Id = 5, Amount = 500 },
                new User { Id = 6, Amount = 600 },
                new User { Id = 7, Amount = 700 },
            });
        }

        public User GetUserById(int id) {
            return users.FirstOrDefault(u => u.Id == id);
        }
    }
}
