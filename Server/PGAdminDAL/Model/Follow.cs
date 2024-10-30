using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGAdminDAL.Model
{
    public class Follow
    {
        public string UserId { get; set; } 
        public UserModel User { get; set; }

        public string FollowerId { get; set; } 
        public UserModel Follower { get; set; }
    }
}
